using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.IO;

namespace LanguageSchool
{
    public class Service
    { 
        public int Id { get; set; }
        public string Title { get; set; }
        public int Time { get; set; }
        public int Sale { get; set; }
        public string MainimagePathID { get; set; }
        public string MainimagePath { get=>Path.Combine( AppDomain.CurrentDomain.BaseDirectory, MainimagePathID); }
        public decimal Cost { get; set; }
        public decimal newCost { get => (Cost*(100-Sale))/100; }
    }
    public class Item
    {
        public string ImagePath { get; set; }
        public string ImagePath_ { get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagePath); }

        public string Text { get; set; }
    }

    public class BD
    {
        string conn ="Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";

        public SqlConnection Connection()
        {
            SqlConnection connection = new SqlConnection(conn);
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            return connection;
        }
    }
    
}