using GroceryStoreAPI.Data;
using GroceryStoreAPI.Models;
using GroceryStoreAPI.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GroceryStoreAPI.Controllers
{
    /// <summary>
    /// CustomersController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IDataRepository<IDataItem> _repository;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            IDataRepository<IDataItem> repository,
            ILoggerFactory loggerFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = loggerFactory?.CreateLogger<CustomersController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                _logger.LogInformation($"{nameof(GetAll)} endpoint called");
                var customers = Enumerable.Cast<Customer>(_repository.GetAll());
                return customers != null ? new OkObjectResult(customers) : new NotFoundResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetAll)} error: {ex.Message}");
                return new InternalServerErrorObjectResult(ex);
            }
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogInformation($"{nameof(Get)} endpoint called with customer id {id}");
                var customer = _repository.Get(id) as Customer;
                return customer != null ? new OkObjectResult(customer) : new NotFoundObjectResult(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Get)} error: {ex.Message}");
                return new InternalServerErrorObjectResult(ex);
            }
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            try
            {
                if (customer is null)
                {
                    throw new ArgumentNullException(nameof(customer));
                }

                _logger.LogInformation($"{nameof(Create)} endpoint called with customer id {customer.Id}");
                var newCustomer = _repository.Create(customer);
                return new CreatedResult($"{Request?.Path}/{newCustomer.Id}", JsonConvert.SerializeObject(newCustomer));
            }
            catch (ArgumentNullException argx)
            {
                _logger.LogError($"{nameof(Create)} error: {argx.Message}");
                return new BadRequestObjectResult(argx);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Create)} error: {ex.Message}");
                return new InternalServerErrorObjectResult(ex);
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(Customer customer)
        {
            try
            {
                if (customer is null)
                {
                    throw new ArgumentNullException(nameof(customer));
                }

                _logger.LogInformation($"{nameof(Update)} endpoint called with customer id {customer.Id}");
                _repository.Update(customer);
                return new NoContentResult();
            }
            catch (ArgumentNullException argx)
            {
                _logger.LogError($"{nameof(Update)} error: {argx.Message}");
                return new BadRequestObjectResult(argx);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"{nameof(Update)} error: Invalid customer id ({customer.Id})");
                return new NotFoundObjectResult(customer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Update)} error: {ex.Message}");
                return new InternalServerErrorObjectResult(ex);
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"{nameof(Delete)} endpoint called with customer id {id}");
                _repository.Delete(id);
                return new NoContentResult();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"{nameof(Delete)} error: Invalid customer id ({id})");
                return new NotFoundObjectResult(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Delete)} error: {ex.Message}");
                return new InternalServerErrorObjectResult(ex);
            }
        }
    }
}
