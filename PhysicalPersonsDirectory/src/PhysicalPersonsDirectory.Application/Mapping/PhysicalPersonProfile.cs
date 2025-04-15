using AutoMapper;
using PhysicalPersonsDirectory.Application.DTOs;
using PhysicalPersonsDirectory.Domain;

namespace PhysicalPersonsDirectory.Application.Mapping;

public class PhysicalPersonProfile : Profile
{
    public PhysicalPersonProfile()
    {
        CreateMap<PhysicalPerson, PhysicalPersonDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers))
            .ForMember(dest => dest.RelatedPersons, opt => opt.MapFrom(src => src.RelatedPersons));

        CreateMap<PhoneNumber, PhoneNumberDto>();
        CreateMap<RelatedPerson, RelatedPersonDto>();
    }
}