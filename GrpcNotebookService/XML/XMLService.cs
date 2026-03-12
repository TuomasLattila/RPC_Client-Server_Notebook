using System.Xml.Linq;

namespace GrpcNotebookService.XML
{
    public class Note // This class represents a note with its atributes
    {
        public string Topic { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Timestamp { get; set; }
        public Note(string topic, string title, string text, string timestamp)
        {
            Topic = topic;
            Title = title;
            Text = text;
            Timestamp = timestamp;
        }
    }

    public class XMLService
    {
        /// TODO:
        /// - Implement locking mechanism to prevent modidying the xml document by multiple threads at the same time
        /// - Implement other needed methods
        /// - 
        ///////////////// 
        

        private readonly string _filePath; // The file path where the XML file will be stored

        public XMLService(string filePath = "db.xml")
        {
            _filePath = filePath;
            DBExists(); // Check if the file exists, if not create it, returns database instance
        }

        public void DBExists()
        {
            if (!File.Exists(_filePath))
            {
                new XDocument(new XElement("data")).Save(_filePath); // Create a new XML file with a root element "data"
            }
        }

        public void AddNode(Note newNote)
        {
            var db = XDocument.Load(_filePath); // Load the XML file 
            var root = db.Root!; // Get the root element of the XML file

            var topics = GetTopics(); // Get the list of existing topics
            XElement note = new XElement("note",
                        new XAttribute("name", newNote.Title),
                        new XElement("text", newNote.Text),
                        new XElement("timestamp", newNote.Timestamp));

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

        public List<string?> GetTopics()
        {
            var root = XDocument.Load(_filePath).Root!; // Load the XML file and get the root element

            return root.Elements().Select(x => x.Attribute("name")?.Value).ToList();
        }
    }
}