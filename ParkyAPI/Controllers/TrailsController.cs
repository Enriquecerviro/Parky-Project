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
    public class TrailsController : ControllerBase
    {

        //repository with DI
        private readonly IMapper _autoMapper;
        private readonly ITrailRepository _trailRepository;


        public TrailsController(ITrailRepository trailRepository, IMapper autoMapper)
        {
            _trailRepository = trailRepository;
            _autoMapper = autoMapper;
        }


        /// <summary>
        /// Get lista de parques nacionales
        /// </summary>
        /// <returns>Lista parques nacionales</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrailDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTrails()
        {
            //para no exponer nuestros modelos usaremos el automapper para enseñar solo las DTOs
            var trails = _trailRepository.GetTrails();

            var trailsDto = trails.Select(trail => _autoMapper.Map<TrailDto>(trail))
                .ToList();

            return (trails.Count == 0)
                ? (IActionResult)NotFound()
                : Ok(trailsDto);
        }


        /// <summary>
        /// Devuelve el parque que se corresponda al id
        /// </summary>
        /// <param name="trailId">Id del parque nacional</param>
        /// <returns>paque nacional</returns>
        [HttpGet("{TrailId:int}", Name = "GetTrail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTrail(int trailId)
        {
            var trail = _trailRepository.GetTrail(trailId);

            //LA IMPORTANCIA DE AUTOMAPPER: si no lo usásemos, habría que hacer una aproach mas menos así:
            //var TrailDto = new TrailDto()
            //{
            //    Created = Trail.Crates,
            //    Established = Trail.Established,
            //    Id = Trail.Id,
            //    Name = Trail.Name
            //};

            return (trail == null)
                ? (IActionResult)NotFound()
                : Ok(_autoMapper.Map<TrailDto>(trail));
        }


        /// <summary>
        /// Crea un nuevo parque nacional
        /// </summary>
        /// <param name="trailUpsertDto">DTO del trail a insertar</param>
        /// <returns>el parque nacional recien creado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrailUpsertDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailUpsertDto trailUpsertDto)
        {
            if (trailUpsertDto == null)
                return BadRequest(ModelState);

            if (_trailRepository.TrailExists(trailUpsertDto.Name))
            {
                ModelState.AddModelError("", $"El trail {trailUpsertDto.Name} ya existe");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trail = _autoMapper.Map<Trail>(trailUpsertDto);
            ModelState.AddModelError("", $"Algo falló al intentar crear el trail {trailUpsertDto.Name}");

            return (_trailRepository.CreateTrail(trail) == true)
                ? CreatedAtRoute("GetTrail", new { TrailId = trail.Id }, trail)
                : StatusCode(500, ModelState);

        }


        /// <summary>
        /// Modifica un parque nacional actual
        /// </summary>
        /// <param name="trailId">Parque nacional a modificar</param>
        /// <param name="trailUpsertDto">nuevos valores para el esquema dto</param>
        /// <returns></returns>
        [HttpPatch("{TrailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailUpsertDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpsertDto trailUpsertDto)
        {
            if (trailUpsertDto == null || trailId != trailUpsertDto.Id)
                return BadRequest(ModelState);

            if (!_trailRepository.TrailExists(trailUpsertDto.Name))
            {
                ModelState.AddModelError("", $"El parque {trailUpsertDto.Name} no existe!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //una vez tenemos las validaciones mapeamos el dto con la clase
            var trail = _autoMapper.Map<Trail>(trailUpsertDto);


            return (_trailRepository.UpdateTrail(trail))
                ? (IActionResult)NoContent()
                : StatusCode(500, ModelState);
        }


        /// <summary>
        /// Borra un parque nacional
        /// </summary>
        /// <param name="trailId">Id del parque nacional a borrar</param>
        /// <returns></returns>
        [HttpDelete("{TrailId:int}", Name = "Trail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {

            if (!_trailRepository.TrailExists(trailId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //al borrar no necesitamos nada del dto, solo localizar en el repo el recurso a borrar
            var Trail = _trailRepository.GetTrail(trailId);


            return (_trailRepository.DeleteTrail(Trail))
                ? (IActionResult)NoContent()
                : StatusCode(500, ModelState);
        }






    }
}
