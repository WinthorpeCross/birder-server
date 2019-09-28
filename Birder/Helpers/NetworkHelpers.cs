﻿using Birder.Data.Model;
using Birder.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birder.Helpers
{
    public static class NetworkHelpers
    {
        // ********** MOVE TO END
        public static NetworkUserViewModel UpdateIsFollowingInNetworkUserViewModel(NetworkUserViewModel viewModel, ICollection<Network> following)
        {
            if (viewModel == null)
                throw new NullReferenceException("The viewModel is null");

            if (following == null)
                throw new NullReferenceException("The following collection is null");

            viewModel.IsFollowing = following.Any(cus => cus.ApplicationUser.UserName == viewModel.UserName);
            return viewModel;
        }

        public static List<string> GetFollowersUserNames(ICollection<Network> followers)
        {
            if(followers == null)
                throw new NullReferenceException("The followers collection is null");
            
            return (from user in followers
                    select user.Follower.UserName).ToList();
        }

        public static List<string> GetFollowingUserNames(ICollection<Network> following)
        {
            if (following == null)
                throw new NullReferenceException("The following collection is null");

            return (from user in following
                    select user.ApplicationUser.UserName).ToList();
        }

        public static IEnumerable<string> GetFollowersNotBeingFollowedUserNames(ApplicationUser loggedinUser)
        {
            if (loggedinUser == null)
                throw new NullReferenceException("The user is null");

            List<string> followersUsernamesList = GetFollowersUserNames(loggedinUser.Followers);
            List<string> followingUsernamesList = GetFollowingUserNames(loggedinUser.Following);
            followingUsernamesList.Add(loggedinUser.UserName); //include own user name
            return followersUsernamesList.Except(followingUsernamesList);
        }
    }
}
