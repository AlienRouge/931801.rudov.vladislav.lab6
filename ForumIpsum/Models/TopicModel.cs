using System.ComponentModel.DataAnnotations;

namespace ForumIpsum.Models
{
    public class TopicModel
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string CreateDate { get; set; }
        public string EditDate { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Text { get; set; }

        public byte[] Image1 { get; set; }

        public byte[] Image2 { get; set; }

        public byte[] Image3 { get; set; }

        public string GetEditDate()
        {
            return EditDate != null ? $", (edited {EditDate})" : "";
        }
    }
}