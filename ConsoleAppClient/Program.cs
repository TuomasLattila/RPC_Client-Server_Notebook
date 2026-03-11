using Grpc.Net.Client;
using ConsoleAppClient;
internal class Program
{
    static async Task Main(string[] args)
    {
        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("https://localhost:7101");
        var client = new NoteService.NoteServiceClient(channel);
        var reply = await client.CreateNoteAsync(
            new NoteRequest { Topic = "Test topic: Animals", Text = "Animals are funny", Timestamp = DateTime.UtcNow.ToString("MM/dd/yy - HH:mm:ss")});
        Console.WriteLine(reply.Message);
        Console.WriteLine("Press any key to exit....");
        Console.ReadLine();
    }
}