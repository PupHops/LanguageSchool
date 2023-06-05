
using Microsoft.EntityFrameworkCore.Diagnostics;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LanguageSchool
{
    /// <summary>
    /// Логика взаимодействия для Service.xaml
    /// </summary>
    public partial class ServiceClient : Window
    {
        private readonly string connectionString = "Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";
        int ServiceID = 0;
        int lastid = 0;
        public ServiceClient(string title, int id)
        {

            InitializeComponent();
            DataContext = this;
            LabelTitle.Content = title;
            IdTEst.Content = id;
            ServiceID = id;
            LoadClients();
            DatePicker.DisplayDateStart = DateTime.Today;

        }
        private void LoadClients()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID, FirstName, LastName, Patronymic FROM Client";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<Client> clients = new List<Client>();
                foreach (DataRow row in dataTable.Rows)
                {
                    int id = (int)row["ID"];
                    string firstName = row["FirstName"].ToString();
                    string lastName = row["LastName"].ToString();
                    string patronymic = row["Patronymic"].ToString();
                    clients.Add(new Client(id, firstName, lastName, patronymic));
                }
                DataContext = new { Clients = clients };
            }
        }

        private void AddServiceClientButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select MAX(ID) from ClientService", connection);
               lastid = Convert.ToInt32(command.ExecuteScalar());
            }
            // Получение выбранной даты из DatePicker
            DateTime selectedDate = DatePicker.SelectedDate ?? DateTime.MinValue;

            // Получение выбранного времени из ComboBox
            string selectedTimeStr = ((ComboBoxItem)TimeComboBox.SelectedItem).Tag.ToString();
            TimeSpan selectedTime = TimeSpan.Parse(selectedTimeStr);

            // Объединение даты и времени в тип datetime
            DateTime selectedDateTime = selectedDate.Date + selectedTime;

            // Форматирование datetime в строку в формате, который можно передать в запрос SQL
            string formattedDateTime = selectedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime dateTime = DateTime.Parse(formattedDateTime);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO ClientService(ID,ClientID,ServiceID,StartTime,Comment) VALUES (@ID,@Idclnt,@ServiceID,@date,@cmnt)", connection);
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("ID",lastid+1);
                cmd.Parameters.AddWithValue("@date", dateTime);
                cmd.Parameters.AddWithValue("@cmnt", Comment.Text);
                cmd.Parameters.AddWithValue("@Idclnt", clientsComboBox.SelectedIndex);
                cmd.Parameters.AddWithValue("@ServiceID", ServiceID);

                cmd.ExecuteNonQuery();
            }
        }
    }
    }

    public class Client
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string FullName => $"{LastName} {FirstName} {Patronymic}";

        public Client(int id, string firstName, string lastName, string patronymic)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
        }
    }



