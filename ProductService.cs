using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NWDBTask.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://services.odata.org/V4/Northwind/Northwind.svc/");
    }

    public async Task<List<Product>> GetProducts()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("Products");
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Product>>(JObject.Parse(responseData)["value"].ToString());
    }

    public async Task<List<OrderDetail>> GetOrderDetails()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("Order_Details");
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<OrderDetail>>(JObject.Parse(responseData)["value"].ToString());
    }

    public async Task<List<Supplier>> GetSuppliers()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("Suppliers");
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Supplier>>(JObject.Parse(responseData)["value"].ToString());
    }

    public IEnumerable<dynamic> GetTotalsBySupplier(List<Product> products, List<OrderDetail> orderDetails, List<Supplier> suppliers)
    {
        var totalsByProduct = orderDetails
            .GroupBy(od => od.ProductID)
            .Select(group => new
            {
                ProductID = group.Key,
                TotalAmount = group.Sum(od => od.UnitPrice * od.Quantity * (1 - od.Discount))
            });

        var totalsBySupplier = products
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

        var results = totalsBySupplier
            .Join(suppliers, t => t.SupplierID, s => s.SupplierID, (t, s) => new
            {
                SupplierID = t.SupplierID,
                SupplierName = s.CompanyName,
                TotalAmount = t.TotalAmount
            });

        return results;
    }
}
