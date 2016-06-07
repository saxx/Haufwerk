using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Haufwerk.Models;

namespace Haufwerk.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20160607071118_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Haufwerk.Models.Issue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInfo");

                    b.Property<DateTime>("CreationDateTime");

                    b.Property<bool>("Ignore");

                    b.Property<string>("Message");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("Source");

                    b.Property<string>("StackTrace");

                    b.Property<string>("User");

                    b.HasKey("Id");

                    b.ToTable("Issues");
                });
        }
    }
}
