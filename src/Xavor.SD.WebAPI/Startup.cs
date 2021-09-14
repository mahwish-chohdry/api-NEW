using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Xavor.SD.Model;
using Xavor.SD.BusinessLayer;
using Xavor.SD.ServiceLayer;
using Xavor.SD.Repository.UnitOfWork;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.UnitOfWork;
using Xavor.SD.ServiceLayer;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.BusinessLayer.Concrete;
using Xavor.SD.WebAPI.Helper;
using Xavor.SD.Common.ViewContracts;
using Swashbuckle.AspNetCore.Swagger;
using Xavor.SD.ServiceLayer.ServiceModel;
using Microsoft.Extensions.DependencyInjection;

using Xavor.SD.ServiceLayer.Transformations;
using Xavor.SD.ServiceLayer.Service;

namespace Xavor.SD.SD.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc().AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //Database instace injecting
            services.AddDbContext<SmartFanDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default"), providerOptions => providerOptions.EnableRetryOnFailure()));
            // injecting Services
            services.AddScoped<IAuth, Auth>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IGroupsService, GroupsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IIOTDeviceService, IOTDeviceService>();
            services.AddScoped<IIOTHubService, IOTHubService>();
            services.AddScoped<IAlarmsandwarningsService, AlarmsandwarningsService>();
            services.AddScoped<IAlarmsService, AlarmsService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IReportingService, ReportingService>();
            services.AddScoped<IInverterService, InverterService>();
            services.AddScoped<IEnvironmentstandardsService, EnvironmentstandardsService>();
            services.AddScoped<IEnvironmentsensorsService, EnvironmentsensorsService>();

            //injecting Business Layer
            services.AddScoped<IUserBL, UserBL>();
            services.AddScoped<IPermissionBL, PermissionBL>();
            services.AddScoped<IRoleBL, RoleBL>();
            services.AddScoped<IRolepermissionBL, RolepermissionBL>();
            services.AddScoped<IUserroleBL, UserroleBL>();
            services.AddScoped<ICustomerBL, CustomerBL>();
            services.AddScoped<IConfigurationsBL, ConfigurationsBL>();
            services.AddScoped<IDeviceBL, DeviceBL>();
            services.AddScoped<IDevicegroupBL, DevicegroupBL>();
            services.AddScoped<IGroupsBL, GroupsBL>();
            services.AddScoped<IUserdeviceBL, UserdeviceBL>();
            services.AddScoped<ICommandhistoryBL, CommandhistoryBL>();
            services.AddScoped<IAlarmsandwarningsBL, AlarmsandwarningsBL>();
            services.AddScoped<IEnvironmentstandardsBL, EnvironmentstandardsBL>();
            services.AddScoped<IEnvironmentsensorsBL, EnvironmentsensorsBL>();
            services.AddScoped<IDevicestatusBL, DevicestatusBL>();
            services.AddScoped<IDevicealarmsBL, DevicealarmsBL>();
            services.AddScoped<IDevicestatushistoryBL, DevicestatushistoryBL>();
            services.AddScoped<IRefreshtokensBL, RefreshtokensBL>();
            services.AddScoped<IDeviceAlarmsHistoryBL, DeviceAlarmsHistoryBL>();
            services.AddScoped<IGroupcommandBL, GroupcommandBL>();
            services.AddScoped<IDevicecommandBL, DevicecommandBL>();
            services.AddScoped<IEmailBL, EmailBL>();
            services.AddScoped<IEmailTemplateBL, EmailTemplateBL>();
            services.AddScoped<IDevicestatushistoryBL, DevicestatushistoryBL>();
            services.AddScoped<IDefaultSettingsBL, DefaultSettingsBL>();
            services.AddScoped<IDeviceBatchNumberBL, DeviceBatchNumberBL>();
            services.AddScoped<IBomBL, BomBL>();
            services.AddScoped<IFirmwareBL, FirmwareBL>();
            services.AddScoped<IPersonaBL, PersonaBL>();
            services.AddScoped<IFormBL, FormBL>();
            services.AddScoped<IPersonapermissionBL, PersonapermissionBL>();
            services.AddScoped<IInverterBL, InverterBL>();
            services.AddScoped<IRuleEngineBL, RuleEngineBL>();

            //Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork<SmartFanDbContext>>();

            //helper
            services.AddScoped<IAccountCreation, AccountCreation>();
            //transformations 
            services.AddScoped<ITransformations, Transformations>();

            services.AddCors();

            //services.AddAutoMapper();

            var Key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JWT_Secrets"].ToString());
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(y =>
            {
                y.RequireHttpsMetadata = true;
                y.SaveToken = false;
                y.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Smart Fan API documentation",
                    Description = "Smart Fan ASP.NET Core 2.0 Web API",
                    TermsOfService = "None",
                    Contact = new Contact()
                    {
                        Name = "Cloud Team",
                        Email = "CloudServices@xavor.com",
                        Url = ""
                    }
                });
                var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + @"Xavor.SD.WebAPI.xml";
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                 { "Bearer", new string[] { } }
                });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime /*, IServiceProvider container*/)
        {
            app.UseStaticFiles();

            // Below are swagger configurations 
            app.UseSwagger(o => { o.RouteTemplate = "docs/{documentName}/docs.json"; });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint("../docs/v1/docs.json", "Smart Fan V1");
                c.InjectStylesheet("/swagger-ui/custom.css");
                c.InjectJavascript("/swagger-ui/custom.js", "text/javascript");
                c.DocumentTitle = "Smart Fan API documentation";

                //Collapse model near example.
                c.DefaultModelExpandDepth(0);

                //Remove separate model definition.
                c.DefaultModelsExpandDepth(-1);
            });
            //ISchedulerFactory schedulerFactory;
            //IJobFactory jobFactory;
            //JobScheduleDTO schedule = new JobScheduleDTO(
            //    jobType: typeof(ScheduledBackgroundService),
            //    cronExpression: "* * * ? * *");
            //var quartz = new QuartzHostedService(schedule);
            //lifetime.ApplicationStarted.Register(quartz.StartAsync);
            //lifetime.ApplicationStopping.Register(quartz.StopAsync);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
