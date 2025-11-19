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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                using (var context = new EvBatterySwapSystemContext())
                {
                    var user = context.Users
                        .Include(u => u.Role)
                        .FirstOrDefault(u => u.Email == email && u.Password == password);

                    if (user != null)
                    {
                        if (user.Status != "Active")
                        {
                            MessageBox.Show("Tài khoản của bạn đã bị khóa!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Save to global session
                        App.CurrentUserId = user.UserId;
                        App.CurrentUserName = user.FullName;
                        App.CurrentUserEmail = user.Email;
                        App.CurrentRoleId = user.RoleId;
                        App.CurrentRoleName = user.Role.Name;

                        MessageBox.Show($"Đăng nhập thành công!\n\nXin chào, {App.CurrentUserName}\nVai trò: {App.CurrentRoleName}",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Email hoặc mật khẩu không đúng!", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnStaffDemo_Click(object sender, RoutedEventArgs e)
        {
            txtEmail.Text = "staff1@vinfast.com";
            txtPassword.Password = "123456";
        }

        private void BtnDriverDemo_Click(object sender, RoutedEventArgs e)
        {
            txtEmail.Text = "driver1@vinfast.com";
            txtPassword.Password = "123456";
        }
    }
}
