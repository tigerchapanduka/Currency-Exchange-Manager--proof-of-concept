using CEM.Model;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEM.Data
{
    public class ExchangeRateData
    {
        CEMDbContext dbContext;
        public ExchangeRateData(CEMDbContext dbcontext)
        {
            dbContext = dbContext;
        }
        public bool Save(SpotRate spotRate)
        {
            dbContext.Add(spotRate);
            dbContext.SaveChanges();
            return true;
        }
    }
}
