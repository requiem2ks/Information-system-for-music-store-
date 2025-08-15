import { useEffect, useState } from 'react';
import axios from 'axios';
import '../styles/profile.css';
import { jwtDecode } from 'jwt-decode';

function getStatusText(statusCode) {
  const statusMap = {
    1: 'Создан',
    2: 'Обрабатывается',
    3: 'Отменен',
    4: 'Передан в доставку',
    5: 'В пути',
    6: 'Доставлен',
    7: 'Неудачная попытка доставки'
  };
  
  const code = typeof statusCode === 'string' ? parseInt(statusCode) : statusCode;
  return statusMap[code] || `Неизвестный статус (${statusCode})`;
}

const formatDate = (dateString) => {
  if (!dateString) return 'Дата не указана';
  try {
    const date = new Date(dateString);
    return isNaN(date.getTime()) ? 'Неверная дата' : date.toLocaleDateString('ru-RU');
  } catch {
    return 'Ошибка даты';
  }
};

const formatAmount = (amount) => {
  try {
    const numAmount = Number(amount) || 0;
    return `${numAmount.toFixed(2)} ₽`.replace('.', ',');
  } catch {
    return '0 ₽';
  }
};

export default function ProfilePage({ onLogout, userData: propUserData }) {
  const [profileData, setProfileData] = useState({
    fio: '',
    email: '',
    phone: '',
    address: ''
  });
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [dateFilter, setDateFilter] = useState({
    fromDate: '',
    toDate: ''
  });

  const fetchProfileData = async () => {
    try {
      const token = localStorage.getItem('token');
      const decoded = jwtDecode(token);
      const clientId = decoded['Clientid'];

      const response = await axios.get('http://localhost:5000/api/Profile/user', {
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        params: { clientId }
      });

      // Преобразуем данные к нужному формату
      const userData = response.data;
      setProfileData({
        fio: userData.fio || 'Не указано',
        email: userData.email || 'Не указано',
        phone: userData.phone || 'Не указано'
      });
      
    } catch (error) {
      console.error('Ошибка загрузки профиля:', error);
      setError('Не удалось загрузить данные профиля');
      if (error.response?.status === 401) onLogout();
    } finally {
      setLoading(false);
    }
  };

  const loadData = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const decoded = jwtDecode(token);
      const clientId = decoded['Clientid'];

      const response = await axios.get(
        'http://localhost:5000/api/shopfunctions/orders-history',
        {
          params: {
            clientId,
            fromDate: dateFilter.fromDate,
            toDate: dateFilter.toDate
          },
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        }
      );

      const rawData = response.data;
      let ordersArray = [];

      if (Array.isArray(rawData)) {
        ordersArray = rawData;
      } else if (rawData?.$values) {
        ordersArray = rawData.$values;
      }

      const formattedOrders = ordersArray.map(order => {
        // Определяем структуру товаров в зависимости от формата ответа
        const products = order.orderLines?.$values || 
                        order.OrderLines?.$values || 
                        order.products || 
                        order.Products || 
                        [];

        return {
          id: order.OrderId || order.orderId || order.id,
          date: formatDate(order.OrderDate || order.orderDate),
          status: getStatusText(order.OrderStatus || order.orderStatus),
          amount: formatAmount(order.PaymentSum || order.paymentSum),
          payment: order.PaymentMethod === 1 ? 'Оплачено' : 'Не оплачено',
          products: products
        };
      });

      setOrders(formattedOrders);
    } catch (error) {
      console.error('Ошибка загрузки данных:', error);
      setError('Ошибка загрузки данных');
      if (error.response?.status === 401) onLogout();
    } finally {
      setLoading(false);
    }
  };

  const handleDateFilterChange = (e) => {
    const { name, value } = e.target;
    setDateFilter(prev => ({ 
      ...prev, 
      [name]: value 
    }));
  };

  const handleApplyFilters = (e) => {
    e.preventDefault();
    loadData();
  };

  useEffect(() => {
    fetchProfileData();
    loadData();
  }, []);

  if (loading) {
    return <div className="loading-container">Загрузка данных профиля...</div>;
  }

  if (error) {
    return (
      <div className="error-container">
        <div className="error-message">
          <h3>Произошла ошибка</h3>
          <p>{error}</p>
          <button onClick={onLogout} className="logout-button">
            Выйти и попробовать снова
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="profile-container">
      <div className="profile-header">
        <h1>Профиль</h1>
      </div>

      <div className="profile-info-section">
        <h2>Личные данные</h2>
        <div className="profile-info-grid">
          <div className="info-item">
            <span className="info-label">ФИО:</span>
            <span className="info-value">{profileData.fio}</span>
          </div>
          <div className="info-item">
            <span className="info-label">Email:</span>
            <span className="info-value">{profileData.email}</span>
          </div>
          <div className="info-item">
            <span className="info-label">Телефон:</span>
            <span className="info-value">{profileData.phone}</span>
          </div>
        </div>
      </div>

      <div className="orders-section">
        <h2>История заказов</h2>
        
        <form onSubmit={handleApplyFilters} className="date-filters">
          <div className="filter-group">
            <label htmlFor="fromDate">С:</label>
            <div className="date-input-wrapper">
              <input
                type="date"
                id="fromDate"
                name="fromDate"
                value={dateFilter.fromDate}
                onChange={handleDateFilterChange}
                max={new Date().toISOString().split('T')[0]}
              />
            </div>
          </div>
          <div className="filter-group">
            <label htmlFor="toDate">По:</label>
            <div className="date-input-wrapper">
              <input
                type="date"
                id="toDate"
                name="toDate"
                value={dateFilter.toDate}
                onChange={handleDateFilterChange}
                min={dateFilter.fromDate}
                max={new Date().toISOString().split('T')[0]}
              />
            </div>
          </div>
          <button type="submit" className="apply-filters-button">
            Применить
          </button>
        </form>
        
        {orders.length > 0 ? (
          <div className="orders-table-container">
            <table className="orders-table">
              <thead>
                <tr>
                  <th>№ заказа</th>
                  <th>Дата заказа</th>
                  <th>Статус</th>
                  <th>Товары</th>
                  <th>Общая сумма</th>
                </tr>
              </thead>
              <tbody>
                {orders.map((order) => (
                  <tr key={order.id}>
                    <td>{order.id}</td>
                    <td>{order.date}</td>
                    <td>
                      <span className={`status-badge ${order.status.toLowerCase().replace(/\s+/g, '-')}`}>
                        {order.status}
                      </span>
                    </td>
                    <td>
                      {order.products?.length > 0 ? (
                        <ul className="products-list">
                          {order.products.map((product) => (
                            <li key={`${product.productId || product.ProductId}`}>
                              {product.productName || product.ProductName} - {product.quantity || product.Quantity} шт.
                            </li>
                          ))}
                        </ul>
                      ) : (
                        <span>Нет информации о товарах</span>
                      )}
                    </td>
                    <td className="total-amount">
                      <strong>{order.amount}</strong>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <div className="no-orders-message">
            <p>У вас пока нет заказов</p>
          </div>
        )}
      </div>

      <div className="logout-section">
        <button onClick={onLogout} className="logout-button">
          Выйти из аккаунта
        </button>
      </div>
    </div>
  );
}