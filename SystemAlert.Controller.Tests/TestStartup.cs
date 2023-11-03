namespace SystemAlert.Controller.Tests
{
    public class TestStartup : Admin.Api.Startup
    {
        public TestStartup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {
        }

        protected override IConfiguration BuildConfiguration(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            hostEnvironment.EnvironmentName = "Local";
            return new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddMcitSecrets(hostEnvironment)
                .AddEnvironmentVariables()
                .Build();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services
                .AddScoped<IEmailHandler>(sp => new Mock<IEmailHandler>().Object)
                .AddScoped<IServiceBusHelper>(sp => new Mock<IServiceBusHelper>().Object)
                .AddScoped<IAdminServiceBusHelper>(sp => new Mock<IAdminServiceBusHelper>().Object);
            services.AddSingleton<UnhandledTestExceptionMiddleware>();
            services.AddLogging(x => x.AddDebug().AddConsole());
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<UnhandledTestExceptionMiddleware>();
            base.Configure(app, env);
        }

        protected override void ConfigureAuthenticationAndAuthorization(IServiceCollection services)
        {
            services
                .AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
                .AddFakeJwtBearer();
            services
                .PostConfigure<FakeJwtBearerOptions>(FakeJwtBearerDefaults.AuthenticationScheme, options =>
                {
                    if (options.Events == null) options.Events = new WebMotions.Fake.Authentication.JwtBearer.Events.JwtBearerEvents();
                    var existingOnTokenValidatedHandler = options.Events?.OnTokenValidated;
                    options.Events.OnTokenValidated = async context =>
                    {
                        await existingOnTokenValidatedHandler(context);
                        await Mcit_AuthExtensions.GetPermissionsForUser(context);
                    };
                });
            //FakeJwtBearerPostConfigureOptions
        }
    }

    public class UnhandledTestExceptionMiddleware : Microsoft.AspNetCore.Http.IMiddleware
    {
        private readonly ILogger<UnhandledTestExceptionMiddleware> _logger;

        public UnhandledTestExceptionMiddleware(ILogger<UnhandledTestExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                //_logger.LogError("Pre-Invoke Unhandled Test Exception");
                await next(context);
                //_logger.LogError("Post-Invoke Unhandled Test Exception");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Test Exception");
            }
        }
    }
}
