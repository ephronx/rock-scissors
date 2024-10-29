using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using rs_request_handler;

namespace rs_request_handler {
    public class Program {

        internal JsonSerializerOptions jsonoptions = new() {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static void Main(string[] args) {

            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            }
             );
            builder.Services.AddSwaggerGen();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddControllersWithViews().AddXmlSerializerFormatters();

            var app = builder.Build();

            app.UseMiddleware<ApiKeyMiddleware>();

            //if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Authentication}/{action=Index}/{id?}");
            });

            app.Run();

        }
    }
}
