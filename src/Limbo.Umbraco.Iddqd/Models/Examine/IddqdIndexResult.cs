using Examine;

namespace Limbo.Umbraco.Iddqd.Models.Examine;

public class IddqdIndexResult {

    public string IndexName { get; }

    public ISearchResult? Result { get; }

    public IddqdIndexResult(string indexName, ISearchResult? result) {
        IndexName = indexName;
        Result = result;
    }

}