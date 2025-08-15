const express = require('express');
const { Pool } = require('pg');
const bodyParser = require('body-parser');
const cors = require('cors');

const app = express();

// Middleware
app.use(cors());
app.use(bodyParser.json());

// Database configuration
const pool = new Pool({
  user: 'postgres',
  host: 'localhost',
  database: 'postgres',
  password: '123',
  search_path: 'public',
  port: 5777,
});

// Order processing endpoint
app.post('http://localhost:5000/api/Order', async (req, res) => {
  const client = await pool.connect();
  
  try {
    await client.query('BEGIN');
    
    const { client: clientData, order: orderData } = req.body;
    
    // 1. Check user existence
    const userQuery = await client.query(
      'SELECT userid FROM users WHERE email = $1',
      [clientData.email]
    );
    
    let clientId;
    if (userQuery.rows.length > 0) {
      // Find client for existing user
      const clientQuery = await client.query(
        'SELECT clientid FROM clients WHERE userid = $1',
        [userQuery.rows[0].userid]
      );
      
      if (clientQuery.rows.length > 0) {
        clientId = clientQuery.rows[0].clientid;
        // Update existing client
        await client.query(
          `UPDATE clients 
           SET fio = $1, phone = $2, address = COALESCE($3, address)
           WHERE clientid = $4`,
          [clientData.name, clientData.phone, clientData.address, clientId]
        );
      } else {
        // Create new client for existing user
        const newClient = await client.query(
          `INSERT INTO clients (userid, fio, phone, address)
           VALUES ($1, $2, $3, $4)
           RETURNING clientid`,
          [userQuery.rows[0].userid, clientData.name, clientData.phone, clientData.address]
        );
        clientId = newClient.rows[0].clientid;
      }
    } else {
      // Create new user and client (implementation depends on your business logic)
      throw new Error('User registration not implemented');
    }
    
    // 2. Create payment
    const payment = await client.query(
      `INSERT INTO payments (paymentmethod, paysum, paymentdate)
       VALUES ($1, $2, NOW())
       RETURNING paymentid`,
      [orderData.paymentMethod, orderData.total]
    );
    const paymentId = payment.rows[0].paymentid;
    
    // 3. Create order
    const order = await client.query(
      `INSERT INTO orders (clientid, orderdate, status, paymentid)
       VALUES ($1, NOW(), 1, $2)
       RETURNING orderid`,
      [clientId, paymentId]
    );
    const orderId = order.rows[0].orderid;
    
    // 4. Process order items
    for (const item of orderData.items) {
      // Add to orderline
      await client.query(
        `INSERT INTO orderline (orderid, productid, quantity, unitprice)
         VALUES ($1, $2, $3, $4)`,
        [orderId, item.id, item.quantity, item.price]
      );
      
      // Handle reservation if needed
      if (item.isReserved) {
        const reservation = await client.query(
          `INSERT INTO reservations (reservationdate, reservationenddate)
           VALUES (NOW(), NOW() + interval '7 days')
           RETURNING reservationid`
        );
        const reservationId = reservation.rows[0].reservationid;
        
        await client.query(
          `INSERT INTO productreservation (reservationid, orderid, productid, 
           quantityofreserved, unitpricereservation, prepaymentpercentage)
           VALUES ($1, $2, $3, $4, $5, 30)`,
          [reservationId, orderId, item.id, item.quantity, item.price * 0.3]
        );
      }
    }
    
    await client.query('COMMIT');
    
    res.status(201).json({
      success: true,
      orderId: orderId,
      message: 'Заказ успешно оформлен'
    });
    
  } catch (error) {
    await client.query('ROLLBACK');
    console.error('Ошибка при оформлении заказа:', error);
    res.status(500).json({
      success: false,
      message: 'Ошибка при оформлении заказа: ' + error.message
    });
  } finally {
    client.release();
  }
});

// Start server
const PORT = process.env.PORT || 5000;
app.listen(PORT, () => {
  console.log(`Сервер запущен на порту ${PORT}`);
});

