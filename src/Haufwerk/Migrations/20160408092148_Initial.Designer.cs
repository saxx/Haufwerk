using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Haufwerk.Models;

namespace Haufwerk.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20160408092148_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
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
                });
        }
    }
}
