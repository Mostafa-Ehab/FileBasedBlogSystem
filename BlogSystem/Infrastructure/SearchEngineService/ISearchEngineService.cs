namespace BlogSystem.Infrastructure.SearchEngineService;

public interface ISearchEngineService<T> where T : class
{
    Task IndexDocumentAsync(T document);
    Task DeleteDocumentAsync(string id);
    Task UpdateDocumentAsync(T document);
    Task<T?> GetDocumentAsync(string id);
    Task<IEnumerable<T>> SearchDocumentsAsync(string query);
}
