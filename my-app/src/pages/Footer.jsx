import { Link } from 'react-router-dom';
import '../styles/footer.css';

const Footer = () => {
  return (
    <footer className="footer">
      <div className="footer-container">
        <div className="footer-copyright">
          © 2025 Музыкальный магазин. Все права защищены.
        </div>
        <div className="footer-links">
          <Link to="/privacy" className="footer-link">
            Политика конфиденциальности
          </Link>
          <Link to="/terms" className="footer-link">
            Условия использования
          </Link>
        </div>
      </div>
    </footer>
  );
};

export default Footer;