import React, { useState, useEffect } from 'react';
import './OrderSummaryBySupplier.css';

const OrderSummaryBySupplier = () => {
  const [orderSummary, setOrderSummary] = useState([]);

  useEffect(() => {
    const fetchOrderSummary = async () => {
        try {
            const response = await fetch('https://localhost:7120/api/northwind/products/totals-by-supplier');
            const data = await response.json();
            setOrderSummary(data);
        } catch (error) {
            console.error('Error fetching order summary:', error);
        }
    };

    fetchOrderSummary();
}, []);

return (
  <div>
    <h1>Order Summary By Supplier</h1>
    <table>
      <thead>
        <tr>
          <th>Supplier ID</th>
          <th>Company Name</th>
          <th>Orders Total</th>
        </tr>
      </thead>
      <tbody>
        {orderSummary.map((item) => (
          <tr key={item.supplierID}>
            <td>{item.supplierID}</td>
            <td>{item.supplierName}</td>
            <td>{item.totalAmount}</td>
          </tr>
        ))}
      </tbody>
    </table>
  </div>

  );
};

export default OrderSummaryBySupplier;
