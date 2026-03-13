using ConsoleAppClient;
using Grpc.Net.Client;
using System.Globalization;

internal class Program
{
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
                    await SendNote(client, CreateNewNote()); // Get note info from user and send it to server
                    break;

                case "2": // Read notes
                    await GetTopics(client); // Get topics from server and print them
                    await GetNotesPerTopic(client, Console.ReadLine() ?? ""); // Get notes for the specified topic from server and print them
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

    private static string? MainLoop() // Show menu and get user input
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("1. Write new note");
        Console.WriteLine("2. Read notes");
        Console.WriteLine("3. Exit");
        return Console.ReadLine();
    }

    private static NoteRequest CreateNewNote() // Get note info from user and return it as NoteRequest object
    {
        NoteRequest newNote = new();
        Console.WriteLine("Enter note topic:");
        newNote.Topic = Console.ReadLine() ?? "";
        Console.WriteLine("Enter note title:");
        newNote.Title = Console.ReadLine() ?? "";
        Console.WriteLine("Enter note text:");
        newNote.Text = Console.ReadLine() ?? "";
        newNote.Timestamp = DateTime.UtcNow.ToString("MM/dd/yy - HH:mm:ss", CultureInfo.InvariantCulture);
        return newNote;
    }

    private static async Task SendNote(NoteService.NoteServiceClient client, NoteRequest newNote) // Send new note to server and print response
    {
        try
        {
            var reply = await client.CreateNoteAsync(newNote);
            Console.WriteLine(reply.Message);
        }
        catch (Exception e) { Console.WriteLine($"Exception oquired: {e}"); }
    }

    private static async Task GetTopics(NoteService.NoteServiceClient client) // Get topics from server and print them
    {
        try
        {
            var reply = await client.GetTopicsAsync(new GetTopicsRequest());
            Console.WriteLine("Choose topic by writing the topic name:");
            for (int i = 1; i <= reply.Topics.Count; i++)
            {
                Console.WriteLine($"{i}. {reply.Topics[i-1]}");
            }
        }
        catch (Exception e) { Console.WriteLine($"Exception oquired: {e}"); }
    }

    private static async Task GetNotesPerTopic(NoteService.NoteServiceClient client, string topic) // Get notes for the specified topic from server and print them
    {
        try
        {
            var reply = await client.GetNotesPerTopicAsync(new GetNotesPerTopicRequest { Topic = topic });
            
            if (reply.Notes.Count == 0) // No notes for the specified topic
            {
                Console.WriteLine($"No notes for topic of '{topic}' found");
                return;
            }

            Console.WriteLine($"Notes for topic of {topic}:\n");
            foreach (var note in reply.Notes) // Print each note
            {
                Console.WriteLine($"Title: {note.Title}\nText: {note.Text}\nTimestamp: {note.Timestamp}\n");
            }
        }
        catch (Exception e) { Console.WriteLine($"Exception oquired: {e}"); }
    }
}