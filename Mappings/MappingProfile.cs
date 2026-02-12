using AutoMapper;
using QUANTM.DTOs.Common;
using QUANTM.DTOs.Parameter;
using QUANTM.DTOs.User;
using QUANTM.Models.Common;
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
            CreateMap<Address, AddressDto>();
            CreateMap<User, UserDetailsDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.ProfileImageId.HasValue
                        ? $"/api/documents/{src.ProfileImageId}/content"
                        : null))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Creator))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.Updater));

            CreateMap<User, UserNestedDto>();

            CreateMap<Models.Session.Session, QUANTM.DTOs.Session.SessionDto>();

            // DTO to Entity mappings
            CreateMap<CodeTypeCreateDto, CodeType>();
            CreateMap<SystemCodeCreateDto, SystemCode>();
            CreateMap<SystemCodeUpdateDto, SystemCode>();
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.IdNo, opt => opt.Ignore())
                .ForMember(dest => dest.Gender, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AddressUpdateDto, Address>()
                .ForMember(dest => dest.State, opt => opt.Ignore())
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}