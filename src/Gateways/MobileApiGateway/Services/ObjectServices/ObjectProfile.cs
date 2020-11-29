using AutoMapper;
using MobileApiGateway.Services;
using MobileApiGateway.Services.ObjectCommentServices;
using MobileApiGateway.Services.ObjectServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApiGateway
{
    public class ObjectProfile : Profile
    {
        public ObjectProfile()
        {
            CreateMap<UpstreamObjectDto, DownstreamObjectDto>();
            CreateMap<UpstreamObjectDtoV1_1, DownstreamObjectDtoV1_1>()
                .ForMember(dest => dest.DistanceInMeters, opt => opt.MapFrom(src => src.DistanceInMeters));
            CreateMap<UpstreamObjectDetailsDto, DownstreamObjectDetailsDto>();
        }
    }

    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<UpstreamCommentListDto, DownstreamCommentListDto>();
            CreateMap<UpstreamCommentDto, DownstreamCommentDto>();
        }
    }
}
