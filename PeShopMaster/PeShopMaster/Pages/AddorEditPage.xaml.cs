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
    /// Логика взаимодействия для AddorEditPage.xaml
    /// </summary>
    public partial class AddorEditPage : Page
    {

        public string FlagAddorEdit = "default";
        public Data.Product CurrentProduct;
        public bool FlagPhoto = false;
        public AddorEditPage(Data.Product _product)
        {
            InitializeComponent();

            if (_product == null)
            {
                FlagAddorEdit = "add";
            }
            else
            {
                CurrentProduct = _product;
                FlagAddorEdit = "edit";
            }

            DataContext = CurrentProduct;
            init();
        }

        public void init()
        {
            try
            {
                CategoryComboBox.ItemsSource = Data.Trade2Entities.GetContext().CategoryProduct.ToList();

                if(FlagAddorEdit == "add")
                {
                    IDBox.Visibility = Visibility.Hidden;
                    IDLable.Visibility = Visibility.Hidden;
                    NameTextBox.Text = string.Empty;
                    CategoryComboBox.SelectedItem = null;
                    UnitBox.Text = string.Empty;
                    SupplierBox.Text = string.Empty;
                    CostBox.Text = string.Empty;
                    CountOnStorageBox.Text = string.Empty;
                    DescriptionBox.Text = string.Empty;
                }
                else if (FlagAddorEdit == "edit")
                {
                    IDBox.Visibility = Visibility.Visible;
                    IDLable.Visibility = Visibility.Visible;
                    IDBox.Text = CurrentProduct.ProductArticleNumber;
                    NameTextBox.Text = CurrentProduct.NameofSupply.Supply;
                    FlagPhoto = true;
                    CategoryComboBox.SelectedItem = Data.Trade2Entities.GetContext().CategoryProduct.Where(d => d.ID == CurrentProduct.IDCategoryProduct).FirstOrDefault();
                    UnitBox.Text = CurrentProduct.Units.Name;
                    SupplierBox.Text = CurrentProduct.Producer.Producer1;
                    CostBox.Text = CurrentProduct.Cost.ToString();
                    CountOnStorageBox.Text = CurrentProduct.CountOnStorage.ToString();
                    DescriptionBox.Text = CurrentProduct.Description;
                }
            }
            catch
            {

            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Manager.MainFrame.CanGoBack)
            {
                Classes.Manager.MainFrame.GoBack();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
