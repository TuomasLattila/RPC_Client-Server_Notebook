using ConsoleAppClient;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

internal class Program
{
    private class Message(string topic, string title, string text, string timestamp)
    {
        public string Topic { get; set; } = topic;
        public string Title { get; set; } = title;
        public string Text { get; set; } = text;
        public string Timestamp { get; set; } = timestamp;
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine("Press any key to start client....");
        Console.ReadLine();

        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("https://localhost:7101");
        var client = new NoteService.NoteServiceClient(channel);

        while (true)
        {
            var input = MainLoop();
            switch (input) 
            {
                case "1": // Create new note
                    await CreateNewNote(client, GetNoteInfo()); // Get note info from user and send it to server
                    break;

                case "2": // Read notes
                    Console.WriteLine("Not implemented yet");
                    break;

                case "3": // Exit
                    Console.WriteLine("Press any key to exit client....");
                    Console.ReadLine();
                    return;

                default: // Invalid input
                    Console.WriteLine("Invalid input, try again...");
                    break;
            }
        }
    }

    private static string? MainLoop()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("1. Write new note");
        Console.WriteLine("2. Read notes");
        Console.WriteLine("3. Exit");
        return Console.ReadLine();
    }

    private static NoteRequest GetNoteInfo()
    {
        NoteRequest newMessage = new();
        Console.WriteLine("Enter note topic:");
        newMessage.Topic = Console.ReadLine() ?? "";
        Console.WriteLine("Enter note title:");
        newMessage.Title = Console.ReadLine() ?? "";
        Console.WriteLine("Enter note text:");
        newMessage.Text = Console.ReadLine() ?? "";
        newMessage.Timestamp = DateTime.UtcNow.ToString("MM/dd/yy - HH:mm:ss", CultureInfo.InvariantCulture);
        return newMessage;
    }

    private static async Task CreateNewNote(NoteService.NoteServiceClient client, NoteRequest newMessage )
    {
        try
        {
            var reply = await client.CreateNoteAsync(newMessage);
            Console.WriteLine(reply.Message);
        }
        catch (Exception e) { Console.WriteLine($"Exception oquired: {e}"); }
    }
}