﻿using Birder.Data.Model;
using Birder.Helpers;
using Birder.Services;
using Birder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Birder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ObservationFeedController : ControllerBase
    {
        private const int pageSize = 10;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IObservationQueryService _observationQueryService;
        private readonly IBirdThumbnailPhotoService _profilePhotosService;

        public ObservationFeedController(ILogger<ObservationFeedController> logger
                                       , UserManager<ApplicationUser> userManager
                                       , IObservationQueryService observationQueryService
                                       , IBirdThumbnailPhotoService profilePhotosService)
        {
            _logger = logger;
            _userManager = userManager;
            _profilePhotosService = profilePhotosService;
            _observationQueryService = observationQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetObservationsFeedAsync(int pageIndex, ObservationFeedFilter filter)
        {
            try
            {
                if (filter == ObservationFeedFilter.Own)
                {
                    var userObservations = await _observationQueryService.GetPagedObservationsFeedAsync(o => o.ApplicationUser.UserName == User.Identity.Name, pageIndex, pageSize);
                    
                    if (userObservations is null)
                    {
                        _logger.LogWarning(LoggingEvents.GetListNotFound, "User Observations list was null at GetObservationsFeedAsync()");
                        return NotFound("Observations not found");
                    }

                    if (userObservations.TotalItems > 0 || pageIndex > 1)
                    {
                        _profilePhotosService.GetUrlForObservations(userObservations.Items);
                        userObservations.ReturnFilter = ObservationFeedFilter.Own;
                        return Ok(userObservations);
                    }
                }

                if (filter == ObservationFeedFilter.Network || filter == ObservationFeedFilter.Own)
                {
                    var requestingUserAndNetwork = await _userManager.GetUserWithNetworkAsync(User.Identity.Name);
                    
                    if (requestingUserAndNetwork is null)
                    {
                        _logger.LogWarning(LoggingEvents.GetItemNotFound, "Requesting user not found");
                        return NotFound("Requesting user not found");
                    }

                    var followingUsernamesList = UserNetworkHelpers.GetFollowingUserNames(requestingUserAndNetwork.Following);

                    followingUsernamesList.Add(requestingUserAndNetwork.UserName);

                    var networkObservations = await _observationQueryService.GetPagedObservationsFeedAsync(o => followingUsernamesList.Contains(o.ApplicationUser.UserName), pageIndex, pageSize);

                    if (networkObservations is null)
                    {
                        _logger.LogWarning(LoggingEvents.GetListNotFound, "Network observations list is null");
                        return NotFound("Observations not found");
                    }

                    if (networkObservations.TotalItems > 0 || pageIndex > 1)
                    {
                        _profilePhotosService.GetUrlForObservations(networkObservations.Items);
                        networkObservations.ReturnFilter = ObservationFeedFilter.Network;
                        return Ok(networkObservations);
                    }
                }

                var publicObservations = await _observationQueryService.GetPagedObservationsFeedAsync(pl => pl.SelectedPrivacyLevel == PrivacyLevel.Public, pageIndex, pageSize);
                
                if (publicObservations is null)
                {
                    _logger.LogWarning(LoggingEvents.GetListNotFound, "Observations list is null");
                    return NotFound("Observations not found");
                }

                _profilePhotosService.GetUrlForObservations(publicObservations.Items);

                publicObservations.ReturnFilter = ObservationFeedFilter.Public;
                return Ok(publicObservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.GetListNotFound, ex, "An error occurred getting the observations feed");
                return BadRequest("An unexpected error occurred");
            }
        }
    }
}
