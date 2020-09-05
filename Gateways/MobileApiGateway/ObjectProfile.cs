using AutoMapper;
using MobileApiGateway.Services;
using MobileApiGateway.Services.ObjectCommentServices;
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
            CreateMap<UpstreamObjectDtoV1_1, DownstreamObjectDtoV1_1>();
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
