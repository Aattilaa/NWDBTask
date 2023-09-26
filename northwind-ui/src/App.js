import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import ProductPage from './pages/ProductPage';

function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/products" element={<ProductPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
