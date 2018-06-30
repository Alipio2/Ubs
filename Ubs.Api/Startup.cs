using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Domain.Contracts.Repositories;
using Ubs.Infra.Repositories;
using Ubs.Infra.DataContexts;

namespace Ubs.Api
{
    public class Startup
    {
        
        public void ConfigureServices(IServiceCollection services)
        {
        
            services.AddMvc();
            services.AddResponseCompression();
            services.AddScoped<UbsDataContext, UbsDataContext>();
            services.AddTransient<IUbssRepository, UbssRepository>();
            //services.AddSwaggerGen(x =>
            //{
            //    x.SwaggerDoc("v1", new Info { Title = "UnidadeBasicaSaude", Version = "v1" });
            //});
        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            app.UseResponseCompression();

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UnidadeBasicaSaude");
            //});
        }
    }
}
