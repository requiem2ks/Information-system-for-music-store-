import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import '../styles/checkout.css';

const CheckoutPage = ({ clearCart }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const { total = 0, itemsCount = 0, cartItems = [] } = location.state || {};
      const reservedItems = cartItems.filter(item => item.isReserved);
    const purchaseItems = cartItems.filter(item => !item.isReserved);
  const [formData, setFormData] = useState({
    phone: '',
    name: '',
    email: '',
    deliveryMethod: 'pickup',
    address: '',
    managerCall: true,
    paymentMethod: 'cash'
  });
  
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState(null);
  const [showSuccessModal, setShowSuccessModal] = useState(false);
  const [orderDetails, setOrderDetails] = useState({
    orderId: null,
    total: 0,
    clientName: ''
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSubmit = async (e) => {
  e.preventDefault();
  setIsSubmitting(true);
  setError(null);

  try {
    const paymentMethodMap = {
      'cash': 1,
      'electronic': 2,
      'sbp': 3
    };

    const paymentResponse = await fetch('http://localhost:5000/api/Payment', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify({
        paymentsum: total,
        paymentmethod: paymentMethodMap[formData.paymentMethod],
        paymentdate: new Date().toISOString().split('T')[0]
      })
    });

    if (!paymentResponse.ok) {
      const errorData = await paymentResponse.json();
      throw new Error(errorData.title || errorData.message || 'Ошибка при создании платежа');
    }

    const paymentResult = await paymentResponse.json();
    const paymentId = paymentResult.paymentid;

    // Затем создаем заказ
    const endDate = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000);
    const formattedDate = endDate.toISOString().split('T')[0];

    const orderData = {
      orderenddate: formattedDate,
      paymentid: paymentId, // Используем ID созданного платежа
      client: {
        fio: formData.name,
        phone: formData.phone,
        email: formData.email,
        address: formData.deliveryMethod === 'courier' ? formData.address : 'Самовывоз'
      },
      items: cartItems.map(item => ({
        productid: item.productid,
        quantity: item.quantity,
        unitprice: item.price,
        settingid: 1

      }))
    };

    const orderResponse = await fetch('http://localhost:5000/api/Order', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify(orderData)
    });

    if (!orderResponse.ok) {
      const errorData = await orderResponse.json();
      throw new Error(errorData.title || errorData.message || 'Ошибка при создании заказа');
    }

    const orderResult = await orderResponse.json();
    const orderId = orderResult.orderid;

  if (purchaseItems.length === 0) {
    console.warn('Корзина пуста!');
    setError('Ваша корзина пуста');
    return;
  }

  // Создаем резервации для зарезервированных товаров
    if (reservedItems.length > 0) {
  try {
    // Подготовка дат
    const reservationDate = new Date();
    const reservationEndDate = new Date(reservationDate);
    reservationEndDate.setDate(reservationEndDate.getDate() + 7);
    
    console.log('Даты резервации:', {
      start: reservationDate.toISOString().split('T')[0],
      end: reservationEndDate.toISOString().split('T')[0]
    });

    // Создаем все резервации и связанные ProductReservation
    const reservationPromises = reservedItems.map(async (item) => {
      // Создаем запись в Reservations
      const reservationData = {
        productid: item.id, // Используем item.id вместо item.productid
        reservationdate: reservationDate.toISOString().split('T')[0],
        reservationenddate: reservationEndDate.toISOString().split('T')[0],
        quantity: item.quantity,
        status: 'active'
      };

      // Добавляем clientid, если он доступен
      if (orderResult.clientid) {
        reservationData.clientid = orderResult.clientid;
      }

      console.log('Отправка резервации:', reservationData);

      const reservationResponse = await fetch('http://localhost:5000/api/Reservation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(reservationData)
      });

      if (!reservationResponse.ok) {
        const error = await reservationResponse.json();
        console.error('Ошибка резервации:', error);
        throw new Error(`Ошибка резервации товара ${item.id}`);
      }

      const reservationResult = await reservationResponse.json();

      // Создаем запись в ProductReservation
      const productReservationData = {
        reservationid: reservationResult.reservationid, 
        orderid: orderResult.orderid, 
        productid: item.id, 
        quantityofreserved: item.quantity, 
        unitpricereservation: item.price, 
        prepaymentpercentage: 30 
      };

      console.log('Отправка ProductReservation:', productReservationData);

      const productReservationResponse = await fetch('http://localhost:5000/api/ProductReservation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(productReservationData)
      });

      if (!productReservationResponse.ok) {
        const error = await productReservationResponse.json();
        console.error('Ошибка ProductReservation:', error);
        throw new Error(`Ошибка создания ProductReservation для товара ${item.id}`);
      }

      const productReservationResult = await productReservationResponse.json();

      return {
        reservation: reservationResult,
        productReservation: productReservationResult
      };
    });

    const results = await Promise.all(reservationPromises);
    console.log('Все резервации и ProductReservation созданы:', results);

  } catch (error) {
    console.error('Ошибка при создании резерваций:', error);
  }
}
    // Успешное завершение
    setOrderDetails({
      orderId: orderResult.orderid,
      total: total,
      clientName: formData.name
    });
    
    setShowSuccessModal(true);
    
    // Очистка корзины 
    if (clearCart) {
      clearCart([]);
    }
    
    localStorage.removeItem('cart');
    
    console.log('Корзина полностью очищена');

  } catch (error) {
    console.error('Полная ошибка оформления:', error);

    setError(error.message || 'Произошла ошибка при оформлении заказа');
  } finally {
    setIsSubmitting(false);
  }
};

  const closeSuccessModal = () => {
    setShowSuccessModal(false);
    navigate('/catalog'); // Перенаправляем на главную после закрытия
  };

  return (
    <div className="checkout-container">
      <h1 className="checkout-title">Оформление заказа</h1>
      
      {error && <div className="error-message">{error}</div>}
      
      <div className="checkout-content">
        {/* Форма заказа */}
        <form className="checkout-form" onSubmit={handleSubmit}>
          {/* Секция 1: Контактные данные */}
          <div className="form-section">
            <h2 className="section-title">1. ВАШИ КОНТАКТНЫЕ ДАННЫЕ</h2>
            
            <div className="input-field">
              <label>Телефон:</label>
              <input
                type="tel"
                name="phone"
                placeholder="+7 (XXX) XXX-XX-XX"
                value={formData.phone}
                onChange={handleChange}
                required
                pattern="[\+]\d{1}\s[\(]\d{3}[\)]\s\d{3}[\-]\d{2}[\-]\d{2}"
                title="Формат: +7 (XXX) XXX-XX-XX"
              />
            </div>
            
            <div className="input-field">
              <label>ФИО:</label>
              <input
                type="text"
                name="name"
                placeholder="Введите имя"
                value={formData.name}
                onChange={handleChange}
                required
                minLength="3"
              />
            </div>
            
            <div className="input-field">
              <label>E-mail:</label>
              <input
                type="email"
                name="email"
                placeholder="example@mail.ru"
                value={formData.email}
                onChange={handleChange}
                required
                pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$"
              />
            </div>
          </div>

          {/* Секция 2: Получение заказа */}
          <div className="form-section">
            <h2 className="section-title">2. ПОЛУЧЕНИЕ ЗАКАЗА</h2>
            
            <div className="radio-group">
              <div className="radio-option">
                <input
                  type="radio"
                  id="pickup"
                  name="deliveryMethod"
                  value="pickup"
                  checked={formData.deliveryMethod === 'pickup'}
                  onChange={handleChange}
                />
                <label htmlFor="pickup">
                  <span className="option-main">Самовывоз</span>
                  <span className="option-detail">Бесплатно, через 2 дня</span>
                </label>
              </div>
              
              <div className="radio-option">
                <input
                  type="radio"
                  id="courier"
                  name="deliveryMethod"
                  value="courier"
                  checked={formData.deliveryMethod === 'courier'}
                  onChange={handleChange}
                />
                <label htmlFor="courier">
                  <span className="option-main">Курьерская доставка</span>
                  <span className="option-detail">Доставим по указанному адресу</span>
                </label>
              </div>
            </div>
            
            {formData.deliveryMethod === 'courier' && (
              <div className="input-field">
                <label>Адрес доставки:</label>
                <input
                  type="text"
                  name="address"
                  placeholder="Город, улица, дом, квартира"
                  value={formData.address}
                  onChange={handleChange}
                  required
                />
              </div>
            )}
          </div>

          {/* Секция 3: Подтверждение заказа */}
          <div className="form-section">
            <h2 className="section-title">3. ПОДТВЕРЖДЕНИЕ ЗАКАЗА</h2>
            
            <div className="checkbox-option">
              <input
                type="checkbox"
                id="managerCall"
                name="managerCall"
                checked={formData.managerCall}
                onChange={handleChange}
                className="custom-checkbox"
              />
              <label htmlFor="managerCall" className="checkbox-label">
                Позвонить для подтверждения заказа
              </label>
            </div>
          </div>

          {/* Секция 4: Способ оплаты */}
          <div className="form-section">
            <h2 className="section-title">4. СПОСОБ ОПЛАТЫ</h2>
            
            <div className="radio-group">
              <div className="radio-option">
                <input
                  type="radio"
                  id="cash"
                  name="paymentMethod"
                  value="cash"
                  checked={formData.paymentMethod === 'cash'}
                  onChange={handleChange}
                />
                <label htmlFor="cash">Наличными при получении</label>
              </div>
              
              <div className="radio-option">
                    <input
                        type="radio"
                        id="electronic"
                        name="paymentMethod"
                        value="electronic"
                        checked={formData.paymentMethod === 'electronic'}
                        onChange={handleChange}
                    />
                    <label htmlFor="electronic">Электронные платежи</label>
                    </div>

                    <div className="radio-option">
                    <input
                        type="radio"
                        id="sbp"
                        name="paymentMethod"
                        value="sbp"
                        checked={formData.paymentMethod === 'sbp'}
                        onChange={handleChange}
                    />
                    <label htmlFor="sbp">СБП / QR-код</label>
                    </div>
                    </div>
                </div>
                </form>

        {/* Блок с суммой заказа */}
        <div className="order-summary">
          <div className="summary-card">
            <h2 className="summary-title">ВАШ ЗАКАЗ</h2>
            
            <div className="order-details">
              <div className="detail-row">
                <span>{itemsCount} товар(а)</span>
                <span>{total.toLocaleString('ru-RU')} ₽</span>
              </div>
              
              <div className="detail-row">
                <span>Доставка:</span>
                <span>
                  {formData.deliveryMethod === 'pickup' ? 'Самовывоз' : 'Курьером'}
                </span>
              </div>
              
              <div className="detail-row">
                <span>Оплата:</span>
                <span>
                  {formData.paymentMethod === 'cash' ? 'Наличными' : 'Картой онлайн'}
                </span>
              </div>
            </div>
            
            <div className="divider"></div>
            
            <div className="total-row">
              <span>Итого:</span>
              <span className="total-price">{total.toLocaleString('ru-RU')} ₽</span>
            </div>
            
            <button 
              className="purchase-btn"
              type="submit"
              onClick={handleSubmit}
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Оформляем...' : 'Подтвердить заказ'}
            </button>
          </div>
        </div>
      </div>

      {/* Модальное окно успешного оформления */}
      {showSuccessModal && (
        <div className="modal-overlay">
          <div className="success-modal">
            <h2>Заказ успешно создан!</h2>
            <div className="order-info">
              <p>Номер заказа: <strong>№{orderDetails.orderId}</strong></p>
              <p>Сумма: <strong>{orderDetails.total.toLocaleString('ru-RU')} ₽</strong></p>
              <p>Спасибо, {orderDetails.clientName}, за ваш заказ!</p>
            </div>
            <button 
              className="close-modal-btn"
              onClick={closeSuccessModal}
            >
              Вернуться в каталог
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default CheckoutPage;