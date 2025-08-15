import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { loginUser } from '../api/auth';
import '../styles/auth.css';

export default function LoginPage({ onAuth }) {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [errors, setErrors] = useState({
    email: '',
    password: '',
    general: ''
  });
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    setErrors(prev => ({ ...prev, [name]: '' }));
  };

  const handleSubmit = async (e) => {
  e.preventDefault();
  setIsLoading(true);
  setErrors({ email: '', password: '', general: '' });

  try {
    if (!formData.email || !formData.password) {
      throw new Error('Заполните все поля');
    }

    const result = await onAuth({
      email: formData.email.trim(),
      password: formData.password
    });

    if (!result) {
      throw new Error('Ошибка авторизации');
    }

    navigate('/profile');
  } catch (error) {
    console.error('Ошибка входа:', error);
    setErrors({
      general: error.message || 'Неверный email или пароль',
      email: error.message.includes('email') ? 'Проверьте email' : '',
      password: error.message.includes('парол') ? 'Проверьте пароль' : ''
    });
  } finally {
    setIsLoading(false);
  }
};

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2 className="auth-title">Вход в систему</h2>
        
        {errors.general && (
          <div className="error-message general-error">
            ⚠ {errors.general}
          </div>
        )}

        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label className="form-label">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className={`form-input ${errors.email ? 'input-error' : ''}`}
              placeholder="Введите ваш email"
              required
            />
            {errors.email && (
              <div className="error-message field-error">
                ⚠ {errors.email}
              </div>
            )}
          </div>

          <div className="form-group">
            <label className="form-label">Пароль</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className={`form-input ${errors.password ? 'input-error' : ''}`}
              placeholder="Введите пароль"
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
            {isLoading ? 'Вход...' : 'Войти'}
          </button>
        </form>
      </div>
    </div>
  );
}