import { useState, useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import axios from 'axios';
import Header from './pages/Header';
import Footer from './pages/Footer';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/HomePage';
import ProfilePage from './pages/ProfilePage';
import Catalog from './pages/Catalog';
import CartPage from './pages/Cart';
import CheckoutPage from './pages/CheckoutPage';
import AdminTablesPage from './pages/AdminTable';
import './App.css';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userData, setUserData] = useState(null);
  const [cartItems, setCartItems] = useState([]);
  const [reservedProducts, setReservedProducts] = useState([]);
  const [modal, setModal] = useState({ show: false, title: '', message: '' });
  const [reservedItems, setReservedItems] = useState([]);

  useEffect(() => {
  const checkAuth = async () => {
    const token = localStorage.getItem('token');
    if (!token) return; 

    try {
      const response = await axios.get('http://localhost:5000/api/auth/me', {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.data) {
        setIsAuthenticated(true);
        setUserData(response.data); 
        localStorage.setItem('userData', JSON.stringify(response.data));
      }
    } catch (error) {
      console.error('Ошибка проверки токена:', error);
      localStorage.removeItem('token'); 
      localStorage.removeItem('userData');
    }
  };

  checkAuth(); 
}, []);
  
  const ProtectedAdminRoute = ({ children }) => {
    const token = localStorage.getItem('token');
    
    if (!token) {
      return <Navigate to="/login" replace />;
    }

    const userData = JSON.parse(localStorage.getItem('userData'));
    const userRole = userData?.rolename || '';

    if (userRole !== 'admin' && userRole !== 'employee') {
      return <Navigate to="/" replace />;
    }

    return children;
  };

  const showModal = (title, message) => {
    setModal({ show: true, title, message });
    setTimeout(() => setModal({...modal, show: false}), 3000);
  };

  const handleRemoveItem = (itemId) => {

  if (reservedItems.includes(itemId)) {
    cancelReservation(itemId);
  }
  removeFromCart(itemId);
};

const addToCart = (product) => {
  setCartItems(prev => {
    const existingItem = prev.find(item => item.id === product.id && !item.isReserved);
    return existingItem 
      ? prev.map(item => 
          item.id === product.id && !item.isReserved
            ? { ...item, quantity: item.quantity + 1 }
            : item
        )
      : [...prev, { ...product, quantity: 1, isReserved: false }];
  });
};

const reserveItem = (product) => {
  setCartItems(prev => {
    const reservedPrice = Math.round(product.price * 0.3); // 30% от цены
    const existingReserved = prev.find(item => item.id === product.id && item.isReserved);
    
    return existingReserved
      ? prev.map(item =>
          item.id === product.id && item.isReserved
            ? { 
                ...item, 
                quantity: item.quantity + 1,
                originalPrice: item.originalPrice || item.price, // Сохраняем оригинальную цену
                price: reservedPrice
              }
            : item
        )
      : [
          ...prev, 
          { 
            ...product, 
            quantity: 1, 
            isReserved: true,
            originalPrice: product.price, // Сохраняем оригинальную цену
            price: reservedPrice // Устанавливаем цену 30% от оригинальной
          }
        ];
  });
};

  const cancelReservation = (productId) => {
    setReservedProducts(prev => prev.filter(id => id !== productId)); // Было setReservedItems
    showModal('Успех', 'Товары из корзины удалены');
    return true;
  };

  const handleAuth = async (credentials) => {
    try {
      const response = await axios.post(
        'http://localhost:5000/api/auth/login',
        {
          email: credentials.email.trim(),
          password: credentials.password,
        },
        { headers: { 'Content-Type': 'application/json' } }
      );
      
      setIsAuthenticated(true);
      setUserData(response.data.user);
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('userData', JSON.stringify(response.data.user));
      return true;
    } catch (error) {
      console.error('Ошибка авторизации:', error.response?.data);
      showModal(
        'Ошибка',
        error.response?.data?.message || 'Неверный email или пароль'
      );
      return false;
    }
  };

  const handleLogout = () => {
    setIsAuthenticated(false);
    setUserData(null);
    localStorage.removeItem('token');
    localStorage.removeItem('userData');
  };

  const ProtectedRoute = ({ children }) => {
    const token = localStorage.getItem('token');
    
    if (!token) {
      return <Navigate to="/login" replace />;
    }

    return children;
  };

  const removeFromCart = (id) => {
    setCartItems(prevItems => prevItems.filter(item => item.id !== id));
  };

  const updateQuantity = (id, newQuantity) => {
    setCartItems(prevItems => 
      prevItems.map(item => 
        item.id === id ? {...item, quantity: Math.max(1, newQuantity)} : item
      )
    );
  };

  const clearCart = () => {
    setCartItems([]); // или другая логика очистки состояния
  };

  return (
    <div className="app">
      {modal.show && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>{modal.title}</h3>
            <p>{modal.message}</p>
          </div>
        </div>
      )}
      
      <Header 
        isAuthenticated={isAuthenticated}
        userData={userData}
        onLogout={handleLogout}
        cartItemsCount={cartItems.reduce((sum, item) => sum + item.quantity, 0)}
      />
      
      <main className="main-content">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route 
            path="/login" 
            element={
              isAuthenticated 
                ? <Navigate to="/profile" replace /> 
                : <LoginPage onAuth={handleAuth} />
            } 
          />
          <Route path="/register" element={<RegisterPage />} />
          <Route 
            path="/profile" 
            element={
              isAuthenticated 
                ? <ProfilePage userData={userData} onLogout={handleLogout} /> 
                : <Navigate to="/login" replace />
            } 
          />
          <Route 
            path="/catalog" 
            element={
              <Catalog 
                addToCart={addToCart}
                reserveItem={reserveItem}
                reservedItems={reservedProducts}
                cartItems={cartItems}
              />
            } 
          />
          <Route
            path="/admin/tables"
            element={
              <ProtectedAdminRoute>
                <AdminTablesPage />
              </ProtectedAdminRoute>
            }
          /> 
          <Route path="/admin" element={<Navigate to="/admin/tables" replace />} />
          <Route 
            path="/cart" 
            element={
              <CartPage 
                cartItems={cartItems}
                onClick={() => handleRemoveItem(item.id)}
                removeFromCart={removeFromCart}  
                updateQuantity={updateQuantity}
                clearCart={clearCart}
                reservedItems={reservedProducts}
                cancelReservation={cancelReservation}
              />
            } 
          />
          <Route
            path="/profile"
            element={
              <ProtectedRoute>
                <ProfilePage userData={userData} onLogout={handleLogout} />
              </ProtectedRoute>
            }
          />
          <Route path="/checkout" element={<CheckoutPage />} />
        </Routes>
      </main>
      <Footer />
    </div>
  );
}

export default App;