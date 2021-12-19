using GroceryStoreAPI.Data;
using GroceryStoreAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GroceryStoreAPI.Tests
{
    public class DataRepositoryTests
    {
        private IDataRepository<IDataItem> _repository;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = ServiceProvider.BuildServices();
            _repository = serviceProvider.GetRequiredService<IDataRepository<IDataItem>>();
        }

        [Test]
        public void Get_all()
        {
            var customers = _repository.GetAll();
            Assert.IsNotNull(customers);
            Assert.AreEqual(customers.Count(), TestData.Customers.Count());
        }

        [Test]
        public void Get_one()
        {
            var customer = _repository.Get(1);
            Assert.IsNotNull(customer);
            Assert.AreEqual(customer, TestData.Customers[1]);
        }

        [Test]
        public void Get_one_invalid_id_returns_null()
        {
            var customer = _repository.Get(5);
            Assert.IsNull(customer);
        }

        [Test]
        public void Create_success()
        {
            var beforeCount = _repository.GetAll().Count();

            var newCustomer = _repository.Create(new Customer("d"));
            Assert.IsNotNull(newCustomer);

            var afterCount = _repository.GetAll().Count();
            Assert.AreEqual(afterCount, beforeCount + 1);
            
            var customer = _repository.Get(newCustomer.Id);
            Assert.AreEqual(newCustomer, customer);
        }

        [Test]
        public void Create_null_throws_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Create(null));
        }

        [Test]
        public void Update_success()
        {
            var customer = _repository.Get(1);
            ((Customer)customer).Name = "x";

            _repository.Update(customer);

            customer = _repository.Get(1);
            Assert.AreEqual(((Customer)customer).Name, "x");
        }

        [Test]
        public void Update_null_throws_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Update(null));
        }

        [Test]
        public void Update_invalid_id_throws_exception()
        {
            Assert.Throws<KeyNotFoundException>(() => _repository.Update(new Customer(5, "x")));
        }

        [Test]
        public void Delete_success()
        {
            var beforeCount = _repository.GetAll().Count();

            _repository.Delete(1);

            var afterCount = _repository.GetAll().Count();
            Assert.AreEqual(afterCount, beforeCount - 1);
        }

        [Test]
        public void Delete_invalid_id_throws_exception()
        {
            Assert.Throws<KeyNotFoundException>(() => _repository.Delete(5));
        }
    }
}