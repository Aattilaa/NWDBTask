using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NWDBTask.Models;

namespace NWDBTask
{
    [Route("api/northwind/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://services.odata.org/V4/Northwind/Northwind.svc/");
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("Products");
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();

                return Content(responseData, "application/json");
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
                HttpResponseMessage productsResponse = await _httpClient.GetAsync("Products");
                productsResponse.EnsureSuccessStatusCode();

                HttpResponseMessage orderDetailsResponse = await _httpClient.GetAsync("Order_Details");
                orderDetailsResponse.EnsureSuccessStatusCode();

                productsResponse.EnsureSuccessStatusCode();
                orderDetailsResponse.EnsureSuccessStatusCode();

                var productsData = await productsResponse.Content.ReadAsStringAsync();
                var orderDetailsData = await orderDetailsResponse.Content.ReadAsStringAsync();

                var productsList = JsonConvert.DeserializeObject<List<Product>>(JObject.Parse(productsData)["value"].ToString());
                var orderDetailsList = JsonConvert.DeserializeObject<List<OrderDetail>>(JObject.Parse(orderDetailsData)["value"].ToString());

                var totalsByProduct = orderDetailsList
                    .GroupBy(od => od.ProductID)
                    .Select(group => new
                    {
                        ProductID = group.Key,
                        TotalAmount = group.Sum(od => od.UnitPrice * od.Quantity * (1 - od.Discount))
                    });

                var totalsBySupplier = productsList
                    .Join(totalsByProduct, p => p.ProductID, t => t.ProductID, (p, t) => new
                    {
                        SupplierID = p.SupplierID,
                        TotalAmount = t.TotalAmount
                    })
                    .GroupBy(p => p.SupplierID)
                    .Select(group => new
                    {
                        SupplierID = group.Key,
                        TotalAmount = group.Sum(p => p.TotalAmount)
                    });

                HttpResponseMessage suppliersResponse = await _httpClient.GetAsync("Suppliers");
                suppliersResponse.EnsureSuccessStatusCode();
                var suppliersData = await suppliersResponse.Content.ReadAsStringAsync();
                var suppliersList = JsonConvert.DeserializeObject<List<Supplier>>(JObject.Parse(suppliersData)["value"].ToString());

                var results = totalsBySupplier
                    .Join(suppliersList, t => t.SupplierID, s => s.SupplierID, (t, s) => new
                    {
                        SupplierID = t.SupplierID,
                        SupplierName = s.CompanyName,
                        TotalAmount = t.TotalAmount
                    });

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}