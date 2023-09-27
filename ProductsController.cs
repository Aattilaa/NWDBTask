using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NWDBTask.Controllers
{
    [Route("api/northwind/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("totals-by-supplier")]
        public async Task<IActionResult> GetTotalsBySupplier()
        {
            try
            {
                var products = await _productService.GetProducts();
                var orderDetails = await _productService.GetOrderDetails();
                var suppliers = await _productService.GetSuppliers();

                var results = _productService.GetTotalsBySupplier(products, orderDetails, suppliers);

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
