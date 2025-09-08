using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YastCleaner.Data.Migrations
{
    /// <inheritdoc />
    public partial class addEmailAndClientTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TblCliente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "TblCliente");
        }
    }
}
