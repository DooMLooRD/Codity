using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Codity.Services.Helpers;
using Codity.Services.Hubs;
using Codity.Services.Resources;
using Codity.WebApi.ExtensionMethods;

namespace Codity.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(ErrorTranslations));
                    });
            services.AddControllers();
            services.AddSwagger();
            services.AddSignalR();
            services.AddDbContext(Configuration);
            services.AddAuthentication(Configuration);
            services.AddAndConfigureLocalization();
            services.AddIdentity();
            services.AddHangfire(Configuration);
            services.AddAndConfigureAutoMapper();
            services.AddServices();
            services.AddHttpClients();
            services.ConfigureOptions(Configuration);
            services.AddRepositories();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataSeeder dataSeeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                dataSeeder.EnsureSeedData();
                app.UseCors(x => x
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader());
            }

            app.UseAuthentication();
            app.ConfigureSwagger();
            app.UseHttpsRedirection();
            app.UseRequestLocalization();
            app.UseRouting();
            app.UseAuthorization();
            HangfireScheduler.ScheduleRecurringJobs();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
        }
    }
}
