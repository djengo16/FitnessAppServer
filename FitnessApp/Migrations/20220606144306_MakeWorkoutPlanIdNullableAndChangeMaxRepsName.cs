using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.Migrations
{
    public partial class MakeWorkoutPlanIdNullableAndChangeMaxRepsName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_WorkoutPlans_WorkoutPlanId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutDays_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutDays");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WorkoutPlanId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WorkoutPlans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WorkoutPlanId",
                table: "WorkoutDays",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "WorkoutPlanId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WorkoutPlanId",
                table: "AspNetUsers",
                column: "WorkoutPlanId",
                unique: true,
                filter: "[WorkoutPlanId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_WorkoutPlans_WorkoutPlanId",
                table: "AspNetUsers",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutDays_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutDays",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_WorkoutPlans_WorkoutPlanId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutDays_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutDays");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WorkoutPlanId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WorkoutPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WorkoutPlanId",
                table: "WorkoutDays",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WorkoutPlanId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WorkoutPlanId",
                table: "AspNetUsers",
                column: "WorkoutPlanId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_WorkoutPlans_WorkoutPlanId",
                table: "AspNetUsers",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutDays_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutDays",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
