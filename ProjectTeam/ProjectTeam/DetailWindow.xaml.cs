using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProjectTeam
{
    /// <summary>
    /// Interaction logic for DetailWindow.xaml
    /// </summary>
    public partial class DetailWindow : Window
    {
        private string module;
        private object selectedItem;
        private bool isAddMode;
        private Dictionary<string, TextBox> inputFields = new Dictionary<string, TextBox>();
        private Dictionary<string, ComboBox> comboFields = new Dictionary<string, ComboBox>();
        private Dictionary<string, DatePicker> dateFields = new Dictionary<string, DatePicker>();

        public DetailWindow(string module, object selectedItem)
        {
            InitializeComponent();
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
                case "VEHICLES": return "Vehicle";
                case "VEHICLE_MODELS": return "Vehicle Model";
                case "RESERVATIONS":
                case "MY_RESERVATIONS": return "Reservation";
                case "SWAP_TRANSACTIONS": return "Swap Transaction";
                case "PAYMENTS": return "Payment";
                case "SUBSCRIPTION_PLANS": return "Subscription Plan";
                case "USER_SUBSCRIPTIONS": return "User Subscription";
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
            AddTextField("Mật khẩu:", "Password", "");
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
            AddComboField("Trạng thái:", "Status", new List<string> { "Full", "Charging", "Low", "Maintenance" },
                GetPropertyValue("TrangThai")?.ToString() ?? "Charging");
            AddTextField("Sức khỏe (%):", "Soh", GetPropertyValue("SucKhoe")?.ToString().Replace("%", ""));
        }

        private void BuildVehicleForm()
        {
            AddComboField("Chủ xe:", "UserId", GetDrivers(), GetPropertyValue("ChuXe"));
            AddTextField("VIN:", "Vin", GetPropertyValue("Vin"));
            AddComboField("Model:", "ModelId", GetVehicleModels(), GetPropertyValue("Model"));
            AddComboField("Pin hiện tại:", "CurrentBatteryId", GetAvailableBatteries(), GetPropertyValue("PinHienTai"));
        }

        private void BuildVehicleModelForm()
        {
            AddTextField("Tên model:", "Name", GetPropertyValue("TenModel"));
            AddTextField("Tên pin:", "BatteryName", GetPropertyValue("TenPin"));
        }

        private void BuildReservationForm()
        {
            if (App.IsDriver)
            {
                AddComboField("Xe:", "VehicleId", GetMyVehicles(), GetPropertyValue("Xe"));
            }
            else
            {
                AddComboField("Người đặt:", "UserId", GetDrivers(), GetPropertyValue("NguoiDat"));
                AddComboField("Xe:", "VehicleId", GetAllVehicles(), GetPropertyValue("Xe"));
            }
            AddComboField("Trạm:", "StationId", GetStations(), GetPropertyValue("Tram"));
            AddDateTimePicker("Thời gian bắt đầu:", "StartTime", GetPropertyValue("ThoiGianBatDau"));
            AddDateTimePicker("Thời gian kết thúc:", "EndTime", GetPropertyValue("ThoiGianKetThuc"));
            AddComboField("Trạng thái:", "Status", new List<string> { "Pending", "Confirmed", "Completed", "Cancelled" },
                GetPropertyValue("TrangThai")?.ToString() ?? "Pending");
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

        // Helper methods to get data for ComboBoxes
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
                return context.Stations.ToDictionary(s => s.StationId, s => s.Name);
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
                    .Where(u => u.Role.Name == "Driver")
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
                    .Where(b => b.Status == "Full")
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
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
                        case "VEHICLES":
                            SaveVehicle(context);
                            break;
                        case "VEHICLE_MODELS":
                            SaveVehicleModel(context);
                            break;
                        case "RESERVATIONS":
                        case "MY_RESERVATIONS":
                            SaveReservation(context);
                            break;
                        case "SUBSCRIPTION_PLANS":
                            SaveSubscriptionPlan(context);
                            break;
                        default:
                            MessageBox.Show("Chức năng lưu đang được phát triển!",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                    }

                    context.SaveChanges();
                    MessageBox.Show("Lưu thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveUser(EvBatterySwapSystemContext context)
        {
            User user;
            if (isAddMode)
            {
                user = new User();
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
            if (!string.IsNullOrEmpty(inputFields["Password"].Text))
                user.Password = inputFields["Password"].Text;
            user.RoleId = GetComboValue("RoleId");
            user.Status = GetComboString("Status");
            user.UpdatedAt = DateTime.Now;
            if (isAddMode)
            {
                user.CreatedAt = DateTime.Now;
                user.Password = "123456"; // Default password
            }
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

        private void SaveVehicle(EvBatterySwapSystemContext context)
        {
            Vehicle vehicle;
            if (isAddMode)
            {
                vehicle = new Vehicle();
                context.Vehicles.Add(vehicle);
            }
            else
            {
                int vehicleId = Convert.ToInt32(GetPropertyValue("VehicleId"));
                vehicle = context.Vehicles.Find(vehicleId);
            }

            vehicle.UserId = GetComboValue("UserId");
            vehicle.Vin = inputFields["Vin"].Text;
            vehicle.ModelId = GetComboValue("ModelId");
            int batteryId = GetComboValue("CurrentBatteryId");
            vehicle.CurrentBatteryId = batteryId == 0 ? null : batteryId;
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

        private void SaveReservation(EvBatterySwapSystemContext context)
        {
            Reservation reservation;
            if (isAddMode)
            {
                reservation = new Reservation();
                context.Reservations.Add(reservation);
            }
            else
            {
                int reservationId = Convert.ToInt32(GetPropertyValue("ReservationId"));
                reservation = context.Reservations.Find(reservationId);
            }

            if (App.IsDriver)
            {
                reservation.UserId = App.CurrentUserId;
            }
            else
            {
                reservation.UserId = GetComboValue("UserId");
            }

            reservation.VehicleId = GetComboValue("VehicleId");
            reservation.StationId = GetComboValue("StationId");
            reservation.StartTime = dateFields["StartTime"].SelectedDate ?? DateTime.Now;
            reservation.EndTime = dateFields["EndTime"].SelectedDate ?? DateTime.Now.AddHours(1);
            reservation.Status = GetComboString("Status");
            reservation.UpdatedAt = DateTime.Now;

            if (isAddMode)
                reservation.CreatedAt = DateTime.Now;
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

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
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

                            case "VEHICLES":
                                int vehicleId = Convert.ToInt32(GetPropertyValue("VehicleId"));
                                var vehicle = context.Vehicles.Find(vehicleId);
                                if (vehicle != null) context.Vehicles.Remove(vehicle);
                                break;

                            case "VEHICLE_MODELS":
                                int modelId = Convert.ToInt32(GetPropertyValue("ModelId"));
                                var model = context.VehicleModels.Find(modelId);
                                if (model != null) context.VehicleModels.Remove(model);
                                break;

                            case "RESERVATIONS":
                            case "MY_RESERVATIONS":
                                int reservationId = Convert.ToInt32(GetPropertyValue("ReservationId"));
                                var reservation = context.Reservations.Find(reservationId);
                                if (reservation != null) context.Reservations.Remove(reservation);
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
                        MessageBox.Show("Xóa thành công!", "Thành công",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}\n\nCó thể dữ liệu này đang được sử dụng ở nơi khác.",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
