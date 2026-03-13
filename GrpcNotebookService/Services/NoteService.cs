using Grpc.Core;
using GrpcNotebookService.XML;

namespace GrpcNotebookService.Services
{
    public class AddNewNote(ILogger<AddNewNote> logger, XMLService xmlService) : NoteService.NoteServiceBase
    {
        public override Task<NoteReply> CreateNote(NoteRequest request, ServerCallContext context)
        {
            logger.LogInformation($"The note is received with topic of {request.Topic}"); // Log the topic of the note for debugging purposes

            xmlService.AddNode(new Note(request.Topic, request.Title, request.Text, request.Timestamp)); // Add the note to the XML file using the XMLService

            return Task.FromResult(new NoteReply
            {
                Message = $"The note with topic of {request.Topic} is received successfully"
            });
        }

        public override Task<GetTopicsReply> GetTopics(GetTopicsRequest request, ServerCallContext context)
        {
            logger.LogInformation("The request to get all topics is received"); // Log the request for debugging purposes
            return Task.FromResult(new GetTopicsReply
            {
                Topics = { xmlService.GetTopics() } // Get the list of topics from the XML file using the XMLService and return it in the response
            });
        }

        public override Task<GetNotesPerTopicReply> GetNotesPerTopic(GetNotesPerTopicRequest request, ServerCallContext context)
        {
            logger.LogInformation($"The request to get notes for topic of {request.Topic} is received"); // Log the request for debugging purposes
            return Task.FromResult(new GetNotesPerTopicReply
            {
                Notes = { xmlService.GetNotesPerTopic(request.Topic) } // Get the list of notes for the specified topic from the XML file using the XMLService and return it in the response
            });
        }
    }
}
