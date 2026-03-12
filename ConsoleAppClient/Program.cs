using Grpc.Net.Client;
using ConsoleAppClient;
using System.Globalization;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Press any key to exit....");
        Console.ReadLine();
        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("https://localhost:7101");
        var client = new NoteService.NoteServiceClient(channel);
        var reply = await client.CreateNoteAsync(
            new NoteRequest { Topic = "Test topic: cars", Title = "ferrari F40 is sick", Text = "please sell me F40!!!!", Timestamp = DateTime.UtcNow.ToString("MM/dd/yy - HH:mm:ss", CultureInfo.InvariantCulture)});
        Console.WriteLine(reply.Message);
        Console.WriteLine("Press any key to exit....");
        Console.ReadLine();
    }
}