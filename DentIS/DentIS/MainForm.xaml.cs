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
using System.Windows.Threading;
using System.Data.SqlClient;
using System.Data;

namespace DentIS
{
    public static class SQLTempl
    {
        static SqlConnection connect = new SqlConnection("Server=adclg1;Integrated security=SSPI;database=Vasiliev_TRPO_KP_4195");
        public static List<string> ValFromID(string Table, string Field, int ID)
        {
            List<string> res = new List<string>();
            try
            {
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при загрузке записей: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    SqlDataAdapter adapt = new SqlDataAdapter($"SELECT * FROM {Table} WHERE ID = {ID}", connect);
                    DataSet dt = new DataSet();
                    adapt.Fill(dt);
                    DataTable dtbl = dt.Tables["Table"];
                    foreach (DataRow dr in dtbl.Rows)
                    {
                        res.Add(dr[$"{Field}"].ToString());
                    }
                    connect.Close();
                }
            }
            return res;
        }
    }
    public static class GridHandler
    {
        public static void HideAllGrids(this List<Grid> grids)
        {
            foreach(Grid grid in grids)
            {
                grid.Visibility = Visibility.Collapsed;
            }
        }
        public static void ShowOne(this List<Grid> grids, Grid showenOne)
        {
            grids.HideAllGrids();
            grids.Find(x => x == showenOne).Visibility = Visibility.Visible;
        }
    }
    public partial class MainForm : Window
    {
        SqlConnection connect = new SqlConnection("Server=adclg1;Integrated security=SSPI;database=Vasiliev_TRPO_KP_4195");
        Window parent;
        public Account user;
        DispatcherTimer Timer = new DispatcherTimer();
        private List<Grid> groups;
        int selectedId = -1;
        public MainForm(Account account, Window parentWin)
        {
            InitializeComponent();

            Timer.Interval = new TimeSpan(0,0,1);
            Timer.Start();
            Timer.Tick += new EventHandler(Timer_Tick);


            parent = parentWin;
            user = account;
            this.Title += " || " + SQLTempl.ValFromID("Roles", "NAME", Convert.ToInt32(user.GetRole())).FirstOrDefault() + " " + user.GetStaff().FIO;
            groups = new List<Grid>() {PatientsGrid, JournalGrid, StaffGrid};
            //Скрытие всех групп
            groups.HideAllGrids();
            selectedData.Content = "Выбранная запись: " + selectedId;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            dateLabel.Content = DateTime.Now.ToString("dd.MM.yyyy: hh:mm:ss");
        }
        //Закрытие родительского окна -> выход из приложения
        private void Window_Closed(object sender, EventArgs e)
        {
            parent.Close();
        }

        private void UpdatePatients()
        {
            try
            {
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при загрузке записей: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    SqlDataAdapter adapt = new SqlDataAdapter("SELECT * FROM Patient", connect);
                    DataSet dt = new DataSet();
                    adapt.Fill(dt);
                    dt.Tables["Table"].Columns[1].ColumnName = "ФИО";
                    dt.Tables["Table"].Columns[2].ColumnName = "Дата рождения";
                    dt.Tables["Table"].Columns[3].ColumnName = "Номер телефона";
                    dt.Tables["Table"].Columns[4].ColumnName = "Адрес";
                    dt.Tables["Table"].Columns[5].ColumnName = "Номер полиса";
                    DataTable dtbl = dt.Tables["Table"];
                    PatientDG.ItemsSource = dtbl.DefaultView;
                    connect.Close();
                }
            }
        }
        private void UpdateStaff()
        {
            try
            {
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при загрузке записей: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    SqlDataAdapter adapt = new SqlDataAdapter("SELECT Staff.ID, Staff.FIO, Staff.birth_date, Staff.login, Staff.password, Specialization.NAME, roles.NAME FROM Staff JOIN Roles ON Staff.role = Roles.ID JOIN Specialization ON Staff.specialization = Specialization.ID", connect);
                    DataSet dt = new DataSet();
                    adapt.Fill(dt);
                    dt.Tables["Table"].Columns[1].ColumnName = "ФИО";
                    dt.Tables["Table"].Columns[2].ColumnName = "Дата рождения";
                    dt.Tables["Table"].Columns[3].ColumnName = "Логин";
                    dt.Tables["Table"].Columns[4].ColumnName = "Пароль";
                    dt.Tables["Table"].Columns[5].ColumnName = "Специализация";
                    dt.Tables["Table"].Columns[6].ColumnName = "Роль";
                    DataTable dtbl = dt.Tables["Table"];
                    StaffDG.ItemsSource = dtbl.DefaultView;
                    connect.Close();
                }
            }
        }
        private void PatientsBtn_Click(object sender, RoutedEventArgs e)
        {
            groups.ShowOne(PatientsGrid);
            //Установка стандартного отступа таблицы
            PatientDG.Margin = new Thickness(10, 10, 5, 56);
            //Блокировка кнопки "назад"
            PatientResetBtn.IsEnabled = false;
            //Сокрытие меню добавления пациента
            PatientAddDataGrid.Visibility = Visibility.Collapsed;
            //Заполнение DataGrid
            UpdatePatients();
        }

        private void JournalBtn_Click(object sender, RoutedEventArgs e)
        {
            groups.ShowOne(JournalGrid);
        }

        private void DateResetBtn_Click(object sender, RoutedEventArgs e)
        {
            //Сброс даты
            DatePick.SelectedDate = null;
        }

        private void DatePick_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Смена даты -> Обновление записей таблицы
        }

        private void StaffBtn_Click(object sender, RoutedEventArgs e)
        {
            groups.ShowOne(StaffGrid);
            //Установка стандартного отступа таблицы
            StaffDG.Margin = new Thickness(10, 10, 5, 56);
            //Блокировка кнопки "назад"
            StaffCloseBtn.IsEnabled = false;
            StaffAddGrid.Visibility = Visibility.Collapsed;
            UpdateStaff();
        }

        private void PatientAddBtn_Click(object sender, RoutedEventArgs e)
        {
            //Сужение таблицы влево
            PatientDG.Margin = new Thickness(10, 10, 330, 56);
            //Доступ к кнопке "назад"
            PatientResetBtn.IsEnabled = true;
            PatientAddDataGrid.Visibility = Visibility.Visible;
        }

        private void PatientResetBtn_Click(object sender, RoutedEventArgs e)
        {
            //Установка стандартного отступа таблицы
            PatientDG.Margin = new Thickness(10, 10, 5, 56);
            //Блокировка кнопки "назад"
            PatientResetBtn.IsEnabled = false;
            PatientAddDataGrid.Visibility = Visibility.Collapsed;
        }

        private void PatientAddDenyBtn_Click(object sender, RoutedEventArgs e)
        {
            FIOTb.Text = "";
            PhoneNumberTb.Text = "";
            AddressTb.Text = "";
            PolicyTb.Text = "";
            BirthDateDp.SelectedDate = null;
            selectedId = -1;
            selectedData.Content = "Выбранная запись: " + selectedId;
        }

        private void PatientAddConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Открытие подключения
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении записи: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if(connect.State == System.Data.ConnectionState.Open)
                {
                    //Обработка добавления пациента
                    //Проверка введенных значений
                    if(true)
                    {
                        string que = $"INSERT INTO Patient VALUES('{FIOTb.Text}', '{BirthDateDp.SelectedDate.GetValueOrDefault().ToString("dd.MM.yyyy")}', '{PhoneNumberTb.Text}', '{AddressTb.Text}', '{PolicyTb.Text}');";
                        SqlCommand sqlCommand = new SqlCommand(que, connect);
                        sqlCommand.ExecuteNonQuery();
                        connect.Close();
                        UpdatePatients();
                    }
                }
            }
        }

        private void PatientChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Открытие подключения
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при изменении записи: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == System.Data.ConnectionState.Open)
                {
                    if (selectedId == -1) { MessageBox.Show("Ошибка при изменении записи: запись не выбрана.", "Ошибка введенных данных"); }
                    else
                    {
                        string que = $"UPDATE Patient SET FIO='{FIOTb.Text}', birth_date='{BirthDateDp.SelectedDate.ToString()}', phone_number='{PhoneNumberTb.Text}', address='{AddressTb.Text}', policy='{PolicyTb.Text}' WHERE ID = {selectedId};";
                        SqlCommand sqlCommand = new SqlCommand(que, connect);
                        sqlCommand.ExecuteNonQuery();
                    }
                    connect.Close();
                    UpdatePatients();
                }
            }
        }
        private void PatientDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Открытие подключения
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == System.Data.ConnectionState.Open)
                {
                    if (selectedId == -1) { MessageBox.Show("Ошибка при удалении записи: запись не выбрана.", "Ошибка введенных данных"); }
                    else
                    {   
                        string que = $"DELETE FROM Patient WHERE ID = {selectedId};";
                        SqlCommand sqlCommand = new SqlCommand(que, connect);
                        sqlCommand.ExecuteNonQuery();
                    }
                    connect.Close();
                    UpdatePatients();
                }
            }
        }

        private void PatientDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count != 0)
            {
                if (e.AddedItems[0] != null)
                {
                    DataRowView dr = e.AddedItems[0] as DataRowView;
                    if (dr != null)
                    {
                        FIOTb.Text = dr.Row.ItemArray[1].ToString();
                        BirthDateDp.SelectedDate = dr.Row.ItemArray[2] as DateTime?;
                        PhoneNumberTb.Text = dr.Row.ItemArray[3].ToString();
                        AddressTb.Text = dr.Row.ItemArray[4].ToString();
                        PolicyTb.Text = dr.Row.ItemArray[5].ToString();
                        selectedId = Convert.ToInt32(dr.Row.ItemArray[0].ToString());
                        selectedData.Content = "Выбранная запись: " + selectedId;
                    }
                }
            }
        }

        private void StaffOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            //Сужение таблицы влево
            StaffDG.Margin = new Thickness(10, 10, 330, 56);
            //Доступ к кнопке "назад"
            StaffCloseBtn.IsEnabled = true;
            StaffAddGrid.Visibility = Visibility.Visible;
            //Загрузка значения в ComboBox
            try
            {
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при загрузке записей для comboBox: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    SqlDataAdapter adapt = new SqlDataAdapter("SELECT * FROM Roles", connect);
                    DataSet dt = new DataSet();
                    adapt.Fill(dt);
                    DataTable dtbl = dt.Tables["Table"];
                    dtbl.Columns.Remove("ID");
                    List<string> rec = new List<string>();
                    foreach(DataRow dr in dtbl.Rows)
                    {
                         rec.Add(dr["NAME"].ToString());
                    }
                    StaffRoleCb.ItemsSource = rec;

                    SqlDataAdapter adapt2 = new SqlDataAdapter("SELECT * FROM Specialization", connect);
                    DataSet dt2 = new DataSet();
                    adapt2.Fill(dt2);
                    DataTable dtbl2 = dt2.Tables["Table"];
                    dtbl2.Columns.Remove("ID");
                    List<string> rec2 = new List<string>();
                    foreach (DataRow dr in dtbl2.Rows)
                    {
                        rec2.Add(dr["NAME"].ToString());
                    }
                    StaffSpecCb.ItemsSource = rec2;
                    StaffSpecCb.SelectedItem = "-";
                    StaffSpecCb.IsEnabled = false;

                    connect.Close();
                }
            }
        }

        private void StaffCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedId = -1;
            StaffDG.Margin = new Thickness(10, 10, 5, 56);
            //Блокировка кнопки "назад"
            StaffCloseBtn.IsEnabled = false;
            //PatientAddDataGrid.Visibility = Visibility.Collapsed;
            StaffAddGrid.Visibility = Visibility.Collapsed;
        }

        private void StaffRoleCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(StaffRoleCb.SelectedValue.ToString() == "медработник")
            {
                StaffSpecCb.IsEnabled = true;
            }
            else
            {
                StaffSpecCb.SelectedValue = "-";
                StaffSpecCb.IsEnabled = false;
            }
        }

        private void StaffAddConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Открытие подключения
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении записи: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == System.Data.ConnectionState.Open)
                {
                    //Обработка добавления сотрудника
                    //Проверка введенных значений
                    if (true)
                    {
                        string que = $"INSERT INTO Staff VALUES('{SFIOTb.Text}', '{SBirthDateDp.SelectedDate.GetValueOrDefault().ToString("dd.MM.yyyy")}', {StaffSpecCb.SelectedIndex + 1}, '{StaffLoginTb.Text}', '{StaffPasswordTb.Text}', {StaffRoleCb.SelectedIndex + 1});";
                        SqlCommand sqlCommand = new SqlCommand(que, connect);
                        sqlCommand.ExecuteNonQuery();
                        connect.Close();
                        UpdateStaff();
                    }
                }
            }
        }

        private void StaffDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                if (e.AddedItems[0] != null)
                {
                    DataRowView dr = e.AddedItems[0] as DataRowView;
                    if (dr != null)
                    {
                        SFIOTb.Text = dr.Row.ItemArray[1].ToString();
                        SBirthDateDp.SelectedDate = dr.Row.ItemArray[2] as DateTime?;
                        StaffSpecCb.SelectedItem = dr.Row.ItemArray[5].ToString();
                        StaffLoginTb.Text = dr.Row.ItemArray[3].ToString();
                        StaffPasswordTb.Text = dr.Row.ItemArray[4].ToString();
                        StaffRoleCb.SelectedItem = dr.Row.ItemArray[6].ToString();
                        selectedId = Convert.ToInt32(dr.Row.ItemArray[0].ToString());
                        StaffSelectedData.Content = "Выбранная запись: " + selectedId;
                    }
                }
            }
        }

        private void StaffAddDenyBtn_Click(object sender, RoutedEventArgs e)
        {
            SFIOTb.Text = "";
            SBirthDateDp.SelectedDate = null;
            StaffSpecCb.SelectedItem = "-";
            StaffLoginTb.Text = "";
            StaffPasswordTb.Text = "";
            StaffRoleCb.SelectedIndex = 0;
            selectedId = -1;
            StaffSelectedData.Content = "Выбранная запись: " + selectedId;
        }

        private void StaffChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Открытие подключения
                connect.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при изменении записи: " + ex.Message, "Ошибка подключения к бд");
            }
            finally
            {
                if (connect.State == System.Data.ConnectionState.Open)
                {
                    if (selectedId == -1) { MessageBox.Show("Ошибка при изменении записи: запись не выбрана.", "Ошибка введенных данных"); }
                    else
                    {
                        string que = $"UPDATE Staff SET FIO='{SFIOTb.Text}', birth_date='{SBirthDateDp.SelectedDate.GetValueOrDefault().ToString("dd.MM.yyyy")}', specialization='{StaffSpecCb.SelectedIndex + 1}', login='{StaffLoginTb.Text}', password='{StaffPasswordTb.Text}', role='{StaffRoleCb.SelectedIndex + 1}' WHERE ID = {selectedId};";
                        SqlCommand sqlCommand = new SqlCommand(que, connect);
                        sqlCommand.ExecuteNonQuery();
                    }
                    connect.Close();
                    UpdateStaff();
                }
            }
        }
    }
}
