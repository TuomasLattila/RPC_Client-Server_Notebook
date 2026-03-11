using Grpc.Core;    

namespace GrpcNotebookService.Services
{
    public class AddNewNote(ILogger<AddNewNote> logger) : NoteService.NoteServiceBase
    {
        public override Task<NoteReply> CreateNote(NoteRequest request, ServerCallContext context)
        {
            logger.LogInformation($"The note is received with topic of {request.Topic}");
            return Task.FromResult(new NoteReply
            {
                Message = $"The note with topic of {request.Topic} is received successfully"
            });
        }
    }
}
