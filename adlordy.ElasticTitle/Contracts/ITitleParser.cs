using adlordy.ElasticTitle.Models;
using System.Collections.Generic;

namespace adlordy.ElasticTitle.Contracts
{
    public interface ITitleParser
    {
        IEnumerable<TitleModel> ParseFile(string file);
    }
}
