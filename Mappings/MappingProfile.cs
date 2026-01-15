using AutoMapper;
using LMS.DTOs.Parameter;
using LMS.DTOs.User;
using LMS.Models.Parameter;
using LMS.Models.User;
namespace LMS.Mappings
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
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            // DTO to Entity mappings
            CreateMap<CodeTypeCreateDto, CodeType>();
            CreateMap<SystemCodeCreateDto, SystemCode>();
            CreateMap<SystemCodeUpdateDto, SystemCode>();
        }
    }
}