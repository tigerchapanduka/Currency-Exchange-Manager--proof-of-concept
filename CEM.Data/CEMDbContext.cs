using CEM.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CEM.Data
{
    public class CEMDbContext : DbContext
    {
        public CEMDbContext(DbContextOptions<CEMDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpotRate>(
            eb =>
            {
                eb.Property(p => p.rates).HasConversion(
                 v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 v => JsonConvert.DeserializeObject<Rates>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                 );
            });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<SpotRate> ExchangeRate { get; set; }
    }
}
