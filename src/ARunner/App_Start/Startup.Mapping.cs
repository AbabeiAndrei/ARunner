using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.BusinessLogic.Models;
using ARunner.DataLayer.Model;
using AutoMapper;

namespace ARunner
{
    public partial class Startup
    {
        private static void ConfigureMapping(IMapperConfigurationExpression config)
        {
            config.CreateMap<Jogging, JoggingViewModel>().ConstructUsing(Convert);
            config.CreateMap<JoggingViewModel, Jogging>().ConstructUsing(Convert);
            config.CreateMap<User, UserModel>().ConstructUsing(Convert);
            config.CreateMap<User, UserBaseModel>().ConstructUsing(Convert);
            config.CreateMap<UserModel, User>().ConstructUsing(Convert);
        }

        private static Jogging Convert(JoggingViewModel arg)
        {
            return new Jogging
                   {
                       Id = arg.Id,
                       Created = arg.Created,
                       Distance = arg.Distance,
                       Time = arg.Time
                   };
        }

        private static JoggingViewModel Convert(Jogging arg)
        {
            return new JoggingViewModel
                   {
                       Id = arg.Id,
                       Created = arg.Created,
                       UserId = arg.User.Id,
                       Distance = arg.Distance,
                       Time = arg.Time
                   };
        }
        
        private static User Convert(UserModel arg)
        {
            return new User(arg.UserName)
                   {
                       Id = arg.Id,
                       Email = arg.Email,
                       State = arg.State,
                       Access = arg.Access,
                       FullName = arg.FullName,
                       Metadata = arg.Metadata,

                   };
        }

        private static UserModel Convert(User arg)
        {
            return new UserModel
                   {
                       Id = arg.Id,
                       Email = arg.Email,
                       FullName = arg.FullName,
                       State = arg.State,
                       Metadata = arg.Metadata,
                       Access = arg.Access,
                       UserName = arg.UserName
                   };
        }
    }
}
