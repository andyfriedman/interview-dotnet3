using GroceryStoreAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace GroceryStoreAPI.Data
{
    public class MockStore : IDataRepository<IDataItem>
    {
        private readonly IDictionary<int, IDataItem> _customers = 
            new Dictionary<int, IDataItem>
            {
                [1] = new Customer(1, "a"),
                [2] = new Customer(2, "b"),
                [3] = new Customer(3, "c")
            };

        public IDataItem? Get(int id)
        {
            return _customers.ContainsKey(id) ? _customers[id] : null;
        }

        public IReadOnlyCollection<IDataItem> GetAll()
        {
            return _customers.Values.ToList().AsReadOnly();
        }

        public IDataItem Create(IDataItem item)
        {
            return item;
        }        

        public void Update(IDataItem item)
        {            
        }

        public void Delete(int id)
        {
        }
    }
}
