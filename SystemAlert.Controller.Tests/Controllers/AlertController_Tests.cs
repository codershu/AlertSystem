using System.Data;
using System.Net;

namespace SystemAlert.Controller.Tests.Controllers
{
    public class AlertController_Test_Setup : ApiClassFixtureBase<TestStartup, AdminContext>
    {
        public AlertController_Test_Setup(GlobalApiTestFixtureBase<TestStartup> globalProvider) : base(globalProvider)
        {
        }

        public Customer TestCustomer { get; private set; }
        public Customer OtherCustomer { get; private set; }
        public User TestUser { get; private set; }
        public User OtherUser { get; private set; }
        public List<AlertType> ExistingAlertTypes { get; private set; }

        public override async Task InitializeAsync()
        {

            using (var context = CreateDataContext())
            {
                // set up data for all the Facts in class
                TestCustomer = Data.Context.Fluent
                    .Create.Customer((Guid?)null, "master customer")
                    .Build();
                context.Add(TestCustomer);

                OtherCustomer = Data.Context.Fluent
                    .Create.Customer((Guid?)null, "master customer")
                    .Build();
                context.Add(OtherCustomer);

                TestUser = Data.Context.Fluent
                    .Create.User(TestCustomer, "master user")
                    .Build();
                context.Add(TestUser);

                OtherUser = Data.Context.Fluent
                    .Create.User(OtherCustomer, "blocking test")
                    .WithEmail("blockeduser@test.com")
                    .Build();
                context.Add(OtherUser);

                // if there is no data in AlertType table, use the following code to initial it
                //ExistingAlertTypes = new List<AlertType>()
                //{
                //    new AlertType() { Id = "Global", Type = "alert_global_type_text", SubType = "alert_global_sub_type_text", ConfigurationTitle="alert_global_configuration_title_text", ConfigurationSubTitle="alert_global_configuration_sub_title_text", EmailTemplateId = "d-d87cb96227e641e9a8fcda2c0b11edc7", Group = "info" },
                //    new AlertType() { Id = "AzureNoAccess", Type = "alert_azure_no_access_type_text", SubType = "alert_azure_no_access_sub_type_text", ConfigurationTitle="alert_azure_no_access_configuration_title_text", ConfigurationSubTitle="alert_azure_no_access_configuration_sub_title_text", EmailTemplateId = "d-d87cb96227e641e9a8fcda2c0b11edc7", Group = "info" },
                //    new AlertType() { Id = "AgentNotReporting", Type = "alert_agent_not_reporting_type_text", SubType = "alert_agent_not_reporting_sub_type_text", ConfigurationTitle="alert_agent_not_reporting_configuration_title_text", ConfigurationSubTitle="alert_agent_not_reporting_configuration_sub_title_text", EmailTemplateId = "d-d87cb96227e641e9a8fcda2c0b11edc7", Group = "info" },
                //};

                //context.AddRange(ExistingAlertTypes);

                await context.SaveChangesAsync();

                var adminRole = context.Set<Role>()
                    .Single(r => r.CustomerId == null && r.Name == GlobalRoles.Administrator);

                context.Add(new UserRole { RoleId = adminRole.Id, UserId = TestUser.Id });
                context.Add(new UserRole { RoleId = adminRole.Id, UserId = OtherUser.Id });

                await context.SaveChangesAsync();
            }
        }

        public override async Task DisposeAsync()
        {
            var context = this.CreateDataContext();
            //context.RemoveRange(ExistingAlertTypes);
            context.Remove(OtherUser);
            context.Remove(TestUser);
            context.Remove(TestCustomer);
            await context.SaveChangesAsync();
        }
    }

    [Trait("Category", "Integration")]
    [Collection(AdminApiTestCollection.Name)]
    public class AlertController_Tests : ApiIntegrationTestBase<AlertController_Test_Setup>
    {
        public readonly object _tokenData;

        public AlertController_Tests(AlertController_Test_Setup classFixture) : base(classFixture)
        {
            _tokenData = new
            {
                oid = this.ClassSetup.TestUser.Id,
                permission = new[] { ApiPermissions.Alert.View, ApiPermissions.Alert.Edit },
            };
        }

        [Fact]
        public async Task GetAlertTypes_ShouldReturnAlertTypes()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;
            string url = $"api/alerts/types";

            var content = await this.GetResponse<List<AlertTypeItem>>(url, HttpStatusCode.OK, _tokenData, customerId);
            content.IsSuccess.ShouldBe(true);
            content.Code.ShouldBe("ALT-S-011");
            content.Result.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetAlertTypes_Forbidden()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;
            string url = $"api/alerts/types";

            var newToken = new
            {
                oid = ClassSetup.OtherUser.Id
            };

            var content = await this.GetResponse<List<AlertTypeItem>>(url, HttpStatusCode.Forbidden, newToken, customerId);

            content.IsSuccess.ShouldBe(false);
            content.Result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAlertConfigurationTypes_ShouldReturnAlertConfigurationTypes()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;
            string url = $"api/alerts/configurations";

            var content = await this.GetResponse<List<AlertConfigurationItem>>(url, HttpStatusCode.OK, _tokenData, customerId);
            content.IsSuccess.ShouldBe(true);
            content.Code.ShouldBe("ALT-S-005");
            content.Result.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetAlertConfigurationTypes_Forbidden()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;
            string url = $"api/alerts/configurations";

            var newToken = new
            {
                oid = ClassSetup.OtherUser.Id
            };

            var content = await this.GetResponse<List<AlertConfigurationItem>>(url, HttpStatusCode.Forbidden, newToken, customerId);

            content.IsSuccess.ShouldBe(false);
            content.Result.ShouldBeNull();
        }

        [Fact]
        public async Task CreateAlertConfigurationTypes_ShouldCreate()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;
            string url = $"api/alerts/configurations";

            var configuration = new AlertConfiguration
            {
                CustomerId = customerId,
                AlertTypeId = "Global",
                SendTo = "Default",
                Email = "test1@mcit.com",
                CoolDown = 15,
            };

            var content = await this.PostResponse<bool?, AlertConfiguration>(url, configuration, HttpStatusCode.OK, _tokenData, customerId);

            using (var context = ClassSetup.CreateDataContext())
            {
                var records = context.Set<AlertConfiguration>()
                    .Where(n => n.CustomerId == configuration.CustomerId && n.AlertTypeId == configuration.AlertTypeId)
                    .ToList();

                context.RemoveRange(records);
                context.SaveChanges();
                records.Count.ShouldBe(1);
            }

            content.IsSuccess.ShouldBe(true);
            content.Code.ShouldBe("ALT-S-006");
            content.Result.ShouldNotBeNull();
        }

        [Fact]
        public async Task CreateAlertConfigurationTypes_Forbidden()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;
            string url = $"api/alerts/configurations";

            var newToken = new
            {
                oid = ClassSetup.OtherUser.Id,
                permission = new[] { ApiPermissions.Alert.View },
            };

            var configuration = new AlertConfiguration
            {
                CustomerId = customerId,
                AlertTypeId = "Global",
                SendTo = "Default",
                Email = "test1@mcit.com",
                CoolDown = 15,
            };

            var content = await this.PostResponse<bool?, AlertConfiguration>(url, configuration, HttpStatusCode.Forbidden, newToken, customerId);

            using (var context = ClassSetup.CreateDataContext())
            {
                var records = context.Set<AlertConfiguration>()
                    .Where(n => n.CustomerId == configuration.CustomerId && n.AlertTypeId == configuration.AlertTypeId)
                    .ToList();

                records.Count.ShouldBe(0);
            }

            content.IsSuccess.ShouldBe(false);
            content.Result.ShouldBeNull();
        }

        [Fact]
        public async Task UpdateAlertConfigurationTypes_ShouldUpdate()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;
            string url = $"api/alerts/configurations";

            var configuration = new AlertConfiguration
            {
                CustomerId = customerId,
                AlertTypeId = "Global",
                SendTo = "Default",
                Email = "test1@mcit.com",
                CoolDown = 15,
            };

            using (var context = ClassSetup.CreateDataContext())
            {
                context.Add(configuration);
                await context.SaveChangesAsync();
            }

            configuration.CoolDown = 30;
            configuration.Email = "test2@mcit.com";

            var content = await this.PutResponse<bool?, AlertConfiguration>(url, configuration, HttpStatusCode.OK, _tokenData, customerId);

            using (var context = ClassSetup.CreateDataContext())
            {
                var record = context.Set<AlertConfiguration>()
                    .Where(n => n.CustomerId == configuration.CustomerId && n.AlertTypeId == configuration.AlertTypeId && n.SendTo == configuration.SendTo)
                    .FirstOrDefault();

                record.CoolDown.ShouldBe(30);
                record.Email.ShouldBe("test2@mcit.com");

                context.Remove(record);
                context.SaveChanges();
            }

            content.IsSuccess.ShouldBe(true);
            content.Code.ShouldBe("ALT-S-007");
            content.Result.ShouldNotBeNull();
        }

        [Fact]
        public async Task UpdateAlertConfigurationTypes_Forbidden()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;
            string url = $"api/alerts/configurations";

            var configuration = new AlertConfiguration
            {
                CustomerId = customerId,
                AlertTypeId = "Global",
                SendTo = "Default",
                Email = "test1@mcit.com",
                CoolDown = 15,
            };

            using (var context = ClassSetup.CreateDataContext())
            {
                context.Add(configuration);
                await context.SaveChangesAsync();
            }

            configuration.CoolDown = 30;
            configuration.Email = "test2@mcit.com";

            var newToken = new
            {
                oid = ClassSetup.OtherUser.Id,
                permission = new[] { ApiPermissions.Alert.View },
            };

            var content = await this.PutResponse<bool?, AlertConfiguration>(url, configuration, HttpStatusCode.Forbidden, newToken, customerId);

            using (var context = ClassSetup.CreateDataContext())
            {
                var record = context.Set<AlertConfiguration>()
                    .Where(n => n.CustomerId == configuration.CustomerId && n.AlertTypeId == configuration.AlertTypeId && n.SendTo == configuration.SendTo)
                    .FirstOrDefault();

                record.CoolDown.ShouldBe(15);
                record.Email.ShouldBe("test1@mcit.com");

                context.Remove(record);
                context.SaveChanges();
            }

            content.IsSuccess.ShouldBe(false);
            content.Result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAlertConfigurationTypes_ShouldDelete()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;

            var configuration = new AlertConfiguration
            {
                CustomerId = customerId,
                AlertTypeId = "Global",
                SendTo = "Default",
                Email = "test1@mcit.com",
                CoolDown = 15,
            };

            using (var context = ClassSetup.CreateDataContext())
            {
                context.Add(configuration);
                await context.SaveChangesAsync();
            }

            string url = $"api/alerts/configurations/{configuration.Id}";

            var content = await this.DeleteResponse<bool?>(url, HttpStatusCode.OK, _tokenData, customerId);

            using (var context = ClassSetup.CreateDataContext())
            {
                var record = context.Set<AlertConfiguration>()
                    .Where(n => n.Id == configuration.Id)
                    .FirstOrDefault();

                record.ShouldBeNull();
            }

            content.IsSuccess.ShouldBe(true);
            content.Code.ShouldBe("ALT-S-008");
            content.Result.ShouldNotBeNull();
        }

        [Fact]
        public async Task DeleteAlertConfigurationTypes_Forbidden()
        {
            var customerId = this.ClassSetup.TestCustomer.Id;

            var configuration = new AlertConfiguration
            {
                CustomerId = customerId,
                AlertTypeId = "Global",
                SendTo = "Default",
                Email = "test1@mcit.com",
                CoolDown = 15,
            };

            using (var context = ClassSetup.CreateDataContext())
            {
                context.Add(configuration);
                await context.SaveChangesAsync();
            }

            var newToken = new
            {
                oid = ClassSetup.OtherUser.Id,
                permission = new[] { ApiPermissions.Alert.View },
            };

            string url = $"api/alerts/configurations/{configuration.Id}";

            var content = await this.DeleteResponse<bool?>(url, HttpStatusCode.Forbidden, newToken, customerId);

            using (var context = ClassSetup.CreateDataContext())
            {
                var record = context.Set<AlertConfiguration>()
                    .Where(n => n.Id == configuration.Id)
                    .FirstOrDefault();

                record.ShouldNotBeNull();
            }

            content.IsSuccess.ShouldBe(false);
            content.Result.ShouldBeNull();
        }
    }
}
