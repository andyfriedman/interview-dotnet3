using GroceryStoreAPI.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Data
{
    /// <summary>
    /// FileBackedMemoryStore - High performance in-memory cache that syncs data to a local file in the background.
    /// </summary>
    public class FileBackedMemoryStore : IDataRepository<IDataItem>
    {
        private static readonly Lazy<ConcurrentDictionary<int, IDataItem>> _memoryStore = 
            new Lazy<ConcurrentDictionary<int, IDataItem>>(() => ReadDataFromFile(), true);

        private static volatile int _lastId = _memoryStore.Value.Keys.OrderByDescending(id => id).FirstOrDefault();

        private readonly Task? _saveDataProc;
        private readonly AutoResetEvent _writeDataEvent = new AutoResetEvent(false);

        private readonly ILogger<FileBackedMemoryStore> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        public FileBackedMemoryStore(
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<FileBackedMemoryStore>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _saveDataProc = Task.Run(SaveDataProc);            
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IDataItem? Get(int id)
        {
            try
            {
                return _memoryStore.Value.GetValueOrDefault(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(Get)}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// GetAll
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IDataItem> GetAll()
        {
            try
            {
                return _memoryStore.Value.Values.ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetAll)}: {ex.Message}");
                return Enumerable.Empty<IDataItem>().ToList();
            }
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IDataItem Create(IDataItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                item.Id = Interlocked.Increment(ref _lastId);
                if (!_memoryStore.Value.TryAdd(item.Id, item))
                {
                    throw new ArgumentException($"An item already exists with id {item.Id}");
                }

                _writeDataEvent.Set();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(Create)}: {ex.Message}");
                throw;
            }            
        }            

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="item"></param>
        public void Update(IDataItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                if (!_memoryStore.Value.ContainsKey(item.Id))
                {
                    throw new KeyNotFoundException(item.Id.ToString());
                }

                _memoryStore.Value[item.Id] = item;
                _writeDataEvent.Set();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(Update)}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            try
            {
                if (!_memoryStore.Value.ContainsKey(id))
                {
                    throw new KeyNotFoundException(id.ToString());
                }

                _memoryStore.Value.TryRemove(id, out IDataItem _);
                _writeDataEvent.Set();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(Delete)}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// SaveDataProc - The idea here is that we have a single thread managing all file writes,
        /// throttled by an auto reset event (triggered by a create/update/delete). This prevents  
        /// the database file from being bombarded by multiple threads, getting bogged down with 
        /// file locks, and potentially corrupting the file. We're trading off atomicity for 
        /// performance and stability.
        /// </summary>
        private void SaveDataProc()
        {
            while (true)
            {
                try
                {
                    _writeDataEvent.WaitOne();
                    WriteDataToFile();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{nameof(SaveDataProc)} error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// ReadDataFromFile
        /// </summary>
        /// <returns></returns>
        private static ConcurrentDictionary<int, IDataItem> ReadDataFromFile()
        {
            var databaseJson = File.ReadAllText("database.json");
            var customerCollection = JsonConvert.DeserializeObject<CustomerCollection>(databaseJson);
            if (customerCollection?.Customers is null)
            {
                return new ConcurrentDictionary<int, IDataItem>();
            }

            var dataItems = Enumerable.Cast<IDataItem>(customerCollection.Customers);
            return new ConcurrentDictionary<int, IDataItem>(dataItems.ToDictionary(x => x.Id));
        }

        /// <summary>
        /// WriteDataToFile
        /// </summary>
        private static void WriteDataToFile()
        {
            var customers = Enumerable.Cast<Customer>(_memoryStore.Value.Values);
            var customersCollection = new CustomerCollection { Customers = customers };
            var databaseJson = JsonConvert.SerializeObject(customersCollection, Formatting.Indented);

            File.WriteAllText("database.json", databaseJson);
        }
    }
}
