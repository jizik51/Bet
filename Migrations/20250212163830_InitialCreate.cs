using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bets",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<int>(type: "integer", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        FirstTeame = table.Column<string>(type: "text", nullable: false),
                        SecondTeam = table.Column<string>(type: "text", nullable: false),
                        whoWin = table.Column<string>(type: "text", nullable: false),
                        gameResult = table.Column<string>(type: "text", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bets", x => x.id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Bets");
        }
    }
}
