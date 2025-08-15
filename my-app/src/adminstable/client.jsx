import { useState, useEffect } from 'react';
import axios from 'axios';
import '../styles/admintable.css';

const ClientTable = ({ onClose }) => {
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingClient, setEditingClient] = useState(null);
  const [deletingId, setDeletingId] = useState(null);
  const [formData, setFormData] = useState({
    fio: '',
    phone: '',
    address: ''
  });

  const API_URL = 'http://localhost:5000/api/Client';

  useEffect(() => {
    fetchClients();
  }, []);

  const fetchClients = async () => {
    setLoading(true);
    try {
      const response = await axios.get(API_URL);
      const data = response.data.$values || response.data;
      setClients(Array.isArray(data) ? data : []);
    } catch (error) {
      console.error('Ошибка загрузки:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleEdit = (client) => {
    setEditingClient(client);
    setFormData({
      fio: client.fio,
      phone: client.phone,
      address: client.address
    });
    setIsEditModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingClient) {
        await axios.put(`${API_URL}/${editingClient.clientid}`, formData);
        alert('Данные клиента обновлены');
        setIsEditModalOpen(false);
      } else {
        await axios.post(API_URL, formData);
        alert('Клиент успешно добавлен');
        setIsAddModalOpen(false);
      }
      await fetchClients();
      setFormData({ fio: '', phone: '', address: '' });
    } catch (error) {
      console.error('Ошибка при сохранении клиента:', error);
      alert(`Ошибка при сохранении данных: ${error.response?.data?.message || error.message}`);
    }
  };

  const handleDelete = async (clientid) => {
    if (window.confirm('Вы уверены, что хотите удалить этого клиента?')) {
        setDeletingId(clientid);
        try {
        await axios.delete(`${API_URL}/${clientid}`);
        alert('Клиент успешно удален');
        await fetchClients();
        } catch (error) {
        console.error('Ошибка при удалении клиента:', error);
        alert(`Ошибка при удалении клиента, у клиента есть заказы или учетная запись`);
        } finally {
        setDeletingId(null);
        }
    }
  };

  const handleAdd = () => {
    setEditingClient(null);
    setFormData({ fio: '', phone: '', address: '' });
    setIsAddModalOpen(true);
  };

  const handleExport = () => {
    const dataStr = JSON.stringify(clients, null, 2);
    const dataUri = 'data:application/json;charset=utf-8,'+ encodeURIComponent(dataStr);
    const link = document.createElement('a');
    link.setAttribute('href', dataUri);
    link.setAttribute('download', 'clients_export.json');
    link.click();
  };

  return (
    <div className="wide-client-table">
      <div className="table-header">
        <div className="header-actions">
          <button className="add-btn" onClick={handleAdd}>Добавить клиента</button>
          <button className="export-btn" onClick={handleExport}>Экспорт данных</button>
          <button className="close-btn" onClick={onClose}>Закрыть</button>
        </div>
      </div>

      <div className="table-scroll-container">
        <table>
          <thead>
            <tr>
              <th className="col-id">ID</th>
              <th className="col-name">ФИО</th>
              <th className="col-phone">Телефон</th>
              <th className="col-address">Адрес</th>
              <th className="col-actions">Действия</th>
            </tr>
          </thead>
          <tbody>
            {clients.map(client => (
              <tr key={client.clientid}>
                <td>{client.clientid}</td>
                <td>{client.fio}</td>
                <td>{client.phone}</td>
                <td>{client.address}</td>
                <td className="actions-cell">
                  <button className="edit-btn" onClick={() => handleEdit(client)}>Редактировать</button>
                  <button 
                    className="delete-btn" 
                    onClick={() => handleDelete(client.clientid)}
                    disabled={deletingId === client.clientid}
                    >
                    {deletingId === client.clientid ? 'Удаление...' : 'Удалить'}
                    </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Модальное окно формы добавления/редактирования */}
      {(isAddModalOpen || isEditModalOpen) && (
        <div className="client-form-modal-overlay">
          <div className="client-form-modal">
            <h2>{editingClient ? 'Редактировать клиента' : 'Добавить клиента'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>ФИО:</label>
                <input
                  type="text"
                  name="fio"
                  value={formData.fio}
                  onChange={handleInputChange}
                  required
                  placeholder="Введите ФИО клиента"
                />
              </div>
              <div className="form-group">
                <label>Телефон:</label>
                <input
                  type="text"
                  name="phone"
                  value={formData.phone}
                  onChange={handleInputChange}
                  required
                  placeholder="Введите телефон"
                />
              </div>
              <div className="form-group">
                <label>Адрес:</label>
                <input
                  name="address"
                  value={formData.address}
                  onChange={handleInputChange}
                  required
                  rows="2"
                  placeholder="Введите адрес"
                />
              </div>
              <div className="form-actions">
                <button type="submit" className="btn-save">Сохранить</button>
                <button type="button" className="btn-cancel" onClick={editingClient ? () => setIsEditModalOpen(false) : () => setIsAddModalOpen(false)}>
                  Отмена
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default ClientTable;