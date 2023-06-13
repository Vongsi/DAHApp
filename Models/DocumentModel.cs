namespace DAHApp.Models
{
    public class DocumentModel
    {
        public Guid Id { get; set; }    
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string Type { get; set; }
    }
}
