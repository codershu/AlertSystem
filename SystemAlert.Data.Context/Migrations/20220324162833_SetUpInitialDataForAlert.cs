using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Data.Context.Migrations
{
    public partial class SetUpInitialDataForAlert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AlertType",
                columns: new[] { "Id", "ConfigurationSubTitle", "ConfigurationTitle", "EmailTemplateId", "Group", "SubType", "Type" },
                values: new object[,]
                {
                    { "AgentNotReporting", "alert_agent_not_reporting_configuration_sub_title_text", "alert_agent_not_reporting_configuration_title_text", "d-750e055098fb4abeb1145bc2667e271c", "", "alert_agent_not_reporting_sub_type_text", "alert_agent_not_reporting_type_text" },
                    { "AutoScalingExceedingCapacity", "alert_auto_scaling_exceeding_capacity_configuration_sub_title_text", "alert_auto_scaling_exceeding_capacity_configuration_title_text", "d-e3e863e406a742d7a76d51caed88f6d9", "", "alert_auto_scaling_exceeding_capacity_sub_type_text", "alert_auto_scaling_exceeding_capacity_type_text" },
                    { "AzureNoAccess", "alert_azure_no_access_configuration_sub_title_text", "alert_azure_no_access_configuration_title_text", "d-ebf4dd95cf0c4e60974651ee51806027", "", "alert_azure_no_access_sub_type_text", "alert_azure_no_access_type_text" },
                    { "Global", "alert_global_configuration_sub_title_text", "alert_global_configuration_title_text", "d-00000000000000000000000000000000", "", "alert_global_sub_type_text", "alert_global_type_text" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlertType",
                keyColumn: "Id",
                keyValue: "AgentNotReporting");

            migrationBuilder.DeleteData(
                table: "AlertType",
                keyColumn: "Id",
                keyValue: "AutoScalingExceedingCapacity");

            migrationBuilder.DeleteData(
                table: "AlertType",
                keyColumn: "Id",
                keyValue: "AzureNoAccess");

            migrationBuilder.DeleteData(
                table: "AlertType",
                keyColumn: "Id",
                keyValue: "Global");
        }
    }
}
