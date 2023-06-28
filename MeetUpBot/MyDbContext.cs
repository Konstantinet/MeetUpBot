using MeetUpBot.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetUpBot
{
    class MyDbContext:DbContext
    {
        public MyDbContext()
        {
        //    Database.EnsureCreated();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=F:\\source\\repos\\MeetUpBot\\MeetUpBot\\sheduledb.mdf;Integrated Security=True");
		}
		public DbSet<User> Users  => Set<User>();
		public DbSet<MeetUp> MeetUps => Set<MeetUp>();
		public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    }
}
