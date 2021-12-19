using System.Collections.Generic;

namespace GroceryStoreAPI.Models
{
    public class CustomerCollection
    {
        public IEnumerable<Customer>? Customers { get; set; }
    }
}
