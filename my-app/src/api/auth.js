import axios from 'axios';

// 1. Создаем экземпляр axios
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json'
  }
});

// 2. Добавляем интерцептор запросов (ДО всех функций)
api.interceptors.request.use(config => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const checkEmailAvailability = async (email) => {
  try {
    const response = await api.get(`/auth/check-email?email=${email}`);
    return response.data;
  } catch (error) {
    console.error('Ошибка проверки email:', error);
    return { available: false };
  }
};

export const loginUser = async (credentials) => {
  try {
    // Отправляем запрос на авторизацию
    const response = await api.post('/auth/login', credentials);

    // Проверяем наличие ответа
    if (!response.data) {
      throw new Error('Сервер не вернул данные');
    }

    // Обрабатываем разные форматы ответа
    const { token, accessToken, user, data } = response.data;
    const authToken = token || accessToken;

    // Проверяем наличие токена
    if (!authToken) {
      throw new Error('Отсутствует токен авторизации');
    }

    // Получаем данные пользователя из разных возможных мест
    const userData = user || data || {
      email: credentials.email
      // Другие поля по умолчанию
    };

    // Сохраняем токен
    localStorage.setItem('authToken', authToken);

    // Возвращаем данные
    return {
      token: authToken,
      user: {
        id: userData.id || null,
        email: userData.email || credentials.email,
        name: userData.name || '',
        // Другие поля пользователя
      }
    };

  } catch (error) {
    console.error('Ошибка авторизации:', error);

    // Улучшенная обработка ошибок
    let errorMessage = 'Ошибка при входе';
    
    if (error.response) {
      switch (error.response.status) {
        case 401:
          errorMessage = 'Неверные учетные данные';
          break;
        case 403:
          errorMessage = 'Доступ запрещен';
          break;
        default:
          errorMessage = error.response.data?.message || 'Ошибка сервера';
      }
    } else if (error.message.includes('Отсутствует токен')) {
      errorMessage = 'Ошибка аутентификации';
    }

    throw new Error(errorMessage);
  }
};

// 3. Функции API (ПОСЛЕ интерцептора)
export const registerClient = async (data) => {
  try {
    const response = await api.post('/auth/register/client', data);
    return response.data;
  } catch (error) {
    // Стандартизированная обработка ошибок от бэкенда
    const backendError = error.response?.data;
    
    if (backendError) {
      // Если бэкенд возвращает структурированную ошибку
      return {
        error: backendError.Message || backendError.message || 'Ошибка регистрации. Данная почта уже существует',
        field: backendError.Field || 'general'
      };
    }
    
    // Для сетевых ошибок и других необработанных случаев
    return {
      error: error.message || 'Произошла неизвестная ошибка при регистрации',
      field: 'general'
    };
  }
};

export const login = async (credentials) => {
  try {
    const response = await api.post('/auth/login', credentials);
    localStorage.setItem('authToken', response.data.token); // Сохраняем токен
    return response.data;
  } catch (error) {
    return {
      error: error.response?.data?.message || 
             'Ошибка входа'
    };
  }
};

export const getProtectedData = async () => {
  try {
    const response = await api.get('/auth/protected');
    return response.data;
  } catch (error) {
    return {
      error: error.response?.data?.message || 
             'Ошибка доступа'
    };
  }
};