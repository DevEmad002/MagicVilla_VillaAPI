using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        //first endpoint
        [HttpGet]
        public IEnumerable<VillaDTO> GetVillas() 
        {
            return VillaStore.villalist;

        }



        [HttpGet("{id:int}")]
        public VillaDTO GetVilla(int id)
        {
            return VillaStore.villalist.FirstOrDefault(v => v.Id == id);
        }


    }
}
