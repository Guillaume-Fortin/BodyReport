using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using BodyReport.Models;

namespace BodyReport.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20151227210759__AddTrainingJournalTables")]
    partial class _AddTrainingJournalTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BodyReport.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("Suspended");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("BodyReport.Models.BodyExerciseRow", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("MuscleId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "BodyExercise");
                });

            modelBuilder.Entity("BodyReport.Models.CityRow", b =>
                {
                    b.Property<int>("CountryId");

                    b.Property<string>("ZipCode")
                        .HasAnnotation("MaxLength", 80);

                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 400);

                    b.HasKey("CountryId", "ZipCode", "Id");

                    b.HasAnnotation("Relational:TableName", "City");
                });

            modelBuilder.Entity("BodyReport.Models.CountryRow", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 400);

                    b.Property<string>("ShortName")
                        .HasAnnotation("MaxLength", 10);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Country");
                });

            modelBuilder.Entity("BodyReport.Models.MuscleRow", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("MuscularGroupId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Muscle");
                });

            modelBuilder.Entity("BodyReport.Models.MuscularGroupRow", b =>
                {
                    b.Property<int>("Id");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "MuscularGroup");
                });

            modelBuilder.Entity("BodyReport.Models.SequencerRow", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<int>("Value");

                    b.HasKey("Id", "Name");

                    b.HasAnnotation("Relational:TableName", "Sequencer");
                });

            modelBuilder.Entity("BodyReport.Models.TrainingJournalDayExerciseRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 450);

                    b.Property<int>("Year");

                    b.Property<int>("WeekOfYear");

                    b.Property<int>("DayOfWeek");

                    b.Property<int>("TrainingDayId");

                    b.Property<int>("BodyExerciseId");

                    b.Property<int>("NumberOfReps");

                    b.Property<int>("NumberOfSets");

                    b.HasKey("UserId", "Year", "WeekOfYear", "DayOfWeek", "TrainingDayId");

                    b.HasAnnotation("Relational:TableName", "TrainingJournalDayExercise");
                });

            modelBuilder.Entity("BodyReport.Models.TrainingJournalDayRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 450);

                    b.Property<int>("Year");

                    b.Property<int>("WeekOfYear");

                    b.Property<int>("DayOfWeek");

                    b.Property<int>("TrainingDayId");

                    b.Property<DateTime>("BeginHour");

                    b.Property<DateTime>("EndHour");

                    b.HasKey("UserId", "Year", "WeekOfYear", "DayOfWeek", "TrainingDayId");

                    b.HasAnnotation("Relational:TableName", "TrainingJournalDay");
                });

            modelBuilder.Entity("BodyReport.Models.TrainingJournalRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 450);

                    b.Property<int>("Year");

                    b.Property<int>("WeekOfYear");

                    b.Property<double>("UserHeight");

                    b.Property<double>("UserWeight");

                    b.HasKey("UserId", "Year", "WeekOfYear");

                    b.HasAnnotation("Relational:TableName", "TrainingJournal");
                });

            modelBuilder.Entity("BodyReport.Models.TranslationRow", b =>
                {
                    b.Property<int>("CultureId");

                    b.Property<string>("Key")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("Value")
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("CultureId", "Key");

                    b.HasAnnotation("Relational:TableName", "Translation");
                });

            modelBuilder.Entity("BodyReport.Models.UserInfoRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 450);

                    b.Property<int>("CountryId");

                    b.Property<double>("Height");

                    b.Property<int>("Sex");

                    b.Property<int>("Unit");

                    b.Property<double>("Weight");

                    b.Property<string>("ZipCode")
                        .HasAnnotation("MaxLength", 80);

                    b.HasKey("UserId");

                    b.HasAnnotation("Relational:TableName", "UserInfo");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("BodyReport.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("BodyReport.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("BodyReport.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
