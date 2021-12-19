using GroceryStoreAPI.Data;

namespace GroceryStoreAPI.Models
{
    public record Customer : IDataItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public Customer()
        { }

        public Customer(string name) => Name = name;

        public Customer(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }    
}
