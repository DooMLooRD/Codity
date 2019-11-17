﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Twitter.Services.Interfaces;
using Twitter.Services.ResponseModels.DTOs.Shared;
using Twitter.Services.ResponseModels.Interfaces;

namespace Twitter.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {
        private readonly IGenderService _genderService;

        public GenderController(IGenderService genderService)
        {
            _genderService = genderService;
        }

        /// <summary>
        /// Fetch all genders
        /// </summary>
        /// <returns>All genders</returns>
        [HttpGet]
        public async Task<ActionResult<ICollectionResponse<GenderDTO>>> GetGenders()
        {

            var response = await _genderService.GetGendersAsync();

            if (response.IsError)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}