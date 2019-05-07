﻿using AutoMapper;
using Birder.Controllers;
using Birder.Data.Model;
using Birder.Helpers;
using Birder.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Birder.Data
{
    public class BirderMappingProfile : Profile
    {
        public BirderMappingProfile()
        {
            CreateMap<Observation, ObservationViewModel>()
              .ForMember(o => o.ObservationId, ex => ex.MapFrom(o => o.ObservationId))
              .ForMember(a => a.User, b => b.MapFrom(a => a.ApplicationUser))
              .ForMember(d => d.Bird, m => m.MapFrom(d => d.Bird))
              .ReverseMap();

            CreateMap<List<Observation>, ObservationAnalysisViewModel>()
                .ForMember(a => a.TotalObservationsCount, b => b.MapFrom(a => a.Count()))
                .ForMember(a => a.UniqueSpeciesCount, b => b.MapFrom(a => a.Select(i => i.BirdId).Distinct().Count()));



            CreateMap<ApplicationUser, UserViewModel>()
              .ForMember(x => x.UserName, y => y.MapFrom(x => x.UserName))
              .ForMember(x => x.ProfileImage, y => y.MapFrom(x => x.ProfileImage))
              .ForMember(x => x.DefaultLocationLatitude, y => y.MapFrom(x => x.DefaultLocationLatitude))
              .ForMember(x => x.DefaultLocationLongitude, y => y.MapFrom(x => x.DefaultLocationLongitude))
              .ReverseMap();

            CreateMap<ApplicationUser, NetworkUserViewModel>()
              .ForMember(x => x.UserName, y => y.MapFrom(x => x.UserName))
              .ReverseMap();

            CreateMap<ApplicationUser, Network>()
            .ReverseMap();

            CreateMap<Network, FollowingViewModel>()
            .ForMember(x => x.UserName, y => y.MapFrom(x => x.ApplicationUser.UserName))
            .ForMember(x => x.ProfileImage, y => y.MapFrom(x => x.ApplicationUser.ProfileImage))
            .ReverseMap();

            CreateMap<Network, FollowerViewModel>()
            .ForMember(x => x.UserName, y => y.MapFrom(x => x.Follower.UserName))
            .ForMember(x => x.ProfileImage, y => y.MapFrom(x => x.Follower.ProfileImage))
            .ReverseMap();

            CreateMap<ApplicationUser, UserProfileViewModel>()
              .ForMember(x => x.UserName, y => y.MapFrom(x => x.UserName))
              .ForMember(x => x.Followers, y => y.MapFrom(x => x.Followers))
              .ForMember(x => x.Following, y => y.MapFrom(x => x.Following))
              .ReverseMap();

            CreateMap<ApplicationUser, ManageProfileViewModel>()
                .ReverseMap();

            CreateMap<Bird, BirdDetailViewModel>()
              .ReverseMap();

            CreateMap<Bird, BirdSummaryViewModel>()
              .ForMember(a => a.ConservationStatus, b => b.MapFrom(a => a.BirdConservationStatus.ConservationList))
              .ForMember(a => a.ConservationListColourCode, b => b.MapFrom(a => a.BirdConservationStatus.ConservationListColourCode))
              .ForMember(a => a.BirderStatus, b => b.MapFrom(a => a.BirderStatus))
              .ReverseMap();

            CreateMap<TweetDay, TweetDayViewModel>()
              .ForMember(d => d.Bird, m => m.MapFrom(d => d.Bird))
             .ReverseMap();
        }
    }
}
