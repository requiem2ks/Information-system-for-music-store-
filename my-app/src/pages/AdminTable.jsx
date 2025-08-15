import { useState } from 'react';
import ClientTable from '../adminstable/client';
import MusicProductTable from '../adminstable/product';
import OrderTable from '../adminstable/order';
import '../styles/admintable.css';

export default function AdminTablesPage() {
  const [selectedTable, setSelectedTable] = useState(null);

  const tables = [
    'Клиенты',
    'Заказы',
    'Музыкальная продукция',
    'Сотрудники',
    'Поставщики',
    'Элементы заказа',
    'Оплаты',
    'Отзывы',
    'Акции',
    'Категории музыкальной продукции',
    'Склад',
    'Доставка',
    'Музыкальная продукция в корзине',
    'Продажи',
    'Курьер',
    'Закупки',
    'Резервирование',
    'Настройка инструментов',
    'Поступления на склад',
    'Персональные скидки'
  ];

  const renderTableContent = () => {
    switch(selectedTable) {
      case 'Клиенты':
        return <ClientTable onClose={() => setSelectedTable(null)} />;
      case 'Музыкальная продукция':
        return <MusicProductTable onClose={() => setSelectedTable(null)} />;
      case 'Заказы':
        return <OrderTable onClose={() => setSelectedTable(null)} />;
      // Другие таблицы...
      default:
        return (
          <div className="default-modal-content">
            <p>Функционал для таблицы "{selectedTable}" в разработке</p>
            <button 
              className="close-btn"
              onClick={() => setSelectedTable(null)}
            >
              Закрыть
            </button>
          </div>
        );
    }
  };

  return (
    <div className="admin-tables-container">
      <h1>Администрирование</h1>
      <div className="tables-grid">
        {tables.map((table, index) => (
          <div 
            key={index} 
            className="table-card"
            onClick={() => setSelectedTable(table)}
          >
            {table}
          </div>
        ))}
      </div>

      {selectedTable && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              <h2>Управление: {selectedTable}</h2>
              <button 
                onClick={() => setSelectedTable(null)} 
                className="close-button"
              >
                ×
              </button>
            </div>
            <div className="modal-body">
              {renderTableContent()}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}