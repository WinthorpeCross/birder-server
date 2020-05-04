﻿using AutoMapper;
using Birder.Controllers;
using Birder.Data;
using Birder.Data.Model;
using Birder.Data.Repository;
using Birder.Helpers;
using Birder.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Birder.Tests.Controller
{
    public class PostFollowUserAsyncTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<NetworkController>> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostFollowUserAsyncTests()
        {
            _userManager = SharedFunctions.InitialiseUserManager();
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BirderMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();
            _logger = new Mock<ILogger<NetworkController>>();
        }

        [Fact]
        public async Task PostFollowUserAsync_ReturnsNotFound_WhenRequestingUserIsNullFromRepository()
        {
            var options = this.CreateUniqueClassOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                //You have to create the database
                context.CreateEmptyViaWipe();
                context.Database.EnsureCreated();
                //context.SeedDatabaseFourBooks();

                //context.ConservationStatuses.Add(new ConservationStatus { ConservationList = "Red", Description = "", CreationDate = DateTime.Now, LastUpdateDate = DateTime.Now });

                context.Users.Add(SharedFunctions.CreateUser("testUser1"));
                context.Users.Add(SharedFunctions.CreateUser("testUser2"));

                context.SaveChanges();

                context.Users.Count().ShouldEqual(2);

                // Arrange

                //*******************
                var userManager = SharedFunctions.InitialiseUserManager(context);
                //**********************

                var mockRepo = new Mock<INetworkRepository>();
                //mockRepo.Setup(x => x.GetUserAndNetworkAsync(It.IsAny<string>()))
                //        .Returns(Task.FromResult<ApplicationUser>(null));

                var mockUnitOfWork = new Mock<IUnitOfWork>();

                var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, userManager);

                string requestingUser = "This requested user does not exist";

                string userToFollow = "This requested user does not exist";

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
                };

                // Act
                var result = await controller.PostFollowUserAsync(GetTestNetworkUserViewModel(userToFollow));

                // Assert
                var objectResult = result as ObjectResult;
                Assert.NotNull(objectResult);
                Assert.IsType<NotFoundObjectResult>(result);
                Assert.True(objectResult is NotFoundObjectResult);
                Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
                Assert.IsType<string>(objectResult.Value);
                Assert.Equal("Requesting user not found", objectResult.Value);
            }
        }




        #region Follow action tests

        [Fact]
        public async Task PostFollowUserAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {

            // Arrange
            var mockRepo = new Mock<INetworkRepository>();

            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, _userManager);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal("example name") }
            };

            //Add model error
            controller.ModelState.AddModelError("Test", "This is a test model error");


            // Act
            var result = await controller.PostFollowUserAsync(GetTestNetworkUserViewModel("Test User"));

            var modelState = controller.ModelState;
            Assert.Equal(1, modelState.ErrorCount);
            Assert.True(modelState.ContainsKey("Test"));
            Assert.True(modelState["Test"].Errors.Count == 1);
            Assert.Equal("This is a test model error", modelState["Test"].Errors[0].ErrorMessage);

            // test response
            var objectResult = result as ObjectResult;
            Assert.NotNull(objectResult);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.True(objectResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            //
            var actual = Assert.IsType<string>(objectResult.Value);

            //Assert.Contains("This is a test model error", "This is a test model error");
            Assert.Equal("Invalid modelstate", actual);
        }



        //[Fact]
        //public async Task PostFollowUserAsync_ReturnsNotFound_WhenUserToFollowIsNullFromRepository()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<INetworkRepository>();
        //    //mockRepo.Setup(x => x.GetUserAndNetworkAsync(It.IsAny<string>()))
        //    //        .Returns(Task.FromResult<ApplicationUser>(null));

        //    var mockUnitOfWork = new Mock<IUnitOfWork>();

        //    var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, _userManager);

        //    string requestingUser = "Tenko";

        //    string userToFollow = "This requested user does not exist";

        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
        //    };

        //    // Act
        //    var result = await controller.PostFollowUserAsync(GetTestNetworkUserViewModel(userToFollow));

        //    // Assert
        //    var objectResult = result as ObjectResult;
        //    Assert.NotNull(objectResult);
        //    Assert.IsType<NotFoundObjectResult>(result);
        //    Assert.True(objectResult is NotFoundObjectResult);
        //    Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        //    Assert.IsType<string>(objectResult.Value);
        //    Assert.Equal("User to follow not found", objectResult.Value);
        //}

        //[Fact]
        //public async Task PostFollowUserAsync_ReturnsBadRequest_FollowerAndToFollowAreEqual()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<INetworkRepository>();

        //    var mockUnitOfWork = new Mock<IUnitOfWork>();

        //    var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, _userManager);

        //    string requestingUser = "Tenko";

        //    string userToFollow = requestingUser;

        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
        //    };

        //    // Act
        //    var result = await controller.PostFollowUserAsync(GetTestNetworkUserViewModel(userToFollow));

        //    // Assert
        //    var objectResult = result as ObjectResult;
        //    Assert.NotNull(objectResult);
        //    Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.True(objectResult is BadRequestObjectResult);
        //    Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        //    var actual = Assert.IsType<string>(objectResult.Value);
        //    Assert.Equal("Trying to follow yourself", actual);
        //}

        //[Fact]
        //public async Task PostFollowUserAsync_ReturnsBadRequestWithstringObject_WhenExceptionIsRaised()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<INetworkRepository>();
        //    mockRepo.Setup(repo => repo.Follow(It.IsAny<ApplicationUser>(), It.IsAny<ApplicationUser>()))
        //        .Verifiable();

        //    var mockUnitOfWork = new Mock<IUnitOfWork>();
        //    mockUnitOfWork.Setup(x => x.CompleteAsync())
        //        .ThrowsAsync(new InvalidOperationException());

        //    var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, _userManager);

        //    string requestingUser = "Tenko";

        //    string userToFollow = "Toucan";

        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
        //    };

        //    // Act
        //    var result = await controller.PostFollowUserAsync(GetTestNetworkUserViewModel(userToFollow));

        //    // Assert
        //    Assert.IsType<BadRequestObjectResult>(result);
        //    var objectResult = result as ObjectResult;
        //    Assert.Equal($"An error occurred trying to follow user: {userToFollow}", objectResult.Value);
        //}

        //[Fact]
        //public async Task PostFollowUserAsync_ReturnsOkObject_WhenRequestIsValid()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<INetworkRepository>();
        //    mockRepo.Setup(repo => repo.Follow(It.IsAny<ApplicationUser>(), It.IsAny<ApplicationUser>()))
        //        .Verifiable();

        //    var mockUnitOfWork = new Mock<IUnitOfWork>();
        //    mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

        //    var controller = new NetworkController(_mapper, mockUnitOfWork.Object, _logger.Object, mockRepo.Object, _userManager);

        //    string requestingUser = "Tenko";

        //    string userToFollow = "Toucan";

        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = SharedFunctions.GetTestClaimsPrincipal(requestingUser) }
        //    };

        //    // Act
        //    var result = await controller.PostFollowUserAsync(GetTestNetworkUserViewModel(userToFollow));

        //    // Assert
        //    var objectResult = result as ObjectResult;
        //    Assert.NotNull(objectResult);
        //    Assert.IsType<OkObjectResult>(result);
        //    Assert.True(objectResult is OkObjectResult);
        //    Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        //    Assert.IsType<NetworkUserViewModel>(objectResult.Value);

        //    var model = objectResult.Value as NetworkUserViewModel;
        //    Assert.Equal(userToFollow, model.UserName);
        //}

        #endregion

        private NetworkUserViewModel GetTestNetworkUserViewModel(string username)
        {
            return new NetworkUserViewModel() { UserName = username };
        }
    }
}
