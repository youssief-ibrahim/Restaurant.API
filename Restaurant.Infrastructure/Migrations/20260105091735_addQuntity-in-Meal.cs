using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addQuntityinMeal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Meals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Meals");
        }
    }
}
