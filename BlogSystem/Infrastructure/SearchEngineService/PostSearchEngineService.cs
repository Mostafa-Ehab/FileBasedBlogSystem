using BlogSystem.Domain.Entities;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace BlogSystem.Infrastructure.SearchEngineService;

public class PostSearchEngineService : ISearchEngineService<Post>
{
    private const LuceneVersion _luceneVersion = LuceneVersion.LUCENE_48;
    private readonly FSDirectory _indexDirectory;
    private readonly IndexWriter _indexWriter;
    private readonly StandardAnalyzer _standardAnalyzer;
    private IndexReader _indexReader = null!;
    private IndexSearcher _searcher = null!;
    private MultiFieldQueryParser _parser = null!;
    public PostSearchEngineService()
    {
        var indexPath = Path.Combine(Environment.CurrentDirectory, "Content", "index");
        _indexDirectory = FSDirectory.Open(indexPath);
        _standardAnalyzer = new StandardAnalyzer(_luceneVersion);

        // Create an index writer
        var indexConfig = new IndexWriterConfig(_luceneVersion, _standardAnalyzer);                          // create/overwrite index
        _indexWriter = new IndexWriter(_indexDirectory, indexConfig);
    }

    private void InitializeIndexReader()
    {
        if (_indexReader == null)
        {
            _indexReader = DirectoryReader.Open(_indexDirectory);
            _searcher = new IndexSearcher(_indexReader);
            _parser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, ["title", "content", "description"], _standardAnalyzer);
        }
    }

    public Task IndexDocumentAsync(Post document)
    {
        _indexWriter.AddDocument(new Document
        {
            new StringField("id", document.Id.ToString(), Field.Store.YES),
            new TextField("title", document.Title, Field.Store.NO),
            new TextField("content", document.Content, Field.Store.NO),
            new TextField("description", document.Description, Field.Store.NO),
            new TextField("slug", document.Slug, Field.Store.YES)
        });
        _indexWriter.Commit();
        return Task.CompletedTask;
    }

    public Task DeleteDocumentAsync(string id)
    {
        _indexWriter.DeleteDocuments(new Term("id", id));
        return Task.CompletedTask;
    }

    public Task UpdateDocumentAsync(Post document)
    {
        _indexWriter.UpdateDocument(new Term("id", document.Id.ToString()), new Document
        {
            new StringField("id", document.Id.ToString(), Field.Store.YES),
            new TextField("title", document.Title, Field.Store.NO),
            new TextField("content", document.Content, Field.Store.NO),
            new TextField("description", document.Description, Field.Store.NO),
            new TextField("slug", document.Slug, Field.Store.YES)
        });
        _indexWriter.Commit();
        return Task.CompletedTask;
    }

    public Task<Post?> GetDocumentAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Post>> SearchDocumentsAsync(string query)
    {
        InitializeIndexReader();

        var hits = _searcher.Search(
            _parser.Parse(query.Trim() + "*"), 10, new Sort(
                new SortField("title", SortFieldType.STRING)
            )
        );
        return Task.FromResult(hits.ScoreDocs.Select(hit => new Post
        {
            Id = _searcher.Doc(hit.Doc).Get("id")
        }));
    }

    public void Dispose()
    {
        _indexWriter.Dispose();
        _indexDirectory.Dispose();
        _indexReader.Dispose();
    }
}
