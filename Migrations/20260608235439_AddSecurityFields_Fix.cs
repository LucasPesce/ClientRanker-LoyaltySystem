using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Client_Ranker.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityFields_Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecoveryPin",
                table: "Configurations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupervisorPassword",
                table: "Configurations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoveryPin",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "SupervisorPassword",
                table: "Configurations");
        }
    }
}
