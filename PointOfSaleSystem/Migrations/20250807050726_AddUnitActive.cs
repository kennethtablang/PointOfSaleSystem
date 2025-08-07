using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointOfSaleSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Units",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Units");
        }
    }
}
