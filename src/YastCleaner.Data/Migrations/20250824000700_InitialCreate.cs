using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YastCleaner.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblCliente",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroCelular = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCliente", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "TblServicio",
                columns: table => new
                {
                    ServicioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<double>(type: "float", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblServicio", x => x.ServicioId);
                });

            migrationBuilder.CreateTable(
                name: "TblUsuario",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblUsuario", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "TblPedido",
                columns: table => new
                {
                    PedidoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoPedido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    MontoAdelantado = table.Column<double>(type: "float", nullable: false),
                    Vuelto = table.Column<double>(type: "float", nullable: false),
                    MontoTotal = table.Column<double>(type: "float", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblPedido", x => x.PedidoId);
                    table.ForeignKey(
                        name: "FK_TblPedido_TblCliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "TblCliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblPedido_TblUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TblUsuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblReporte",
                columns: table => new
                {
                    ReporteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    MontoGenerado = table.Column<double>(type: "float", nullable: false),
                    FechaReporte = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblReporte", x => x.ReporteId);
                    table.ForeignKey(
                        name: "FK_TblReporte_TblUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TblUsuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblDetallePedido",
                columns: table => new
                {
                    DetallePedidoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    CodigoPedido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<double>(type: "float", nullable: false),
                    SubTotal = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDetallePedido", x => x.DetallePedidoId);
                    table.ForeignKey(
                        name: "FK_TblDetallePedido_TblPedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TblPedido",
                        principalColumn: "PedidoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblDetallePedido_TblServicio_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "TblServicio",
                        principalColumn: "ServicioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblPedidoAnulado",
                columns: table => new
                {
                    PedidoAnuladoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblPedidoAnulado", x => x.PedidoAnuladoId);
                    table.ForeignKey(
                        name: "FK_TblPedidoAnulado_TblPedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TblPedido",
                        principalColumn: "PedidoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblPedidoEntregado",
                columns: table => new
                {
                    PedidoEntregadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblPedidoEntregado", x => x.PedidoEntregadoId);
                    table.ForeignKey(
                        name: "FK_TblPedidoEntregado_TblPedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TblPedido",
                        principalColumn: "PedidoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblDetallePedido_PedidoId",
                table: "TblDetallePedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TblDetallePedido_ServicioId",
                table: "TblDetallePedido",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPedido_ClienteId",
                table: "TblPedido",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPedido_UsuarioId",
                table: "TblPedido",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPedidoAnulado_PedidoId",
                table: "TblPedidoAnulado",
                column: "PedidoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblPedidoEntregado_PedidoId",
                table: "TblPedidoEntregado",
                column: "PedidoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblReporte_UsuarioId",
                table: "TblReporte",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblDetallePedido");

            migrationBuilder.DropTable(
                name: "TblPedidoAnulado");

            migrationBuilder.DropTable(
                name: "TblPedidoEntregado");

            migrationBuilder.DropTable(
                name: "TblReporte");

            migrationBuilder.DropTable(
                name: "TblServicio");

            migrationBuilder.DropTable(
                name: "TblPedido");

            migrationBuilder.DropTable(
                name: "TblCliente");

            migrationBuilder.DropTable(
                name: "TblUsuario");
        }
    }
}
