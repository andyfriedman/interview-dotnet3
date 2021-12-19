using GroceryStoreAPI.Data;
using GroceryStoreAPI.Models;
using System.Collections.Generic;

namespace GroceryStoreAPI.Tests
{
    public class TestData
    {
        public static readonly IReadOnlyDictionary<int, IDataItem> Customers = 
            new Dictionary<int, IDataItem>
            {
                [1] = new Customer(1, "a"),
                [2] = new Customer(2, "b"),
                [3] = new Customer(3, "c")
            };
    }
}
