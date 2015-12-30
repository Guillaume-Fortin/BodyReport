using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Entity.Infrastructure;
using BodyReport.Framework;

namespace BodyReport.Models
{
    /// <summary>
    /// Read https://github.com/aspnet/EntityFramework/wiki/Configuring-a-DbContext
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //Define translation data table
            var transactionTable = builder.Entity<TranslationRow>().ToTable("Translation");
            transactionTable.Property(p => p.CultureId).ValueGeneratedNever();
            transactionTable.Property(p => p.Key).HasMaxLength(256).ValueGeneratedNever();
            transactionTable.Property(p => p.Value).HasMaxLength(2000);
            transactionTable.HasKey(s => new { s.CultureId, s.Key });

            var muscularGroupTable = builder.Entity<MuscularGroupRow>().ToTable("MuscularGroup");
            muscularGroupTable.Property(p => p.Id).ValueGeneratedNever();
            muscularGroupTable.HasKey(s => new { s.Id });

            var sequencerTable = builder.Entity<SequencerRow>().ToTable("Sequencer");
            sequencerTable.Property(p => p.Id).ValueGeneratedNever();
            sequencerTable.Property(p => p.Name).HasMaxLength(100).ValueGeneratedNever();
            sequencerTable.Property(p => p.Value);
            sequencerTable.HasKey(s => new { s.Id, s.Name });

            var bodyExerciseTable = builder.Entity<BodyExerciseRow>().ToTable("BodyExercise");
            bodyExerciseTable.Property(p => p.Id).ValueGeneratedNever();
            bodyExerciseTable.HasKey(s => new { s.Id });

            var muscleTable = builder.Entity<MuscleRow>().ToTable("Muscle");
            muscleTable.Property(p => p.Id).ValueGeneratedNever();
            muscleTable.HasKey(s => new { s.Id });

            var userInfoTable = builder.Entity<UserInfoRow>().ToTable("UserInfo");
            userInfoTable.Property(p => p.UserId).HasMaxLength(450).ValueGeneratedNever();
            userInfoTable.Property(p => p.ZipCode).HasMaxLength(80).ValueGeneratedNever();
            userInfoTable.HasKey(s => new { s.UserId });

            var cityTable = builder.Entity<CityRow>().ToTable("City");
            cityTable.Property(p => p.CountryId).ValueGeneratedNever();
            cityTable.Property(p => p.ZipCode).HasMaxLength(80).ValueGeneratedNever();
            cityTable.Property(p => p.Id).ValueGeneratedNever();
            cityTable.Property(p => p.Name).HasMaxLength(400).ValueGeneratedNever();
            cityTable.HasKey(c => new { c.CountryId, c.ZipCode, c.Id });

            var countryTable = builder.Entity<CountryRow>().ToTable("Country");
            countryTable.Property(p => p.Id).ValueGeneratedNever();
            countryTable.Property(p => p.Name).HasMaxLength(400);
            countryTable.Property(p => p.ShortName).HasMaxLength(10);
            countryTable.HasKey(c => new { c.Id });

            var trainingJournalTable = builder.Entity<TrainingWeekRow>().ToTable("TrainingWeek");
            trainingJournalTable.Property(p => p.UserId).ValueGeneratedNever().HasMaxLength(450);
            trainingJournalTable.Property(p => p.Year).ValueGeneratedNever();
            trainingJournalTable.Property(p => p.WeekOfYear).ValueGeneratedNever();
            trainingJournalTable.HasKey(t => new { t.UserId, t.Year, t.WeekOfYear });

            var trainingJournalDayTable = builder.Entity<TrainingDayRow>().ToTable("TrainingDay");
            trainingJournalDayTable.Property(p => p.UserId).ValueGeneratedNever().HasMaxLength(450);
            trainingJournalDayTable.Property(p => p.Year).ValueGeneratedNever();
            trainingJournalDayTable.Property(p => p.WeekOfYear).ValueGeneratedNever();
            trainingJournalDayTable.Property(p => p.DayOfWeek).ValueGeneratedNever();
            trainingJournalDayTable.Property(p => p.TrainingDayId).ValueGeneratedNever();
            trainingJournalDayTable.HasKey(t => new { t.UserId, t.Year, t.WeekOfYear, t.DayOfWeek, t.TrainingDayId });

            var trainingJournalDayExerciseTable = builder.Entity<TrainingExerciseRow>().ToTable("TrainingExercise");
            trainingJournalDayExerciseTable.Property(p => p.UserId).ValueGeneratedNever().HasMaxLength(450);
            trainingJournalDayExerciseTable.Property(p => p.Year).ValueGeneratedNever();
            trainingJournalDayExerciseTable.Property(p => p.WeekOfYear).ValueGeneratedNever();
            trainingJournalDayExerciseTable.Property(p => p.DayOfWeek).ValueGeneratedNever();
            trainingJournalDayExerciseTable.Property(p => p.TrainingDayId).ValueGeneratedNever();
            trainingJournalDayExerciseTable.Property(p => p.BodyExerciseId).ValueGeneratedNever();
            trainingJournalDayExerciseTable.HasKey(t => new { t.UserId, t.Year, t.WeekOfYear, t.DayOfWeek, t.TrainingDayId, t.BodyExerciseId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(WebAppConfiguration.DatabaseConnectionString);
        }

        public DbSet<TranslationRow> Translations { get; set; }
        public DbSet<MuscularGroupRow> MuscularGroups { get; set; }
        public DbSet<SequencerRow> Sequencers { get; set; }
        public DbSet<BodyExerciseRow> BodyExercises { get; set; }
        public DbSet<MuscleRow> Muscles { get; set; }
        public DbSet<UserInfoRow> UserInfos { get; set; }
        public DbSet<CityRow> Cities { get; set; }
        public DbSet<CountryRow> Countries { get; set; }
        public DbSet<TrainingWeekRow> TrainingWeeks { get; set; }
        public DbSet<TrainingDayRow> TrainingDays { get; set; }
        public DbSet<TrainingExerciseRow> TrainingExercises { get; set; }
    }
}
