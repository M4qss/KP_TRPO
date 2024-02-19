using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace DentIS
{
    /// <summary>
    /// Логика взаимодействия для AutForm.xaml
    /// </summary>
    public partial class AutForm : Window
    {
        
        List<Account> accounts = new List<Account>() { };
        SqlConnection connect;
        public AutForm()
        {
            InitializeComponent();
            connect = new SqlConnection("Server=adclg1;Integrated security=SSPI;database=Vasiliev_TRPO_KP_4195");

            try
            {
                //Открытие подключения
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Не удалось загрузить данные об аккаунтах: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == System.Data.ConnectionState.Open)
                {
                    if (true)
                    {
                        SqlDataAdapter adapt = new SqlDataAdapter("SELECT * FROM Staff", connect);
                        DataSet dt = new DataSet();
                        adapt.Fill(dt);
                        //dt содержит все аккаунты пользователей
                        DataTable dtbl = dt.Tables["Table"];
                        foreach (DataRow dr in dtbl.Rows)
                        {
                            accounts.Add(new Account(dr["login"].ToString(), dr["password"].ToString(), dr["role"].ToString(), new Staff() { ID = Convert.ToInt32(dr["ID"].ToString()), FIO = dr["FIO"].ToString(), BirthDate = dr["birth_date"].ToString(), Specialization = dr["specialization"].ToString()}));
                        }
                        connect.Close();
                    }
                }
            }


        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (loginTb.Text == "Логин")
            {
                loginTb.Clear();
                loginTb.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            }
        }

        private void LoginTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (loginTb.Text == "")
            {
                loginTb.Text = "Логин";
                loginTb.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7E7E7E"));
            }
        }
        private void PasswordTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTb.Text == "Пароль")
            {
                passwordTb.Clear();
                passwordTb.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            }
        }

        private void PasswordTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTb.Text == "")
            {
                passwordTb.Text = "Пароль";
                passwordTb.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7E7E7E"));
            }
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            Account acc = Account.Get(accounts, loginTb.Text, passwordTb.Text);
            if (acc != null)
            {
                MainForm mainForm = new MainForm(acc, this);
                this.Hide();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Неверно введён логин или пароль.", "Ошибка", MessageBoxButton.OK);
            }
        }
    }
}

