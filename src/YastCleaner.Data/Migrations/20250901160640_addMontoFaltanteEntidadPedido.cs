using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YastCleaner.Data.Migrations
{
    /// <inheritdoc />
    public partial class addMontoFaltanteEntidadPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Vuelto",
                table: "TblPedido",
                newName: "MontoFaltante");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "TblPedido",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "TblPedido");

            migrationBuilder.RenameColumn(
                name: "MontoFaltante",
                table: "TblPedido",
                newName: "Vuelto");
        }
    }
}
