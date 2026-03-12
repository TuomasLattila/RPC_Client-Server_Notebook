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
    }
}
