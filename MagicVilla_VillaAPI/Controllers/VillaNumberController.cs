using AutoMapper;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Models.Dto.VillaNumber;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberController : ControllerBase
    {

            protected APIResponse _response;
            //private readonly ILogger<VillaAPIController> _logger;
            private readonly ILogging _logger;
            private readonly IVillaRepository _dbVilla;
            
            private readonly IVillaNumberRepository _dbVillaNumber;

            private readonly IMapper _mapper;

            public VillaNumberController(IVillaNumberRepository dbVillaNumber, ILogging logger, IMapper mapper , IVillaRepository dbVilla)
            {
                //_logger = logger; ILogger<VillaAPIController> logger
                _logger = logger;
                _dbVillaNumber = dbVillaNumber;
                _mapper = mapper;
                _dbVilla = dbVilla;
            this._response = new();
            }



            //first endpoint
            [HttpGet]
            public async Task<ActionResult<APIResponse>> GetVillasNumber()
            {
                try
                {
                    IEnumerable<VillaNumber> villaNumberlist  = await _dbVillaNumber.GetAllAsync();
                    _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberlist);
                    _response.StatusCode = HttpStatusCode.OK;
                    _logger.Log("Getting all Villas", "");
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



            [HttpGet("{id:int}", Name = "GetVillaNumber")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
            {
                try
                {
                    if (id == 0)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _logger.Log("Get Villa Error With Id " + id, "Error");
                        return BadRequest(_response);
                    }
                    var villaNumber = await _dbVillaNumber.GetAsync((v => v.VillaNo == id));

                    if (villaNumber == null)
                    {
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_response);
                    }

                    _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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
            public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
            {

                try
                {
                    if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
                    {
                        ModelState.AddModelError("CustomError", "Villa Number is Already Exists");
                        return BadRequest(ModelState);
                    }

                if (await _dbVilla.GetAsync(u => u.Id == createDTO.VillaID) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }
               
                    if (createDTO == null)
                    {
                        return BadRequest(createDTO);
                    }


                    VillaNumber model = _mapper.Map<VillaNumber>(createDTO);



                    await _dbVillaNumber.CreateAsync(model);

                    _response.Result = _mapper.Map<VillaNumberDTO>(model);

                    _response.StatusCode = HttpStatusCode.Created;

                    return CreatedAtRoute("GetVilla", new { id = model.VillaNo }, _response);
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
            [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
            public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
            {
                try
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }

                    var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);

                    if (villaNumber == null)
                    {

                        return NotFound();
                    }

                    await _dbVillaNumber.RemoveAsync(villaNumber);

                    _response.Result = _mapper.Map<VillaDTO>(villaNumber);
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
            [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
            public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
            {

                try
                {
                    if (updateDTO == null || id != updateDTO.VillaNo)
                    {
                        return BadRequest();
                    }


                if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }

                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);


                    await _dbVillaNumber.UpdateAsync(model);
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
            [HttpPatch("{id:int}", Name = "UpdatePartialNumberVilla")]
            public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
            {
                if (patchDTO == null || id == 0)
                {
                    return BadRequest();
                }

                var villa = await _dbVillaNumber.GetAsync(v => v.VillaNo == id, tracked: false);

                if (villa == null)
                {
                    return NotFound();
                }

                VillaNumberUpdateDTO villaNumberDTO = _mapper.Map<VillaNumberUpdateDTO>(villa);

                patchDTO.ApplyTo(villaNumberDTO, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _mapper.Map(villaNumberDTO, villa);

                await _dbVillaNumber.UpdateAsync(villa);


                return NoContent();
            }

        }
    }




