using AutoMapper;
using JapaneseTrainer.Api.DTOs.Dictionary;
using JapaneseTrainer.Api.DTOs.Grammar;
using JapaneseTrainer.Api.DTOs.Users;
using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Mapping
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            // User
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username ?? string.Empty))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            // Dictionary / Core learning entities
            CreateMap<Item, ItemDto>();
            CreateMap<DictionaryEntry, DictionaryEntryDto>();
            CreateMap<Kanji, KanjiDto>();
            CreateMap<ExampleSentence, ExampleSentenceDto>();
            CreateMap<Audio, AudioDto>();

            // Grammar
            CreateMap<GrammarMaster, GrammarMasterDto>();
            CreateMap<GrammarPackage, GrammarPackageDto>()
                .ForMember(dest => dest.Master, opt => opt.MapFrom(src => src.GrammarMaster));
        }
    }
}
