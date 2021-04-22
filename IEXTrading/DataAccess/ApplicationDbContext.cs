using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IEXTrading.Models;

namespace IEXTrading.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Equity> Equities { get; set; }

        public DbSet<Companys> CompCompanies { get; set; }
        public DbSet<customer> Customers { get; set; }
        public DbSet<DoNotCallComplaints> Complaints { get; set; }

        public DbSet<NoRoboComplaints> Robocalls { get; set; }

    }
}
