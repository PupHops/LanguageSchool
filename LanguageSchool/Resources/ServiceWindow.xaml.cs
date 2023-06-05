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

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LanguageSchool.Resources
{
    /// <summary>
    /// Логика взаимодействия для ServiceWindow.xaml
    /// </summary>
    public partial class ServiceWindow : Window
    {
        private DataTable dataTable = new DataTable();

        public ServiceWindow()
        {
            InitializeComponent();
            FillData();
            CreateGridView();            

        }
        private void FillData()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;"))
            {
                string query = "SELECT * FROM ClientService";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }
        }
        //private void UpdateData()
        //{
        //    using (SqlConnection connection = new SqlConnection("Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;"))
        //    {
        //        string query = "UPDATE ClientService SET ServiceID = @ServiceID, StartTime = @StartTime, Comment = @Comment WHERE ClientID = @ClientID";
        //        SqlCommand command = new SqlCommand(query, connection);
        //        command.Parameters.Add("@ClientID", SqlDbType.Int, 4, "ClientID");
        //        command.Parameters.Add("@ServiceID", SqlDbType.Int, 4, "ServiceID");
        //        command.Parameters.Add("@StartTime", SqlDbType.DateTime, 8, "StartTime");
        //        command.Parameters.Add("@Comment", SqlDbType.VarChar, 255, "Comment");
        //        SqlDataAdapter adapter = new SqlDataAdapter();
        //        adapter.UpdateCommand = command;
        //        adapter.Update(dataTable);
        //    }
        //}
        private void CreateGridView()
        {
            // Создание гридвью
            DataGrid gridView = new DataGrid();
            gridView.AutoGenerateColumns = false;

            // Создание столбцов гридвью
            DataGridTextColumn columnClientID = new DataGridTextColumn();
            columnClientID.Header = "Client ID";
            columnClientID.Binding = new Binding("ClientID");
            gridView.Columns.Add(columnClientID);

            DataGridTextColumn columnServiceID = new DataGridTextColumn();
            columnServiceID.Header = "Service ID";
            columnServiceID.Binding = new Binding("ServiceID");
            gridView.Columns.Add(columnServiceID);

            DataGridTextColumn columnStartTime = new DataGridTextColumn();
            columnStartTime.Header = "Start Time";
            columnStartTime.Binding = new Binding("StartTime");
            gridView.Columns.Add(columnStartTime);

            DataGridTextColumn columnComment = new DataGridTextColumn();
            columnComment.Header = "Comment";
            columnComment.Binding = new Binding("Comment");
            gridView.Columns.Add(columnComment);

            // Заполнение гридвью данными из DataTable
            gridView.ItemsSource = dataTable.DefaultView;

            // Добавление гридвью на форму
            this.Content = gridView;
        }

        private void MyDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataRowView rowView = e.Row.Item as DataRowView;

            // Обновить данные в БД
            using (SqlConnection connection = new SqlConnection("Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;"))
            {
                string query = "UPDATE ClientService SET ServiceID = @ServiceID, StartTime = @StartTime, Comment = @Comment WHERE ClientID = @ClientID";
                SqlCommand command = new SqlCommand(query, connection);

                // Добавить параметры команды SQL
                command.Parameters.AddWithValue("@ServiceID", rowView["ServiceID"]);
                command.Parameters.AddWithValue("@StartTime", rowView["StartTime"]);
                command.Parameters.AddWithValue("@Comment", rowView["Comment"]);
                command.Parameters.AddWithValue("@ClientID", rowView["ClientID"]);

                // Открыть соединение и выполнить команду SQL
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
