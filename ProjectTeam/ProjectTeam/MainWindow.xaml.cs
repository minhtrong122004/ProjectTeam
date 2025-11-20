using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DAL.Entities;
using BLL.Services;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTeam
{
    public partial class MainWindow : Window
    {
        private string currentModule = "";

        // Services
        private readonly BatteryService _batteryService;
        private readonly ReservationService _reservationService;
        private readonly SwapTransactionService _swapTransactionService;
        private readonly UserService _userService;
        private readonly UserSubcriptionService _userSubscriptionService;
        private readonly VehicleService _vehicleService;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize services
            _batteryService = new BatteryService();
            _reservationService = new ReservationService();
            _swapTransactionService = new SwapTransactionService();
            _userService = new UserService();
            _userSubscriptionService = new UserSubcriptionService();
            _vehicleService = new VehicleService();

            LoadUserInfo();
            SetupMenuByRole();
        }

        private void LoadUserInfo()
        {
            txtUserInfo.Text = $"👤 {App.CurrentUserName} ({App.CurrentRoleName})";
        }

        private void SetupMenuByRole()
        {
            if (App.IsStaff)
            {
                AddMenuItem("👥 Quản lý Users", "USERS");
                AddMenuItem("🏢 Quản lý Stations", "STATIONS");
                AddMenuItem("🔋 Quản lý Batteries", "BATTERIES");
                AddMenuItem("🚗 Quản lý Vehicles", "VEHICLES");
                AddMenuItem("🚙 Quản lý Vehicle Models", "VEHICLE_MODELS");
                AddMenuItem("📋 Quản lý Reservations", "RESERVATIONS");
                AddMenuItem("🔄 Giao dịch thay pin", "SWAP_TRANSACTIONS");
                AddMenuItem("💰 Quản lý Payments", "PAYMENTS");
                AddMenuItem("📦 Gói Subscription", "SUBSCRIPTION_PLANS");
                AddMenuItem("👥 User Subscriptions", "USER_SUBSCRIPTIONS");
            }
            else if (App.IsDriver)
            {
                AddMenuItem("🚗 Xe của tôi", "MY_VEHICLES");
                AddMenuItem("📋 Đặt chỗ của tôi", "MY_RESERVATIONS");
                AddMenuItem("🔄 Lịch sử thay pin", "MY_SWAPS");
                AddMenuItem("💳 Gói đăng ký của tôi", "MY_SUBSCRIPTIONS");
                AddMenuItem("💰 Thanh toán của tôi", "MY_PAYMENTS");
                AddMenuItem("🏢 Danh sách Trạm", "STATIONS");
                AddMenuItem("🔋 Danh sách Pin", "BATTERIES");
            }

            if (sidebarMenu.Children.Count > 0)
            {
                var firstButton = sidebarMenu.Children[0] as Button;
                firstButton?.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void AddMenuItem(string text, string module)
        {
            Button btn = new Button
            {
                Content = text,
                Height = 50,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057")),
                FontSize = 14,
                FontWeight = FontWeights.Normal,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(25, 0, 0, 0),
                Cursor = Cursors.Hand,
                Tag = module
            };

            btn.Click += MenuItem_Click;
            sidebarMenu.Children.Add(btn);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            currentModule = btn.Tag.ToString();

            foreach (var child in sidebarMenu.Children)
            {
                if (child is Button b)
                {
                    b.Background = Brushes.Transparent;
                    b.FontWeight = FontWeights.Normal;
                }
            }
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E7F3FF"));
            btn.FontWeight = FontWeights.Bold;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                using (var context = new EvBatterySwapSystemContext())
                {
                    object data = null;

                    switch (currentModule)
                    {
                        case "USERS":
                            data = context.Users
                                .Include(u => u.Role)
                                .Select(u => new
                                {
                                    u.UserId,
                                    HoTen = u.FullName,
                                    u.Email,
                                    DienThoai = u.Phone,
                                    VaiTro = u.Role.Name,
                                    TrangThai = u.Status
                                }).ToList();
                            break;

                        case "STATIONS":
                            data = context.Stations
                                .Select(s => new
                                {
                                    s.StationId,
                                    TenTram = s.Name,
                                    DiaChi = s.Address,
                                    TrangThai = s.Status,
                                    SucChua = s.Capacity
                                }).ToList();
                            break;

                        case "BATTERIES":
                            var batteries = await _batteryService.GetAllAsync();
                            data = batteries.Select(b => new
                            {
                                b.BatteryId,
                                Tram = b.Station?.Name ?? "N/A",
                                ModelXe = b.Model?.Name ?? "N/A",
                                CongSuat = b.CapacityKw + " kW",
                                TrangThai = b.Status,
                                SucKhoe = b.Soh + "%"
                            }).ToList();
                            break;

                        case "VEHICLES":
                            data = context.Vehicles
                                .Include(v => v.User)
                                .Include(v => v.Model)
                                .Include(v => v.CurrentBattery)
                                .Select(v => new
                                {
                                    v.VehicleId,
                                    ChuXe = v.User.FullName,
                                    v.Vin,
                                    Model = v.Model.Name,
                                    PinHienTai = v.CurrentBattery != null ? v.CurrentBattery.BatteryId.ToString() : "Chưa có"
                                }).ToList();
                            break;

                        case "VEHICLE_MODELS":
                            data = context.VehicleModels
                                .Select(vm => new
                                {
                                    vm.ModelId,
                                    TenModel = vm.Name,
                                    TenPin = vm.BatteryName
                                }).ToList();
                            break;

                        case "RESERVATIONS":
                            var allReservations = await _reservationService.GetAllReservationsAsync();
                            data = allReservations.Select(r => new
                            {
                                r.ReservationId,
                                NguoiDat = r.User?.FullName ?? "N/A",
                                Tram = r.Station?.Name ?? "N/A",
                                Xe = r.Vehicle?.Vin ?? "N/A",
                                ThoiGianBatDau = r.StartTime,
                                ThoiGianKetThuc = r.EndTime,
                                TrangThai = r.Status
                            }).ToList();
                            break;

                        case "SWAP_TRANSACTIONS":
                            data = context.SwapTransactions
                                .Include(st => st.User)
                                .Include(st => st.Station)
                                .Include(st => st.Staff)
                                .Include(st => st.Vehicle)
                                .Select(st => new
                                {
                                    st.SwapId,
                                    TaiXe = st.User.FullName,
                                    Tram = st.Station.Name,
                                    Xe = st.Vehicle.Vin,
                                    ThoiGian = st.SwapTime,
                                    NhanVien = st.Staff != null ? st.Staff.FullName : "Chưa có",
                                    TrangThai = st.Status
                                }).ToList();
                            break;

                        case "PAYMENTS":
                            data = context.Payments
                                .Include(p => p.User)
                                .Select(p => new
                                {
                                    p.PaymentId,
                                    NguoiThanhToan = p.User.FullName,
                                    SoTien = p.Amount.ToString("N0") + " VND",
                                    PhuongThuc = p.Method,
                                    TrangThai = p.Status,
                                    ThoiGian = p.PaidAt
                                }).ToList();
                            break;

                        case "SUBSCRIPTION_PLANS":
                            data = context.SubscriptionPlans
                                .Select(sp => new
                                {
                                    sp.PlanId,
                                    TenGoi = sp.Name,
                                    MoTa = sp.Description,
                                    Gia = sp.Price.ToString("N0") + " VND",
                                    GioiHanDoiPin = sp.SwapLimit,
                                    UuTienDatCho = sp.PriorityBooking == true ? "Có" : "Không"
                                }).ToList();
                            break;

                        case "USER_SUBSCRIPTIONS":
                            data = context.UserSubscriptions
                                .Include(us => us.User)
                                .Include(us => us.Plan)
                                .Include(us => us.Vehicle)
                                .Select(us => new
                                {
                                    us.SubscriptionId,
                                    NguoiDung = us.User.FullName,
                                    Xe = us.Vehicle.Vin,
                                    GoiDangKy = us.Plan.Name,
                                    NgayBatDau = us.StartDate,
                                    NgayKetThuc = us.EndDate,
                                    SoLanConLai = us.SwapLimit,
                                    TrangThai = us.Status
                                }).ToList();
                            break;

                        // Driver modules
                        case "MY_VEHICLES":
                            data = context.Vehicles
                                .Include(v => v.Model)
                                .Include(v => v.CurrentBattery)
                                .Where(v => v.UserId == App.CurrentUserId)
                                .Select(v => new
                                {
                                    v.VehicleId,
                                    v.Vin,
                                    Model = v.Model.Name,
                                    PinHienTai = v.CurrentBattery != null ? v.CurrentBattery.BatteryId.ToString() : "Chưa có",
                                    SucKhoePin = v.CurrentBattery != null ? v.CurrentBattery.Soh + "%" : "N/A"
                                }).ToList();
                            break;

                        case "MY_RESERVATIONS":
                            var myReservations = await _reservationService.GetReservationsByUserIdAsync(App.CurrentUserId);
                            data = myReservations.Select(r => new
                            {
                                r.ReservationId,
                                Tram = r.Station?.Name ?? "N/A",
                                Xe = r.Vehicle?.Vin ?? "N/A",
                                ThoiGianBatDau = r.StartTime,
                                ThoiGianKetThuc = r.EndTime,
                                TrangThai = r.Status
                            }).ToList();
                            break;

                        case "MY_SWAPS":
                            data = context.SwapTransactions
                                .Include(st => st.Station)
                                .Include(st => st.Staff)
                                .Include(st => st.Vehicle)
                                .Where(st => st.UserId == App.CurrentUserId)
                                .Select(st => new
                                {
                                    st.SwapId,
                                    Tram = st.Station.Name,
                                    Xe = st.Vehicle.Vin,
                                    ThoiGian = st.SwapTime,
                                    NhanVien = st.Staff != null ? st.Staff.FullName : "Chưa có",
                                    TrangThai = st.Status
                                }).ToList();
                            break;

                        case "MY_SUBSCRIPTIONS":
                            data = context.UserSubscriptions
                                .Include(us => us.Plan)
                                .Include(us => us.Vehicle)
                                .Where(us => us.UserId == App.CurrentUserId)
                                .Select(us => new
                                {
                                    us.SubscriptionId,
                                    GoiDangKy = us.Plan.Name,
                                    Xe = us.Vehicle.Vin,
                                    NgayBatDau = us.StartDate,
                                    NgayKetThuc = us.EndDate,
                                    SoLanConLai = us.SwapLimit,
                                    TrangThai = us.Status
                                }).ToList();
                            break;

                        case "MY_PAYMENTS":
                            data = context.Payments
                                .Where(p => p.UserId == App.CurrentUserId)
                                .Select(p => new
                                {
                                    p.PaymentId,
                                    SoTien = p.Amount.ToString("N0") + " VND",
                                    PhuongThuc = p.Method,
                                    TrangThai = p.Status,
                                    ThoiGian = p.PaidAt
                                }).ToList();
                            break;
                    }

                    dataGrid.ItemsSource = data as System.Collections.IEnumerable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DetailWindow detailWindow = new DetailWindow(currentModule, dataGrid.SelectedItem);
                detailWindow.ShowDialog();
                LoadData();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText) || searchText == "Tìm kiếm...")
            {
                LoadData();
                return;
            }

            MessageBox.Show($"Tìm kiếm: {searchText}\n(Chức năng đang phát triển)",
                "Thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            DetailWindow detailWindow = new DetailWindow(currentModule, null);
            detailWindow.ShowDialog();
            LoadData();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            MessageBox.Show("Dữ liệu đã được làm mới!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUserId = 0;
                App.CurrentUserName = string.Empty;
                App.CurrentUserEmail = string.Empty;
                App.CurrentRoleId = 0;
                App.CurrentRoleName = string.Empty;

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}