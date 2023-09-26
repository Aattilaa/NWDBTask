import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import ProductPage from './pages/ProductPage';
import OrderSummaryBySupplierPage from './pages/OrderSummaryBySupplierPage';

function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/products" element={<ProductPage />} />
          <Route path="/order-summary" element={<OrderSummaryBySupplierPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
