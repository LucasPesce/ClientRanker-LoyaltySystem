using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Client_Ranker.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigTableV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthlySpending",
                table: "Customers",
                newName: "LastMonthSpending");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentMonthSpending",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastClosedMonth = table.Column<int>(type: "INTEGER", nullable: false),
                    LastClosedYear = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropColumn(
                name: "CurrentMonthSpending",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "LastMonthSpending",
                table: "Customers",
                newName: "MonthlySpending");
        }
    }
}
