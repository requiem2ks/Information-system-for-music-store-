import React, { useState, useEffect } from 'react';
import axios from 'axios';
import ProductModal from '../pages/ProductModel'
import '../styles/catalog.css';

const PriceFilter = ({ priceRange, setPriceRange, maxPrice }) => (
  <div className="price-filter-container">
    <h3>Фильтр по цене</h3>
    <div className="price-inputs">
      <div className="price-input-group">
        <label>От</label>
        <input
          type="number"
          value={priceRange.min}
          onChange={(e) => setPriceRange({
            ...priceRange,
            min: Math.min(Number(e.target.value) || 0, priceRange.max)
          })}
          min="0"
          max={priceRange.max}
          className="price-input"
        />
      </div>
      <div className="price-input-group">
        <label>До</label>
        <input
          type="number"
          value={priceRange.max}
          onChange={(e) => setPriceRange({
            ...priceRange,
            max: Math.max(Number(e.target.value) || maxPrice, priceRange.min)
          })}
          min={priceRange.min}
          max={maxPrice}
          className="price-input"
        />
      </div>
    </div>
  </div>
);

const Catalog = ({ addToCart, reserveItem, reservedItems = [], cartItems = [] }) => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [activeCategory, setActiveCategory] = useState('Все');
  const [priceRange, setPriceRange] = useState({ min: 0, max: 100000 });
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [maxPrice, setMaxPrice] = useState(100000);
  const [error, setError] = useState(null);
  const [storageData, setStorageData] = useState([]);
  
  
  useEffect(() => {
    const fetchData = async () => {
  try {
    setLoading(true);
    setError(null);

    const productsResponse = await axios.get('http://localhost:5000/api/product/withcategories');
        
        
        // Загружаем данные склада
        const storageResponse = await axios.get('http://localhost:5000/api/storage');
        setStorageData(storageResponse.data.$values || []);
    const response = await axios.get('http://localhost:5000/api/product/withcategories');
    
    const productsArray = response.data.$values || [];
    
    const productsData = productsArray.map(item => ({
      id: item.product?.productid || Math.random().toString(36).substr(2, 9),
      name: item.product?.productname || 'Без названия',
      price: item.product?.price || 0,
      description: item.product?.description || '',
      category: item.category?.namecategory || 'Без категории',
      image: item.product?.image || '/placeholder-product.jpg'
    }));

    const prices = productsData.map(p => p.price).filter(price => !isNaN(price));
    const calculatedMaxPrice = prices.length > 0 ? Math.max(...prices, 100000) : 100000;
    setMaxPrice(calculatedMaxPrice);
    setPriceRange(prev => ({ ...prev, max: calculatedMaxPrice }));

    const categoryNames = productsArray
      .map(item => item.category?.namecategory)
      .filter(name => name);

    const uniqueCategories = ['Все', ...new Set(categoryNames)];

    setProducts(productsData);
    setCategories(uniqueCategories);
  } catch (error) {
    console.error("Ошибка загрузки:", error);
    setError('Не удалось загрузить данные. Пожалуйста, попробуйте позже.');
  } finally {
    setLoading(false);
  }
};

    fetchData();
  }, []);

  const handleAddToCart = (product) => {
    addToCart(product);
  };

  const handleReserve = async (product) => {
    try {
      const success = await reserveItem(product);
      if (success) setSelectedProduct(null);
    } catch (error) {
      console.error('Ошибка:', error);
    }
  };

  const filteredProducts = products.filter(product => {
    const categoryMatch = activeCategory === 'Все' || product.category === activeCategory;
    const priceMatch = product.price >= priceRange.min && product.price <= priceRange.max;
    return categoryMatch && priceMatch;
  });

  const isInCart = (productId) => {
    return Array.isArray(cartItems) && cartItems.some(item => item.id === productId);
  };

  const isReserved = (productId) => {
    return reservedItems.includes(productId);
  };

  if (loading) return <div className="loading">Загрузка...</div>;
  if (error) return <div className="error-message">{error}</div>;

  return (
    <div className="catalog-page">
      <h1 className="store-title">Музыкальный магазин</h1>
      <h2 className="catalog-title">Каталог товаров</h2>

      <div className="catalog-container">
        <div className="categories-sidebar">
          <h3>Категории</h3>
          <ul>
            {categories.map(category => (
              <li
                key={category}
                className={activeCategory === category ? 'active' : ''}
                onClick={() => setActiveCategory(category)}
              >
                {category}
              </li>
            ))}
          </ul>

          <PriceFilter
            priceRange={priceRange}
            setPriceRange={setPriceRange}
            maxPrice={maxPrice}
          />
        </div>

        <div className="products-grid">
          {filteredProducts.length > 0 ? (
            filteredProducts.map(product => (
              <div
                key={product.id}
                className={`product-card ${isReserved(product.id) ? 'reserved' : ''}`}
              >
                <div className="product-image-container">
                  <img
                    src={product.image}
                    alt={product.name}
                    className="product-image"
                    onError={(e) => {
                      e.target.onerror = null;
                      e.target.src = '/placeholder-product.jpg';
                    }}
                  />
                  {isReserved(product.id) && (
                    <div className="reserved-badge">Зарезервировано</div>
                  )}
                </div>
                <div className="product-info">
                  <h3>{product.name}</h3>
                  <p className="price">{product.price.toLocaleString('ru-RU')} ₽</p>
                  <div className="product-actions">
                    <button
                      className="details-btn"
                      onClick={() => setSelectedProduct(product)}
                    >
                      Подробнее
                    </button>
                    <button
                      className="add-btn"
                      onClick={() => handleAddToCart(product)}
                      disabled={isInCart(product.id)}
                    >
                      {isInCart(product.id) ? 'В корзину' : 'В корзину'}
                    </button>
                    <button
                      className="reserve-btn"
                      onClick={() => handleReserve(product)}
                      disabled={isInCart(product.id)}
                    >
                      {isReserved(product.id) ? 'Зарезервирован' : 'Зарезервировать'}
                    </button>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="no-products">Нет товаров по выбранным фильтрам</div>
          )}
        </div>
      </div>

      {selectedProduct && (
        <ProductModal
          product={selectedProduct}
          storageData={storageData}
          onClose={() => setSelectedProduct(null)}
        />
      )}
    </div>
  );
};

export default Catalog;