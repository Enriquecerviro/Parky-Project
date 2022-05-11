using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {

        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails.ToList();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int npId)
        {
            return _db.Trails.Include(c => c.NationalPark).Where(x => x.NationalParkId == npId).ToList();
        }

        public Trail GetTrail(int trailId)
        {
            return _db.Trails.FirstOrDefault(x => x.Id == trailId);
        }

        public bool TrailExists(string name)
        {
            return _db.Trails.Any(x => x.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool TrailExists(int id)
        {
            return _db.Trails.Any(x => x.Id == id);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }

        #region CRUD
        public bool CreateTrail(Trail trail)
        {
            _db.Trails.Add(trail);
            return Save();
        }

        public bool UpdateTrail(Trail trail)
        {
            _db.Trails.Update(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _db.Trails.Remove(trail);
            return Save();
        }

        #endregion

    }
}
