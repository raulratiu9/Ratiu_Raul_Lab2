using LibraryModel.Models;

namespace Ratiu_Raul_Lab2.Models.LibraryViewModels
{
    public class CustomersIndexData
    {
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<City> Cities { get; set; }
    }
}
