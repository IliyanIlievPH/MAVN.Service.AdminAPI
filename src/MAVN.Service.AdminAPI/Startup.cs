using System;
using System.Buffers;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Falcon.Common.Middleware.Filters;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.Common.ApiLibrary.Filters;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeSanitizing;
using Lykke.MonitoringServiceApiCaller;
using Lykke.SettingsReader;
using MAVN.Service.AdminAPI.Infrastructure.LykkeApiError;
using MAVN.Service.AdminAPI.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MAVN.Service.AdminAPI
{
    public class Startup
    {
        private const string ApiVersion = "v1";
        private const string ApiTitle = "Admin API";

        private AppSettings _appSettings;

        private IContainer ApplicationContainer { get; set; }
        private IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var settingsManager = Configuration.LoadSettings<AppSettings>(options =>
            {
                options.SetConnString(x => x.SlackNotifications.AzureQueue.ConnectionString);
                options.SetQueueName(x => x.SlackNotifications.AzureQueue.QueueName);
                options.SenderName = $"{AppEnvironment.Name} {AppEnvironment.Version}";
            });

#if !DEBUG
            services.AddApplicationInsightsTelemetry();
#endif

            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMvcOptions(opt => { opt.Filters.Add(typeof(NoContentFilter)); })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.DefaultContractResolver();
                })
                .AddFluentValidation(opt =>
                {
                    opt.RegisterValidatorsFromAssembly(Assembly.GetEntryAssembly());
                });
            
            services.Configure<MvcOptions>(opts =>
            {
                var formatter = opts.OutputFormatters.FirstOrDefault(i => i.GetType() == typeof(JsonOutputFormatter));
                var jsonFormatter = formatter as JsonOutputFormatter;
                var formatterSettings = jsonFormatter == null
                    ? JsonSerializerSettingsProvider.CreateSerializerSettings()
                    : jsonFormatter.PublicSerializerSettings;
                if (formatter != null)
                    opts.OutputFormatters.RemoveType<JsonOutputFormatter>();
                formatterSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ";
                var jsonOutputFormatter = new JsonOutputFormatter(formatterSettings, ArrayPool<char>.Create());
                opts.OutputFormatters.Insert(0, jsonOutputFormatter);
            });

            _appSettings = settingsManager.CurrentValue;

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration(ApiVersion, ApiTitle);
                options.OperationFilter<ObsoleteOperationDescriptionFilter>();
                options.OperationFilter<ApiKeyHeaderOperationFilter>();
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                // Wrap failed model state into LykkeApiErrorResponse.
                //options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.CreateInvalidModelResponse;
            });

            services.AddAuthentication(o =>
            {
                o.DefaultForbidScheme = IISDefaults.AuthenticationScheme;
            });

            services.AddLykkeLogging(
                settingsManager.ConnectionString(s => s.AdminApiService.Db.LogsConnString),
                "AdminApiServiceLogs",
                _appSettings.SlackNotifications.AzureQueue.ConnectionString,
                _appSettings.SlackNotifications.AzureQueue.QueueName,
                logBuilder => logBuilder
                    .AddSanitizingFilter(new Regex(@"(\\?""?[Pp]assword\\?""?:\s*\\?"")(.*?)(\\?"")"), "$1*$3")
                    .AddSanitizingFilter(new Regex(@"(\\?""?[Ll]ogin\\?""?:\s*\\?"")(.*?)(\\?"")"), "$1*$3")
                    .AddSanitizingFilter(new Regex(@"(\\?""?[Ee]mail\\?""?:\s*\\?"")(.*?)(\\?"")"), "$1*$3"));

            var builder = new ContainerBuilder();

            builder.Populate(services);

            builder.RegisterModule(new AutofacModule(settingsManager));
            var adminSettings = settingsManager.CurrentValue.AdminApiService;
            builder.RegisterModule(
                new DomainServices.AutofacModule(
                    adminSettings.TokenSymbol,
                    adminSettings.IsPublicBlockchainFeatureDisabled,
                    adminSettings.MobileAppImageDoOptimization,
                    adminSettings.MobileAppImageMinWidth,
                    adminSettings.MobileAppImageWarningFileSizeInKB,
                    adminSettings.SuggestedAdminPasswordLength,
                    adminSettings.IsPhoneVerificationDisabled));

            ApplicationContainer = builder.Build();
            InvalidModelStateResponseFactory.Logger = ApplicationContainer.Resolve<ILogFactory>()
                .CreateLog(nameof(InvalidModelStateResponseFactory));

            return new AutofacServiceProvider(ApplicationContainer);
        }

        [UsedImplicitly]
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime appLifetime,
            IMapper mapper)
        {
            try
            {
                app.UseCors(builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(host => true)
                    .AllowCredentials()
                );

                app.Use(async (context, next) =>
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("");
                    }
                    else
                    {
                        await next.Invoke();
                    }
                });

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseLykkeMiddleware(ex => new {message = "Technical problem"});
                app.UseMiddleware<LykkeApiErrorMiddleware>();
                app.UseMiddleware<ClientServiceApiExceptionMiddleware>();
                
                app.Use(next => context =>
                {
                    context.Request.EnableRewind();

                    return next(context);
                });

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default-to-swagger",
                        template: "{controller=Swagger}");
                });

                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.UseDefaultFiles();

                app.UseSwagger();
                app.UseSwaggerUI(o =>
                {
                    o.EnableValidator();
                    o.DisplayOperationId();
                    o.RoutePrefix = "swagger/ui";
                    o.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiVersion);
                });

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(CleanUp);

                mapper.ConfigurationProvider.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                try
                {
                    var log = app.ApplicationServices.GetService<ILogFactory>().CreateLog(typeof(Startup).FullName);
                    log?.Critical(ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(e);
                }

                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                var healthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();
                healthNotifier.Notify("Started");

                await Configuration.RegisterInMonitoringServiceAsync(
                    _appSettings.MonitoringServiceClient.MonitoringServiceUrl, healthNotifier);
            }
            catch (Exception ex)
            {
                try
                {
                    var log = ApplicationContainer.Resolve<ILogFactory>().CreateLog(typeof(Startup).FullName);
                    log?.Critical(ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(e);
                }

                throw;
            }
        }

        private void CleanUp()
        {
            try
            {
                // NOTE: Service can't receive and process requests here, so you can destroy all resources
                var healthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();
                healthNotifier.Notify("Terminating");

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                try
                {
                    var log = ApplicationContainer.Resolve<ILogFactory>().CreateLog(typeof(Startup).FullName);
                    log?.Critical(ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(e);
                }

                throw;
            }
        }
    }
}
