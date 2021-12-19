using Microsoft.AspNetCore.Mvc;

namespace GroceryStoreAPI.Results
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value)
            : base(value) => StatusCode = 500;
    }
}
