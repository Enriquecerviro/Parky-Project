using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParkyAPI.Models;

namespace ParkyAPI.Data
{
#pragma warning disable CS1591 // Falta el comentario XML para el tipo o miembro visible públicamente
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }

        public DbSet<NationalPark> NationalParks { get; set; }
    }
}
#pragma warning restore CS1591 // Falta el comentario XML para el tipo o miembro visible públicamente

