using System.Collections.Generic;

namespace GroceryStoreAPI.Data
{
    public interface IDataRepository<TItem>
    {
        public TItem? Get(int id);
        public IReadOnlyCollection<TItem> GetAll();
        public TItem Create(TItem item);
        public void Update(TItem item);
        public void Delete(int id);      
    }
}
