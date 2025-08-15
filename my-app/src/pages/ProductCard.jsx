import React from 'react';
import '../styles/productCard.css';

const ProductCard = ({ 
  product, 
  addToCart, 
  reserveItem, 
  cartItems,
  isAuthenticated
}) => {
  // Считаем количество обычных и зарезервированных товаров
  const regularCount = cartItems
    .filter(item => item.id === product.id && !item.isReserved)
    .reduce((sum, item) => sum + item.quantity, 0);

  const reservedCount = cartItems
    .filter(item => item.id === product.id && item.isReserved)
    .reduce((sum, item) => sum + item.quantity, 0);

  const handleReserve = () => {
    if (!isAuthenticated) {
      alert('Для резервирования необходимо авторизоваться');
      return;
    }
    reserveItem(product);
  };

  return (
    <div className="product-card">
      <img 
        src={product.image || '/placeholder-product.jpg'} 
        alt={product.name} 
        className="product-image"
      />
      <div className="product-info">
        <h3>{product.name}</h3>
        <p className="price">{product.price.toLocaleString('ru-RU')} ₽</p>
        <p className="description">{product.description}</p>
        
        <div className="product-actions">
          <button 
            onClick={() => addToCart(product)}
            className={`add-to-cart-btn ${regularCount > 0 ? 'active' : ''}`}
          >
            В корзину {regularCount > 0 && `(${regularCount})`}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProductCard;