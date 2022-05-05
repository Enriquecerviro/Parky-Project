using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

            var nationalParksDto = nationalParks.Select(nationalPark => _autoMapper.Map<NationalParkDto>(nationalPark)).ToList();

            return (nationalParks.Count == 0)
                ? (IActionResult)NotFound()
                : Ok(nationalParksDto);
        }

        [HttpGet("{nationalParkId:int}")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var nationalPark = _nationalParkRepository.GetNationalPark(nationalParkId);

            return (nationalPark == null)
                ? (IActionResult) NotFound()
                : Ok(_autoMapper.Map<NationalParkDto>(nationalPark));
        }

    }
}
