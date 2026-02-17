using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingRoomCampus.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaPeminjam = table.Column<string>(type: "TEXT", nullable: false),
                    Ruangan = table.Column<string>(type: "TEXT", nullable: false),
                    Tanggal = table.Column<DateTime>(type: "TEXT", nullable: false),
                    JamMulai = table.Column<string>(type: "TEXT", nullable: false),
                    JamSelesai = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}
