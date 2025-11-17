INSERT INTO USERS (full_name, email, phone, password, role_id, status, created_at, updated_at)
VALUES
('Nguyen Van Staff', 'staff1@vinfast.com', '0900000001', '123456', 1, 'Active', GETDATE(), GETDATE()),
('Tran Van Driver', 'driver1@vinfast.com', '0900000002', '123456', 2, 'Active', GETDATE(), GETDATE()),
('Le Thi Driver', 'driver2@vinfast.com', '0900000003', '123456', 2, 'Active', GETDATE(), GETDATE());
INSERT INTO STATIONS (name, address, status, capacity)
VALUES
('VinFast Station 01', '456 Nguyen Hue, District 1, HCMC', 'Active', 10);
INSERT INTO VEHICLE_MODELS (name, battery_name)
VALUES
('VinFast Klara S', 'KlaraS-Battery'),
('VinFast Evo 200', 'Evo200-Battery');
INSERT INTO VEHICLES (user_id, vin, model_id, current_battery_id)
VALUES
(4, 'RLVSEV20YPC012345', 1, NULL),   -- Klara S
(5, 'RLVSEV10YMB004567', 2, NULL);   -- Evo200
INSERT INTO BATTERIES (station_id, model_id, capacity_kw, status, soh)
VALUES
(1, 1, 30, 'Full', 95.0),
(1, 1, 30, 'Charging', 88.5),
(1, 1, 30, 'Maintenance', 75.0),

(1, 2, 35, 'Full', 96.0),
(1, 2, 35, 'Charging', 82.0),
(1, 2, 35, 'Maintenance', 70.0);
INSERT INTO SUBSCRIPTION_PLANS(name, Description, Price, swap_limit, priority_booking)
VALUES
('Basic Plan', 'Includes up to 10 battery swaps per month.', 350000, 10, 0),
('Premium Plan', 'Includes up to 30 battery swaps per month .', 990000, 30, 1);

INSERT INTO ROLES (name, permissions_json)
VALUES
('Staff', NULL),
('Driver', NULL);