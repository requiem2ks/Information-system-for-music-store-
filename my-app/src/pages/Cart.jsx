import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import '../styles/cart.css';

const Cart = ({
  cartItems: propsCartItems,
  removeFromCart,
  updateQuantity,
  clearCart,
  cancelReservation
}) => {
  const [localCartItems, setLocalCartItems] = useState(() => {
    try {
      const savedCart = localStorage.getItem('cart');
      return savedCart ? JSON.parse(savedCart) : propsCartItems || [];
    } catch (error) {
      console.error('Ошибка при чтении корзины из localStorage:', error);
      return propsCartItems || [];
    }
  });

  const cartItems = localCartItems.length > 0 ? localCartItems : propsCartItems || [];

  useEffect(() => {
    try {
      localStorage.setItem('cart', JSON.stringify(localCartItems));
    } catch (error) {
      console.error('Ошибка при сохранении корзины в localStorage:', error);
    }
  }, [localCartItems]);

  useEffect(() => {
    if (propsCartItems && propsCartItems.length > 0) {
      setLocalCartItems(prevItems => {
        const newItems = [...prevItems];
        propsCartItems.forEach(newItem => {
          if (!prevItems.some(item => item.id === newItem.id)) {
            newItems.push(newItem);
          }
        });
        return newItems;
      });
    }
  }, [propsCartItems]);

  const handleRemoveItem = (item) => {
    const newItems = localCartItems.filter(i => i.id !== item.id);
    setLocalCartItems(newItems);
    if (item.isReserved && cancelReservation) {
      cancelReservation(item.id);
    }
    if (removeFromCart) {
      removeFromCart(item.id);
    }
  };

  const handleUpdateQuantity = (id, newQuantity) => {
    const updatedItems = localCartItems.map(item =>
      item.id === id ? { ...item, quantity: Math.max(1, newQuantity) } : item
    );
    setLocalCartItems(updatedItems);
    if (updateQuantity) {
      updateQuantity(id, newQuantity);
    }
  };

  const handleClearCart = () => {
    if (localCartItems.some(item => item.isReserved) && cancelReservation) {
      localCartItems.forEach(item => {
        if (item.isReserved) cancelReservation(item.id);
      });
    }
    setLocalCartItems([]);
    if (clearCart) {
      clearCart();
    }
  };

  const total = cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
  const reservedItems = cartItems.filter(item => item.isReserved);
  const regularItems = cartItems.filter(item => !item.isReserved);

  const renderItem = (item) => (
    <div
      key={item.id}
      className={`cart-item ${item.isReserved ? 'reserved-item' : ''}`}
    >
      <div className="item-image-container">
        <img
          src={item.image}
          alt={item.name}
          className="cart-item-image"
          onError={(e) => {
            e.target.onerror = null;
            e.target.src = '/placeholder-product.jpg';
          }}
        />
        {item.isReserved && (
          <div className="reserved-badge">Резерв</div>
        )}
      </div>

      <div className="item-details">
        <h3 className="item-title">{item.name}</h3>
        <p className="item-price">{item.price.toLocaleString('ru-RU')} ₽</p>
        {item.isReserved && (
          <p className="reserved-text">(Зарезервировано)</p>
        )}
      </div>

      <div className="quantity-controls">
        <button
          onClick={() => handleUpdateQuantity(item.id, item.quantity - 1)}
          disabled={item.quantity <= 1}
          className="quantity-btn minus"
        >
          -
        </button>
        <span className="quantity-value">{item.quantity}</span>
        <button
          onClick={() => handleUpdateQuantity(item.id, item.quantity + 1)}
          className="quantity-btn plus"
        >
          +
        </button>
      </div>

      <div className="item-total">
        <p>{(item.price * item.quantity).toLocaleString('ru-RU')} ₽</p>
      </div>

      <button
        onClick={() => handleRemoveItem(item)}
        className="remove-btn"
      >
        Удалить
      </button>
    </div>
  );

  return (
    <div className="cart-layout">
      <div className="cart-items-section">
        <div className="cart-header">
          <h1>Ваша корзина</h1>
          {cartItems.length > 0 && (
            <button onClick={handleClearCart} className="clear-cart-btn">
              Очистить корзину
            </button>
          )}
        </div>

        {cartItems.length === 0 ? (
          <div className="empty-cart">
            <p>Ваша корзина пуста</p>
            <Link to="/catalog" className="continue-shopping-btn">
              Вернуться в каталог
            </Link>
          </div>
        ) : (
          <>
            {reservedItems.length > 0 && (
              <div className="reserved-section">
                <h2>Зарезервированные товары</h2>
                <div className="cart-items-list">
                  {reservedItems.map(renderItem)}
                </div>
              </div>
            )}

            {regularItems.length > 0 && (
              <div className="regular-section">
                <h2>Товары для покупки</h2>
                <div className="cart-items-list">
                  {regularItems.map(renderItem)}
                </div>
              </div>
            )}
          </>
        )}
      </div>

      {cartItems.length > 0 && (
        <div className="cart-summary-section">
          <div className="summary-box">
            <h2>Итого: {total.toLocaleString('ru-RU')} ₽</h2>
            <Link 
              to="/checkout" 
              state={{ 
                total,
                itemsCount: cartItems.reduce((count, item) => count + item.quantity, 0),
                cartItems: cartItems.map(item => ({
                  id: item.id,
                  name: item.name,
                  price: item.price,
                  quantity: item.quantity,
                  productid: item.id, 
                  isReserved: item.isReserved || false
                }))
              }}
              className="checkout-btn"
            >
              Оформить заказ
            </Link>
          </div>
        </div>
      )}
    </div>
  );
};

export default Cart;