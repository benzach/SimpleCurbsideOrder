using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Dat.Domain
{
    public class UserProfileProfile:Profile
    {
        public UserProfileProfile()
        {
            CreateMap<UserProfileDTO, UserProfile>()
                .ForMember(
                dest => dest.Claims, 
                opt => opt.MapFrom(dto =>new DatClaim() { 
                    address= dto.Claims.FirstOrDefault(x => x.Type == nameof(DatClaim.address)).Value +"",
                    company = dto.Claims.FirstOrDefault(x => x.Type == nameof(DatClaim.company)).Value +"",
                    family_name = dto.Claims.FirstOrDefault(x => x.Type == nameof(DatClaim.family_name)).Value+"",
                    given_name = dto.Claims.FirstOrDefault(x => x.Type == nameof(DatClaim.given_name)).Value+"",
                    roles = dto.Claims.Where(x => x.Type == nameof(DatClaim.roles)).Select(x=>x.Value).ToList()
                }));
            CreateMap<UserProfile, UserProfileEdit>()
                .ForMember(
                    nameof(UserProfileEdit.isStoreOwner),
                    opt => opt.MapFrom(dm => dm.Claims.roles.Any(x => x.Equals("Store Owner", StringComparison.OrdinalIgnoreCase))))
                .ForMember(
                    nameof(UserProfileEdit.isAdmin),
                    opt => opt.MapFrom(dm => dm.Claims.roles.Any(x => x.Equals("Admin", StringComparison.OrdinalIgnoreCase))))
                .ForMember(
                    nameof(UserProfileEdit.isCustomer),
                    opt => opt.MapFrom(dm => dm.Claims.roles.Any(x => x.Equals("Customer", StringComparison.OrdinalIgnoreCase))));

            CreateMap<UserProfileEdit, UserProfile>()
                .ForMember(
                    dest => dest.Claims,
                    opt => opt.MapFrom((dedit, dm) => {
                        if (dedit.Claims != null)
                        {
                            var claims = new DatClaim();

                            claims = dedit.Claims;
                            List<string> roles = new List<string>();
                            if (dedit.isAdmin)
                            {
                                roles.Add("Admin");
                            }
                            if (dedit.isCustomer)
                            {
                                roles.Add("Customer");
                            }
                            if (dedit.isStoreOwner)
                            {
                                roles.Add("Store Owner");
                            }
                            claims.roles = roles;
                            return claims;
                        }
                        return null;
                    }
                ));
            CreateMap<UserProfile, UserProfileDTO>()
                .ForMember(
                    dest=>dest.Claims,
                    opt=> opt.MapFrom((dm,dto)=> {
                        var a = new List<Claim>();
                        if(!string.IsNullOrEmpty(dm.Claims.address))
                        {
                            a.Add(new Claim(nameof(dm.Claims.address), dm.Claims.address));
                        }
                            if (!string.IsNullOrEmpty(dm.Claims.company))
                        {
                            a.Add(new Claim(nameof(dm.Claims.company), dm.Claims.company));
                        }
                        if (!string.IsNullOrEmpty(dm.Claims.family_name))
                        {
                            a.Add(new Claim(nameof(dm.Claims.family_name), dm.Claims.family_name));
                        }
                        if (!string.IsNullOrEmpty(dm.Claims.given_name))
                        {
                            a.Add(new Claim(nameof(dm.Claims.given_name), dm.Claims.given_name));
                        }
                        a.AddRange(dm.Claims.roles.Where(x=>!string.IsNullOrEmpty(x)).Select(x => new Claim("role", x)).ToArray());
                        return a;
                    })
                );
        }
    }
}
