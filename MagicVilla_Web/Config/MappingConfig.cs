

using AutoMapper;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Models.Dto.VillaNumber;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.Dto.Villa;

namespace MagicVilla_Web.Config
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {

            CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
            CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();

            CreateMap<VillaNumberDTO , VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumberDTO , VillaNumberUpdateDTO>().ReverseMap();

        }
    }
}
