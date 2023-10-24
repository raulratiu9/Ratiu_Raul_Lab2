using System.ComponentModel.DataAnnotations.Schema;

namespace Ratiu_Raul_Lab2.Models
{
    public class Book
    {
        public int ID { get; set; }
        public int AuthorID { get; set; }
        public Author Author { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<PublishedBook> PublishedBooks { get; set; }
    }
}
