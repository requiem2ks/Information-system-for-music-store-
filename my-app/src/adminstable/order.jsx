import { useState, useEffect } from 'react';
import axios from 'axios';
import '../styles/admintable.css';

const OrderTable = ({ onClose }) => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingOrder, setEditingOrder] = useState(null);
  const [formData, setFormData] = useState({
    orderenddate: '',
    paymentid: '',
    employeeid: '',
    statusoforder: '',
    discount: 0
  });
  const [clientData, setClientData] = useState({
    fio: '',
    phone: '',
    address: '',
  });
  const [items, setItems] = useState([{
    productid: '',
    quantity: 1,
    unitprice: '',
    settingid: ''
  }]);

  const API_URL = 'http://localhost:5000/api/Order';

  const getAuthToken = () => localStorage.getItem('token') || localStorage.getItem('authToken');

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    setLoading(true);
    try {
      const token = getAuthToken();
      if (!token) {
        throw new Error('No authentication token found');
      }
      
      const response = await axios.get(API_URL, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      
      const data = response.data.$values || response.data;
      setOrders(Array.isArray(data) ? data : []);
    } catch (error) {
      console.error('Ошибка загрузки:', error);
      alert('Ошибка загрузки заказов. Возможно, требуется авторизация.');
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleClientChange = (e) => {
    const { name, value } = e.target;
    setClientData(prev => ({ ...prev, [name]: value }));
  };

  const handleItemChange = (index, e) => {
    const { name, value } = e.target;
    const newItems = [...items];
    newItems[index] = { ...newItems[index], [name]: value };
    setItems(newItems);
  };

  const addItem = () => {
    setItems([...items, { productid: '', quantity: 1, unitprice: '', settingid: '' }]);
  };

  const removeItem = (index) => {
    const newItems = [...items];
    newItems.splice(index, 1);
    setItems(newItems);
  };

  const handleEdit = (order) => {
    setEditingOrder(order);
    setFormData({
      orderenddate: order.orderenddate,
      paymentid: order.paymentid,
      employeeid: order.employeeid,
      statusoforder: order.statusoforder,
      discount: order.discount || 0
    });
    setIsEditModalOpen(true);
  };

  const handleSubmitEdit = async (e) => {
    e.preventDefault();
    try {
      const token = getAuthToken();
      if (!token) {
        throw new Error('No authentication token found');
      }

      await axios.put(`${API_URL}/${editingOrder.orderid}`, formData, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      alert('Данные заказа обновлены');
      setIsEditModalOpen(false);
      await fetchOrders();
    } catch (error) {
      console.error('Ошибка при обновлении заказа:', error);
      alert(`Ошибка при обновлении данных: ${error.response?.data?.message || error.message}`);
    }
  };

  const handleSubmitAdd = async (e) => {
  e.preventDefault();
  try {
    const token = getAuthToken();
    if (!token) {
      throw new Error('No authentication token found');
    }

    // Получаем информацию о текущем пользователе
    const userResponse = await axios.get('http://localhost:5000/api/Auth/me', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    
    const currentUser = userResponse.data;
    const isEmployee = currentUser.role === 'Employee'; // предполагая, что у вас есть поле role

    const orderData = {
      orderenddate: formData.orderenddate,
      paymentid: formData.paymentid,
      statusoforder: 1, // pending по умолчанию
      employeeid: isEmployee ? currentUser.userId : formData.employeeid,
      discount: formData.discount || 0,
      client: isEmployee ? null : clientData, // не отправляем данные клиента для сотрудника
      items: items.filter(item => item.productid && item.quantity > 0)
    };

    await axios.post(API_URL, orderData, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    alert('Заказ успешно добавлен');
    setIsAddModalOpen(false);
    await fetchOrders();
    // Сброс формы...
  } catch (error) {
    console.error('Ошибка при создании заказа:', error);
    alert(`Ошибка при создании заказа: ${error.response?.data?.message || error.message}`);
  }
};

  const handleDelete = async (orderid) => {
    if (window.confirm('Вы уверены, что хотите удалить этот заказ?')) {
      try {
        const token = getAuthToken();
        if (!token) {
          throw new Error('No authentication token found');
        }

        await axios.delete(`${API_URL}/${orderid}`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
        alert('Заказ успешно удален');
        await fetchOrders();
      } catch (error) {
        console.error('Ошибка при удалении заказа:', error);
        alert(`Ошибка при удалении заказа: ${error.response?.data?.message || error.message}`);
      }
    }
  };

  const handleAdd = () => {
    setEditingOrder(null);
    setFormData({ 
      orderenddate: '', 
      paymentid: '', 
      employeeid: '', 
      statusoforder: '',
      discount: 0
    });
    setClientData({
      fio: '',
      phone: '',
      address: '',
    });
    setItems([{
      productid: '',
      quantity: 1,
      unitprice: '',
      settingid: ''
    }]);
    setIsAddModalOpen(true);
  };

  const handleExport = () => {
    const dataStr = JSON.stringify(orders, null, 2);
    const dataUri = 'data:application/json;charset=utf-8,'+ encodeURIComponent(dataStr);
    const link = document.createElement('a');
    link.setAttribute('href', dataUri);
    link.setAttribute('download', 'orders_export.json');
    link.click();
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'Не указана';
    const date = new Date(dateString);
    return date.toLocaleDateString('ru-RU');
  };

  const getStatusName = (statusCode) => {
    const statuses = {
      1: 'Создан',
      2: 'Обрабатывается',
      3: 'Отменен',
      4: 'Передан в доставку',
      5: 'В пути',
      6: 'Доставлен',
      7: 'Неудачная попытка доставки'
    };
    return statuses[statusCode] || statusCode;
  };

  return (
    <div className="wide-client-table">
      <div className="table-header">
        <div className="header-actions">
          <button className="add-btn" onClick={handleAdd}>Добавить заказ</button>
          <button className="export-btn" onClick={handleExport}>Экспорт данных</button>
          <button className="close-btn" onClick={onClose}>Закрыть</button>
        </div>
      </div>

      <div className="table-scroll-container">
        <table>
          <thead>
            <tr>
              <th className="col-id">ID</th>
              <th className="col-date">Дата заказа</th>
              <th className="col-enddate">Дата завершения</th>
              <th className="col-client">ID клиента</th>
              <th className="col-payment">ID оплаты</th>
              <th className="col-employee">ID сотрудника</th>
              <th className="col-status">Статус</th>
              <th className="col-discount">Скидка</th>
              <th className="col-actions">Действия</th>
            </tr>
          </thead>
          <tbody>
            {orders.map(order => (
              <tr key={order.orderid}>
                <td>{order.orderid}</td>
                <td>{formatDate(order.orderdate)}</td>
                <td>{formatDate(order.orderenddate)}</td>
                <td>{order.clientid}</td>
                <td>{order.paymentid}</td>
                <td>{order.employeeid}</td>
                <td>{getStatusName(order.statusoforder)}</td>
                <td>{order.discount || 0}%</td>
                <td className="actions-cell">
                  <button className="edit-btn" onClick={() => handleEdit(order)}>Редактировать</button>
                  <button className="delete-btn" onClick={() => handleDelete(order.orderid)}>Удалить</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Модальное окно редактирования */}
      {isEditModalOpen && (
        <div className="client-form-modal-overlay">
          <div className="client-form-modal">
            <h2>Редактировать заказ</h2>
            <form onSubmit={handleSubmitEdit}>
              <div className="form-group">
                <label>Дата завершения:</label>
                <input
                  type="date"
                  name="orderenddate"
                  value={formData.orderenddate}
                  onChange={handleInputChange}
                />
              </div>
              <div className="form-group">
                <label>ID оплаты:</label>
                <input
                  type="number"
                  name="paymentid"
                  value={formData.paymentid}
                  onChange={handleInputChange}
                  required
                  placeholder="Введите ID оплаты"
                />
              </div>
              <div className="form-group">
                <label>ID сотрудника:</label>
                <input
                  type="number"
                  name="employeeid"
                  value={formData.employeeid}
                  onChange={handleInputChange}
                  required
                  placeholder="Введите ID сотрудника"
                />
              </div>
              <div className="form-group">
                <label>Скидка (%):</label>
                <input
                  type="number"
                  name="discount"
                  value={formData.discount}
                  onChange={handleInputChange}
                  min="0"
                  max="100"
                  placeholder="Введите размер скидки"
                />
              </div>
              <div className="form-group">
                <label>Статус заказа:</label>
                <select
                  name="statusoforder"
                  value={formData.statusoforder}
                  onChange={handleInputChange}
                  required
                >
                  <option value="">Выберите статус</option>
                  <option value="1">В ожидании</option>
                  <option value="2">В обработке</option>
                  <option value="3">Выполняется</option>
                  <option value="4">Завершен</option>
                  <option value="5">Отменен</option>
                </select>
              </div>
              <div className="form-actions">
                <button type="submit" className="btn-save">Сохранить</button>
                <button type="button" className="btn-cancel" onClick={() => setIsEditModalOpen(false)}>
                  Отмена
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Модальное окно добавления */}
      {isAddModalOpen && (
        <div className="client-form-modal-overlay">
          <div className="client-form-modal wide-modal">
            <div className="modal-header">
              <h2>Добавить заказ</h2>
            </div>
            
            <div className="modal-scroll-content">
              <form onSubmit={handleSubmitAdd}>
                <div className="form-section">
                  <h3>Информация о клиенте</h3>
                  <div className="form-group">
                    <label>ФИО:</label>
                    <input
                      type="text"
                      name="fio"
                      value={clientData.fio}
                      onChange={handleClientChange}
                      required
                      placeholder="Введите ФИО клиента"
                    />
                  </div>
                  <div className="form-group">
                    <label>Телефон:</label>
                    <input
                      type="text"
                      name="phone"
                      value={clientData.phone}
                      onChange={handleClientChange}
                      required
                      placeholder="Введите телефон клиента"
                    />
                  </div>
                
                  <div className="form-group">
                    <label>Адрес:</label>
                    <input
                      type="text"
                      name="address"
                      value={clientData.address}
                      onChange={handleClientChange}
                      required
                      placeholder="Введите адрес клиента"
                    />
                  </div>
                </div>

                <div className="form-section">
                  <h3>Информация о заказе</h3>
                  <div className="form-group">
                    <label>Дата завершения:</label>
                    <input
                      type="date"
                      name="orderenddate"
                      value={formData.orderenddate}
                      onChange={handleInputChange}
                    />
                  </div>
                  <div className="form-group">
                    <label>ID оплаты:</label>
                    <input
                      type="number"
                      name="paymentid"
                      value={formData.paymentid}
                      onChange={handleInputChange}
                      required
                      placeholder="Введите ID оплаты"
                    />
                  </div>
                  <div className="form-group">
                    <label>ID сотрудника:</label>
                    <input
                      type="number"
                      name="employeeid"
                      value={formData.employeeid}
                      onChange={handleInputChange}
                      required
                      placeholder="Введите ID сотрудника"
                    />
                  </div>
                  <div className="form-group">
                    <label>Скидка (%):</label>
                    <input
                      type="number"
                      name="discount"
                      value={formData.discount}
                      onChange={handleInputChange}
                      min="0"
                      max="100"
                      placeholder="Введите размер скидки"
                    />
                  </div>
                </div>

                <div className="form-section">
                  <h3>Позиции заказа</h3>
                  {items.map((item, index) => (
                    <div key={index} className="order-item-form">
                      <div className="form-group">
                        <label>ID продукта:</label>
                        <input
                          type="number"
                          name="productid"
                          value={item.productid}
                          onChange={(e) => handleItemChange(index, e)}
                          required
                          placeholder="ID продукта"
                        />
                      </div>
                      <div className="form-group">
                        <label>Количество:</label>
                        <input
                          type="number"
                          name="quantity"
                          value={item.quantity}
                          onChange={(e) => handleItemChange(index, e)}
                          required
                          min="1"
                        />
                      </div>
                      <div className="form-group">
                        <label>Цена за единицу:</label>
                        <input
                          type="number"
                          name="unitprice"
                          value={item.unitprice}
                          onChange={(e) => handleItemChange(index, e)}
                          required
                          step="0.01"
                          min="0"
                        />
                      </div>
                      <div className="form-group">
                        <label>ID настройки:</label>
                        <input
                          type="number"
                          name="settingid"
                          value={item.settingid}
                          onChange={(e) => handleItemChange(index, e)}
                          placeholder="ID настройки (необязательно)"
                        />
                      </div>
                      {items.length > 1 && (
                        <button type="button" className="remove-item-btn" onClick={() => removeItem(index)}>
                          Удалить позицию
                        </button>
                      )}
                    </div>
                  ))}
                  <button type="button" className="add-item-btn" onClick={addItem}>
                    Добавить позицию
                  </button>
                </div>

                <div className="form-actions">
                  <button type="submit" className="btn-save">Создать заказ</button>
                  <button type="button" className="btn-cancel" onClick={() => setIsAddModalOpen(false)}>
                    Отмена
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default OrderTable;  