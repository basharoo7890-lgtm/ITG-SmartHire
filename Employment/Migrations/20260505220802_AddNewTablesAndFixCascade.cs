using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employment.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTablesAndFixCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ai_Analysis_Applicant_App_Id",
                table: "Ai_Analysis");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicant_Jobs_JobId",
                table: "Applicant");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicant_Users_UserId",
                table: "Applicant");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_CreatedBy",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Ai_Analysis_App_Id",
                table: "Ai_Analysis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applicant",
                table: "Applicant");

            migrationBuilder.RenameTable(
                name: "Applicant",
                newName: "Application");

            migrationBuilder.RenameIndex(
                name: "IX_Applicant_UserId",
                table: "Application",
                newName: "IX_Application_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Applicant_JobId",
                table: "Application",
                newName: "IX_Application_JobId");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Jobs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "Jobs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Jobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Jobs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MinExperience",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RequiredEducation",
                table: "Jobs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryMax",
                table: "Jobs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryMin",
                table: "Jobs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Jobs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DetectedLanguage",
                table: "Ai_Analysis",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GapReport",
                table: "Ai_Analysis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "YearsOfExperience",
                table: "Application",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Application",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Application",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedSalary",
                table: "Application",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EducationLevel",
                table: "Application",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CVText",
                table: "Application",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CVFileName",
                table: "Application",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Application",
                table: "Application",
                column: "AppId");

            migrationBuilder.CreateTable(
                name: "Job_Skill",
                columns: table => new
                {
                    Skill_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinYearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    ImportantLevel = table.Column<int>(type: "int", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Skill", x => x.Skill_Id);
                });

            migrationBuilder.CreateTable(
                name: "Skill_Synonyms",
                columns: table => new
                {
                    SkillSynonymId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Main_Skill_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Synonym_Skill = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill_Synonyms", x => x.SkillSynonymId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ai_Analysis_App_Id",
                table: "Ai_Analysis",
                column: "App_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ai_Analysis_Application_App_Id",
                table: "Ai_Analysis",
                column: "App_Id",
                principalTable: "Application",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Jobs_JobId",
                table: "Application",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Users_UserId",
                table: "Application",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_CreatedBy",
                table: "Jobs",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ai_Analysis_Application_App_Id",
                table: "Ai_Analysis");

            migrationBuilder.DropForeignKey(
                name: "FK_Application_Jobs_JobId",
                table: "Application");

            migrationBuilder.DropForeignKey(
                name: "FK_Application_Users_UserId",
                table: "Application");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_CreatedBy",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "Job_Skill");

            migrationBuilder.DropTable(
                name: "Skill_Synonyms");

            migrationBuilder.DropIndex(
                name: "IX_Ai_Analysis_App_Id",
                table: "Ai_Analysis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Application",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "MinExperience",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "RequiredEducation",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SalaryMax",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SalaryMin",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "DetectedLanguage",
                table: "Ai_Analysis");

            migrationBuilder.DropColumn(
                name: "GapReport",
                table: "Ai_Analysis");

            migrationBuilder.RenameTable(
                name: "Application",
                newName: "Applicant");

            migrationBuilder.RenameIndex(
                name: "IX_Application_UserId",
                table: "Applicant",
                newName: "IX_Applicant_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Application_JobId",
                table: "Applicant",
                newName: "IX_Applicant_JobId");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "YearsOfExperience",
                table: "Applicant",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Applicant",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Applicant",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedSalary",
                table: "Applicant",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "EducationLevel",
                table: "Applicant",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CVText",
                table: "Applicant",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CVFileName",
                table: "Applicant",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applicant",
                table: "Applicant",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_Ai_Analysis_App_Id",
                table: "Ai_Analysis",
                column: "App_Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ai_Analysis_Applicant_App_Id",
                table: "Ai_Analysis",
                column: "App_Id",
                principalTable: "Applicant",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicant_Jobs_JobId",
                table: "Applicant",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicant_Users_UserId",
                table: "Applicant",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_CreatedBy",
                table: "Jobs",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
