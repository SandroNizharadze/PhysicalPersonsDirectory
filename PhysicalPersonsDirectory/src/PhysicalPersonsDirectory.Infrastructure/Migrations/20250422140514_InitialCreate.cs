using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhysicalPersonsDirectory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumber_PhysicalPersons_PhysicalPersonId",
                table: "PhoneNumber");

            migrationBuilder.DropForeignKey(
                name: "FK_RelatedPerson_PhysicalPersons_PhysicalPersonId",
                table: "RelatedPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_RelatedPerson_PhysicalPersons_RelatedPhysicalPersonId",
                table: "RelatedPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RelatedPerson",
                table: "RelatedPerson");

            migrationBuilder.DropIndex(
                name: "IX_RelatedPerson_RelatedPhysicalPersonId",
                table: "RelatedPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneNumber",
                table: "PhoneNumber");

            migrationBuilder.RenameTable(
                name: "RelatedPerson",
                newName: "RelatedPersons");

            migrationBuilder.RenameTable(
                name: "PhoneNumber",
                newName: "PhoneNumbers");

            migrationBuilder.RenameIndex(
                name: "IX_PhoneNumber_PhysicalPersonId",
                table: "PhoneNumbers",
                newName: "IX_PhoneNumbers_PhysicalPersonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RelatedPersons",
                table: "RelatedPersons",
                columns: new[] { "PhysicalPersonId", "RelatedPhysicalPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneNumbers",
                table: "PhoneNumbers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumbers_PhysicalPersons_PhysicalPersonId",
                table: "PhoneNumbers",
                column: "PhysicalPersonId",
                principalTable: "PhysicalPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RelatedPersons_PhysicalPersons_PhysicalPersonId",
                table: "RelatedPersons",
                column: "PhysicalPersonId",
                principalTable: "PhysicalPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumbers_PhysicalPersons_PhysicalPersonId",
                table: "PhoneNumbers");

            migrationBuilder.DropForeignKey(
                name: "FK_RelatedPersons_PhysicalPersons_PhysicalPersonId",
                table: "RelatedPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RelatedPersons",
                table: "RelatedPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneNumbers",
                table: "PhoneNumbers");

            migrationBuilder.RenameTable(
                name: "RelatedPersons",
                newName: "RelatedPerson");

            migrationBuilder.RenameTable(
                name: "PhoneNumbers",
                newName: "PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_PhoneNumbers_PhysicalPersonId",
                table: "PhoneNumber",
                newName: "IX_PhoneNumber_PhysicalPersonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RelatedPerson",
                table: "RelatedPerson",
                columns: new[] { "PhysicalPersonId", "RelatedPhysicalPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneNumber",
                table: "PhoneNumber",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedPerson_RelatedPhysicalPersonId",
                table: "RelatedPerson",
                column: "RelatedPhysicalPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumber_PhysicalPersons_PhysicalPersonId",
                table: "PhoneNumber",
                column: "PhysicalPersonId",
                principalTable: "PhysicalPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RelatedPerson_PhysicalPersons_PhysicalPersonId",
                table: "RelatedPerson",
                column: "PhysicalPersonId",
                principalTable: "PhysicalPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RelatedPerson_PhysicalPersons_RelatedPhysicalPersonId",
                table: "RelatedPerson",
                column: "RelatedPhysicalPersonId",
                principalTable: "PhysicalPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
