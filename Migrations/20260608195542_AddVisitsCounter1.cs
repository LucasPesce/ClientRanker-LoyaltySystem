using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Client_Ranker.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitsCounter1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentMonthVisits",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentMonthVisits",
                table: "Customers");
        }
    }
}
