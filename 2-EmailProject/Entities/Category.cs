namespace _2_EmailProject.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public List<Message> Messages { get; set; }
    }
}
