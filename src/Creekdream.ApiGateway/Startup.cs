﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using SkyWalking.AspNetCore;

namespace Creekdream.ApiGateway
{
    /// <inheritdoc />
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <inheritdoc />
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot().AddConsul();

            var directServers = _configuration.GetValue<string>("SkyWalking:DirectServers");
            var applicationCode = _configuration.GetValue<string>("SkyWalking:ApplicationCode");
            if (!string.IsNullOrEmpty(directServers))
            {
                if (string.IsNullOrEmpty(applicationCode))
                {
                    applicationCode = _configuration.GetValue<string>("AuthorizationCenter:AppKey");
                }
                services.AddSkyWalking(options =>
                {
                    options.ApplicationCode = applicationCode;
                    options.DirectServers = directServers;
                });
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOcelot().Wait();
        }
    }
}
