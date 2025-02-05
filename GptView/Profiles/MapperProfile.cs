using AutoMapper;
using GptView.Models;
using GptView.ViewModels;
using System.Drawing;

namespace GptView.Profiles
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserVM>()
                .ForMember(x => x.submittype, y => y.Ignore())
                .ReverseMap();

            CreateMap<GoogleUserInfo, User>()
                .ForMember(x => x.email, y => y.MapFrom(o => o.email))
                .ForMember(x => x.userid, y => y.Ignore())
                .AfterMap((gUser, user) =>
                {
                    user.username = gUser.email.Split("@")[0];
                    user.nickname = gUser.email.Split("@")[0];
                });

            CreateMap<PointXY, PointF>()
                .ForMember(x => x.X, y => y.MapFrom(o => o.x))
                .ForMember(x => x.Y, y => y.MapFrom(o => o.y));         
        }

        public string SplitFirst(string input)
        {
            return input.Split("@")[0];
        }
    }
}
