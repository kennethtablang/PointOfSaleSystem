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

            CreateMap<UserCreateDto, ApplicationUser>();

            CreateMap<UserUpdateDto, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Session & Logs
            CreateMap<UserSession, UserSessionDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src =>
                    string.Join(" ", new[] { src.User.FirstName, src.User.MiddleName, src.User.LastName }
                        .Where(n => !string.IsNullOrWhiteSpace(n)))));

            CreateMap<SystemLog, SystemLogDto>()
                .ForMember(dest => dest.PerformedBy, opt => opt.MapFrom(src =>
                    src.User != null ? string.Join(" ", new[] { src.User.FirstName, src.User.MiddleName, src.User.LastName }
                        .Where(n => !string.IsNullOrWhiteSpace(n))) : null));

            CreateMap<LoginAttemptLog, LoginAttemptLogDto>();
        }
    }
}
