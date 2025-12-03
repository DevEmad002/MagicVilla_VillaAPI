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
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        protected APIResponse _response;
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
            this._response = new();
        }



        //first endpoint
        [HttpGet]
        public async Task< ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> villalist = await _dbVilla.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villalist);
                _response.StatusCode = HttpStatusCode.OK;
                _logger.Log("Getting all Villas", "");
                return Ok(_response);
            }
            catch (Exception ex) 
            {
                
            _response.IsSuccess= false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;
        }



        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task< ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    _logger.Log("Get Villa Error With Id " + id, "Error");
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync((v => v.Id == id));

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpPost]
        public async Task <ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {

            try
            {
                if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa is Already Exists");
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }


                Villa model = _mapper.Map<Villa>(createDTO);



                await _dbVilla.CreateAsync(model);

                _response.Result = _mapper.Map<VillaDTO>(model);

                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = model.Id }, _response);
            }


            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            } 
            return _response;

        }
            


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try { 
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

            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;

        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}" , Name ="UpdateVilla")]
        public async Task< ActionResult <APIResponse>> UpdateVilla(int id , [FromBody] VillaUpdateDTO updateDTO)
        {

            try
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
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;
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
