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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PeShopMaster.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            ProductsListView.ItemsSource = Data.Trade2Entities.GetContext().Product.ToList();
            CountOfLabel.Content = $"{Data.Trade2Entities.GetContext().Product.Count()}" + $"/{Data.Trade2Entities.GetContext().Product.Count()}";
            if (Classes.Manager.User != null)
            {
                FIOLabel.Content = $"{Classes.Manager.User.UserSurname}" + $"{Classes.Manager.User.UserName}" + $"{Classes.Manager.User.UserPatronymic}";
            }
            SearchTextBox.Text = string.Empty;
            SortDownRadioButton.IsChecked = false;
            SortUpRadioButton.IsChecked = false;


            var ProduceList = Data.Trade2Entities.GetContext().Producer.ToList();
            ProduceList.Insert(0, new Data.Producer { Producer1 = "Все производители" });
            ProducerCombobox.ItemsSource = ProduceList;
            ProducerCombobox.SelectedIndex = 0;
        }

        public List<Data.Product> _currentProducts = Data.Trade2Entities.GetContext().Product.ToList();

        public void Update()
        {
            try
            {
                _currentProducts = Data.Trade2Entities.GetContext().Product.ToList();

                _currentProducts = (from item in Data.Trade2Entities.GetContext().Product
                                    where item.NameofSupply.Supply.ToLower().Contains(SearchTextBox.Text) ||
                                    item.Description.ToLower().Contains(SearchTextBox.Text) ||
                                    item.Producer.Producer1.ToLower().Contains(SearchTextBox.Text) ||
                                    item.Cost.ToString().ToLower().Contains(SearchTextBox.Text) ||
                                    item.CountOnStorage.ToString().ToLower().Contains(SearchTextBox.Text)
                                    select item).ToList();

                if (SortUpRadioButton.IsChecked == true)
                {
                    _currentProducts = _currentProducts.OrderBy(d => d.Cost).ToList();
                }

                if (SortDownRadioButton.IsChecked == true)
                {
                    _currentProducts = _currentProducts.OrderByDescending(d => d.Cost).ToList();
                }

                var selected = ProducerCombobox.SelectedItem as Data.Producer;
                if (selected != null && selected.Producer1 != "Все производители")
                {
                    _currentProducts = _currentProducts.Where(d => d.IDProducer == selected.ID).ToList();
                }

                CountOfLabel.Content = $"{_currentProducts.Count()}/" +
                    $"{Data.Trade2Entities.GetContext().Product.Count()}";

                ProductsListView.ItemsSource = _currentProducts;
            }
            catch (Exception)
            {

            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void SortUpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = (sender as Button).DataContext as Data.Product;
                var forDelete = Data.Trade2Entities.GetContext().OrderProduct.Where(d => d.ProductArticleNumber == selected.ProductArticleNumber).ToList();
                if (forDelete.Count() > 0)
                {
                    MessageBox.Show("Товар, который присутствует в заказе, удалить нельзя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Data.Trade2Entities.GetContext().Product.Remove(selected);
                    MessageBox.Show("Товар успешно удалён", "Успех!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Manager.MainFrame.CanGoBack)
            {
                Classes.Manager.MainFrame.GoBack();
            }
        }

        private void ProducerCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
