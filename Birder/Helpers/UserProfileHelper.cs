﻿using Birder.Data.Model;
using Birder.ViewModels;
using System.Linq;

namespace Birder.Helpers
{
    public static class UserProfileHelper
    {
        /// <summary>
        /// Updates the UserProfileViewModel IsFollowing property
        /// </summary>
        /// <param name="requestedUserProfileViewModel"></param>
        /// <param name="requestedUser"></param>
        /// <param name="requestingUser"></param>
        /// <returns></returns>
        public static UserProfileViewModel UpdateIsFollowingProperty(UserProfileViewModel requestedUserProfileViewModel, ApplicationUser requestedUser, ApplicationUser requestingUser) //, string loggedinUsername)
        {
            requestedUserProfileViewModel.IsFollowing = requestedUser.Followers.Any(cus => cus.Follower.UserName == requestingUser.UserName);
            return requestedUserProfileViewModel;
        }

        /// <summary>
        /// Updates the requested user's 'Following' collection (IsFollowing and IsOwnProfile properties)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="requestingUser"></param>
        /// <returns></returns>
        public static UserProfileViewModel UpdateFollowingCollection(UserProfileViewModel viewModel, ApplicationUser requestingUser) //, string loggedinUsername)
        {
            for (int i = 0; i < viewModel.Following.Count(); i++)
            {
                viewModel.Following.ElementAt(i).IsFollowing = requestingUser.Following.Any(cus => cus.ApplicationUser.UserName == viewModel.Following.ElementAt(i).UserName);
                viewModel.Following.ElementAt(i).IsOwnProfile = viewModel.Following.ElementAt(i).UserName == requestingUser.UserName;
            }

            return viewModel;
        }

        /// <summary>
        /// Updates the requested user's 'Followers' collection (IsFollowing and IsOwnProfile properties)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="requestingUser"></param>
        /// <returns></returns>
        public static UserProfileViewModel UpdateFollowersCollection(UserProfileViewModel viewModel, ApplicationUser requestingUser) //, string loggedinUsername)
        {
            for (int i = 0; i < viewModel.Followers.Count(); i++)
            {
                viewModel.Followers.ElementAt(i).IsFollowing = requestingUser.Following.Any(cus => cus.ApplicationUser.UserName == viewModel.Followers.ElementAt(i).UserName);
                viewModel.Followers.ElementAt(i).IsOwnProfile = viewModel.Followers.ElementAt(i).UserName == requestingUser.UserName;
            }

            return viewModel;
        }
    }
}
