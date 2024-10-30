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
using System.Windows.Shapes;

namespace PeShopMaster.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddorEditPage.xaml
    /// </summary>
    public partial class AddorEditPage : Page
    {

        public AdminPage AdminPage { get; set; }
        public ClientPage ClientPage { get; set; }
        public ManagerPage ManagerPage { get; set; }
        public string FlagAddOrEdit = "default";
        public bool FlagPhoto = false;
        public Data.Product CurrentProduct = new Data.Product();
        public AddorEditPage(Data.Product _product)
        {
            InitializeComponent();

            if (_product == null)
            {
                FlagAddOrEdit = "add";
            }
            else
            {
                CurrentProduct = _product;
                FlagAddOrEdit = "edit";
            }

            DataContext = CurrentProduct;
            Init();
        }

        public void Init()
        {
            try
            {
                CategoryComboBox.ItemsSource = Data.Trade2Entities.GetContext().CategoryProduct.ToList();

                if(FlagAddOrEdit == "add")
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
                else if (FlagAddOrEdit == "edit")
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
            try
            {
                StringBuilder errors = new StringBuilder();

                if (string.IsNullOrEmpty(NameTextBox.Text))
                {
                    errors.AppendLine("Заполните наименование");
                }
                if (CategoryComboBox.SelectedItem == null)
                {
                    errors.AppendLine("Выберите категорию");
                }
                if (string.IsNullOrEmpty(CountOnStorageBox.Text))
                {
                    errors.AppendLine("Заполните количество");
                }
                else
                {
                    var tryCount = Int32.TryParse(CountOnStorageBox.Text, out var resultCount);
                    if (!tryCount)
                    {
                        errors.AppendLine("Количество - целое число");
                    }
                }
                if (string.IsNullOrEmpty(UnitBox.Text))
                {
                    errors.AppendLine("Заполните ед.измерения");
                }
                if (string.IsNullOrEmpty(SupplierBox.Text))
                {
                    errors.AppendLine("Заполните поставщика");
                }
                if (string.IsNullOrEmpty(CostBox.Text))
                {
                    errors.AppendLine("Заполните стоимость");
                }
                else
                {
                    var tryCost = Decimal.TryParse(CostBox.Text, out var resultCost);
                    if (!tryCost)
                    {
                        errors.AppendLine("Стоимость - дробное число");
                    }
                    else
                    {
                        var costText = CostBox.Text.Trim();

                        var parts = costText.Split(new[] { ',', '.' });

                        if (parts.Length > 1 && parts[1].Length > 2)
                        {
                            errors.AppendLine("Дробная часть может содержать только 2 знака после запятой");
                        }
                    }

                    if (tryCost && resultCost < 0)
                    {
                        errors.AppendLine("Стоимость не может быть отрицательной!");
                    }
                }

                if (string.IsNullOrEmpty(DescriptionBox.Text))
                {
                    errors.AppendLine("Заполните описание");
                }

                if (FlagPhoto == false)
                {
                    errors.AppendLine("Выберите изображение!");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                var selectedCategory = CategoryComboBox.SelectedItem as Data.CategoryProduct;
                CurrentProduct.IDCategoryProduct = selectedCategory.ID;

                if (FlagAddOrEdit == "add")
                {
                    CurrentProduct.ProductArticleNumber = RandomArticle();
                }
                CurrentProduct.Cost = Convert.ToDecimal(CostBox.Text);
                CurrentProduct.CountOnStorage = Convert.ToInt32(CountOnStorageBox.Text);
                CurrentProduct.Description = DescriptionBox.Text;

                var searchName = (from obj in Data.Trade2Entities.GetContext().NameofSupply
                                  where obj.Supply == NameTextBox.Text
                                  select obj).FirstOrDefault();

                if (searchName != null)
                {
                    CurrentProduct.IDSupply = searchName.ID;
                }
                else
                {
                    Data.NameofSupply productName = new Data.NameofSupply()
                    {
                        Supply = NameTextBox.Text
                    };
                    Data.Trade2Entities.GetContext().NameofSupply.Add(productName);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    CurrentProduct.IDSupply = productName.ID;
                }


                var searchUnit = (from obj in Data.Trade2Entities.GetContext().Units
                                  where obj.Name == UnitBox.Text
                                  select obj).FirstOrDefault();

                if (searchUnit != null)
                {
                    CurrentProduct.IDUnits = searchUnit.ID;
                }
                else
                {
                    Data.Units UnitName = new Data.Units()
                    {
                        Name = UnitBox.Text
                    };
                    Data.Trade2Entities.GetContext().Units.Add(UnitName);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    CurrentProduct.IDUnits = UnitName.ID;
                }

                var searchProvider = (from obj in Data.Trade2Entities.GetContext().Provider
                                      where obj.Provider1 == SupplierBox.Text
                                      select obj).FirstOrDefault();

                if (searchProvider != null)
                {
                    CurrentProduct.IDProvider = searchProvider.ID;
                }
                else
                {
                    Data.Provider ProviderName = new Data.Provider()
                    {
                        Provider1 = SupplierBox.Text
                    };
                    Data.Trade2Entities.GetContext().Provider.Add(ProviderName);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    CurrentProduct.IDProvider = ProviderName.ID;
                }

                var searchProducer = (from obj in Data.Trade2Entities.GetContext().Producer
                                      where obj.Producer1 == SupplierBox.Text
                                      select obj).FirstOrDefault();

                if (searchProducer != null)
                {
                    CurrentProduct.IDProducer = searchProducer.ID;
                }
                else
                {
                    Data.Producer ProducerName = new Data.Producer()
                    {
                        Producer1 = SupplierBox.Text
                    };
                    Data.Trade2Entities.GetContext().Producer.Add(ProducerName);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    CurrentProduct.IDProducer = ProducerName.ID;
                }


                if (FlagAddOrEdit == "edit")
                {
                    Data.Trade2Entities.GetContext().SaveChanges();

                    MessageBox.Show("Успешно сохранено!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FlagAddOrEdit == "add")
                {
                    Data.Trade2Entities.GetContext().Product.Add(CurrentProduct);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    MessageBox.Show("Успешно добавлено!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (AdminPage != null)
                {
                    AdminPage.Update();
                    AdminPage.Init();
                }
                if (ClientPage != null)
                {
                    ClientPage.Update();
                    ClientPage.Init();
                }
                if (ManagerPage != null)
                {
                    ManagerPage.Update();
                    ManagerPage.Init();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string RandomArticle()
        {
            string allowChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string randomarticle = "";
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                randomarticle += allowChars[random.Next(allowChars.Length)];
            }
            return randomarticle;
        }


        private void ProductImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == true)
                {
                    var imageSource = new BitmapImage(new Uri(openFileDialog.FileName));

                    if (imageSource.PixelWidth <= 300 && imageSource.PixelHeight <= 200)
                    {
                        FlagPhoto = true;
                        ProductImage.Source = imageSource;

                        byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                        string imageName = System.IO.Path.GetFileName(openFileDialog.FileName);

                        CurrentProduct.ProductName = imageName;
                        CurrentProduct.ProductPhoto = imageBytes;
                    }
                    else
                    {
                        MessageBox.Show("Выберите изображение с разрешением не более 300x200 пикселей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выборе изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
