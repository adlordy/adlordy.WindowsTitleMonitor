using adlordy.ElasticTitle.Contracts;
using adlordy.ElasticTitle.Extensions;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;

namespace adlordy.ElasticTitle.Services
{
    public class Processor
    {
        private readonly CancellationToken _token;
        private readonly ITitleParser _parser;
        private readonly IElasticClient _client;
        private ILogger<Processor> _logger;

        public Processor(CancellationToken token, ITitleParser parser, IElasticClient client, ILoggerFactory factory)
        {
            _token = token;
            _parser = parser;
            _client = client;
            _logger = factory.CreateLogger<Processor>();
        }

        private class ParseState
        {
            private readonly TaskCompletionSource<bool> _source;

            public ParseState(string file)
            {
                File = file;
                _source = new TaskCompletionSource<bool>();
            }

            public string File { get; }
            public Task Task => _source.Task;
            public void Complete() => _source.SetResult(true);
        }

        public void Process()
        {
            var path = Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), "WindowTitleMonitor");
            var files = Directory.GetFiles(path, "????-??-??.txt");
            var parseStates = new List<ParseState>();

            var parseBlock = new ActionBlock<ParseState>(state =>
            {
                _logger.LogInformation("Processing file {0}", state.File);
                try
                {
                    var lines = _parser.ParseFile(state.File);
                    var result = _client.IndexMany(lines, state.File.IndexName(), "title");
                    if (result.IsValid)
                    {
                        _logger.LogInformation("Items {0} uploaded in {1}", result.Items.Count, result.Took);
                    }
                    else
                    {
                        _logger.LogError(new EventId(), result.OriginalException, "Failed to upload {0} items", result.Items.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(), ex, "Error parsing file {0}", state.File);
                }
                finally
                {
                    state.Complete();
                }
            }, new ExecutionDataflowBlockOptions { CancellationToken = _token, MaxDegreeOfParallelism = Environment.ProcessorCount });
            
            foreach (var file in files)
            {
                var indexResult = _client.IndexExists(file.IndexName());
                if (!indexResult.IsValid)
                    throw new InvalidOperationException("Failed to check if index exists", indexResult.OriginalException);
                if (indexResult.Exists)
                    _logger.LogInformation("File {0} already uploaded", file);
                else
                {
                    parseStates.Add(new ParseState(file));
                }
            }

            foreach (var parseState in parseStates)
                parseBlock.Post(parseState);

            Task.WaitAll(parseStates.Select(s => s.Task).ToArray(), _token);
        }
    }
}
