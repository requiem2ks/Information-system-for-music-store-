import { useState, useEffect } from 'react';
import axios from 'axios';
import '../styles/admintable.css';

const MusicProductTable = ({ onClose }) => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [formData, setFormData] = useState({
    productname: '',
    description: '',
    price: '',
    productcategoryid: '',
    image: ''
  });

  const API_URL = 'http://localhost:5000/api/Product';

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    setLoading(true);
    try {
      const response = await axios.get(API_URL);
      const data = response.data.$values || response.data;
      setProducts(Array.isArray(data) ? data : []);
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

  const handleEdit = (product) => {
    setEditingProduct(product);
    setFormData({
      productname: product.productname,
      description: product.description,
      price: product.price,
      productcategoryid: product.productcategoryid,
      image: product.image
    });
    setIsEditModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingProduct) {
        await axios.put(`${API_URL}/${editingProduct.productid}`, formData);
        alert('Данные продукта обновлены');
        setIsEditModalOpen(false);
      } else {
        await axios.post(API_URL, formData);
        alert('Продукт успешно добавлен');
        setIsAddModalOpen(false);
      }
      await fetchProducts();
      setFormData({ productname: '', description: '', price: '', productcategoryid: '', image: '' });
    } catch (error) {
      console.error('Ошибка при сохранении продукта:', error);
      alert(`Ошибка при сохранении данных: ${error.response?.data?.message || error.message}`);
    }
  };

  const handleDelete = async (productid) => {
    if (window.confirm('Вы уверены, что хотите удалить этот продукт?')) {
      try {
        await axios.delete(`${API_URL}/${productid}`);
        alert('Продукт успешно удален');
        await fetchProducts();
      } catch (error) {
        console.error('Ошибка при удалении продукта:', error);
        alert(`Ошибка при удалении продукта: ${error.response?.data?.message || error.message}`);
      }
    }
  };

  const handleAdd = () => {
    setEditingProduct(null);
    setFormData({ productname: '', description: '', price: '', productcategoryid: '', image: '' });
    setIsAddModalOpen(true);
  };

  const handleExport = () => {
    const dataStr = JSON.stringify(products, null, 2);
    const dataUri = 'data:application/json;charset=utf-8,'+ encodeURIComponent(dataStr);
    const link = document.createElement('a');
    link.setAttribute('href', dataUri);
    link.setAttribute('download', 'music_products_export.json');
    link.click();
  };

  return (
    <div className="wide-client-table">
      <div className="table-header">
        <div className="header-actions">
          <button className="add-btn" onClick={handleAdd}>Добавить продукт</button>
          <button className="export-btn" onClick={handleExport}>Экспорт данных</button>
          <button className="close-btn" onClick={onClose}>Закрыть</button>
        </div>
      </div>

      <div className="table-scroll-container">
        <table>
          <thead>
            <tr>
              <th className="col-id">ID</th>
              <th className="col-name">Название</th>
              <th className="col-description">Описание</th>
              <th className="col-price">Цена</th>
              <th className="col-category">Категория</th>
              <th className="col-image">Изображение</th>
              <th className="col-actions">Действия</th>
            </tr>
          </thead>
          <tbody>
            {products.map(product => (
              <tr key={product.productid}>
                <td>{product.productid}</td>
                <td>{product.productname}</td>
                <td>{product.description}</td>
                <td>{product.price}</td>
                <td>{product.productcategoryid}</td>
                <td>{product.image ? 'Есть' : 'Нет'}</td>
                <td className="actions-cell">
                  <button className="edit-btn" onClick={() => handleEdit(product)}>Редактировать</button>
                  <button className="delete-btn" onClick={() => handleDelete(product.productid)}>Удалить</button>
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
            <h2>{editingProduct ? 'Редактировать продукт' : 'Добавить продукт'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Название:</label>
                <input
                  type="text"
                  name="productname"
                  value={formData.productname}
                  onChange={handleInputChange}
                  required
                  placeholder="Введите название продукта"
                />
              </div>
              <div className="form-group">
                <label>Описание:</label>
                <textarea
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  required
                  rows="3"
                  placeholder="Введите описание"
                />
              </div>
              <div className="form-group">
                <label>Цена:</label>
                <input
                  type="number"
                  name="price"
                  value={formData.price}
                  onChange={handleInputChange}
                  required
                  placeholder="Введите цену"
                />
              </div>
              <div className="form-group">
                <label>ID категории:</label>
                <input
                  type="number"
                  name="productcategoryid"
                  value={formData.productcategoryid}
                  onChange={handleInputChange}
                  required
                  placeholder="Введите ID категории"
                />
              </div>
              <div className="form-group">
                <label>Ссылка на изображение:</label>
                <input
                  type="text"
                  name="image"
                  value={formData.image}
                  onChange={handleInputChange}
                  placeholder="Введите URL изображения"
                />
              </div>
              <div className="form-actions">
                <button type="submit" className="btn-save">Сохранить</button>
                <button type="button" className="btn-cancel" onClick={editingProduct ? () => setIsEditModalOpen(false) : () => setIsAddModalOpen(false)}>
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

export default MusicProductTable;