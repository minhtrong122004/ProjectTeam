-------------------------------------------
-- 1. CREATE DATABASE
-------------------------------------------
IF DB_ID('EvBatterySwapSystem') IS NULL
BEGIN
    CREATE DATABASE EvBatterySwapSystem;
END
GO

USE EvBatterySwapSystem;
GO

-------------------------------------------
-- 2. TABLE: ROLES
-------------------------------------------
CREATE TABLE ROLES (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    permissions_json TEXT NULL
);

-------------------------------------------
-- 3. TABLE: USERS
-------------------------------------------
CREATE TABLE USERS (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name VARCHAR(150) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    phone VARCHAR(20),
    password VARCHAR(255) NOT NULL,
    role_id INT NOT NULL,
    status VARCHAR(50) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_USERS_ROLES FOREIGN KEY (role_id) REFERENCES ROLES(role_id)
);

-------------------------------------------
-- 4. TABLE: VEHICLE_MODELS
-------------------------------------------
CREATE TABLE VEHICLE_MODELS (
    model_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    battery_name VARCHAR(100) NOT NULL
);

-------------------------------------------
-- 5. TABLE: STATIONS
-------------------------------------------
CREATE TABLE STATIONS (
    station_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    address VARCHAR(255),
    status VARCHAR(50),
    capacity INT NOT NULL
);

-------------------------------------------
-- 6. TABLE: BATTERIES
-------------------------------------------
CREATE TABLE BATTERIES (
    battery_id INT IDENTITY(1,1) PRIMARY KEY,
    station_id INT NOT NULL,
    model_id INT NOT NULL,
    capacity_kw INT NOT NULL,
    status VARCHAR(50) NOT NULL,
    soh FLOAT NOT NULL,
    CONSTRAINT FK_BATTERIES_STATIONS FOREIGN KEY (station_id) REFERENCES STATIONS(station_id),
    CONSTRAINT FK_BATTERIES_MODEL FOREIGN KEY (model_id) REFERENCES VEHICLE_MODELS(model_id)
);

-------------------------------------------
-- 7. TABLE: VEHICLES
-------------------------------------------
CREATE TABLE VEHICLES (
    vehicle_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    vin VARCHAR(100) UNIQUE NOT NULL,
    model_id INT NOT NULL,
    current_battery_id INT NULL,

    CONSTRAINT FK_VEHICLES_USERS FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_VEHICLES_MODEL FOREIGN KEY (model_id) REFERENCES VEHICLE_MODELS(model_id),
    CONSTRAINT FK_VEHICLES_BATTERY FOREIGN KEY (current_battery_id) REFERENCES BATTERIES(battery_id),

    -- mỗi user 1 xe
    CONSTRAINT UQ_VEHICLE_USER UNIQUE (user_id)
);

-------------------------------------------
-- 8. TABLE: RESERVATIONS
-------------------------------------------
CREATE TABLE RESERVATIONS (
    reservation_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    vehicle_id INT NOT NULL,
    station_id INT NOT NULL,
    battery_id INT NULL,
    start_time DATETIME NOT NULL,
    end_time DATETIME NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE(),
    status VARCHAR(50) DEFAULT 'Pending',

    CONSTRAINT FK_RESERV_USER FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_RESERV_VEHICLE FOREIGN KEY (vehicle_id) REFERENCES VEHICLES(vehicle_id),
    CONSTRAINT FK_RESERV_STATION FOREIGN KEY (station_id) REFERENCES STATIONS(station_id),
    CONSTRAINT FK_RESERV_BATTERY FOREIGN KEY (battery_id) REFERENCES BATTERIES(battery_id)
);

-------------------------------------------
-- 9. TABLE: SWAP_TRANSACTIONS
-------------------------------------------
CREATE TABLE SWAP_TRANSACTIONS (
    swap_id INT IDENTITY(1,1) PRIMARY KEY,
    reservation_id INT NULL,
    user_id INT NOT NULL,
    vehicle_id INT NOT NULL,
    station_id INT NOT NULL,
    from_battery_id INT NOT NULL,
    to_battery_id INT NOT NULL,
    swap_time DATETIME DEFAULT GETDATE(),
    staff_id INT NOT NULL,
    status VARCHAR(50),

    CONSTRAINT FK_SWAP_RESERV FOREIGN KEY (reservation_id) REFERENCES RESERVATIONS(reservation_id),
    CONSTRAINT FK_SWAP_USER FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_SWAP_VEHICLE FOREIGN KEY (vehicle_id) REFERENCES VEHICLES(vehicle_id),
    CONSTRAINT FK_SWAP_STATION FOREIGN KEY (station_id) REFERENCES STATIONS(station_id),
    CONSTRAINT FK_SWAP_FROM_BAT FOREIGN KEY (from_battery_id) REFERENCES BATTERIES(battery_id),
    CONSTRAINT FK_SWAP_TO_BAT FOREIGN KEY (to_battery_id) REFERENCES BATTERIES(battery_id),
    CONSTRAINT FK_SWAP_STAFF FOREIGN KEY (staff_id) REFERENCES USERS(user_id)
);

-------------------------------------------
-- 10. TABLE: SUBSCRIPTION_PLANS
-------------------------------------------
CREATE TABLE SUBSCRIPTION_PLANS (
    plan_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    price DECIMAL(18,2) NOT NULL,
    swap_limit INT NULL,
    priority_booking BIT DEFAULT 0
);

-------------------------------------------
-- 11. TABLE: USER_SUBSCRIPTIONS
-------------------------------------------
CREATE TABLE USER_SUBSCRIPTIONS (
    subscription_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    vehicle_id INT NOT NULL,
    plan_id INT NOT NULL,
    start_date DATETIME NOT NULL,
    end_date DATETIME NOT NULL,
    status VARCHAR(50) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_USERSUB_USER FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_USERSUB_VEHICLE FOREIGN KEY (vehicle_id) REFERENCES VEHICLES(vehicle_id),
    CONSTRAINT FK_USERSUB_PLAN FOREIGN KEY (plan_id) REFERENCES SUBSCRIPTION_PLANS(plan_id)
);

-------------------------------------------
-- 12. TABLE: PAYMENTS
-------------------------------------------
CREATE TABLE PAYMENTS (
    payment_id INT IDENTITY(1,1) PRIMARY KEY,
    swap_id INT NULL,
    reservation_id INT NULL,
    subscription_id INT NULL,
    user_id INT NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    method VARCHAR(50),
    status VARCHAR(50),
    paid_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_PAY_SWAP FOREIGN KEY (swap_id) REFERENCES SWAP_TRANSACTIONS(swap_id),
    CONSTRAINT FK_PAY_RESERV FOREIGN KEY (reservation_id) REFERENCES RESERVATIONS(reservation_id),
    CONSTRAINT FK_PAY_SUB FOREIGN KEY (subscription_id) REFERENCES USER_SUBSCRIPTIONS(subscription_id),
    CONSTRAINT FK_PAY_USER FOREIGN KEY (user_id) REFERENCES USERS(user_id)
);
