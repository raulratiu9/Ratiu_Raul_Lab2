using Ratiu_Raul_Lab2.Models;

namespace LibraryModel.Models
{
    public class City
    {
        public int ID { get; set; }
        public string CityName { get; set; }
        public ICollection<Customer>? Customers { get; set; }
    }
}
