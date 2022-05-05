using ParkyAPI.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ParkyAPI.Controllers;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;

namespace ParkyApi.Unit.Tests.ControllerTests
{

    public class NationalParksControllerTests : IDisposable
    {
        private readonly NationalParksController _nationalParksController;
        

        public void Dispose()
        {
            //
        }


        [Test]
        public async Task GetNationalParks_WhenCalled_ReturnTheTotalOfElements()
        {
            IActionResult actionResultTest = _nationalParksController.GetNationalParks();

            var okResult = actionResultTest as OkObjectResult;

            Assert.IsNotNull(okResult);
        }



    }
}
