using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Models;

namespace PublicTalkSchedule.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Congregation> Congregation { get; set; }
        public DbSet<TalkList> TalkList { get; set; }
        public DbSet<TalkCategory> TalkCategory { get; set; }


        public DbSet<Speaker> Speaker { get; set; }
        public DbSet<SpeakerTalk> SpeakerTalk { get; set; }
        public DbSet<TalkShoppingCart> TalkShoppingCart { get; set; }


        public DbSet<ScheduleIn> ScheduleIn { get; set; }
        public DbSet<ScheduleOut> ScheduleOut { get; set; }


        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
