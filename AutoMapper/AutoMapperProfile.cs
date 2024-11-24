using AutoMapper;
using RecycleBuild.API.DTO;
using RecycleBuild.API.Models;

namespace RecycleBuild.API.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, PerfilUsuarioDto>();
            CreateMap<LixoReciclado, LixoRecicladoDto>().ReverseMap();

            CreateMap<LixoReciclado, TopDoadoresDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Peso))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => (int)src.Peso));
        }
    }
}
