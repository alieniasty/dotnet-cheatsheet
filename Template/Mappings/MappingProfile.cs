namespace MassTransitSample.Template.Mappings
{
    using AutoMapper;
    using DomainModels;
    using Newtonsoft.Json;
    using ViewModels;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CommandSomethingViewModel, CommandSomethingModel>()
                .ForMember(dest => dest.TextDomainInfo, opt => opt.MapFrom(src => src.StringField))
                .ForMember(dest => dest.NumberDomainInfo, opt => opt.MapFrom(src => src.IntField))
                .ForMember(dest => dest.SomeObject, opt =>
                    {
                        opt.MapFrom(src => JsonConvert.SerializeObject(src.SomeObject));
                    });

            CreateMap<ImportantProcessingViewModel, ImportantProcessingModel>()
                .ForMember(dest => dest.TextDomainInfo, opt => opt.MapFrom(src => src.StringField))
                .ForMember(dest => dest.NumberDomainInfo, opt => opt.MapFrom(src => src.IntField))
                .ForMember(dest => dest.SomeObject, opt =>
                    {
                        opt.MapFrom(src => JsonConvert.SerializeObject(src.SomeObject));
                    });
        }
    }
}
