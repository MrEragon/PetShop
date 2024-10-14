using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace PeShopMaster.Classes
{
    class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Data.User User { get; set; }

        public static void GetImageData()
        {
            try
            {
                var list = Data.Trade2Entities.GetContext().Product.ToList();
                foreach (var Item in list)
                {
                    string path = Directory.GetCurrentDirectory() + @"\img\" + Item.ProductName;
                    if (File.Exists(path))
                    {
                        Item.ProductPhoto = File.ReadAllBytes(path);
                    }
                }
                Data.Trade2Entities.GetContext().SaveChanges();
            }
            catch (Exception)
            {

            }
        }
    }
}
