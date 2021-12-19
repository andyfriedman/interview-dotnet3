using GroceryStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GroceryStoreAPI.Data
{
    /// <summary>
    /// InMemoryStore - Simple in-memory repository to inject for unit testing.
    /// Constructor accepts a collection of seed data.
    /// </summary>
    public class InMemoryStore : IDataRepository<IDataItem>
    {
        private readonly IDictionary<int, IDataItem> _customers;

        public InMemoryStore(IDictionary<int, IDataItem> customers) => _customers = customers;

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
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var newId = _customers.Keys.OrderByDescending(id => id).First() + 1;
            var name = ((Customer)item)?.Name ?? newId.ToString();
            var customer = new Customer(newId, name);
            
            _customers.Add(newId, customer);
            return customer;
        }        

        public void Update(IDataItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!_customers.ContainsKey(item.Id))
            {
                throw new KeyNotFoundException(item.Id.ToString());
            }

            _customers[item.Id] = (Customer)item;
        }

        public void Delete(int id)
        {
            if (!_customers.ContainsKey(id))
            {
                throw new KeyNotFoundException(id.ToString());
            }

            _customers.Remove(id);
        }
    }
}
