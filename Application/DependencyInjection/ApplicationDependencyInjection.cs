using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Services;
using Application.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {

        // we use IServiceCollection to register the services our Application layer will use (need)
        // this will:
        // 1. make Program.cs cleaner and then we add by builder.services.AddApplication();
        // 
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // to call mapster and config it as global settings
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;

            // 1. Scan will scan the whole layer for Mapster mapping
            // 2. Assembly will assemble the code as dll file to look at it
            // SO: go and assemble the whole code into dll file--> then Scan the whole layer for any Map includings.
            // Note: Scan is Mapster method that finds Mapster using something called Reflection
            mapsterConfig.Scan(Assembly.GetExecutingAssembly());


            // we used Singelton here because Mapster config will never be changed. Hence better for memory and cpu.
            services.AddSingleton(mapsterConfig);

            services.AddScoped<IMapper, ServiceMapper>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
