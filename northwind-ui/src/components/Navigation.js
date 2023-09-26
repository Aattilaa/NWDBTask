import React from 'react';
import { Link } from 'react-router-dom';

const Navigation = () => {
  return (
    <div>
      <Link to="/products">Products</Link>
      <Link to="/order-summary">Order Summary</Link>
    </div>
  );
};

export default Navigation;