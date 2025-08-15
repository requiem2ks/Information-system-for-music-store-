import React, { useState, useEffect } from 'react';
import '../styles/HomePage.css'; 

export default function HomePage() {
  const [currentSlide, setCurrentSlide] = useState(0);
  const slides = [
   /* {
      image: 'https://spark.ru/upload/other/b_5904dd2a6026b.jpghttps://s14.stc.all.kpcdn.net/afisha/msk/wp-content/uploads/sites/5/2022/08/music-expert.jpg',
      alt: 'Музыкальные инструменты',
      caption: 'Лучшие инструменты для профессионалов'
    },*/
    {
      image: 'https://basis-tp.ru/wp-content/uploads/3/d/5/3d5ca0cedaf82b9a4e375c56f3f0c05a.jpeg',
      alt: 'Концертное оборудование',
      caption: 'Полное оснащение для выступлений'
    },
    {
      image: 'https://avatars.mds.yandex.net/get-altay/10702775/2a0000018dc6f995b4383e27842c095a592f/XXXL',
      alt: 'Студийная аппаратура',
      caption: 'Профессиональное студийное оборудование'
    }
  ];

    useEffect(() => {
    const interval = setInterval(() => {
      setCurrentSlide((prev) => (prev + 1) % slides.length);
    }, 5000);
    return () => clearInterval(interval);
  }, [slides.length]);

  const nextSlide = () => setCurrentSlide((prev) => (prev + 1) % slides.length);
  const prevSlide = () => setCurrentSlide((prev) => (prev - 1 + slides.length) % slides.length);

  return (
    <div className="home-page">
      <div className="fullscreen-slider">
        {slides.map((slide, index) => (
          <div 
            key={index}
            className={`slide ${index === currentSlide ? 'active' : ''}`}
          >
            <img 
              src={slide.image} 
              alt={slide.alt}
              className="slide-image"
            />
          </div>
        ))}
        
        <button className="slider-nav prev" onClick={prevSlide}>&#10094;</button>
        <button className="slider-nav next" onClick={nextSlide}>&#10095;</button>
        
        <div className="slider-dots">
          {slides.map((_, index) => (
            <span 
              key={index}
              className={`dot ${index === currentSlide ? 'active' : ''}`}
              onClick={() => setCurrentSlide(index)}
            />
          ))}
        </div>
      </div>

      <div className="welcome-text">
        <h1>Добро пожаловать в музыкальный магазин</h1>
        <p>Лучший магазин музыкальных инструментов в Рязани</p>
      </div>

      <div className="features">
        <div className="feature-card">
          <h3>Широкий ассортимент</h3>
          <p>Более 1000 товаров от ведущих производителей</p>
        </div>
        
        <div className="feature-card">
          <h3>Быстрая доставка</h3>
          <p>По всей стране за 1-3 дня</p>
        </div>
      </div>
    </div>
  );
}