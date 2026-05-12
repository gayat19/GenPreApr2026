using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnderstandingEfCoreApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCustomerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "customers");
        }
    }
}
