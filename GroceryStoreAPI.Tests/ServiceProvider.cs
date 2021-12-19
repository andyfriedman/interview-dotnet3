using GroceryStoreAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GroceryStoreAPI.Tests
{
    public class ServiceProvider
    {
        public static IServiceProvider BuildServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging();

            services.AddTransient<IDataRepository<IDataItem>>(provider => 
                new InMemoryStore(new Dictionary<int, IDataItem>(TestData.Customers)));

            return services.BuildServiceProvider();
        }
    }
}
