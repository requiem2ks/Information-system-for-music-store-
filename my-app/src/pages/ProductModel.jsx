import React from 'react';
import '../styles/modal.css';

const ProductModal = ({ product, storageData = [], onClose }) => {
  // Находим все записи склада для этого товара
  const stockItems = storageData.filter(item => item.productid == product?.id);
  
  // Суммируем quantity (защита от undefined/null)
  const totalQuantity = stockItems.reduce((sum, item) => sum + (Number(item.quantity) || 0), 0);

  return (
    <div className="modal-overlay">
      <div className="compact-modal">
        <button className="close-modal-btn" onClick={onClose}>
          &times;
        </button>
        
        <div className="modal-content-container">
          <div className="modal-image-column">
            <img 
              src={product.image || '/placeholder-product.jpg'} 
              alt={product.name || 'Изображение товара'} 
              className="compact-product-image"
              onError={(e) => {
                e.target.onerror = null;
                e.target.src = '/placeholder-product.jpg';
              }}
            />
          </div>
          
          <div className="modal-info-column">
            <h2>{product.name || 'Название товара'}</h2>
            <p className="modal-price">
              {product.price ? `${product.price.toLocaleString('ru-RU')} ₽` : 'Цена не указана'}
            </p>
            <p className="modal-description">{product.description || 'Описание отсутствует'}</p>
            
            {/* Отображаем общее количество со всех складов */}
            <p className="modal-quantity">
              Количество товара на складе: {totalQuantity}
              {stockItems.length > 1 && ` (из ${stockItems.length} складов)`}
            </p>
            
            <div className="modal-actions">
              {/* Ваши кнопки действий, если нужны */}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductModal;