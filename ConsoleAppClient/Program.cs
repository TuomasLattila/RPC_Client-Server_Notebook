using ConsoleAppClient;
using Grpc.Net.Client;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Press any key to start client....");
        Console.ReadLine();

        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("https://localhost:7101");
        var client = new NoteService.NoteServiceClient(channel);
        var wikiClient = new WikiService.WikiServiceClient(channel);

        while (true)
        {
            var input = MainLoop();
            switch (input) 
            {
                case "1": // Create new note
                    await SendNote(client, CreateNewNote()); // Get note info from user, create a new NoteRequest object and send it to server
                    break;

                case "2": // Read notes
                    var topic = await GetTopics(client); // Get topics from server and print them
                    if (topic == null) { break; }
                    await GetNotesPerTopic(client, topic); // Get notes for the specified topic (user chosen) from server and print them.
                    break;

                case "3": // Query wikipedia article smmary and ask user to save it
                    Console.WriteLine("\nSearch article:");
                    NoteRequest? reply = await GetWikiArticle(wikiClient, Console.ReadLine());
                    if (reply == null) { break; }
                    
                    Console.WriteLine("Do you want to append this summary to existing note topic? (y/n)");
                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        var choise = await GetTopics(client); // Get topics from server and print them
                        if (choise == null) { break; }
                        reply.Topic = choise;
                        reply.Timestamp = DateTime.UtcNow.ToString("MM/dd/yy - HH:mm:ss", CultureInfo.InvariantCulture);
                        await SendNote(client, reply);
                    }
                    else { Console.WriteLine("\nWikipedia article summary not saved!\n"); }
                    break;

                case "4": // Exit
                    Console.WriteLine("Press any key to exit client....");
                    Console.ReadLine();
                    channel.Dispose();
                    return;

                default: // Invalid input
                    Console.WriteLine("\nInvalid input, try again...");
                    break;
            }
        }
    }

    // Print menu and get user input
    private static string? MainLoop()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("1. Write new note");
        Console.WriteLine("2. Read notes");
        Console.WriteLine("3. Get Wikipedia article summary");
        Console.WriteLine("4. Exit");
        return Console.ReadLine();
    }

    // Get note info from user and create a new NoteRequest object
    private static NoteRequest CreateNewNote()
    {
        NoteRequest newNote = new(); // Create new NoteRequest object which is defined in Protos/Note.proto on the server
        Console.WriteLine("\nEnter note topic:");
        newNote.Topic = Console.ReadLine() ?? "";
        Console.WriteLine("\nEnter note title:");
        newNote.Title = Console.ReadLine() ?? "";
        Console.WriteLine("\nEnter note text:");
        newNote.Text = Console.ReadLine() ?? "";
        newNote.Timestamp = DateTime.UtcNow.ToString("MM/dd/yy - HH:mm:ss", CultureInfo.InvariantCulture);
        return newNote;
    }

    // Send the specified note to server and print the response message
    private static async Task SendNote(NoteService.NoteServiceClient client, NoteRequest newNote)
    {
        try
        {
            var reply = await client.CreateNoteAsync(newNote); // gRPC to remote server procedure 'CreateNote'
            Console.WriteLine("\n"+reply.Message+"\n");
        }
        catch (Exception e) { Console.WriteLine($"Exception oquired: {e}"); }
    }

    // Get topics from server and print them
    private static async Task<string?> GetTopics(NoteService.NoteServiceClient client)
    {
        try
        {
            var reply = await client.GetTopicsAsync(new GetTopicsRequest()); // gRPC to remote server procedure 'GetTopics'
            if (reply.Topics.Count == 0) { Console.WriteLine("\nNo topics found!\n"); return null; }
            Console.WriteLine("\nChoose topic number:");
            for (int i = 1; i <= reply.Topics.Count; i++)
            {
                Console.WriteLine($"{i}. {reply.Topics[i-1]}");
            }
            if (!int.TryParse(Console.ReadLine(), out int topicIndex)) { Console.WriteLine("\nWrong input!\n"); return null; } //Return null if can't parsse integer
            if (topicIndex > reply.Topics.Count) { Console.WriteLine("\nOut of index!\n"); return null; } //Return null if answer out of index
            return reply.Topics[topicIndex-1]; // return the chosen topic as string value
        }
        catch (Exception e) { Console.WriteLine($"Exception oquired: {e}"); return null; }
    }

    // Get notes for the specified topic from server and print them
    private static async Task GetNotesPerTopic(NoteService.NoteServiceClient client, string topic)
    {
        try
        {
            var reply = await client.GetNotesPerTopicAsync(new GetNotesPerTopicRequest { Topic = topic }); // gRPC to remote server procedure 'GetNotesPerTopic'

            if (reply.Notes.Count == 0) // No notes for the specified topic
            {
                Console.WriteLine($"\nNo notes for topic of '{topic}' found\n");
                return;
            }

            Console.WriteLine($"\nNotes for topic of {topic}:\n");
            foreach (var note in reply.Notes) // Print each note
            {
                Console.WriteLine($"Title: {note.Title}\nText: {note.Text}\nTimestamp: {note.Timestamp}\n");
            }
        }
        catch (Exception e) { Console.WriteLine($"Exception oquired: {e}"); }
    }

    private static async Task<NoteRequest?> GetWikiArticle(WikiService.WikiServiceClient client, string? article)
    {
        if (article == null) { return null; }
        try
        {
            var res = await client.QueryWikiArticleAsync(new ArticleRequest { Article = article }); //gRPC to remote procedure 'GetWikiArticle'
            Console.WriteLine($"\nArticle: {res.Article} \nSummary: {res.Content} \nLink: {res.Link}\n");

            return new NoteRequest{ Title=res.Article, Text=res.Content };
        }
        catch (Exception e) { Console.WriteLine(e.Message); return null; }
    }
}