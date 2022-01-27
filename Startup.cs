using Eauction_Buyer_API.DataAccess;
using Eauction_Buyer_API.MessageSharer;
using Eauction_Buyer_API.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eauction_Buyer_API
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EauctionBuyerAPI", Version = "v1" });
            });

            //services.AddScoped<IRabbitMQCreator, RabbitMQCreator>();

            //services.AddSingleton(service => {
            //    var _config = Configuration.GetSection("RabbitMQ");
            //    return new ConnectionFactory()
            //    {
            //        HostName = _config["HostName"],
            //        UserName = _config["UserName"],
            //        Password = _config["Password"],
            //        Port = Convert.ToInt32(_config["Port"]),
            //        VirtualHost = _config["VirtualHost"],
            //    };
            //});

            services.AddScoped<ICosmosBuyerService, CosmosBuyerService>();
            services.AddSingleton<IBuyerRepository>(InitializeCosmosClientIntance(Configuration.GetSection("Cosmos")).GetAwaiter().GetResult());
            services.AddCors(c => { c.AddPolicy("AllowOrigin", option => option.AllowAnyOrigin()); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eauction_Buyer_API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static async Task<IBuyerRepository> InitializeCosmosClientIntance(IConfigurationSection configurationSection)
        {
            var account = configurationSection["AccountURL"];
            var key = configurationSection["AuthKey"];
            var databaseName = configurationSection["DatabaseId"];
            var containerName = configurationSection["CollectionId"];

            CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
            {
                HttpClientFactory = () =>
                {
                    HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    return new HttpClient(httpMessageHandler);
                },
                ConnectionMode = ConnectionMode.Gateway
            };

            var cosmosClient = new CosmosClient(account, key, cosmosClientOptions);
            var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            await db.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
            var buyerRepository = new BuyerRepository(cosmosClient, databaseName, containerName);
            return buyerRepository;
        }
    }
}
