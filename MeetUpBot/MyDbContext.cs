using MeetUpBot.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetUpBot
{
    class MyDbContext:DbContext
    {
        public MyDbContext()
        {
            
            Database.EnsureCreated();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=F:\\source\\repos\\MeetUpBot\\MeetUpBot\\sheduledb.mdf;Integrated Security=True");
		}
		public DbSet<User> Users  => Set<User>();
		public DbSet<MeetUp> MeetUps => Set<MeetUp>();
		public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<MeetUp>()
                .HasMany(c => c.Participants)
                .WithMany(s => s.Meetings)
                .UsingEntity<Invitation>(
                   j => j
                    .HasOne(pt => pt.User)
                    .WithMany(t => t.Invitations)
                    .HasForeignKey(pt => pt.UserId),
                j => j
                    .HasOne(pt => pt.MeetUp)
                    .WithMany(p => p.Invitations)
                    .HasForeignKey(pt => pt.MeetUpId),
                j =>
                {
                    j.Property(pt => pt.TimeApproved).HasDefaultValue(false);
                    j.HasKey(t => new { t.MeetUpId, t.UserId });
                    j.ToTable("Invitations");
                });
        }
    }
}
