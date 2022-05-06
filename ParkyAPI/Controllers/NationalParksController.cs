using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {

        //repository with DI
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly IMapper _autoMapper;


        public NationalParksController(INationalParkRepository nationalParkRepository, IMapper autoMapper)
        {
            _nationalParkRepository = nationalParkRepository;
            _autoMapper = autoMapper;
        }



        [HttpGet]
        public IActionResult GetNationalParks()
        {
            //para no exponer nuestros modelos usaremos el automapper para enseñar solo las DTOs
            var nationalParks = _nationalParkRepository.GetNationalParks();

            var nationalParksDto = nationalParks.Select(nationalPark => _autoMapper.Map<NationalParkDto>(nationalPark))
                .ToList();

            return (nationalParks.Count == 0)
                ? (IActionResult) NotFound()
                : Ok(nationalParksDto);
        }


        [HttpGet("{nationalParkId:int}" , Name = "GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var nationalPark = _nationalParkRepository.GetNationalPark(nationalParkId);

            return (nationalPark == null)
                ? (IActionResult) NotFound()
                : Ok(_autoMapper.Map<NationalParkDto>(nationalPark));
        }


        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
                return BadRequest(ModelState);

            if (_nationalParkRepository.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", $"El parque {nationalParkDto.Name} ya existe");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nationalPark = _autoMapper.Map<NationalPark>(nationalParkDto);
            ModelState.AddModelError("", $"Algo falló al intentar crear el parque {nationalParkDto.Name}");

            return (_nationalParkRepository.CreateNationalPark(nationalPark) == true)
                ? CreatedAtRoute("GetNationalPark", new {nationalparkId = nationalPark.Id}, nationalPark)
                : StatusCode(500, ModelState);

        }


        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || nationalParkId != nationalParkDto.Id)
                return BadRequest(ModelState);

            if (!_nationalParkRepository.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", $"El parque {nationalParkDto.Name} no existe!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //una vez tenemos las validaciones mapeamos el dto con la clase
            var nationalPark = _autoMapper.Map<NationalPark>(nationalParkDto);


            return (_nationalParkRepository.UpdateNationalPark(nationalPark))
                ? (IActionResult)NoContent()
                : StatusCode(500, ModelState);
        }

    }
}
