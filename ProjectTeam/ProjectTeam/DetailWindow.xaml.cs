using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DAL.Entities;
using BLL.Services;
using Microsoft.EntityFrameworkCore;

namespace ProjectTeam
{
    public partial class DetailWindow : Window
    {
        private string module;
        private object selectedItem;
        private bool isAddMode;
        private Dictionary<string, TextBox> inputFields = new Dictionary<string, TextBox>();
        private Dictionary<string, ComboBox> comboFields = new Dictionary<string, ComboBox>();
        private Dictionary<string, DatePicker> dateFields = new Dictionary<string, DatePicker>();

        // Services
        private readonly BatteryService _batteryService;
        private readonly ReservationService _reservationService;
        private readonly SwapTransactionService _swapTransactionService;
        private readonly UserService _userService;
        private readonly UserSubcriptionService _userSubscriptionService;
        private readonly VehicleService _vehicleService;

        public DetailWindow(string module, object selectedItem)
        {
            InitializeComponent();

            // Initialize services
            _batteryService = new BatteryService();
            _reservationService = new ReservationService();
            _swapTransactionService = new SwapTransactionService();
            _userService = new UserService();
            _userSubscriptionService = new UserSubcriptionService();
            _vehicleService = new VehicleService();

            this.module = module;
            this.selectedItem = selectedItem;
            this.isAddMode = selectedItem == null;

            SetupForm();
        }

        private void SetupForm()
        {
            if (isAddMode)
            {
                txtTitle.Text = "Thêm mới " + GetModuleName();
                btnDelete.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtTitle.Text = "Chi tiết " + GetModuleName();
                btnDelete.Visibility = Visibility.Visible;
            }

            BuildForm();
        }

        private string GetModuleName()
        {
            switch (module)
            {
                case "USERS": return "User";
                case "STATIONS": return "Station";
                case "BATTERIES": return "Battery";
                case "VEHICLES":
                case "MY_VEHICLES": return "Vehicle";
                case "VEHICLE_MODELS": return "Vehicle Model";
                case "RESERVATIONS":
                case "MY_RESERVATIONS": return "Reservation";
                case "SWAP_TRANSACTIONS":
                case "MY_SWAPS": return "Swap Transaction";
                case "PAYMENTS":
                case "MY_PAYMENTS": return "Payment";
                case "SUBSCRIPTION_PLANS": return "Subscription Plan";
                case "USER_SUBSCRIPTIONS":
                case "MY_SUBSCRIPTIONS": return "User Subscription";
                default: return module;
            }
        }

        private void BuildForm()
        {
            formPanel.Children.Clear();
            inputFields.Clear();
            comboFields.Clear();
            dateFields.Clear();

            switch (module)
            {
                case "USERS":
                    BuildUserForm();
                    break;
                case "STATIONS":
                    BuildStationForm();
                    break;
                case "BATTERIES":
                    BuildBatteryForm();
                    break;
                case "VEHICLES":
                case "MY_VEHICLES":
                    BuildVehicleForm();
                    break;
                case "VEHICLE_MODELS":
                    BuildVehicleModelForm();
                    break;
                case "RESERVATIONS":
                case "MY_RESERVATIONS":
                    BuildReservationForm();
                    break;
                case "SUBSCRIPTION_PLANS":
                    BuildSubscriptionPlanForm();
                    break;
                case "USER_SUBSCRIPTIONS":
                case "MY_SUBSCRIPTIONS":
                    BuildUserSubscriptionForm();
                    break;
                case "SWAP_TRANSACTIONS":
                case "MY_SWAPS":
                    BuildSwapTransactionForm();
                    break;
                default:
                    formPanel.Children.Add(new TextBlock
                    {
                        Text = "Form đang được phát triển...",
                        FontSize = 14,
                        Foreground = Brushes.Gray
                    });
                    btnSave.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                    break;
            }
        }

        private void BuildUserForm()
        {
            AddTextField("Họ tên:", "FullName", GetPropertyValue("HoTen"));
            AddTextField("Email:", "Email", GetPropertyValue("Email"));
            AddTextField("Điện thoại:", "Phone", GetPropertyValue("DienThoai"));
            if (isAddMode)
            {
                AddTextField("Mật khẩu:", "Password", "");
            }
            AddComboField("Vai trò:", "RoleId", GetRoles(), GetPropertyValue("VaiTro"));
            AddComboField("Trạng thái:", "Status", new List<string> { "Active", "Inactive" },
                GetPropertyValue("TrangThai")?.ToString() ?? "Active");
        }

        private void BuildStationForm()
        {
            AddTextField("Tên trạm:", "Name", GetPropertyValue("TenTram"));
            AddTextField("Địa chỉ:", "Address", GetPropertyValue("DiaChi"));
            AddTextField("Sức chứa:", "Capacity", GetPropertyValue("SucChua"));
            AddComboField("Trạng thái:", "Status", new List<string> { "Active", "Inactive", "Maintenance" },
                GetPropertyValue("TrangThai")?.ToString() ?? "Active");
        }

        private void BuildBatteryForm()
        {
            AddComboField("Trạm:", "StationId", GetStations(), GetPropertyValue("Tram"));
            AddComboField("Model xe:", "ModelId", GetVehicleModels(), GetPropertyValue("ModelXe"));
            AddTextField("Công suất (kW):", "CapacityKw", GetPropertyValue("CongSuat")?.ToString().Replace(" kW", ""));
            AddComboField("Trạng thái:", "Status",
                new List<string> { "Available", "InUse", "Charging", "Maintenance", "Decommissioned" },
                GetPropertyValue("TrangThai")?.ToString() ?? "Charging");
            AddTextField("Sức khỏe (%):", "Soh", GetPropertyValue("SucKhoe")?.ToString().Replace("%", ""));
        }

        private void BuildVehicleForm()
        {
            if (module == "MY_VEHICLES")
            {
                // Driver chỉ thêm xe cho chính mình
                AddTextField("VIN:", "Vin", GetPropertyValue("Vin"));
                AddComboField("Model:", "ModelId", GetVehicleModels(), GetPropertyValue("Model"));
            }
            else
            {
                AddComboField("Chủ xe:", "UserId", GetDrivers(), GetPropertyValue("ChuXe"));
                AddTextField("VIN:", "Vin", GetPropertyValue("Vin"));
                AddComboField("Model:", "ModelId", GetVehicleModels(), GetPropertyValue("Model"));
                AddComboField("Pin hiện tại:", "CurrentBatteryId", GetAvailableBatteries(), GetPropertyValue("PinHienTai"));
            }
        }

        private void BuildVehicleModelForm()
        {
            AddTextField("Tên model:", "Name", GetPropertyValue("TenModel"));
            AddTextField("Tên pin:", "BatteryName", GetPropertyValue("TenPin"));
        }

        private void BuildReservationForm()
        {
            if (isAddMode)
            {
                // Chỉ cho phép tạo mới từ Driver
                if (App.IsDriver)
                {
                    AddComboField("Xe:", "VehicleId", GetMyVehicles(), null);
                    AddComboField("Trạm:", "StationId", GetStations(), null);
                    AddDateTimePicker("Thời gian đặt:", "StartTime", DateTime.Now);
                }
                else
                {
                    MessageBox.Show("Staff không thể tạo Reservation mới. Chỉ có thể cập nhật!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnSave.IsEnabled = false;
                }
            }
            else
            {
                // Chỉ hiển thị thông tin
                AddReadOnlyField("Người đặt:", GetPropertyValue("NguoiDat")?.ToString() ?? "N/A");
                AddReadOnlyField("Xe:", GetPropertyValue("Xe")?.ToString() ?? "N/A");
                AddReadOnlyField("Trạm:", GetPropertyValue("Tram")?.ToString() ?? "N/A");
                AddReadOnlyField("Thời gian bắt đầu:", GetPropertyValue("ThoiGianBatDau")?.ToString() ?? "N/A");
                AddReadOnlyField("Thời gian kết thúc:", GetPropertyValue("ThoiGianKetThuc")?.ToString() ?? "N/A");

                // Staff có thể cập nhật trạng thái
                if (App.IsStaff)
                {
                    AddComboField("Trạng thái:", "Status",
                        new List<string> { "Pending", "Confirmed", "Completed", "Cancelled" },
                        GetPropertyValue("TrangThai")?.ToString() ?? "Pending");
                }
                else
                {
                    AddReadOnlyField("Trạng thái:", GetPropertyValue("TrangThai")?.ToString() ?? "N/A");
                    btnSave.IsEnabled = false;
                }
            }
        }

        private void BuildUserSubscriptionForm()
        {
            if (isAddMode)
            {
                if (App.IsDriver)
                {
                    AddComboField("Xe:", "VehicleId", GetMyVehicles(), null);
                    AddComboField("Gói đăng ký:", "PlanId", GetSubscriptionPlans(), null);
                }
                else
                {
                    AddComboField("Người dùng:", "UserId", GetDrivers(), null);
                    AddComboField("Xe:", "VehicleId", GetAllVehicles(), null);
                    AddComboField("Gói đăng ký:", "PlanId", GetSubscriptionPlans(), null);
                }
            }
            else
            {
                AddReadOnlyField("Người dùng:", GetPropertyValue("NguoiDung")?.ToString() ?? "N/A");
                AddReadOnlyField("Xe:", GetPropertyValue("Xe")?.ToString() ?? "N/A");
                AddReadOnlyField("Gói đăng ký:", GetPropertyValue("GoiDangKy")?.ToString() ?? "N/A");
                AddReadOnlyField("Ngày bắt đầu:", GetPropertyValue("NgayBatDau")?.ToString() ?? "N/A");
                AddReadOnlyField("Ngày kết thúc:", GetPropertyValue("NgayKetThuc")?.ToString() ?? "N/A");
                AddReadOnlyField("Số lần còn lại:", GetPropertyValue("SoLanConLai")?.ToString() ?? "N/A");

                if (App.IsStaff)
                {
                    AddComboField("Trạng thái:", "Status",
                        new List<string> { "Pending", "Active", "Expired", "Cancelled" },
                        GetPropertyValue("TrangThai")?.ToString() ?? "Pending");
                }
                else
                {
                    AddReadOnlyField("Trạng thái:", GetPropertyValue("TrangThai")?.ToString() ?? "N/A");
                    btnSave.IsEnabled = false;
                }
            }
        }

        private void BuildSwapTransactionForm()
        {
            if (isAddMode)
            {
                // Chỉ Staff mới có thể tạo Swap Transaction
                if (App.IsStaff)
                {
                    AddComboField("Reservation:", "ReservationId", GetConfirmedReservations(), null);
                }
                else
                {
                    MessageBox.Show("Chỉ Staff mới có thể tạo giao dịch thay pin!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    btnSave.IsEnabled = false;
                }
            }
            else
            {
                AddReadOnlyField("Tài xế:", GetPropertyValue("TaiXe")?.ToString() ?? "N/A");
                AddReadOnlyField("Trạm:", GetPropertyValue("Tram")?.ToString() ?? "N/A");
                AddReadOnlyField("Xe:", GetPropertyValue("Xe")?.ToString() ?? "N/A");
                AddReadOnlyField("Thời gian:", GetPropertyValue("ThoiGian")?.ToString() ?? "N/A");
                AddReadOnlyField("Nhân viên:", GetPropertyValue("NhanVien")?.ToString() ?? "N/A");

                if (App.IsStaff)
                {
                    AddComboField("Trạng thái:", "Status",
                        new List<string> { "Pending", "Completed", "Failed" },
                        GetPropertyValue("TrangThai")?.ToString() ?? "Pending");
                }
                else
                {
                    AddReadOnlyField("Trạng thái:", GetPropertyValue("TrangThai")?.ToString() ?? "N/A");
                    btnSave.IsEnabled = false;
                }
            }
        }

        private void BuildSubscriptionPlanForm()
        {
            AddTextField("Tên gói:", "Name", GetPropertyValue("TenGoi"));
            AddTextField("Mô tả:", "Description", GetPropertyValue("MoTa"));
            AddTextField("Giá:", "Price", GetPropertyValue("Gia")?.ToString().Replace(" VND", ""));
            AddTextField("Giới hạn đổi pin:", "SwapLimit", GetPropertyValue("GioiHanDoiPin"));
            AddComboField("Ưu tiên đặt chỗ:", "PriorityBooking",
                new List<string> { "Có", "Không" }, GetPropertyValue("UuTienDatCho")?.ToString() ?? "Không");
        }

        private void AddReadOnlyField(string label, string value)
        {
            TextBlock lbl = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8)
            };
            formPanel.Children.Add(lbl);

            Border border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 15),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"))
            };

            TextBlock txt = new TextBlock
            {
                Text = value,
                FontSize = 13,
                Padding = new Thickness(12),
                VerticalAlignment = VerticalAlignment.Center
            };

            border.Child = txt;
            formPanel.Children.Add(border);
        }

        private void AddTextField(string label, string propertyName, object value)
        {
            TextBlock lbl = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8)
            };
            formPanel.Children.Add(lbl);

            Border border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCC")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 15)
            };

            TextBox txt = new TextBox
            {
                Height = 40,
                FontSize = 13,
                Padding = new Thickness(12, 0, 12, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderThickness = new Thickness(0),
                Text = value?.ToString() ?? ""
            };

            border.Child = txt;
            formPanel.Children.Add(border);
            inputFields[propertyName] = txt;
        }

        private void AddComboField(string label, string propertyName, Dictionary<int, string> items, object selectedValue)
        {
            TextBlock lbl = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8)
            };
            formPanel.Children.Add(lbl);

            Border border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCC")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 15)
            };

            ComboBox cmb = new ComboBox
            {
                Height = 40,
                FontSize = 13,
                Padding = new Thickness(12, 0, 12, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderThickness = new Thickness(0)
            };

            foreach (var item in items)
            {
                cmb.Items.Add(new ComboBoxItem { Content = item.Value, Tag = item.Key });
            }

            if (selectedValue != null)
            {
                foreach (ComboBoxItem item in cmb.Items)
                {
                    if (item.Content.ToString() == selectedValue.ToString())
                    {
                        cmb.SelectedItem = item;
                        break;
                    }
                }
            }

            if (cmb.SelectedItem == null && cmb.Items.Count > 0)
                cmb.SelectedIndex = 0;

            border.Child = cmb;
            formPanel.Children.Add(border);
            comboFields[propertyName] = cmb;
        }

        private void AddComboField(string label, string propertyName, List<string> items, string selectedValue)
        {
            TextBlock lbl = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8)
            };
            formPanel.Children.Add(lbl);

            Border border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCC")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 15)
            };

            ComboBox cmb = new ComboBox
            {
                Height = 40,
                FontSize = 13,
                Padding = new Thickness(12, 0, 12, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderThickness = new Thickness(0),
                ItemsSource = items,
                SelectedItem = selectedValue ?? items[0]
            };

            border.Child = cmb;
            formPanel.Children.Add(border);
            comboFields[propertyName] = cmb;
        }

        private void AddDateTimePicker(string label, string propertyName, object value)
        {
            TextBlock lbl = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8)
            };
            formPanel.Children.Add(lbl);

            Border border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCC")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 15)
            };

            DatePicker dp = new DatePicker
            {
                Height = 40,
                FontSize = 13,
                Padding = new Thickness(12, 0, 12, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderThickness = new Thickness(0),
                SelectedDate = value != null && DateTime.TryParse(value.ToString(), out DateTime dt)
                    ? dt : DateTime.Now
            };

            border.Child = dp;
            formPanel.Children.Add(border);
            dateFields[propertyName] = dp;
        }

        private object GetPropertyValue(string propertyName)
        {
            if (selectedItem == null) return null;
            var prop = selectedItem.GetType().GetProperty(propertyName);
            return prop?.GetValue(selectedItem);
        }

        // Helper methods
        private Dictionary<int, string> GetRoles()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                return context.Roles.ToDictionary(r => r.RoleId, r => r.Name);
            }
        }

        private Dictionary<int, string> GetStations()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                return context.Stations
                    .Where(s => s.Status == "Active")
                    .ToDictionary(s => s.StationId, s => s.Name);
            }
        }

        private Dictionary<int, string> GetVehicleModels()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                return context.VehicleModels.ToDictionary(vm => vm.ModelId, vm => vm.Name);
            }
        }

        private Dictionary<int, string> GetDrivers()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                return context.Users
                    .Where(u => u.Role.Name == "Driver" && u.Status == "Active")
                    .Include(u => u.Role)
                    .ToDictionary(u => u.UserId, u => u.FullName);
            }
        }

        private Dictionary<int, string> GetAvailableBatteries()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                var dict = new Dictionary<int, string> { { 0, "Không có" } };
                var batteries = context.Batteries
                    .Where(b => b.Status == "Available")
                    .ToDictionary(b => b.BatteryId, b => $"Battery #{b.BatteryId} - {b.Soh}%");

                foreach (var b in batteries)
                    dict.Add(b.Key, b.Value);

                return dict;
            }
        }

        private Dictionary<int, string> GetMyVehicles()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                return context.Vehicles
                    .Where(v => v.UserId == App.CurrentUserId)
                    .ToDictionary(v => v.VehicleId, v => v.Vin);
            }
        }

        private Dictionary<int, string> GetAllVehicles()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                return context.Vehicles.ToDictionary(v => v.VehicleId, v => v.Vin);
            }
        }

        private Dictionary<int, string> GetSubscriptionPlans()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                return context.SubscriptionPlans
                    .ToDictionary(sp => sp.PlanId, sp => $"{sp.Name} - {sp.Price:N0} VND");
            }
        }

        private Dictionary<int, string> GetConfirmedReservations()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                return context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Vehicle)
                    .Where(r => r.Status == "Confirmed")
                    .ToDictionary(r => r.ReservationId,
                        r => $"#{r.ReservationId} - {r.User.FullName} - {r.Vehicle.Vin}");
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (module)
                {
                    case "VEHICLES":
                    case "MY_VEHICLES":
                        await SaveVehicleAsync();
                        break;
                    case "RESERVATIONS":
                    case "MY_RESERVATIONS":
                        await SaveReservationAsync();
                        break;
                    case "USER_SUBSCRIPTIONS":
                    case "MY_SUBSCRIPTIONS":
                        await SaveUserSubscriptionAsync();
                        break;
                    case "SWAP_TRANSACTIONS":
                        await SaveSwapTransactionAsync();
                        break;
                    default:
                        SaveWithContext();
                        break;
                }

                MessageBox.Show("Lưu thành công!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveVehicleAsync()
        {
            if (isAddMode)
            {
                int userId = module == "MY_VEHICLES" ? App.CurrentUserId : GetComboValue("UserId");
                int modelId = GetComboValue("ModelId");
                string vin = inputFields["Vin"].Text.Trim();

                await _vehicleService.RegisterVehicleAsync(userId, modelId, vin);
            }
            else
            {
                using (var context = new EvBatterySwapSystemContext())
                {
                    int vehicleId = Convert.ToInt32(GetPropertyValue("VehicleId"));
                    var vehicle = await context.Vehicles.FindAsync(vehicleId);

                    if (module != "MY_VEHICLES")
                    {
                        vehicle.UserId = GetComboValue("UserId");
                        int batteryId = GetComboValue("CurrentBatteryId");
                        vehicle.CurrentBatteryId = batteryId == 0 ? null : batteryId;
                    }

                    vehicle.ModelId = GetComboValue("ModelId");
                    vehicle.Vin = inputFields["Vin"].Text.Trim();

                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task SaveReservationAsync()
        {
            if (isAddMode)
            {
                int vehicleId = GetComboValue("VehicleId");
                int stationId = GetComboValue("StationId");
                DateTime startTime = dateFields["StartTime"].SelectedDate ?? DateTime.Now;

                await _reservationService.BookReservationAsync(
                    App.CurrentUserId, vehicleId, stationId, startTime);
            }
            else
            {
                int reservationId = Convert.ToInt32(GetPropertyValue("ReservationId"));
                string status = GetComboString("Status");

                await _reservationService.UpdateStatusReservationAsync(reservationId, status);
            }
        }

        private async Task SaveUserSubscriptionAsync()
        {
            if (isAddMode)
            {
                int userId = App.IsDriver ? App.CurrentUserId : GetComboValue("UserId");
                int vehicleId = GetComboValue("VehicleId");
                int planId = GetComboValue("PlanId");

                await _userSubscriptionService.RegisterSubscriptionAsync(userId, vehicleId, planId);
            }
            else
            {
                int subscriptionId = Convert.ToInt32(GetPropertyValue("SubscriptionId"));
                string status = GetComboString("Status");

                await _userSubscriptionService.UpdateStatusUserSubcription(subscriptionId, status);
            }
        }

        private async Task SaveSwapTransactionAsync()
        {
            if (isAddMode)
            {
                int reservationId = GetComboValue("ReservationId");
                await _swapTransactionService.CreateSwapTransactionByReservationAsync(reservationId);
            }
            else
            {
                int swapId = Convert.ToInt32(GetPropertyValue("SwapId"));
                string status = GetComboString("Status");

                await _swapTransactionService.UpdateStatusByUserAsync(App.CurrentUserId, swapId, status);
            }
        }

        private void SaveWithContext()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                switch (module)
                {
                    case "USERS":
                        SaveUser(context);
                        break;
                    case "STATIONS":
                        SaveStation(context);
                        break;
                    case "BATTERIES":
                        SaveBattery(context);
                        break;
                    case "VEHICLE_MODELS":
                        SaveVehicleModel(context);
                        break;
                    case "SUBSCRIPTION_PLANS":
                        SaveSubscriptionPlan(context);
                        break;
                }
                context.SaveChanges();
            }
        }

        private void SaveUser(EvBatterySwapSystemContext context)
        {
            User user;
            if (isAddMode)
            {
                user = new User
                {
                    CreatedAt = DateTime.UtcNow,
                    Password = inputFields["Password"].Text
                };
                context.Users.Add(user);
            }
            else
            {
                int userId = Convert.ToInt32(GetPropertyValue("UserId"));
                user = context.Users.Find(userId);
            }

            user.FullName = inputFields["FullName"].Text;
            user.Email = inputFields["Email"].Text;
            user.Phone = inputFields["Phone"].Text;
            user.RoleId = GetComboValue("RoleId");
            user.Status = GetComboString("Status");
            user.UpdatedAt = DateTime.UtcNow;
        }

        private void SaveStation(EvBatterySwapSystemContext context)
        {
            Station station;
            if (isAddMode)
            {
                station = new Station();
                context.Stations.Add(station);
            }
            else
            {
                int stationId = Convert.ToInt32(GetPropertyValue("StationId"));
                station = context.Stations.Find(stationId);
            }

            station.Name = inputFields["Name"].Text;
            station.Address = inputFields["Address"].Text;
            station.Capacity = int.Parse(inputFields["Capacity"].Text);
            station.Status = GetComboString("Status");
        }

        private void SaveBattery(EvBatterySwapSystemContext context)
        {
            Battery battery;
            if (isAddMode)
            {
                battery = new Battery();
                context.Batteries.Add(battery);
            }
            else
            {
                int batteryId = Convert.ToInt32(GetPropertyValue("BatteryId"));
                battery = context.Batteries.Find(batteryId);
            }

            battery.StationId = GetComboValue("StationId");
            battery.ModelId = GetComboValue("ModelId");
            battery.CapacityKw = int.Parse(inputFields["CapacityKw"].Text);
            battery.Status = GetComboString("Status");
            battery.Soh = double.Parse(inputFields["Soh"].Text);
        }

        private void SaveVehicleModel(EvBatterySwapSystemContext context)
        {
            VehicleModel model;
            if (isAddMode)
            {
                model = new VehicleModel();
                context.VehicleModels.Add(model);
            }
            else
            {
                int modelId = Convert.ToInt32(GetPropertyValue("ModelId"));
                model = context.VehicleModels.Find(modelId);
            }

            model.Name = inputFields["Name"].Text;
            model.BatteryName = inputFields["BatteryName"].Text;
        }

        private void SaveSubscriptionPlan(EvBatterySwapSystemContext context)
        {
            SubscriptionPlan plan;
            if (isAddMode)
            {
                plan = new SubscriptionPlan();
                context.SubscriptionPlans.Add(plan);
            }
            else
            {
                int planId = Convert.ToInt32(GetPropertyValue("PlanId"));
                plan = context.SubscriptionPlans.Find(planId);
            }

            plan.Name = inputFields["Name"].Text;
            plan.Description = inputFields["Description"].Text;
            plan.Price = decimal.Parse(inputFields["Price"].Text);
            plan.SwapLimit = int.Parse(inputFields["SwapLimit"].Text);
            plan.PriorityBooking = GetComboString("PriorityBooking") == "Có";
        }

        private int GetComboValue(string propertyName)
        {
            if (comboFields.ContainsKey(propertyName))
            {
                var selectedItem = comboFields[propertyName].SelectedItem;
                if (selectedItem is ComboBoxItem item)
                {
                    return Convert.ToInt32(item.Tag);
                }
            }
            return 0;
        }

        private string GetComboString(string propertyName)
        {
            if (comboFields.ContainsKey(propertyName))
            {
                var selectedItem = comboFields[propertyName].SelectedItem;
                if (selectedItem is ComboBoxItem item)
                {
                    return item.Content.ToString();
                }
                return selectedItem?.ToString() ?? "";
            }
            return "";
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    switch (module)
                    {
                        case "VEHICLES":
                        case "MY_VEHICLES":
                            await DeleteVehicleAsync();
                            break;
                        case "RESERVATIONS":
                        case "MY_RESERVATIONS":
                            await DeleteReservationAsync();
                            break;
                        default:
                            DeleteWithContext();
                            break;
                    }

                    MessageBox.Show("Xóa thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}\n\nCó thể dữ liệu này đang được sử dụng ở nơi khác.",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task DeleteVehicleAsync()
        {
            int vehicleId = Convert.ToInt32(GetPropertyValue("VehicleId"));
            await _vehicleService.DeleteAsync(vehicleId);
        }

        private async Task DeleteReservationAsync()
        {
            int reservationId = Convert.ToInt32(GetPropertyValue("ReservationId"));
            await _reservationService.DeleteReservationAsync(reservationId);
        }

        private void DeleteWithContext()
        {
            using (var context = new EvBatterySwapSystemContext())
            {
                switch (module)
                {
                    case "USERS":
                        int userId = Convert.ToInt32(GetPropertyValue("UserId"));
                        var user = context.Users.Find(userId);
                        if (user != null) context.Users.Remove(user);
                        break;

                    case "STATIONS":
                        int stationId = Convert.ToInt32(GetPropertyValue("StationId"));
                        var station = context.Stations.Find(stationId);
                        if (station != null) context.Stations.Remove(station);
                        break;

                    case "BATTERIES":
                        int batteryId = Convert.ToInt32(GetPropertyValue("BatteryId"));
                        var battery = context.Batteries.Find(batteryId);
                        if (battery != null) context.Batteries.Remove(battery);
                        break;

                    case "VEHICLE_MODELS":
                        int modelId = Convert.ToInt32(GetPropertyValue("ModelId"));
                        var model = context.VehicleModels.Find(modelId);
                        if (model != null) context.VehicleModels.Remove(model);
                        break;

                    case "SUBSCRIPTION_PLANS":
                        int planId = Convert.ToInt32(GetPropertyValue("PlanId"));
                        var plan = context.SubscriptionPlans.Find(planId);
                        if (plan != null) context.SubscriptionPlans.Remove(plan);
                        break;

                    default:
                        MessageBox.Show("Chức năng xóa đang được phát triển!",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                }

                context.SaveChanges();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}