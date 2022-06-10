using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineLab.Migrations
{
    public partial class fixDaatabese : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    frame = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartStatus = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordConfirm = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InitialParams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    x0 = table.Column<double>(type: "float", nullable: false),
                    nju = table.Column<double>(type: "float", nullable: false),
                    dif = table.Column<double>(type: "float", nullable: false),
                    dt = table.Column<double>(type: "float", nullable: false),
                    dx = table.Column<double>(type: "float", nullable: false),
                    v0 = table.Column<double>(type: "float", nullable: false),
                    rs = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitialParams_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhysDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Shot = table.Column<int>(type: "int", nullable: false),
                    lab3au1string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3au2string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3av1string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3av2string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3aPsi1string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3aPsi2string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3aVi1string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3aVi2string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3aNd1string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3aNd2string = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lab3afstring = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysDatas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InitialParams_UserId",
                table: "InitialParams",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysDatas_UserId",
                table: "PhysDatas",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "InitialParams");

            migrationBuilder.DropTable(
                name: "PhysDatas");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
