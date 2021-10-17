using Errands.Users.Domain.Commands;
using Errands.Users.Domain.Queries;
using FireRepository;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Errands.Users.Api
{
    public class Startup
    {
        private readonly string rootPath;
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            rootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Errands.Users.Api", Version = "v1" });
            });
            services.AddSingleton(p => FirebaseAdmin.FirebaseApp.Create(new FirebaseAdmin.AppOptions
            {
                Credential = Google.Apis.Auth.OAuth2.GoogleCredential
               .FromFile(Path.Combine(rootPath, "Content", "errandng-users-app-service-account.json")),
                ServiceAccountId = "errandng-users-app@errandng-273e8.iam.gserviceaccount.com",
                ProjectId = "errandng-273e8",
            }));

            services.AddSingleton(p =>
            {
                var fireDb = new FirestoreDbBuilder()
                {
                    CredentialsPath = Path.Combine(rootPath, "Content", "errandng-users-app-service-account.json"),
                    ProjectId = "errandng-273e8",
                }.Build();
                return fireDb;
            });
            services.AddScoped<IAccountQuery, AccountQuery>();
            services.AddScoped<IAccountCommand, AccountCommand>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Errands.Users.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
