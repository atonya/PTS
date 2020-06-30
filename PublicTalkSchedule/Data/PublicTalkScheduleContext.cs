using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Models;

namespace PublicTalkSchedule.Data
{
    public class PublicTalkScheduleContext : DbContext
    {
        public PublicTalkScheduleContext (DbContextOptions<PublicTalkScheduleContext> options)
            : base(options)
        {
        }

        public DbSet<PublicTalkSchedule.Models.ScheduleOut> ScheduleOut { get; set; }

        public static implicit operator PublicTalkScheduleContext(ApplicationDbContext db)
        {
            throw new NotImplementedException();
        }
    }
}
