using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Button = System.Windows.Controls.Button;
using LanguageSchool.Resources;

namespace LanguageSchool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        
        List<Item> items = new List<Item>();
        List<Page> list;
        BD datebase = new BD();
        Service[] lists;
        private List<string[]> filter = null;
        public string DestinationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Catalog";
        string imagePath = "";
        string result = "";
        int id = 0;
        private int maxItemCount = 0;

        public MainWindow()
        {
           
            InitializeComponent();
            //скрываем
            //Hidee();
            PictureChange();

            this.list = new List<Page>();
            MinHeight = 720;
            MinWidth = 1280;
        lists = GetCatalog("select Title, Time, Sale, PhotoPath as MainimagePathID , Cost, Service.ID from Service,ServicePhoto  where MainimagePathID = ServicePhoto.ID ").ToArray();
        if (lists.Length == 0) MessageBox.Show("Нет товаров в каталоге");
            else 
            {
                BookCatalog.ItemsSource = lists;
            }
            CountLoadList();

            //Search.Text = "Поиск...";
        }



        IEnumerable<Service> GetCatalog(string sql)
        { 
            using(var conn = datebase.Connection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@uL", SqlDbType.VarChar, 1000).Value = Search.Text;

                SqlDataReader read = cmd.ExecuteReader();
                   using (read)
                {
                    while (true)
                    {
                        if (read.Read() == false) break;
                        Service user = new Service()
                        {
                            Title = read.GetString(0),
                            Time = read.GetInt32(1)/60,
                            Sale = read.GetInt32(2),
                            MainimagePathID = read.GetString(3).Replace('\\','/'),
                            Cost = (int)read.GetDecimal(4),
                            Id = read.GetInt32(5),
                        };
                        yield return user;
                    }
                }
            }
        }

        public void Admin()
        {
        }
        private void Search_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Search.Text = ".l.pl";
        }

        private void Search_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Search.Text = "";
        }

        private void Search_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Search.Text = "";
        }

        private void Compilations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Admin_wh admin_Wh = new Admin_wh();
            this.Close();
            admin_Wh.ShowDialog();
        }
        public void Hidee()
        {
            GridList.ColumnDefinitions[1].Width = new GridLength(0);            
        }
        public void Show(object sender)
        {         
            GridList.ColumnDefinitions[1].Width = new GridLength(0.5, GridUnitType.Star);
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();
            CountLoadList();

        }

        private void Sort(object sender, SelectionChangedEventArgs e)
        {
            Refresh();   
            
            CountLoadList();

        }
        private void Refresh()
        {
            IEnumerable<Service> newlist = new List<Service>();
            newlist = lists;
            if(!string.IsNullOrEmpty(Search.Text.ToLower()))
            {
                newlist = lists.Where(x=>x.Title.ToLower().Contains(Search.Text.ToLower()));
            }
            if (Sort_.SelectedIndex==0)
            {
                newlist = newlist.OrderByDescending(x => x.newCost);
            }
            else if (Sort_.SelectedIndex == 1)
            {
                newlist = newlist.OrderBy(x => x.newCost);
            }

            if (Sort_1.SelectedIndex == 0)
            { 
            newlist = newlist.OrderBy(x =>x.Sale).Where(x =>x.Sale <5);
            }

            else if (Sort_1.SelectedIndex == 1)
            {
                newlist = newlist.OrderBy(x => x.Sale).Where(x => x.Sale >= 5 && x.Sale < 15);
            }

            else if (Sort_1.SelectedIndex == 2)
            {
                newlist = newlist.OrderBy(x => x.Sale).Where(x => x.Sale >= 15 && x.Sale < 30);
            }

            else if (Sort_1.SelectedIndex == 3)
            {
                newlist = newlist.OrderBy(x => x.Sale).Where(x => x.Sale >= 30 && x.Sale < 70);
            }

            else if (Sort_1.SelectedIndex == 4)
            {
                newlist = newlist.OrderBy(x => x.Sale).Where(x => x.Sale >= 70 && x.Sale < 100);
            }

            BookCatalog.ItemsSource = newlist;

        }
        private void AddTargetImage()
        {
            // Получить путь к изображению из свойства Source элемента Image с именем "ImageContainer"
            imagePath = ((BitmapImage)ImageContainer.Source).UriSource.LocalPath;
            // Сформировать путь к новому файлу, используя текущий путь к проекту и имя исходного файла
            string newImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Услугишколы", Path.GetFileName(imagePath));
            string connectionString = "Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";
            LinkToServicePhoto();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID FROM ServicePhoto WHERE PhotoPath = @titleToFind";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@titleToFind", result);

                object result1 = command.ExecuteScalar();

                if (result1 == null)
                {
                    File.Copy(imagePath, newImagePath);
                    MessageBox.Show("Изображение сохранено в папку Images в BaseDirectory проекта.");

                }
            }

            //if (File.Exists(newImagePath))
            //{
                
            //}
            //else
            //{
            //    File.Copy(imagePath, newImagePath);
            //}

            // Копировать файл по новому пути
            //string connectionString = "Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";
            //int lastid = 0;
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    SqlCommand command = new SqlCommand("select MAX(ID) from ServicePhoto", connection);
            //    lastid = Convert.ToInt32(command.ExecuteScalar());
            //}
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    SqlCommand command = new SqlCommand("insert into ServicePhoto(ID,PhotoPath) values (@ID,@PhotoPath)", connection);
            //    command.Parameters.AddWithValue("@PhotoPath", result);
            //    command.Parameters.AddWithValue("@ID", lastid + 1);

            //    command.ExecuteNonQuery();

            //}
            //PictureChange();


        }

        private void LinkToServicePhoto()
        {

            string photolink = ImageContainer.Source.ToString();
            string originalText = ImageContainer.Source.ToString();
            char separator = '/';
            string targetWord = "Услугишколы";

            int index = originalText.LastIndexOf(separator);
            if (index != -1)
            {
                //trimmedText = originalText.Substring(index);
                result = targetWord + originalText.Substring(index);
                result = result.Replace("/", "\\");

                // trimmedText теперь содержит слово "Услугишколы" и текст после него

            }

            ImageContainer.Source = new BitmapImage(new Uri(photolink));
        }

        private void Checkcombobox()
        {
            if (SelectPicture.Items.Contains("someValue"))
            {
                // ComboBox содержит элемент с указанным значением
                // выполнить необходимые действия здесь
            }
            else
            {
                // ComboBox не содержит элемент с указанным значением
                // выполнить необходимые действия здесь
            }

        }



        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            LinkToServicePhoto();

            string textToFind =result;
            string connectionString = "Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";
            int lastid = 0;
            int lsstidPHoto = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select MAX(ID) from ServicePhoto", connection);
                lsstidPHoto = Convert.ToInt32(command.ExecuteScalar());
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select MAX(ID) from Service", connection);
                lastid = Convert.ToInt32(command.ExecuteScalar());
            }
            AddTargetImage();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlCommand cmd = new SqlCommand("IF EXISTS(SELECT * FROM ServicePhoto WHERE PhotoPath = @ph) Insert into Service (ID,Title,Cost,Sale,MainimagePathID,Time) values (@ID,@ttl,@cst,@sl,(select ID from ServicePhoto where PhotoPath = @ph),@tm) ELSE INSERT INTO ServicePhoto(ID,PhotoPath) VALUES (@IDP,@ph) Insert into Service (ID,Title,Cost,Sale,MainimagePathID,Time) values (@ID,@ttl,@cst,@sl,(select max(ID) from ServicePhoto),@tm)", connection);
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@ph", textToFind);
                    cmd.Parameters.AddWithValue("@ttl", Name_t.Text);
                    cmd.Parameters.AddWithValue("@cst", Cost_t.Text);
                    cmd.Parameters.AddWithValue("@sl", Sale.Text);
                    cmd.Parameters.AddWithValue("@tm", Time_t.Text);
                    cmd.Parameters.AddWithValue("@ID", lastid + 1);
                    cmd.Parameters.AddWithValue("@IDP", lsstidPHoto + 1);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
            PictureChange();
            lists = GetCatalog("select Title, Time, Sale, PhotoPath as MainimagePathID , Cost from Service,ServicePhoto  where MainimagePathID = ServicePhoto.ID ").ToArray();

            CountLoadList();

        }

        

        private void PictureChange()
        {
            SelectPicture.ItemsSource = null;
            string connectionString = "Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "  select PhotoPath, SUBSTRING(PhotoPath,13,100) as PhotoName  from ServicePhoto;";
            SqlCommand command = new SqlCommand(query, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                string imagePath = row["PhotoPath"].ToString().Replace('\\', '/');
                string text = row["PhotoName"].ToString().Replace('\\', '/');
                Item item = new Item { ImagePath = imagePath, Text = text };
                items.Add(item);
            }
            SelectPicture.ItemsSource = items;

        }

        private void AddImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName;
                ImageContainer.Source = new BitmapImage(new Uri(imagePath));
            }
        }
        private void SelectPicture_Change(object sender, SelectionChangedEventArgs e)
        {
            // Получить выбранный элемент ComboBox
            Item selectedItem = SelectPicture.SelectedItem as Item;

            // Если элемент выбран и у него есть путь к изображению, отобразить изображение в элементе Image
            if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.ImagePath))
            {
                string imagePath = selectedItem.ImagePath;
                Uri imageUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath), UriKind.RelativeOrAbsolute);
                ImageContainer.Source = new BitmapImage(imageUri);
            }
            else
            {
                // Если элемент не выбран или у него нет пути к изображению, очистить элемент Image
                ImageContainer.Source = null;
            }
        }

        private void ItemChange(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            // Получаем элемент ListBoxItem, в котором находится кнопка
            ListBoxItem listBoxItem = FindAncestor<ListBoxItem>(button);

            // Получаем объект Service, связанный с этим элементом ListBoxItem
            Service service = listBoxItem.DataContext as Service;

            // Отображаем данные объекта Service в текстовых блоках
            Name_t.Text = service.Title;
            Cost_t.Text= service.Cost.ToString();
            Sale.Text = service.Sale.ToString();
            int ttime = service.Time;
            ttime = ttime * 60;
            Time_t.Text = ttime.ToString();
            ImageContainer.Source = new BitmapImage(new Uri(service.MainimagePath.ToString()));

            string connectionString = "Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";

            // запрос для поиска записи
            string query = "SELECT ID FROM Service WHERE Title = @Title AND Cost = @Cost AND Sale = @Sale AND Time = @Time";

            // создание объекта подключения к базе данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // создание объекта команды
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // добавление параметров для поиска записи
                    command.Parameters.AddWithValue("@Title", service.Title);
                    command.Parameters.AddWithValue("@Cost", service.Cost.ToString());
                    command.Parameters.AddWithValue("@Sale", service.Sale.ToString());                     
                    command.Parameters.AddWithValue("@Time", ttime);

                    // открытие подключения к базе данных
                    connection.Open();

                    // выполнение запроса и получение результата
                    object result = command.ExecuteScalar();

                    // проверка результата на null и преобразование к типу int
                     id = result != null ? (int)result : -1;

                    
                    
                }
            }


            //string imagePath = service.MainimagePath;
            //Uri imageUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath), UriKind.RelativeOrAbsolute);
            //ImageContainer.Source = new BitmapImage(imageUri);

        }
        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T) return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void Cost_t_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int s = Convert.ToInt32(Cost_t.Text);
            }
            catch 
            {
              Cost_t.Text = "";
                MessageBox.Show("Программа не будет работать пока вы не переведете на номер +89064565657 100 рубей");

            }
        }

        private void Sale_TextChanged(object sender, TextChangedEventArgs e)
        {
            int s=0;
            try
            {
                 s = Convert.ToInt32(Sale.Text);
            }
            catch
            {
                Sale.Text = "";
            }
            if (s > 100)
            {
                Sale.Text = "";
                MessageBox.Show("Программа не будет работать пока вы не переведете на номер +89064565657 100 рубей");

            }
        }

        private void Time_t_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int s = Convert.ToInt32(Time_t.Text);
            }
            catch
            {
                Time_t.Text = "";
                MessageBox.Show("Программа не будет работать пока вы не переведете на номер +89064565657 100 рубей");

            }
        }
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);

                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }

        private void SaveChanged(object sender, RoutedEventArgs e)
        {
            LinkToServicePhoto();

            string textToFind = result;
            string connectionString = "Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";
            int lastid = 0;
            int lsstidPHoto = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select MAX(ID) from ServicePhoto", connection);
                lsstidPHoto = Convert.ToInt32(command.ExecuteScalar());
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select MAX(ID) from Service", connection);
                lastid = Convert.ToInt32(command.ExecuteScalar());
            }
            AddTargetImage();
            

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlCommand cmd = new SqlCommand("IF EXISTS(SELECT * FROM ServicePhoto WHERE PhotoPath = @ph)\r\nBEGIN\r\n    UPDATE Service SET Title = @ttl, Cost = @cst, Sale = @sl, MainimagePathID = (SELECT ID FROM ServicePhoto WHERE PhotoPath = @ph), Time = @tm WHERE ID = @ID\r\nEND\r\nELSE\r\nBEGIN\r\n    INSERT INTO ServicePhoto(ID, PhotoPath) VALUES (@IDP, @ph)\r\n    UPDATE Service SET Title = @ttl, Cost = @cst, Sale = @sl, MainimagePathID = (SELECT MAX(ID) FROM ServicePhoto), Time = @tm WHERE ID = @ID\r\nEND", connection);
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@ph", textToFind);
                    cmd.Parameters.AddWithValue("@ttl", Name_t.Text);
                    cmd.Parameters.AddWithValue("@cst", Cost_t.Text);
                    cmd.Parameters.AddWithValue("@sl", Sale.Text);
                    cmd.Parameters.AddWithValue("@tm", Time_t.Text);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@IDP", lsstidPHoto + 1);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
            PictureChange();
            lists = GetCatalog("select Title, Time, Sale, PhotoPath as MainimagePathID , Cost from Service,ServicePhoto  where MainimagePathID = ServicePhoto.ID ").ToArray();
            CountLoadList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=192.168.227.14;Initial Catalog = learn_23;database=learn_23;user=user06;password=06;";
            // Получаем выбранный элемент из листбокса
            Service service = (Service)BookCatalog.SelectedItem;


            // запрос для поиска записи
            string query = "SELECT ID FROM Service WHERE Title = @Title AND Cost = @Cost AND Sale = @Sale";

            // создание объекта подключения к базе данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // создание объекта команды
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // добавление параметров для поиска записи
                    command.Parameters.AddWithValue("@Title", service.Title);
                    command.Parameters.AddWithValue("@Cost", service.Cost.ToString());
                    command.Parameters.AddWithValue("@Sale", service.Sale.ToString());

                    // открытие подключения к базе данных
                    connection.Open();

                    // выполнение запроса и получение результата
                    object result = command.ExecuteScalar();

                    // проверка результата на null и преобразование к типу int
                    id = result != null ? (int)result : -1;



                }
            }

            if (service != null)
            {
                // Выводим диалог подтверждения удаления
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить этот элемент?", "Подтвердите удаление", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем элемент из базы данных
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Service WHERE ID=@id", connection);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }


                }
            }
            Refresh();
            lists = GetCatalog("select Title, Time, Sale, PhotoPath as MainimagePathID , Cost from Service,ServicePhoto  where MainimagePathID = ServicePhoto.ID ").ToArray();

        }
        public void CountLoadList()
        {
            int displayedItemCount = BookCatalog.Items.Count;
            if (displayedItemCount > maxItemCount)
            {
                maxItemCount = displayedItemCount;
            }
            LabelCount.Content = $"{displayedItemCount}/{maxItemCount}";

        }


        private void ButtonServiceClient_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string title = (button.DataContext as Service)?.Title;
            int id = (button.DataContext as Service).Id;

            if (!string.IsNullOrEmpty(title))
            {
                ServiceClient newWindow = new ServiceClient(title,id);
              
                    newWindow.ShowDialog();
        
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ServiceWindow serviceWindow = new ServiceWindow();
           serviceWindow.ShowDialog();
        }
    }

}
