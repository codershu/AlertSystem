using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SystemAlert.Service.Tests
{
    public class AlertHandlerTests : HandlerTestBase<AlertHandler>
    {
        public AlertHandlerTests()
        {
        }

        [Theory, AutoData]
        public async Task GetAlertTypes_ReturnAlertTypes(AlertType[] alertTypes)
        {
            // Arrange
            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);

            // Act
            var response = await handler.GetAlertTypes();

            // Assert
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-011");
        }

        [Theory, AutoData]
        public async Task GetAlertConfigurations_ReturnAlertConfigurations(AlertConfiguration[] alertConfigurations)
        {
            // Arrange
            var customerId = Guid.NewGuid();
            foreach (var item in alertConfigurations)
            {
                item.CustomerId = customerId;
            }
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);

            // Act
            var response = await handler.GetAlertConfigurations(customerId);

            // Assert
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-005");
        }

        [Theory, AutoData]
        public async Task UpdateAlertConfigurations_Updated(AlertConfiguration[] alertConfigurations)
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var target = alertConfigurations[0];
            target.CustomerId = customerId;
            target.AlertTypeId = "Global";

            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);

            var updateTo = new AlertConfigurationItem
            {
                CustomerId = customerId,
                AlertTypeId = target.AlertTypeId,
                SendTo = "custom",
                Email = "new@mcit.com",
                CoolDown = 30
            };

            // Act
            var response = await handler.UpdateAlertConfiguration(customerId, updateTo);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-007");

            var checkRecord = contextMock.Object.AlertConfiguration.Where(x => x.CustomerId == customerId && x.AlertTypeId == target.AlertTypeId).FirstOrDefault();
            checkRecord.ShouldNotBeNull();
            checkRecord.Email.ShouldBe(updateTo.Email);
            checkRecord.CoolDown.ShouldBe(updateTo.CoolDown);
        }

        [Theory, AutoData]
        public async Task UpdateAlertConfigurations_NoRecordFound(AlertConfiguration[] alertConfigurations)
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var target = alertConfigurations[0];
            target.CustomerId = customerId;
            target.AlertTypeId = "Global";

            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);

            var updateTo = new AlertConfigurationItem
            {
                CustomerId = Guid.NewGuid(),
                AlertTypeId = target.AlertTypeId,
                SendTo = "custom",
                Email = "new@mcit.com",
                CoolDown = 30
            };

            // Act
            var response = await handler.UpdateAlertConfiguration(customerId, updateTo);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeFalse();
            response.Code.ShouldBe("ALT-E-007");

            var checkRecord = contextMock.Object.AlertConfiguration.Where(x => x.CustomerId == customerId && x.AlertTypeId == target.AlertTypeId).FirstOrDefault();
            checkRecord.ShouldNotBeNull();
            checkRecord.Email.ShouldBe(target.Email);
            checkRecord.CoolDown.ShouldBe(target.CoolDown);
        }

        [Theory, AutoData]
        public async Task CreateAlertConfiguration_Created(AlertConfiguration[] alertConfigurations)
        {
            // Arrange
            var customerId = Guid.NewGuid();

            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);

            var toBeAdded = new AlertConfigurationItem
            {
                CustomerId = customerId,
                AlertTypeId = "Global",
                SendTo = "custom",
                Email = "new@mcit.com",
                CoolDown = 30
            };

            // Act
            var response = await handler.CreateAlertConfiguration(customerId, toBeAdded);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-006");
        }

        [Theory, AutoData]
        public async Task CreateAlertConfiguration_Duplicated(AlertConfiguration[] alertConfigurations)
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var duplicate = alertConfigurations[0];
            duplicate.CustomerId = customerId;
            duplicate.AlertTypeId = "Global";

            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);

            var toBeAdded = new AlertConfigurationItem
            {
                CustomerId = customerId,
                AlertTypeId = "Global",
                SendTo = "custom",
                Email = "new@mcit.com",
                CoolDown = 30
            };

            // Act
            var response = await handler.CreateAlertConfiguration(customerId, toBeAdded);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeFalse();
            response.Code.ShouldBe("ALT-E-006");
        }

        [Theory, AutoData]
        public async Task DeleteAlertConfiguration_Deleted(AlertConfiguration[] alertConfigurations)
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var target = alertConfigurations[0];

            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);

            // Act
            var response = await handler.DeleteAlertConfiguration(target.Id);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-008");
        }

        [Theory, AutoData]
        public async Task DeleteAlertConfiguration_NotFound(AlertConfiguration[] alertConfigurations)
        {
            // Arrange
            var customerId = Guid.NewGuid();

            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);

            // Act
            var response = await handler.DeleteAlertConfiguration(Guid.NewGuid());

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeFalse();
            response.Code.ShouldBe("ALT-E-008");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_Ok(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = customer.Id;

            var content = new
            {
                DeploymentName = "test_deployment",
                DeploymentId = Guid.NewGuid().ToString(),
                CollectionName = "test_collection",
                CollectionId = Guid.NewGuid().ToString(),
                LastReportOn = DateTime.UtcNow.AddMinutes(-30),
                SomeBoolean = false,
                SomeNumber = 54,
                SomeDecimal = 1.543m
            };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            var firstAlert = alerts.FirstOrDefault();
            firstAlert.CustomerId = customerId;
            firstAlert.AlertTypeId = alert.AlertTypeId;
            firstAlert.ResourceId = alert.ResourceId;
            firstAlert.ResolvedOn = null;
            firstAlert.SentOn = null;
            firstAlert.Content = alert.Content;

            var firstType = alertTypes.FirstOrDefault();
            firstType.Id = alert.AlertTypeId;

            var firstTypeConfiguration = alertConfigurations.FirstOrDefault();
            firstTypeConfiguration.AlertTypeId = alert.AlertTypeId;
            firstTypeConfiguration.CustomerId = customerId;
            firstTypeConfiguration.SendTo = AlertSendToTypes.Global;
            firstTypeConfiguration.Email = "custom@mcit.com";

            var secondTypeConfiguration = alertConfigurations.Skip(1).FirstOrDefault();
            secondTypeConfiguration.AlertTypeId = AlertTypes.Global;
            secondTypeConfiguration.CustomerId = customerId;
            secondTypeConfiguration.SendTo = AlertSendToTypes.Global;
            secondTypeConfiguration.SendTo = "global@mcit.com";


            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-015");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_CollectionSilenceAlert_Ok(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = customer.Id;

            var content = new { DeploymentName = "test_deployment", DeploymentId = Guid.NewGuid().ToString(), CollectionName = "test_collection", CollectionId = Guid.NewGuid().ToString(), CollectionSilenceAlert = true, LastReportOn = DateTime.UtcNow.AddMinutes(-30) };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            var firstAlert = alerts.FirstOrDefault();
            firstAlert.CustomerId = customerId;
            firstAlert.AlertTypeId = alert.AlertTypeId;
            firstAlert.ResourceId = alert.ResourceId;
            firstAlert.ResolvedOn = null;
            firstAlert.SentOn = null;
            firstAlert.Content = alert.Content;

            var firstType = alertTypes.FirstOrDefault();
            firstType.Id = alert.AlertTypeId;

            var firstTypeConfiguration = alertConfigurations.FirstOrDefault();
            firstTypeConfiguration.AlertTypeId = alert.AlertTypeId;
            firstTypeConfiguration.CustomerId = customerId;
            firstTypeConfiguration.SendTo = AlertSendToTypes.Global;
            firstTypeConfiguration.Email = "custom@mcit.com";

            var secondTypeConfiguration = alertConfigurations.Skip(1).FirstOrDefault();
            secondTypeConfiguration.AlertTypeId = AlertTypes.Global;
            secondTypeConfiguration.CustomerId = customerId;
            secondTypeConfiguration.SendTo = AlertSendToTypes.Global;
            secondTypeConfiguration.SendTo = "global@mcit.com";


            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-016");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_NoAlertType(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = customer.Id;

            var content = new { DeploymentName = "test_deployment", DeploymentId = Guid.NewGuid().ToString(), CollectionName = "test_collection", CollectionId = Guid.NewGuid().ToString(), LastReportOn = DateTime.UtcNow.AddMinutes(-30) };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-016");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_NoAlertConfiguration(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = customer.Id;

            var content = new { DeploymentName = "test_deployment", DeploymentId = Guid.NewGuid().ToString(), CollectionName = "test_collection", CollectionId = Guid.NewGuid().ToString(), LastReportOn = DateTime.UtcNow.AddMinutes(-30) };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            var firstAlert = alerts.FirstOrDefault();
            firstAlert.CustomerId = customerId;
            firstAlert.AlertTypeId = alert.AlertTypeId;
            firstAlert.ResourceId = alert.ResourceId;
            firstAlert.ResolvedOn = null;

            var firstType = alertTypes.FirstOrDefault();
            firstType.Id = alert.AlertTypeId;

            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-016");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_SendTo_Off(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = customer.Id;

            var content = new { DeploymentName = "test_deployment", DeploymentId = Guid.NewGuid().ToString(), CollectionName = "test_collection", CollectionId = Guid.NewGuid().ToString(), LastReportOn = DateTime.UtcNow.AddMinutes(-30) };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            var firstAlert = alerts.FirstOrDefault();
            firstAlert.CustomerId = customerId;
            firstAlert.AlertTypeId = alert.AlertTypeId;
            firstAlert.ResourceId = alert.ResourceId;
            firstAlert.ResolvedOn = null;

            var firstType = alertTypes.FirstOrDefault();
            firstType.Id = alert.AlertTypeId;

            var firstTypeConfiguration = alertConfigurations.FirstOrDefault();
            firstTypeConfiguration.AlertTypeId = alert.AlertTypeId;
            firstTypeConfiguration.CustomerId = customerId;
            firstTypeConfiguration.SendTo = AlertSendToTypes.Off;
            firstTypeConfiguration.Email = "custom@mcit.com";

            var secondTypeConfiguration = alertConfigurations.Skip(1).FirstOrDefault();
            secondTypeConfiguration.AlertTypeId = AlertTypes.Global;
            secondTypeConfiguration.CustomerId = customerId;
            secondTypeConfiguration.SendTo = AlertSendToTypes.Off;
            secondTypeConfiguration.SendTo = "global@mcit.com";


            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-016");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_CustomerNotFound(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = Guid.NewGuid();

            var content = new { DeploymentName = "test_deployment", DeploymentId = Guid.NewGuid().ToString(), CollectionName = "test_collection", CollectionId = Guid.NewGuid().ToString(), LastReportOn = DateTime.UtcNow.AddMinutes(-30) };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            var firstAlert = alerts.FirstOrDefault();
            firstAlert.CustomerId = customerId;
            firstAlert.AlertTypeId = alert.AlertTypeId;
            firstAlert.ResourceId = alert.ResourceId;
            firstAlert.ResolvedOn = null;

            var firstType = alertTypes.FirstOrDefault();
            firstType.Id = alert.AlertTypeId;

            var firstTypeConfiguration = alertConfigurations.FirstOrDefault();
            firstTypeConfiguration.AlertTypeId = alert.AlertTypeId;
            firstTypeConfiguration.CustomerId = customerId;
            firstTypeConfiguration.SendTo = AlertSendToTypes.Global;
            firstTypeConfiguration.Email = "custom@mcit.com";

            var secondTypeConfiguration = alertConfigurations.Skip(1).FirstOrDefault();
            secondTypeConfiguration.AlertTypeId = AlertTypes.Global;
            secondTypeConfiguration.CustomerId = customerId;
            secondTypeConfiguration.SendTo = AlertSendToTypes.Global;
            secondTypeConfiguration.SendTo = "global@mcit.com";


            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeFalse();
            response.Code.ShouldBe("ALT-E-018");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_EmailNotSent(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = customer.Id;

            var content = new { DeploymentName = "test_deployment", DeploymentId = Guid.NewGuid().ToString(), CollectionName = "test_collection", CollectionId = Guid.NewGuid().ToString(), LastReportOn = DateTime.UtcNow.AddMinutes(-30) };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            var firstAlert = alerts.FirstOrDefault();
            firstAlert.CustomerId = customerId;
            firstAlert.AlertTypeId = alert.AlertTypeId;
            firstAlert.ResourceId = alert.ResourceId;
            firstAlert.ResolvedOn = null;
            firstAlert.Content = alert.Content;
            firstAlert.SentOn = DateTime.UtcNow.AddMinutes(-1);

            var firstType = alertTypes.FirstOrDefault();
            firstType.Id = alert.AlertTypeId;

            var firstTypeConfiguration = alertConfigurations.FirstOrDefault();
            firstTypeConfiguration.AlertTypeId = alert.AlertTypeId;
            firstTypeConfiguration.CustomerId = customerId;
            firstTypeConfiguration.SendTo = AlertSendToTypes.Global;
            firstTypeConfiguration.Email = "custom@mcit.com";

            var secondTypeConfiguration = alertConfigurations.Skip(1).FirstOrDefault();
            secondTypeConfiguration.AlertTypeId = AlertTypes.Global;
            secondTypeConfiguration.CustomerId = customerId;
            secondTypeConfiguration.SendTo = AlertSendToTypes.Global;
            secondTypeConfiguration.SendTo = "global@mcit.com";


            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Error(ResponseCode.CodeNumbers.ALTE017);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-016");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_NoNeedToSendEmail(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = customer.Id;

            var content = new { DeploymentName = "test_deployment", DeploymentId = Guid.NewGuid().ToString(), CollectionName = "test_collection", CollectionId = Guid.NewGuid().ToString(), LastReportOn = DateTime.UtcNow.AddMinutes(-30) };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            var firstAlert = alerts.FirstOrDefault();
            firstAlert.CustomerId = customerId;
            firstAlert.AlertTypeId = alert.AlertTypeId;
            firstAlert.ResourceId = alert.ResourceId;
            firstAlert.ResolvedOn = null;
            firstAlert.SentOn = DateTime.UtcNow;

            var firstType = alertTypes.FirstOrDefault();
            firstType.Id = alert.AlertTypeId;

            var firstTypeConfiguration = alertConfigurations.FirstOrDefault();
            firstTypeConfiguration.AlertTypeId = alert.AlertTypeId;
            firstTypeConfiguration.CustomerId = customerId;
            firstTypeConfiguration.SendTo = AlertSendToTypes.Global;
            firstTypeConfiguration.Email = "custom@mcit.com";

            var secondTypeConfiguration = alertConfigurations.Skip(1).FirstOrDefault();
            secondTypeConfiguration.AlertTypeId = AlertTypes.Global;
            secondTypeConfiguration.CustomerId = customerId;
            secondTypeConfiguration.SendTo = AlertSendToTypes.Global;
            secondTypeConfiguration.SendTo = "global@mcit.com";


            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-016");
        }

        [Theory, AutoData]
        public async Task ProcessAlertMessage_VerifyAlertContent(AlertType[] alertTypes, AlertConfiguration[] alertConfigurations, Alert[] alerts)
        {
            // Arrange
            var customer = fixture.Build<Customer>()
                .WithAutoProperties()
                .Create();
            customer.IsActive = true;
            customer.Name = "test_customer";
            var customerId = customer.Id;

            var reportedOn = DateTime.UtcNow.AddMinutes(-30).ToString();
            var content = new { DeploymentName = "test_deployment", DeploymentId = Guid.NewGuid().ToString(), CollectionName = "test_collection", CollectionId = Guid.NewGuid().ToString(), LastReportOn = reportedOn, SelfDefinedError = "Self Defined Error", SystemError = "System Exception Message" };
            var alert = new AlertView()
            {
                CustomerId = customerId,
                AlertTypeId = AlertTypes.AgentNotReporting,
                ResourceId = $"deployment/{content.DeploymentId}",
                CreatedOn = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(content, JsonOpts.Web)
            };

            var firstAlert = alerts.FirstOrDefault();
            firstAlert.CustomerId = customerId;
            firstAlert.AlertTypeId = alert.AlertTypeId;
            firstAlert.ResourceId = alert.ResourceId;
            firstAlert.ResolvedOn = null;
            firstAlert.SentOn = null;
            firstAlert.Content = alert.Content;

            var firstType = alertTypes.FirstOrDefault();
            firstType.Id = alert.AlertTypeId;

            var firstTypeConfiguration = alertConfigurations.FirstOrDefault();
            firstTypeConfiguration.AlertTypeId = alert.AlertTypeId;
            firstTypeConfiguration.CustomerId = customerId;
            firstTypeConfiguration.SendTo = AlertSendToTypes.Global;
            firstTypeConfiguration.Email = "custom@mcit.com";

            var secondTypeConfiguration = alertConfigurations.Skip(1).FirstOrDefault();
            secondTypeConfiguration.AlertTypeId = AlertTypes.Global;
            secondTypeConfiguration.CustomerId = customerId;
            secondTypeConfiguration.SendTo = AlertSendToTypes.Global;
            secondTypeConfiguration.SendTo = "global@mcit.com";


            SetupMockData<AdminContext, AlertType>(c => c.AlertType, alertTypes);
            SetupMockData<AdminContext, AlertConfiguration>(c => c.AlertConfiguration, alertConfigurations);
            SetupMockData<AdminContext, Customer>(c => c.Customer, customer);
            SetupMockData<AdminContext, Alert>(c => c.Alert, alerts);

            var emailResponse = Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            var emailMock = mocker.GetMock<IEmailHandler>();
            emailMock.Setup(x => x.SendEmailWithTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                .Callback<string, string, string, Dictionary<string, string>, Dictionary<string, string>>((email, name, templateId, dynamicTemplateData, attachments) =>
                {
                    Assert.Equal(dynamicTemplateData["CustomerId"], customerId.ToString());
                    Assert.Equal(dynamicTemplateData["CustomerName"], customer.Name);
                    Assert.Equal(dynamicTemplateData["CollectionId"], content.CollectionId);
                    Assert.Equal(dynamicTemplateData["DeploymentId"], content.DeploymentId);
                    Assert.Equal(dynamicTemplateData["DeploymentName"], content.DeploymentName);
                    Assert.Equal(dynamicTemplateData["CollectionName"], content.CollectionName);
                    Assert.Equal(dynamicTemplateData["LastReportOn"], reportedOn);
                    Assert.Equal(dynamicTemplateData["SelfDefinedError"], content.SelfDefinedError);
                    Assert.Equal(dynamicTemplateData["SystemError"], content.SystemError);
                })
                .Returns(Task.FromResult(emailResponse));

            // Act
            var response = await handler.ProcessAlertMessage(alert);

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Code.ShouldBe("ALT-S-015");
        }
    }
}
