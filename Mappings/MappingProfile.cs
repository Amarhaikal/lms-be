using AutoMapper;
using QUANTM.DTOs.Parameter;
using QUANTM.DTOs.User;
using QUANTM.Models.Parameter;
using QUANTM.Models.User;
namespace QUANTM.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<CodeType, CodeTypeDto>();
            CreateMap<SystemCode, SystemCodeDto>();
            CreateMap<SystemCode, SystemCodeNestedDto>();
            CreateMap<User, UserDetailsDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.ProfileImageId.HasValue
                        ? $"/api/documents/{src.ProfileImageId}/content"
                        : null));
            CreateMap<Models.Session.Session, QUANTM.DTOs.Session.SessionDto>();

            // DTO to Entity mappings
            CreateMap<CodeTypeCreateDto, CodeType>();
            CreateMap<SystemCodeCreateDto, SystemCode>();
            CreateMap<SystemCodeUpdateDto, SystemCode>();
        }
    }
}