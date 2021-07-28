using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sgm.OC.Security.Entities;

namespace Sgm.OC
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

            services.AddControllers();
            services.AddSwaggerGen();

            services.Configure<FormOptions>(options =>
            {
                //Maximo 3mb 
                //https://convertlive.com/es/u/convertir/megabytes/a/bytes#3
                options.MultipartBodyLengthLimit = 3145728;
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("DiarcoDiarcoToken")),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidIssuer = "Diarco",
                        ValidAudience = "Diarco"
                    };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactLocal",
                      builder =>
                      {
                          builder.WithOrigins("*")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            Sgm.OC.Framework.Settings.ConnectionString = Configuration.GetConnectionString("SgmOC");
            Sgm.OC.Framework.Settings.FilePath = Configuration.GetValue<string>("filePath");
            Sgm.OC.Framework.Settings.SMTPServerAddress = Configuration.GetValue<string>("SMTPServerAddress");
            Sgm.OC.Framework.Settings.SMTPServerPort = Configuration.GetValue<int>("SMTPServerPort");
            Sgm.OC.Framework.Settings.SMTPServerUsesSSL = Configuration.GetValue<bool>("SMTPServerUsesSSL");
            Sgm.OC.Framework.Settings.SMTPServerPassword = Configuration.GetValue<string>("SMTPServerPassword");
            Sgm.OC.Framework.Settings.SMTPServerUsername = Configuration.GetValue<string>("SMTPServerUsername");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGM OC Api V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowReactLocal");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }

}
