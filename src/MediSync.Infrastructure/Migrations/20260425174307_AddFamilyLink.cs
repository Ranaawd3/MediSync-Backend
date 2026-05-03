using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFamilyLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyLinks_Users_CaregiverId",
                table: "FamilyLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyLinks_Users_PatientId",
                table: "FamilyLinks");

            migrationBuilder.DropColumn(
                name: "ViewAdherence",
                table: "FamilyLinks");

            migrationBuilder.RenameColumn(
                name: "ViewMedications",
                table: "FamilyLinks",
                newName: "CanEditMeds");

            migrationBuilder.AddColumn<bool>(
                name: "CaregiverAlerted",
                table: "Reminders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "InviteToken",
                table: "FamilyLinks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CaregiverId",
                table: "FamilyLinks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "FamilyLinks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaregiverEmail",
                table: "FamilyLinks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InviteExpiry",
                table: "FamilyLinks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_FamilyLinks_InviteToken",
                table: "FamilyLinks",
                column: "InviteToken",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyLinks_Users_CaregiverId",
                table: "FamilyLinks",
                column: "CaregiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyLinks_Users_PatientId",
                table: "FamilyLinks",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyLinks_Users_CaregiverId",
                table: "FamilyLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyLinks_Users_PatientId",
                table: "FamilyLinks");

            migrationBuilder.DropIndex(
                name: "IX_FamilyLinks_InviteToken",
                table: "FamilyLinks");

            migrationBuilder.DropColumn(
                name: "CaregiverAlerted",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "FamilyLinks");

            migrationBuilder.DropColumn(
                name: "CaregiverEmail",
                table: "FamilyLinks");

            migrationBuilder.DropColumn(
                name: "InviteExpiry",
                table: "FamilyLinks");

            migrationBuilder.RenameColumn(
                name: "CanEditMeds",
                table: "FamilyLinks",
                newName: "ViewMedications");

            migrationBuilder.AlterColumn<string>(
                name: "InviteToken",
                table: "FamilyLinks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "CaregiverId",
                table: "FamilyLinks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ViewAdherence",
                table: "FamilyLinks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyLinks_Users_CaregiverId",
                table: "FamilyLinks",
                column: "CaregiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyLinks_Users_PatientId",
                table: "FamilyLinks",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
