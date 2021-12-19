using GroceryStoreAPI.Controllers;
using GroceryStoreAPI.Data;
using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GroceryStoreAPI.Tests
{
    public class CustomerControllerTests
    {        
        private CustomersController _customersController;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = ServiceProvider.BuildServices();
            
            var repository = serviceProvider.GetRequiredService<IDataRepository<IDataItem>>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            _customersController = new CustomersController(repository, loggerFactory);
        }

        [Test]
        public void Get_all()
        {
            var response = _customersController.GetAll();
            Assert.IsInstanceOf<OkObjectResult>(response);

            var customers = ((ObjectResult)response).Value as IEnumerable<IDataItem>;
            Assert.IsNotNull(customers);
            Assert.AreEqual(customers.Count(), TestData.Customers.Count());
        }

        [Test]
        public void Get_one()
        {
            var response = _customersController.Get(1);
            Assert.IsInstanceOf<OkObjectResult>(response);

            var customer = ((ObjectResult)response).Value as IDataItem;
            Assert.IsNotNull(customer);
            Assert.AreEqual(customer, TestData.Customers[1]);
        }

        [Test]
        public void Get_one_invalid_id_returns_not_found()
        {
            var response = _customersController.Get(5);
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
        }

        [Test]
        public void Create_success()
        {
            var response = _customersController.GetAll();
            var customers = ((ObjectResult)response).Value as IEnumerable<IDataItem>;
            var beforeCount = customers.Count();

            response = _customersController.Create(new Customer("d"));
            Assert.IsInstanceOf<CreatedResult>(response);
            
            var newCustomerJson = ((ObjectResult)response).Value as string;
            Assert.IsNotNull(newCustomerJson);
            IDataItem newCustomer = JsonConvert.DeserializeObject<Customer>(newCustomerJson);
            
            response = _customersController.GetAll();
            customers = ((ObjectResult)response).Value as IEnumerable<IDataItem>;
            var afterCount = customers.Count();
            Assert.AreEqual(afterCount, beforeCount + 1);

            response = _customersController.Get(newCustomer.Id);
            var customer = ((ObjectResult)response).Value as IDataItem;
            Assert.AreEqual(newCustomer, customer);
        }

        [Test]
        public void Create_null_returns_bad_request()
        {
            var response = _customersController.Create(null);
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public void Update_success()
        {
            var response = _customersController.Get(1);
            var customer = ((ObjectResult)response).Value as Customer;
            customer.Name = "x";

            response = _customersController.Update(customer);
            Assert.IsInstanceOf<NoContentResult>(response);

            response = _customersController.Get(1);
            customer = ((ObjectResult)response).Value as Customer;
            Assert.AreEqual(customer.Name, "x");
        }

        [Test]
        public void Update_null_returns_bad_request()
        {
            var response = _customersController.Update(null);
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public void Update_invalid_id_returns_not_found()
        {
            var response = _customersController.Update(new Customer(5, "x"));
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
        }

        [Test]
        public void Delete_success()
        {
            var response = _customersController.GetAll();
            var customers = ((ObjectResult)response).Value as IEnumerable<IDataItem>;
            var beforeCount = customers.Count();

            response = _customersController.Delete(1);
            Assert.IsInstanceOf<NoContentResult>(response);

            response = _customersController.GetAll();
            customers = ((ObjectResult)response).Value as IEnumerable<IDataItem>;
            var afterCount = customers.Count();
            Assert.AreEqual(afterCount, beforeCount - 1);
        }

        [Test]
        public void Delete_invalid_id_returns_not_found()
        {
            var response = _customersController.Delete(5);
            Assert.IsInstanceOf<NotFoundObjectResult>(response);
        }
    }
}