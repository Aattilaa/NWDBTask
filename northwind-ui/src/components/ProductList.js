import React, { useState, useEffect } from 'react';
import './ProductList.css'

function ProductList() {
  const [products, setProducts] = useState([]);
  const [error, setError] = useState(null);
  const [searchQuery, setSearchQuery] = useState('');

  const fetchAllProducts = async () => {
    try {
      const response = await fetch('https://localhost:7078/api/northwind/Products');
      const data = await response.json();
      console.log(data); 
      setProducts(data);
    } catch (error) {
      setError(error);
    }
  };

  const handleSearch = (e) => {
    setSearchQuery(e.target.value);
  };

  useEffect(() => {
    fetchAllProducts();
  }, []);

  const filteredProducts = products.filter(product =>
    product.productName.toLowerCase().includes(searchQuery.toLowerCase()) &&
    !product.discontinued);

  if (error) {
    return <div>Error fetching data: {error.message}</div>;
  }

  return (
    <div className="container">
      <h1>Product List</h1>

      <input
        type="text"
        value={searchQuery}
        onChange={handleSearch}
        placeholder="Search products..."
      />

      <table>
        <thead>
          <tr>
            <th>Product Name</th>
            <th>Price</th>
          </tr>
        </thead>
        <tbody>
          {filteredProducts.map(product => (
            <tr key={product.productID}>
              <td>{product.productName}</td>
              <td>{product.unitPrice}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default ProductList;
