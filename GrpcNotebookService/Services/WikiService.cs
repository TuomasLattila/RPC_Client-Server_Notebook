using Grpc.Core;
using GrpcNotebookService.HttpClients;
using GrpcNotebookService.XML;

namespace GrpcNotebookService.Services
{
    public class WikiOperations(ILogger<WikiOperations> logger, XMLService xmlService, WikiClient wikiClient) : WikiService.WikiServiceBase
    {
        public override async Task<ArticleReply> QueryWikiArticle(ArticleRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Reseived request to querry wikipedia API with article '{request.Article}'");

            var res = await wikiClient.GetOneArticle(request.Article); //fetch one requested article and return ArticleRely object wit the fetched data

            res.Article = request.Article;
            return res;
        }
    }
}
