using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IniParser;
using IniParser.Model;
using System.Text;

namespace CHAIRAPI
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
            //Added a Nuget Package to be able to read config files. I stored there the key
            var parser = new FileIniDataParser();
            IniData config = parser.ReadFile("Config/config.ini");
            string signingKeyString = config["JWTSettings"]["JWTSigningKey"];
            string issuer = config["JWTSettings"]["Issuer"];
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyString));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         //What to validate
                         ValidateIssuer = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = issuer,
                         ValidateAudience = false,
                         IssuerSigningKey = symmetricSecurityKey
                     };
                 });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
