using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        //private readonly ILogger<VillaAPIController> _logger;
        private readonly ILogging _logger;

        private readonly IVillaRepository _dbVilla;

        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, ILogging logger , IMapper mapper)
        {
            //_logger = logger; ILogger<VillaAPIController> logger
            _logger = logger;
            _dbVilla=dbVilla;

            _mapper = mapper;

        }






        //first endpoint
        [HttpGet]
        public async Task< ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa>villalist=await _dbVilla.GetAllAsync();
            _logger.Log("Getting all Villas" , "");
            return Ok(_mapper.Map<List<VillaDTO>>(villalist));

        }



        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task< ActionResult<VillaDTO>> GetVilla(int id)
        { 

            if (id == 0)
            {
                _logger.Log("Get Villa Error With Id " + id, "Error");
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync((v => v.Id == id));

            if (villa == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<VillaDTO>(villa));
        }


        [HttpPost]
        public async Task <ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            //if (!ModelState.IsValid) 
            //{
            //    return BadRequest(ModelState);
            //}

            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa is Already Exists");
                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            //if (villaDTO.Id > 0)
            //{

            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            //villaDTO.Id = _db.Villas.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            //VillaStore.villalist.Add(villaDTO);
            Villa model = _mapper.Map<Villa>(createDTO);

            //Villa model = new Villa()
            //{
            //    Name = createDTO.Name,
            //    Sqft = createDTO.Sqft,
            //    Occupancy = createDTO.Occupancy,
            //    Rate = createDTO.Rate,
            //    Details = createDTO.Details,
            //    Amenity = createDTO.Amenity,
            //    ImageUrl = createDTO.ImageUrl,
            //    CreateDate = DateTime.Now,
            //    UpdateDate = DateTime.Now
            //};

            await _dbVilla.CreateAsync(model);


            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);

        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id);
           
            if (villa == null)
            {

                return NotFound();
            }

            await _dbVilla.RemoveAsync(villa);
            return NoContent();


        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}" , Name ="UpdateVilla")]
        public async Task< IActionResult> UpdateVilla(int id , [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id) 
            {
                return BadRequest();
            }

            //var villa = VillaStore.villalist.FirstOrDefault(v => v.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Occupancy = villaDTO.Occupancy;
            //villa.Sqft = villaDTO.Sqft;
            
            Villa model = _mapper.Map<Villa>(updateDTO);

            //Villa model = new Villa()
            //{
            //    Name = villaDTO.Name,
            //    Sqft = villaDTO.Sqft,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Details = villaDTO.Details,
            //    Amenity = villaDTO.Amenity,
            //    ImageUrl = villaDTO.ImageUrl,
            //    CreateDate = DateTime.Now,
            //    UpdateDate = DateTime.Now
            //};
            await _dbVilla.UpdateAsync(model);
            return NoContent();
        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id , tracked : false);

            if (villa == null)
            {
                return NotFound();
            }

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            patchDTO.ApplyTo(villaDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(villaDTO, villa);

            await _dbVilla.UpdateAsync(villa);


            return NoContent();
        }

    }
}
