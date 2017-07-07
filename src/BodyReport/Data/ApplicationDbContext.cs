using Microsoft.EntityFrameworkCore;
using BodyReport.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BodyReport.Models;
using System;

namespace BodyReport.Data
{
    /// <summary>
    /// Read https://github.com/aspnet/EntityFramework/wiki/Configuring-a-DbContext
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(ApplicationDbContext));

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            var defaultDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
            bodyExerciseTable.Property(p => p.MuscleId).ValueGeneratedNever();
            bodyExerciseTable.Property(p => p.ExerciseCategoryType).ValueGeneratedNever();
            bodyExerciseTable.Property(p => p.ExerciseUnitType).ValueGeneratedNever();
            bodyExerciseTable.HasKey(s => new { s.Id });

            var muscleTable = builder.Entity<MuscleRow>().ToTable("Muscle");
            muscleTable.Property(p => p.Id).ValueGeneratedNever();
            muscleTable.HasKey(s => new { s.Id });

            var userInfoTable = builder.Entity<UserInfoRow>().ToTable("UserInfo");
            userInfoTable.Property(p => p.UserId).HasMaxLength(450).ValueGeneratedNever();
            userInfoTable.Property(p => p.ZipCode).HasMaxLength(80).ValueGeneratedNever();
            userInfoTable.Property(p => p.TimeZoneName).HasMaxLength(100).ValueGeneratedNever();
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

            var trainingWeekTable = builder.Entity<TrainingWeekRow>().ToTable("TrainingWeek");
            trainingWeekTable.Property(p => p.UserId).ValueGeneratedNever().HasMaxLength(450);
            trainingWeekTable.Property(p => p.Year).ValueGeneratedNever();
            trainingWeekTable.Property(p => p.WeekOfYear).ValueGeneratedNever();
            trainingWeekTable.Property(p => p.ModificationDate).HasDefaultValue(defaultDate);
            trainingWeekTable.HasKey(t => new { t.UserId, t.Year, t.WeekOfYear });

            var trainingDayTable = builder.Entity<TrainingDayRow>().ToTable("TrainingDay");
            trainingDayTable.Property(p => p.UserId).ValueGeneratedNever().HasMaxLength(450);
            trainingDayTable.Property(p => p.Year).ValueGeneratedNever();
            trainingDayTable.Property(p => p.WeekOfYear).ValueGeneratedNever();
            trainingDayTable.Property(p => p.DayOfWeek).ValueGeneratedNever();
            trainingDayTable.Property(p => p.TrainingDayId).ValueGeneratedNever();
            trainingDayTable.Property(p => p.BeginHour).HasDefaultValue(defaultDate);
            trainingDayTable.Property(p => p.EndHour).HasDefaultValue(defaultDate);
            trainingDayTable.Property(p => p.ModificationDate).HasDefaultValue(defaultDate);
            trainingDayTable.Property(p => p.Unit).ValueGeneratedNever();
            trainingDayTable.HasKey(t => new { t.UserId, t.Year, t.WeekOfYear, t.DayOfWeek, t.TrainingDayId });

            var trainingExerciseTable = builder.Entity<TrainingExerciseRow>().ToTable("TrainingExercise");
            trainingExerciseTable.Property(p => p.UserId).ValueGeneratedNever().HasMaxLength(450);
            trainingExerciseTable.Property(p => p.Year).ValueGeneratedNever();
            trainingExerciseTable.Property(p => p.WeekOfYear).ValueGeneratedNever();
            trainingExerciseTable.Property(p => p.DayOfWeek).ValueGeneratedNever();
            trainingExerciseTable.Property(p => p.TrainingDayId).ValueGeneratedNever();
            trainingExerciseTable.Property(p => p.Id).ValueGeneratedNever();
            trainingExerciseTable.Property(p => p.ModificationDate).HasDefaultValue(defaultDate);
            trainingExerciseTable.Property(p => p.ExerciseUnitType).ValueGeneratedNever();
            trainingExerciseTable.HasKey(t => new { t.UserId, t.Year, t.WeekOfYear, t.DayOfWeek, t.TrainingDayId, t.Id });

            var trainingExerciseSetTable = builder.Entity<TrainingExerciseSetRow>().ToTable("TrainingExerciseSet");
            trainingExerciseSetTable.Property(p => p.UserId).ValueGeneratedNever().HasMaxLength(450);
            trainingExerciseSetTable.Property(p => p.Year).ValueGeneratedNever();
            trainingExerciseSetTable.Property(p => p.WeekOfYear).ValueGeneratedNever();
            trainingExerciseSetTable.Property(p => p.DayOfWeek).ValueGeneratedNever();
            trainingExerciseSetTable.Property(p => p.TrainingDayId).ValueGeneratedNever();
            trainingExerciseSetTable.Property(p => p.TrainingExerciseId).ValueGeneratedNever();
            trainingExerciseSetTable.Property(p => p.Id).ValueGeneratedNever();
            trainingExerciseSetTable.Property(p => p.ModificationDate).HasDefaultValue(defaultDate);
            trainingExerciseSetTable.Property(p => p.ExecutionTime).ValueGeneratedNever();
            trainingExerciseSetTable.HasKey(t => new { t.UserId, t.Year, t.WeekOfYear, t.DayOfWeek, t.TrainingDayId, t.TrainingExerciseId, t.Id });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(WebAppConfiguration.TDataBaseServerType == Message.TDataBaseServerType.PostgreSQL)
                optionsBuilder.UseNpgsql(WebAppConfiguration.DatabaseConnectionString);
            else if (WebAppConfiguration.TDataBaseServerType == Message.TDataBaseServerType.SqlServer)
                optionsBuilder.UseSqlServer(WebAppConfiguration.DatabaseConnectionString);
            else
                _logger.LogError("Unknown database connection type");
        }

        public DbSet<TranslationRow> Translation { get; set; }
        public DbSet<MuscularGroupRow> MuscularGroup { get; set; }
        public DbSet<SequencerRow> Sequencer { get; set; }
        public DbSet<BodyExerciseRow> BodyExercise { get; set; }
        public DbSet<MuscleRow> Muscle { get; set; }
        public DbSet<UserInfoRow> UserInfo { get; set; }
        public DbSet<CityRow> City { get; set; }
        public DbSet<CountryRow> Country { get; set; }
        public DbSet<TrainingWeekRow> TrainingWeek { get; set; }
        public DbSet<TrainingDayRow> TrainingDay { get; set; }
        public DbSet<TrainingExerciseRow> TrainingExercise { get; set; }
        public DbSet<TrainingExerciseSetRow> TrainingExerciseSet { get; set; }
    }
}
