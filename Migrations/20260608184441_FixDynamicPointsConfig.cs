using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Client_Ranker.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicPointsConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiamondThreshold",
                table: "Configurations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldThreshold",
                table: "Configurations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PesosPorPunto",
                table: "Configurations",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PlatinumThreshold",
                table: "Configurations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SilverThreshold",
                table: "Configurations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MonthlySummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalTickets = table.Column<int>(type: "INTEGER", nullable: false),
                    UniqueCustomers = table.Column<int>(type: "INTEGER", nullable: false),
                    BronzeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SilverCount = table.Column<int>(type: "INTEGER", nullable: false),
                    GoldCount = table.Column<int>(type: "INTEGER", nullable: false),
                    PlatinumCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DiamondCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlySummaries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlySummaries");

            migrationBuilder.DropColumn(
                name: "DiamondThreshold",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "GoldThreshold",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "PesosPorPunto",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "PlatinumThreshold",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "SilverThreshold",
                table: "Configurations");
        }
    }
}
