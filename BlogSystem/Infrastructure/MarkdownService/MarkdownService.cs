namespace BlogSystem.Infrastructure.MarkdownService
{
    public class MarkdownService
    {
        private readonly Markdig.MarkdownPipeline pipeline;
        public MarkdownService()
        {
            pipeline = new Markdig.MarkdownPipelineBuilder().Build();
        }

        public string RenderMarkdown(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return string.Empty;
            }

            return Markdig.Markdown.ToHtml(markdown, pipeline);
        }
    }
}
