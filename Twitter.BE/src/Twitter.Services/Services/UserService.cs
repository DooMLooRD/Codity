﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Twitter.Data.Model;
using Twitter.Repositories.Interfaces;
using Twitter.Services.Interfaces;
using Twitter.Services.RequestModels.User;
using Twitter.Services.Resources;
using Twitter.Services.ResponseModels;
using Twitter.Services.ResponseModels.DTOs.User;
using Twitter.Services.ResponseModels.Interfaces;

namespace Twitter.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<Follow> _followRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly INotificationGeneratorService _notificationGeneratorService;
        private readonly IMapper _mapper;

        public UserService(
            IBaseRepository<Follow> followRepository,
            IBaseRepository<User> userRepository,
            INotificationGeneratorService notificationGeneratorService,
            IMapper mapper)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
            _notificationGeneratorService = notificationGeneratorService;
            _mapper = mapper;
        }

        public async Task<IResponse<UserDTO>> GetUserAsync(int userId)
        {
            var response = new Response<UserDTO>();

            var user = await _userRepository.GetByAsync(
                c => c.Id == userId,
                false,
                c => c.Gender,
                c => c.Followers,
                c => c.Following);

            if (user == null)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.UserNotFound
                });
            }

            response.Model = _mapper.Map<UserDTO>(user);

            return response;
        }

        public async Task<ICollectionResponse<BaseUserDTO>> GetUsersAsync()
        {
            var response = new CollectionResponse<BaseUserDTO>();

            var users = await _userRepository.GetAllAsync();

            response.Models = _mapper.Map<IEnumerable<BaseUserDTO>>(users);

            return response;
        }

        public async Task<IBaseResponse> UnfollowUserAsync(int userId, FollowingRequest following)
        {
            var response = new BaseResponse();

            var follow = await _followRepository
                .GetByAsync(c => c.FollowerId == userId && c.FollowingId == following.FollowingId);

            if (follow == null)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.FollowNotFound
                });

                return response;
            }

            await _followRepository.RemoveAsync(follow);

            return response;
        }

        public async Task<IBaseResponse> FollowUserAsync(int userId, FollowingRequest following)
        {
            var response = new BaseResponse();
            var followerUser = await _userRepository.GetAsync(userId);
            var followingUser = await _userRepository.GetAsync(following.FollowingId);

            if (followerUser == null)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.UserNotFound
                });

                return response;
            }

            if (followingUser == null)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.UserNotFound
                });

                return response;
            }

            var follow = await _followRepository
                .GetByAsync(c => c.FollowerId == userId && c.FollowingId == following.FollowingId);

            if (follow != null)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.FollowAlreadyExists
                });

                return response;
            }

            follow = new Follow
            {
                FollowerId = userId,
                FollowingId = following.FollowingId
            };

            await _followRepository.AddAsync(follow);

            await _notificationGeneratorService.CreateFollowNotification(followerUser, followingUser);

            return response;
        }

        public async Task<ICollectionResponse<BaseUserDTO>> GetFollowersAsync(int userId)
        {
            var response = new CollectionResponse<BaseUserDTO>();

            var follows = await _followRepository
                .GetAllByAsync(c => c.FollowingId == userId, false, c => c.Follower);

            if (!follows.Any())
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.FollowersNotFound
                });

                return response;
            }

            var followers = follows.Select(c => c.Follower);
            response.Models = _mapper.Map<IEnumerable<BaseUserDTO>>(followers);

            return response;
        }

        public async Task<ICollectionResponse<BaseUserDTO>> GetFollowingAsync(int userId)
        {
            var response = new CollectionResponse<BaseUserDTO>();

            var follows = await _followRepository
                .GetAllByAsync(c => c.FollowerId == userId, false, c => c.Following);

            if (!follows.Any())
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.FollowingNotFound
                });

                return response;
            }

            var following = follows.Select(c => c.Following);
            response.Models = _mapper.Map<IEnumerable<BaseUserDTO>>(following);

            return response;
        }

        public async Task<IPagedResponse<BaseUserDTO>> GetUsersAsync(SearchUserRequest searchRequest)
        {
            var response = new PagedResponse<BaseUserDTO>();

            var searchExpression = CreateSearchExpression(searchRequest);
            var users = await _userRepository.GetAllByAsync(
                searchExpression,
                searchRequest.PageNumber,
                searchRequest.PageSize);

            _mapper.Map(users, response);

            return response;
        }

        public async Task<IBaseResponse> UpdateUserProfileAsync(int userId, UserProfileRequest userProfile)
        {
            var response = new BaseResponse();

            var user = await _userRepository.GetAsync(userId);

            _mapper.Map(userProfile, user);

            await _userRepository.UpdateAsync(user);

            return response;
        }

        private Expression<Func<User, bool>> CreateSearchExpression(SearchUserRequest searchRequest)
        {
            var query = searchRequest.Query.ToLower();

            Expression<Func<User, bool>> searchExpression = c => true;

            if (!string.IsNullOrEmpty(searchRequest.Query))
            {
                searchExpression = c => c.FirstName.ToLower().Contains(query) ||
                  c.LastName.ToLower().Contains(query) ||
                  c.AboutMe.ToLower().Contains(query);
            }

            return searchExpression;
        }
    }
}