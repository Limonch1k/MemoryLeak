using Microsoft.EntityFrameworkCore;
using OnlineLab.Context.Models;
using Steema1.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steema1.Context
{
    public class PhysDataContext : DbContext
    {
        public PhysDataContext(DbContextOptions<PhysDataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public PhysDataContext() 
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PhysData> PhysDatas { get; set; }
        public DbSet<InitialParam> InitialParams { get; set; }
        public DbSet<ImagePresentation> Images { get; set; }        
        public DbSet<TaskExecution> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection = "Server=(localdb)\\mssqllocaldb;Database=AirFlowDb;Trusted_Connection=True;";
            optionsBuilder.UseSqlServer(connection);
        }
    }
}
