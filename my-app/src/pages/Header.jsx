import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import '../styles/header.css';

export default function Header({ isAuthenticated, userData, onLogout }) {
  const [showProfileMenu, setShowProfileMenu] = useState(false);
  const [cartItemsCount, setCartItemsCount] = useState(0);
  const [currentUserData, setCurrentUserData] = useState(userData);
  const navigate = useNavigate();

  // Обновляем данные пользователя при изменении пропсов
  useEffect(() => {
    setCurrentUserData(userData);
  }, [userData]);

  // Определяем роль пользователя
  const userRole = currentUserData?.rolename || '';
  const isEmployee = userRole === 'admin' || userRole === 'employee';

  // Получаем ФИО в зависимости от типа пользователя
  const getUserDisplayName = () => {
    if (currentUserData?.employeeId) {
      return currentUserData.employeeFio || currentUserData.email;
    }
    return currentUserData?.fio || currentUserData?.email || 'Профиль';
  };

  // Функция для подсчета товаров в корзине
  const getCartCount = () => {
    try {
      const cart = localStorage.getItem('cart');
      if (!cart) return 0;
      return JSON.parse(cart).reduce((total, item) => total + (item.quantity || 1), 0);
    } catch {
      return 0;
    }
  };

  useEffect(() => {
    const updateCartCount = () => setCartItemsCount(getCartCount());
    
    updateCartCount();
    window.addEventListener('cartUpdated', updateCartCount);
    window.addEventListener('storage', (e) => e.key === 'cart' && updateCartCount());

    return () => {
      window.removeEventListener('cartUpdated', updateCartCount);
      window.removeEventListener('storage', updateCartCount);
    };
  }, []);

  const handleLogout = () => {
    onLogout();
    navigate('/');
    setShowProfileMenu(false);
  };

  return (
    <header className="navbar">
      <div className="nav-container">
        <Link to="/" className="logo">Музыкальный магазин</Link>
        
        <nav className="main-nav">
          <div className="nav-links-container">
            <Link to="/catalog" className="nav-link">Каталог товаров</Link>
            
              <Link to="/cart" className="nav-link cart-link">
                Корзина
                {cartItemsCount > 0 && (
                  <span className="cart-badge">{cartItemsCount}</span>
                )}
              </Link>

            {/* Панель управления для сотрудников */}
            {isEmployee && (
              <Link to="/admin/tables" className="nav-link admin-link">
                <span className="admin-icon">⚙️</span> Панель управления
              </Link>
            )}
          </div>
        </nav>

        {isAuthenticated ? (
          <div className="profile-menu-container">
            <button
              className="profile-button"
              onClick={() => setShowProfileMenu(!showProfileMenu)}
              aria-expanded={showProfileMenu}
            >
              {getUserDisplayName()}
              {isEmployee && <span className="employee-badge"></span>}
              <span className="dropdown-arrow">▼</span>
            </button>

            {showProfileMenu && (
              <div className="profile-dropdown">
                <Link
                  to="/profile"
                  className="dropdown-item"
                  onClick={() => setShowProfileMenu(false)}
                >
                  Мой профиль
                </Link>

                <button
                  onClick={handleLogout}
                  className="dropdown-item logout-item"
                >
                  Выйти
                </button>
              </div>
            )}
          </div>
        ) : (
          <div className="auth-links">
            <Link to="/login" className="nav-link">Вход</Link>
            <Link to="/register" className="nav-link register-link">
              Регистрация
            </Link>
          </div>
        )}
      </div>
    </header>
  );
}