using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LakoDoStana.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(maxLength: 20, nullable: false),
                    Prezime = table.Column<string>(maxLength: 20, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    DatumRodjenja = table.Column<DateTime>(nullable: false),
                    DatumKreiranjaNaloga = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(maxLength: 30, nullable: false),
                    Password = table.Column<string>(maxLength: 30, nullable: false),
                    Pol = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    BrojObrisanihNaloga = table.Column<int>(nullable: true),
                    BrojPostavljenihOglasa = table.Column<int>(nullable: true),
                    BrojUkupnihPregleda = table.Column<int>(nullable: true),
                    BrojPregledanihOglasa = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Oglasi",
                columns: table => new
                {
                    OglasId = table.Column<int>(nullable: false),
                    Grad = table.Column<string>(maxLength: 20, nullable: false),
                    Adresa = table.Column<string>(maxLength: 30, nullable: false),
                    Cena = table.Column<int>(nullable: false),
                    Kvadratura = table.Column<int>(nullable: false),
                    BrojSoba = table.Column<int>(nullable: false),
                    TipObjekta = table.Column<int>(nullable: false),
                    Opis = table.Column<string>(maxLength: 2000, nullable: false),
                    DatumObjavljivanja = table.Column<DateTime>(nullable: false),
                    BrojPregleda = table.Column<int>(nullable: false),
                    TipOglasa = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oglasi", x => x.OglasId);
                });

            migrationBuilder.CreateTable(
                name: "Poruke",
                columns: table => new
                {
                    PorukaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PosiljalacId = table.Column<int>(nullable: false),
                    PrimalacId = table.Column<int>(nullable: false),
                    DatumSlanja = table.Column<DateTime>(nullable: false),
                    Tekst = table.Column<string>(nullable: false),
                    Seen = table.Column<bool>(nullable: false),
                    AdministratorID = table.Column<int>(nullable: true),
                    OglasivacID = table.Column<int>(nullable: true),
                    PosetilacID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poruke", x => x.PorukaId);
                    table.ForeignKey(
                        name: "FK_Poruke_Korisnici_AdministratorID",
                        column: x => x.AdministratorID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Poruke_Korisnici_OglasivacID",
                        column: x => x.OglasivacID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Poruke_Korisnici_PosetilacID",
                        column: x => x.PosetilacID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Poruke_Korisnici_PosiljalacId",
                        column: x => x.PosiljalacId,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KorisniciOglasi",
                columns: table => new
                {
                    KorisnikOglasId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(nullable: false),
                    OglasId = table.Column<int>(nullable: false),
                    Datum = table.Column<DateTime>(nullable: false),
                    TipVeze = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisniciOglasi", x => x.KorisnikOglasId);
                    table.ForeignKey(
                        name: "FK_KorisniciOglasi_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorisniciOglasi_Oglasi_OglasId",
                        column: x => x.OglasId,
                        principalTable: "Oglasi",
                        principalColumn: "OglasId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KorisniciOglasi_KorisnikId",
                table: "KorisniciOglasi",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_KorisniciOglasi_OglasId",
                table: "KorisniciOglasi",
                column: "OglasId");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_AdministratorID",
                table: "Poruke",
                column: "AdministratorID");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_OglasivacID",
                table: "Poruke",
                column: "OglasivacID");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_PosetilacID",
                table: "Poruke",
                column: "PosetilacID");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_PosiljalacId",
                table: "Poruke",
                column: "PosiljalacId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KorisniciOglasi");

            migrationBuilder.DropTable(
                name: "Poruke");

            migrationBuilder.DropTable(
                name: "Oglasi");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}
