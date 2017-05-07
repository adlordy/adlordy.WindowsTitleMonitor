using adlordy.ElasticTitle.Contracts;
using adlordy.ElasticTitle.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace adlordy.ElasticTitle.Services
{
    public class TitleParser : ITitleParser
    {
        private readonly IElasticClient _client;
        private readonly ILogger<TitleParser> _logger;

        public TitleParser(IElasticClient client, ILoggerFactory factory)
        {
            _client = client;
            _logger = factory.CreateLogger<TitleParser>();
        }

        public IEnumerable<TitleModel> ParseFile(string file)
        {
            _logger.LogInformation("Processing {0}", file);
            using (var reader = File.OpenText(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var item = ParseLine(line);
                    if (item!=null)
                        yield return item;
                }
            }
        }

        private TitleModel ParseLine(string line)
        {
            var tabIndex = line.IndexOf('\t');
            if (tabIndex == -1 || tabIndex == line.Length - 1)
                return null;

            var dateString = line.Substring(0, tabIndex);
            var title = line.Substring(tabIndex + 1, line.Length - tabIndex - 1);
            var date = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            return new TitleModel(date, title);
        }

        
    }
}
