using AutoMapper;
using LMS.DTOs.Parameter;
using LMS.Models.Parameter;
namespace LMS.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<CodeType, CodeTypeDto>();
            CreateMap<SystemCode, SystemCodeDto>();

            // DTO to Entity mappings
            CreateMap<CodeTypeCreateDto, CodeType>();
            CreateMap<SystemCodeCreateDto, SystemCode>();
            CreateMap<SystemCodeUpdateDto, SystemCode>();
        }
    }
}