﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twitter.Data.Model;
using Twitter.Repositories.Interfaces;
using Twitter.Services.Interfaces;
using Twitter.Services.RequestModels.Tweet;
using Twitter.Services.Resources;
using Twitter.Services.ResponseModels;
using Twitter.Services.ResponseModels.DTOs.Tweet;
using Twitter.Services.ResponseModels.Interfaces;

namespace Twitter.Services.Services
{
    public class CommentService : ICommentService
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly IBaseRepository<Comment> _commentRepository;
        private readonly IBaseRepository<Notification> _notificationRepository;
        private readonly INotificationGeneratorService _notificationGeneratorService;
        private readonly IMapper _mapper;

        public CommentService(
            ITweetRepository tweetRepository,
            IBaseRepository<Comment> commentRepository,
            IBaseRepository<Notification> notificationRepository,
            INotificationGeneratorService notificationGeneratorService,
            IMapper mapper)
        {
            _tweetRepository = tweetRepository;
            _commentRepository = commentRepository;
            _notificationRepository = notificationRepository;
            _notificationGeneratorService = notificationGeneratorService;
            _mapper = mapper;
        }

        public async Task<IResponse<CommentDTO>> CreateCommentAsync(int userId, CommentRequest comment)
        {
            var response = new Response<CommentDTO>();

            var tweet = await _tweetRepository.GetByAsync(c => c.Id == comment.TweetId);

            if (tweet == null)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.TweetNotFound
                });

                return response;
            }

            var commentEntity = new Comment
            {
                AuthorId = userId,
                TweetId = comment.TweetId,
                Text = comment.Text,
                CreationDate = DateTime.Now
            };

            await _commentRepository.AddAsync(commentEntity);

            var result = await _commentRepository.GetByAsync(c => c.Id == commentEntity.Id, false, c => c.Author);

            response.Model = _mapper.Map<CommentDTO>(result);

            var commentAuthor = result.Author;
            var tweetAuthor = tweet.Author;

            await _notificationGeneratorService.CreateCommentNotification(tweet, commentAuthor, tweetAuthor, result.Text);

            return response;
        }

        public async Task<ICollectionResponse<CommentDTO>> GetCommentsAsync(int tweetId)
        {
            var response = new CollectionResponse<CommentDTO>();

            var comments = await _commentRepository.GetAllByAsync(c => c.TweetId == tweetId, false, c => c.Author);

            response.Models = _mapper.Map<IEnumerable<CommentDTO>>(comments);

            return response;
        }

        public async Task<IBaseResponse> RemoveCommentAsync(int userId, int commentId)
        {
            var response = new BaseResponse();

            var commentEntity = await _commentRepository.GetByAsync(c => c.Id == commentId);

            if (commentEntity == null)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.CommentNotFound
                });

                return response;
            }

            if (commentEntity.AuthorId != userId)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.NotYourCommentRemove
                });
            }

            await _commentRepository.RemoveAsync(commentEntity);

            return response;
        }

        public async Task<IBaseResponse> UpdateCommentAsync(int userId, int commentId, UpdateCommentRequest comment)
        {
            var response = new BaseResponse();

            var commentEntity = await _commentRepository.GetByAsync(c => c.Id == commentId);

            if (commentEntity == null)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.CommentNotFound
                });

                return response;
            }

            if (commentEntity.AuthorId != userId)
            {
                response.AddError(new Error
                {
                    Message = ErrorTranslations.NotYourCommentUpdate
                });
            }

            commentEntity.Text = comment.Text;

            await _commentRepository.UpdateAsync(commentEntity);

            return response;
        }
    }
}
