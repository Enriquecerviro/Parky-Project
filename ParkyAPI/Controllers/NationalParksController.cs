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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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


        /// <summary>
        /// Get lista de parques nacionales
        /// </summary>
        /// <returns>Lista parques nacionales</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NationalParkDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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


        /// <summary>
        /// Devuelve el parque que se corresponda al id
        /// </summary>
        /// <param name="nationalParkId">Id del parque nacional</param>
        /// <returns>paque nacional</returns>
        [HttpGet("{nationalParkId:int}" , Name = "GetNationalPark")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var nationalPark = _nationalParkRepository.GetNationalPark(nationalParkId);

            return (nationalPark == null)
                ? (IActionResult) NotFound()
                : Ok(_autoMapper.Map<NationalParkDto>(nationalPark));
        }


        /// <summary>
        /// Crea un nuevo parque nacional
        /// </summary>
        /// <param name="nationalParkDto">DTO del parque nacional a insertar</param>
        /// <returns>el parque nacional recien creado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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


        /// <summary>
        /// Modifica un parque nacional actual
        /// </summary>
        /// <param name="nationalParkId">Parque nacional a modificar</param>
        /// <param name="nationalParkDto">nuevos valores para el esquema dto</param>
        /// <returns></returns>
        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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


        /// <summary>
        /// Borra un parque nacional
        /// </summary>
        /// <param name="nationalParkId">Id del parque nacional a borrar</param>
        /// <returns></returns>
        [HttpDelete("{nationalParkId:int}", Name = "NationalNationalPark")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {

            if (!_nationalParkRepository.NationalParkExists(nationalParkId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //al borrar no necesitamos nada del dto, solo localizar en el repo el recurso a borrar
            var nationalPark = _nationalParkRepository.GetNationalPark(nationalParkId);


            return (_nationalParkRepository.DeleteNationalPark(nationalPark))
                ? (IActionResult)NoContent()
                : StatusCode(500, ModelState);
        }






    }
}
