using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        //first endpoint
        [HttpGet]
        public IEnumerable<Villa> GetVillas() 
        {
            return new List<Villa>
            {
                new Villa { Id = 1, Name = "Royal Villa" },
                new Villa { Id = 2, Name = "Premium Pool Villa" }
            };

        }


    }
}
