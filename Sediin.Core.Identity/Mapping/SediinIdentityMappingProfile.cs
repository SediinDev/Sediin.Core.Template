using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Entities.DTO;

namespace Sediin.Core.Identity.Mapping
{
   


    public class SediinIdentityMappingProfile : Profile
    {
        public SediinIdentityMappingProfile()
        {
            CreateMap<SediinIdentityUser, SediinIdentityUser_DTO>();
            CreateMap<SediinIdentityUser_DTO, SediinIdentityUser>();
        }
    }

}
