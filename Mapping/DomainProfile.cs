using AutoMapper;
using JapaneseTrainer.Api.DTOs.AI;
using JapaneseTrainer.Api.DTOs.Community;
using JapaneseTrainer.Api.DTOs.Dashboard;
using JapaneseTrainer.Api.DTOs.Dictionary;
using JapaneseTrainer.Api.DTOs.Exercises;
using JapaneseTrainer.Api.DTOs.Grammar;
using JapaneseTrainer.Api.DTOs.Packages;
using JapaneseTrainer.Api.DTOs.Study;
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

            // Package/Lesson
            CreateMap<Package, PackageDto>();
            CreateMap<Lesson, LessonDto>();

            // Exercise
            CreateMap<Exercise, ExerciseDto>();

            // Study / SRS
            CreateMap<StudyProgress, StudyProgressDto>();
            CreateMap<ReviewSession, ReviewSessionDto>();

            // Community
            CreateMap<Comment, CommentDto>();
            CreateMap<Report, ReportDto>();

            // Dashboard (map StudyProgress -> short queue item, difficult item sẽ map thủ công vì cần priority)
            CreateMap<StudyProgress, StudyQueueItemShortDto>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemId))
                .ForMember(dest => dest.Japanese, opt => opt.MapFrom(src => src.Item.Japanese))
                .ForMember(dest => dest.Meaning, opt => opt.MapFrom(src => src.Item.Meaning))
                .ForMember(dest => dest.Skill, opt => opt.MapFrom(src => src.Skill))
                .ForMember(dest => dest.Stage, opt => opt.MapFrom(src => src.Stage))
                .ForMember(dest => dest.NextReviewAt, opt => opt.MapFrom(src => src.NextReviewAt));

            // AI Queue
            CreateMap<AIQueue, AIJobDto>();
        }
    }
}
