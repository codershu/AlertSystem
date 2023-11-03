using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Text.Json.Serialization;

namespace SystemAlert.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = BuildConfiguration(configuration, hostEnvironment);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // logging
            services.AddSingleton(this.Configuration);
            services.Mcit_AddApplicationInsights();

            // auth
            ConfigureAuthenticationAndAuthorization(services);

            // common services
            services
                .AddHttpClient()
                .AddAutoMapper(typeof(AdminProfile))
                .AddCommonMiddleware()
                .AddTransient<IAlertHelper, AlertServiceBusHelper>()
                .AddTransient<IUserContext, WebUserContext2>()
                .AddSingleton<AdminServiceBusHelper>()
                .AddSingleton<IServiceBusHelper>(sp => sp.GetService<AdminServiceBusHelper>())
                .AddSingleton<IAdminServiceBusHelper>(sp => sp.GetService<AdminServiceBusHelper>())
                .AddSingleton<AzureKeyVaultHelper>()
                .AddSingleton<ISecretsProvider>(sp => sp.GetService<AzureKeyVaultHelper>())
                .AddTransient<IMailHandler, MailHandler>(sp => new MailHandler(MailConfig.GetMailConfConfigs()))
                ;

            services
                .AddHealthChecks()
                    .AddDbContextCheck<AdminContext>()
                    .AddCommonKeyVaultHealthChecks(new[] {
                        "ServiceBusConnectionString",
                        "ServiceBus--ConnectionString",
                        "AzureAd--ApplicationSecret",
                        "AzureAd--ClientSecret",
                        "AzureAdApplicationSecret",
                        "AzureAdClientSecret"
                    })
                    ;

            services
                .AddAdminServices()
                .AddAutoMapper(typeof(AdminProfile));

            ConfigureDataContexts(services);

            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(opts => {
                    opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    opts.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter()); //temporary - remove when common updated
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                });

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.ToString());
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Admin", Version = "v1" });
            });
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCommonMiddleware();
            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCommonHealthChecks("/api/health");

            app.UseMvc();
        }

        protected virtual void ConfigureDataContexts(IServiceCollection services)
        {
            var connectionstring = this.Configuration.GetConnectionString("default") ??
                this.Configuration["AdminDatabase"];
            services
                .AddMemoryCache()
                .AddHttpContextAccessor()
                .AddDbContext<AdminContext>((sp, options) => options
                    .UseSqlServer(connectionstring, builder =>
                    {
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    })
                );
        }

        protected virtual IConfiguration BuildConfiguration(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {

            return new ConfigurationBuilder()

                .BuildMcitStandardConfig(configuration, hostEnvironment)
                .Build();
        }

        protected virtual void ConfigureAuthenticationAndAuthorization(IServiceCollection services)
        {
            services
                .AddAzureAD_Authentication(this.Configuration);
        }
    }
}
