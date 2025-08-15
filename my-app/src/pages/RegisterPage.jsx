import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { registerClient } from '../api/auth';
import '../styles/auth.css';
import '../styles/modal.css';

export default function RegisterPage() {
  const [formData, setFormData] = useState({
    Fio: '',
    Email: '',
    phone: '',
    Password: '',
    confirmPassword: ''
  });
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState({ 
    email: '', 
    general: '',
    password: ''
  });
  const [showSuccessModal, setShowSuccessModal] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
    if (errors[e.target.name]) {
      setErrors({...errors, [e.target.name]: ''});
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setErrors({ email: '', general: '', password: '' });

    try {
      // Валидация паролей
      if (formData.Password !== formData.confirmPassword) {
        setErrors({...errors, password: 'Пароли не совпадают'});
        return;
      }

      // Отправка данных на сервер
      const result = await registerClient({
        Fio: formData.Fio,
        Email: formData.Email,
        phone: formData.phone,
        Password: formData.Password
      });
      
      // Обработка ошибок
      if (result.error) {
        if (result.field === 'email') {
          setErrors({...errors, email: result.error});
        } else {
          setErrors({...errors, general: result.error});
        }
        return;
      }

      // Показ модального окна при успехе
      setShowSuccessModal(true);
      
    } catch (error) {
      setErrors({...errors, general: error.message || 'Произошла ошибка'});
    } finally {
      setIsLoading(false);
    }
  };

  const closeModal = () => {
    setShowSuccessModal(false);
    navigate('/login');
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2 className="auth-title">Регистрация</h2>
        
        {errors.general && (
          <div className="error-message general-error">
            ⚠ {errors.general}
          </div>
        )}

        <form onSubmit={handleSubmit} className="register-form">
          {/* Поле ФИО */}
          <div className="form-group">
            <label className="form-label">ФИО</label>
            <input
              type="text"
              name="Fio"
              value={formData.Fio}
              onChange={handleChange}
              className="form-input"
              placeholder="Иванов Иван Иванович"
              required
            />
          </div>

          {/* Поле Email */}
          <div className="form-group">
            <label className="form-label">Email</label>
            <input
              type="email"
              name="Email"
              value={formData.Email}
              onChange={handleChange}
              className={`form-input ${errors.email ? 'input-error' : ''}`}
              placeholder="example@mail.com"
              required
            />
            {errors.email && (
              <div className="error-message field-error">
                ⚠ {errors.email}
              </div>
            )}
          </div>

          {/* Поле Телефон */}
          <div className="form-group">
            <label className="form-label">Телефон</label>
            <input
              type="tel"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
              className="form-input"
              placeholder="+7 (999) 123-45-67"
            />
          </div>

          {/* Поле Пароль */}
          <div className="form-group">
            <label className="form-label">Пароль</label>
            <input
              type="password"
              name="Password"
              value={formData.Password}
              onChange={handleChange}
              className={`form-input ${errors.password ? 'input-error' : ''}`}
              placeholder="Не менее 8 символов"
              required
              minLength={6}
            />
          </div>

          {/* Поле Подтверждение пароля */}
          <div className="form-group">
            <label className="form-label">Подтвердите пароль</label>
            <input
              type="password"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              className={`form-input ${errors.password ? 'input-error' : ''}`}
              placeholder="Повторите ваш пароль"
              required
            />
            {errors.password && (
              <div className="error-message field-error">
                ⚠ {errors.password}
              </div>
            )}
          </div>

          <button 
            type="submit" 
            className="auth-btn"
            disabled={isLoading}
          >
            {isLoading ? 'Регистрация...' : 'Зарегистрироваться'}
          </button>
        </form>
      </div>

      {showSuccessModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>Регистрация успешна!</h3>
            <p>Ваш аккаунт был успешно создан. Теперь вы можете войти в систему.</p>
            <button 
              onClick={closeModal}
              className="modal-btn"
            >
              Перейти к входу
            </button>
          </div>
        </div>
      )}
    </div>
  );
}