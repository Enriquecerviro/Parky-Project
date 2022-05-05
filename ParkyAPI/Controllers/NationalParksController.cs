using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {

        //repository with DI
        private INationalParkRepository _nationalParkRepository;
        private readonly IMapper _autoMapper;


        public NationalParksController(INationalParkRepository nationalParkRepository, IMapper autoMapper)
        {
            _nationalParkRepository = nationalParkRepository;
            _autoMapper = autoMapper;
        }


        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var nationalParks = _nationalParkRepository.getNationalParks();

            return Ok(nationalParks);
        }

    }
}
