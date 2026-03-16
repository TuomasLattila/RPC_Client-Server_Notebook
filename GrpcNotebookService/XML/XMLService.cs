using System.Xml.Linq;

namespace GrpcNotebookService.XML
{
    public class Note(string topic, string title, string text, string timestamp) // This class represents a note with its atributes
    {
        public string Topic { get; set; } = topic;
        public string Title { get; set; } = title;
        public string Text { get; set; } = text;
        public string Timestamp { get; set; } = timestamp;
    }

    public class XMLService
    {
        /// TODO:
        /// - 
        ///////////////// 


        private readonly string _filePath; // The file path where the XML file will be stored
        private static readonly Lock _fileLocker = new (); //lock that gets passed to a one thread at a time when entered critical section

        public XMLService(string filePath = "db.xml")
        {
            _filePath = filePath;
            DBExists(); // Check if the file exists, if not create it, returns database instance
        }

        // Check if DB file exists, if not, create new 
        private void DBExists()
        {
            if (!File.Exists(_filePath))
            {
                new XDocument(new XElement("data")).Save(_filePath); // Create a new XML file with a root element "data"
            }
        }

        //Add new note to the DB with the input from client
        public void AddNode(Note newNote)
        {
            XElement note = new("note", new XAttribute("name", newNote.Title), new XElement("text", newNote.Text), new XElement("timestamp", newNote.Timestamp));
            _fileLocker.Enter(); //Enter critical section
            try
            {
                var db = XDocument.Load(_filePath); // Load the XML file 
                var root = db.Root!; // Get the root element of the XML file

                var topics = GetTopics(); // Get the list of existing topics

                if (!topics.Contains(newNote.Topic)) // If the topic of the new note does not exist, create a new topic element
                {
                    root.Add(new XElement("topic", new XAttribute("name", newNote.Topic), note)); // Add the new note to the new topic element
                }
                else // If the topic of the new note already exists, add the new note to the existing topic element
                {
                    var topic = root.Elements().FirstOrDefault(x => x.Attribute("name")?.Value == newNote.Topic)!; // Find the existing topic element
                    topic.Add(note); // Add the new note to the existing topic element

                }
                db.Save(_filePath); // Save the changes to the XML file
            }
            finally { _fileLocker.Exit(); } // Exit critical section
        }

        // Get the list of existing topics from the XML file
        public List<string?> GetTopics() 
        {
            _fileLocker.Enter(); //Enter critical section
            try
            {
                var root = XDocument.Load(_filePath).Root!; // Load the XML file and get the root element
                return root.Elements().Select(x => x.Attribute("name")?.Value).ToList();
            } 
            finally { _fileLocker.Exit(); } // Exit critical section
        }

        // Get the list of notes for a specific topic and return it as a list of NoteRequest objects
        public List<NoteRequest> GetNotesPerTopic(string topic) 
        {
            _fileLocker.Enter(); //Enter critical section
            try
            {
                var root = XDocument.Load(_filePath).Root!; // Load the XML file and get the root element
                var topicElement = root.Elements().FirstOrDefault(x => x.Attribute("name")?.Value == topic); // Find the topic element with the requested topic name
                if (topicElement == null) return []; // If the topic element does not exist, return an empty list

                return topicElement.Elements("note").Select(x => new NoteRequest
                {
                    Topic = topic,
                    Title = x.Attribute("name")?.Value ?? "",
                    Text = x.Element("text")?.Value ?? "",
                    Timestamp = x.Element("timestamp")?.Value ?? ""
                }).ToList(); // Create a list of Note objects from the note elements of the topic element and return it
            } 
            finally { _fileLocker.Exit(); } // Exit critical section
        }
    }
}