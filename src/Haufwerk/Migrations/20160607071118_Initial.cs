using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Haufwerk.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Issues",
                table => new
                {
                    Id = table.Column<Guid>(),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    CreationDateTime = table.Column<DateTime>(),
                    Ignore = table.Column<bool>(),
                    Message = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true),
                    User = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Issues");
        }
    }
}
