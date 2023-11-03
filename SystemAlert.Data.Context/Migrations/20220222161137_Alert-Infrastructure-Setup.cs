using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SystemAlert.Data.Context.Migrations
{
    public partial class AlertInfrastructureSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SubType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ConfigurationTitle = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ConfigurationSubTitle = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EmailTemplateId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Group = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertType", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Alert",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlertTypeId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "Datetime2(7)", nullable: false, defaultValueSql: "(GetDate())"),
                    SentOn = table.Column<DateTime>(type: "Datetime2(7)", nullable: true),
                    ResolvedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolvedOn = table.Column<DateTime>(type: "Datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alert", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Alert_AlertType_AlertTypeId",
                        column: x => x.AlertTypeId,
                        principalTable: "AlertType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlertConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlertTypeId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SendTo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CoolDown = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertConfigurations", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_AlertConfigurations_AlertType_AlertTypeId",
                        column: x => x.AlertTypeId,
                        principalTable: "AlertType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "AlertConfigurationsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.InsertData(
                table: "Permission",
                column: "Id",
                values: new object[]
                {
                    "alert:create",
                    "alert:delete",
                    "alert:edit",
                    "alert:view"
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { "alert:create", new Guid("3e7ef202-4b3d-47ae-bebc-24256d31b541") },
                    { "alert:edit", new Guid("3e7ef202-4b3d-47ae-bebc-24256d31b541") },
                    { "alert:view", new Guid("3e7ef202-4b3d-47ae-bebc-24256d31b541") },
                    { "alert:view", new Guid("87d2e69e-0417-4684-92ab-2c1e3e60b32b") },
                    { "alert:create", new Guid("a284dfe1-5aa8-4c2a-ae1b-7d8a1c333a2f") },
                    { "alert:delete", new Guid("a284dfe1-5aa8-4c2a-ae1b-7d8a1c333a2f") },
                    { "alert:edit", new Guid("a284dfe1-5aa8-4c2a-ae1b-7d8a1c333a2f") },
                    { "alert:view", new Guid("a284dfe1-5aa8-4c2a-ae1b-7d8a1c333a2f") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_AlertTypeId",
                table: "Alert",
                columns: new[] { "AlertTypeId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_CreatedOn",
                table: "Alert",
                column: "CreatedOn")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Alert_ResourceId_AlertTypeId_ResolvedOn",
                table: "Alert",
                columns: new[] { "CustomerId", "ResourceId", "AlertTypeId", "ResolvedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_AlertConfiguration_AlertTypeId",
                table: "AlertConfigurations",
                columns: new[] { "AlertTypeId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_AlertConfiguration_SentTo_AlertTypeId",
                table: "AlertConfigurations",
                columns: new[] { "CustomerId", "SendTo", "AlertTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_AlertType_EmailTemplateId",
                table: "AlertType",
                column: "EmailTemplateId")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alert");

            migrationBuilder.DropTable(
                name: "AlertConfigurations")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "AlertConfigurationsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "AlertType");

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "alert:create", new Guid("3e7ef202-4b3d-47ae-bebc-24256d31b541") });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "alert:edit", new Guid("3e7ef202-4b3d-47ae-bebc-24256d31b541") });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "alert:view", new Guid("3e7ef202-4b3d-47ae-bebc-24256d31b541") });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "alert:view", new Guid("87d2e69e-0417-4684-92ab-2c1e3e60b32b") });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "alert:create", new Guid("a284dfe1-5aa8-4c2a-ae1b-7d8a1c333a2f") });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "alert:delete", new Guid("a284dfe1-5aa8-4c2a-ae1b-7d8a1c333a2f") });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "alert:edit", new Guid("a284dfe1-5aa8-4c2a-ae1b-7d8a1c333a2f") });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "alert:view", new Guid("a284dfe1-5aa8-4c2a-ae1b-7d8a1c333a2f") });

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: "alert:create");

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: "alert:delete");

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: "alert:edit");

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: "alert:view");
        }
    }
}
