using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parcial2DDA.Migrations
{
    /// <inheritdoc />
    public partial class cambioReporte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "diferenciaTiempo",
                table: "Reportes",
                newName: "DiferenciaTiempo");

            migrationBuilder.RenameColumn(
                name: "diferenciaPeso",
                table: "Reportes",
                newName: "DiferenciaPeso");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiferenciaTiempo",
                table: "Reportes",
                newName: "diferenciaTiempo");

            migrationBuilder.RenameColumn(
                name: "DiferenciaPeso",
                table: "Reportes",
                newName: "diferenciaPeso");
        }
    }
}
