//AutoMapper
using AutoMapper;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Models.Auth;

namespace PointOfSaleSystem.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            // User mappings
            CreateMap<ApplicationUser, UserReadDto>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src =>
                               string.Join(" ", new[] { src.FirstName, src.MiddleName, src.LastName }
                                   .Where(n => !string.IsNullOrWhiteSpace(n)))));

            // Map UserCreateDto → ApplicationUser, including Role
            CreateMap<UserCreateDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<UserUpdateDto, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Session & Logs
            CreateMap<UserSession, UserSessionDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src =>
                    string.Join(" ", new[] { src.User.FirstName, src.User.MiddleName, src.User.LastName }
                        .Where(n => !string.IsNullOrWhiteSpace(n)))));

            // In your AutoMapper profile (e.g., SupplierProfile or a dedicated AuthProfile)
            CreateMap<SystemLog, SystemLogDto>()
                .ForMember(dest => dest.PerformedBy, opt => opt.MapFrom(src =>
                    src.User != null
                        ? string.Join(" ", new[] { src.User.FirstName, src.User.MiddleName, src.User.LastName }
                            .Where(n => !string.IsNullOrWhiteSpace(n)))
                        : null))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.DataBefore, opt => opt.MapFrom(src => src.DataBefore))
                .ForMember(dest => dest.DataAfter, opt => opt.MapFrom(src => src.DataAfter))
                .ForMember(dest => dest.Module, opt => opt.MapFrom(src => src.Module))
                .ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => src.ActionType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IPAddress, opt => opt.MapFrom(src => src.IPAddress));

            CreateMap<LoginAttemptLog, LoginAttemptLogDto>();

            CreateMap<UserSessionDto, UserSession>();
            CreateMap<SystemLogDto, SystemLog>();
            CreateMap<LoginAttemptLogDto, LoginAttemptLog>();
        }
    }
}
