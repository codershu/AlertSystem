using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemAlert.Data.Context.Models;

namespace SystemAlert.Data.Context.Mappings
{
    public class AlertType_Map : IEntityTypeConfiguration<AlertType>
    {
        public void Configure(EntityTypeBuilder<AlertType> builder)
        {
            builder.HasKey(x => x.Id).IsClustered(true);

            builder.HasIndex(x => x.EmailTemplateId).IsClustered(false);

            builder.Property(x => x.Id).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Type).HasMaxLength(128).IsRequired();
            builder.Property(x => x.SubType).HasMaxLength(128).IsRequired();
            builder.Property(x => x.ConfigurationTitle).HasMaxLength(128).IsRequired();
            builder.Property(x => x.ConfigurationSubTitle).HasMaxLength(128).IsRequired();
            builder.Property(x => x.EmailTemplateId).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Group).HasMaxLength(128).IsRequired();

            var initialTypes = new[]
            {
                new AlertType { Id = "Global", Type = "alert_global_type_text", SubType = "alert_global_sub_type_text", ConfigurationTitle = "alert_global_configuration_title_text", ConfigurationSubTitle = "alert_global_configuration_sub_title_text", EmailTemplateId = "d-00000000000000000000000000000000", Group = "" },
                new AlertType { Id = "AzureNoAccess", Type = "alert_azure_no_access_type_text", SubType = "alert_azure_no_access_sub_type_text", ConfigurationTitle = "alert_azure_no_access_configuration_title_text", ConfigurationSubTitle = "alert_azure_no_access_configuration_sub_title_text", EmailTemplateId = "d-ebf4dd95cf0c4e60974651ee51806027", Group = "" },
                new AlertType { Id = "AgentNotReporting", Type = "alert_agent_not_reporting_type_text", SubType = "alert_agent_not_reporting_sub_type_text", ConfigurationTitle = "alert_agent_not_reporting_configuration_title_text", ConfigurationSubTitle = "alert_agent_not_reporting_configuration_sub_title_text", EmailTemplateId = "d-750e055098fb4abeb1145bc2667e271c", Group = "" },
                new AlertType { Id = "AutoScalingExceedingCapacity", Type = "alert_auto_scaling_exceeding_capacity_type_text", SubType = "alert_auto_scaling_exceeding_capacity_sub_type_text", ConfigurationTitle = "alert_auto_scaling_exceeding_capacity_configuration_title_text", ConfigurationSubTitle = "alert_auto_scaling_exceeding_capacity_configuration_sub_title_text", EmailTemplateId = "d-e3e863e406a742d7a76d51caed88f6d9", Group = "" },
                new AlertType { Id = "VirtualMachineActionSchedule", Type = "alert_virtual_machine_action_schedule_type_text", SubType = "alert_virtual_machine_action_schedule_sub_type_text", ConfigurationTitle = "alert_virtual_machine_action_schedule_configuration_title_text", ConfigurationSubTitle = "alert_virtual_machine_action_schedule_configuration_sub_title_text", EmailTemplateId = "d-3b864b446f6943ffb0077a3c423ea19d", Group = "" },
                new AlertType { Id = "DeploymentIsAtCapacity", Type = "deployment_is_at_capacity_type_text", SubType = "deployment_is_at_capacity_sub_type_text", ConfigurationTitle = "deployment_is_at_capacity_configuration_title_text", ConfigurationSubTitle = "deployment_is_at_capacity_configuration_sub_title_text", EmailTemplateId = "d-7fa87136681349b0a162f231ddb169c3", Group = "" }
            };
            builder.HasData(initialTypes);
        }
    }
}
