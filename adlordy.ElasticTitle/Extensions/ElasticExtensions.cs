using adlordy.ElasticTitle.Models;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace adlordy.ElasticTitle.Extensions
{
    public static class ElasticExtensions
    {
        public static IServiceCollection AddElastic(this IServiceCollection services)
        {
            var settings = new ConnectionSettings(new Uri("http://docker:9200"));
            settings.InferMappingFor<TitleModel>(m => m.TypeName("title"));
            var client = new ElasticClient(settings);
            return services
                .AddSingleton<IElasticClient>(client);
        }
    }
}
