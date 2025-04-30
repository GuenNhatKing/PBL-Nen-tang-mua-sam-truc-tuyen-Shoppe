using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class AllProductInfo
    {
        public List<ItemInfo> productInfos = new List<ItemInfo>();
        public List<Danhmuc> categories = null!;
        public string? danhMuc = null;
        public static string ProcessQuantity(int quantity)
        {
            double curr = quantity;
            int expo = 0;
            string[] symbol = {"", "K", "M", "B", "T", "Q", "Qi"};
            while(curr >= 1000)
            {
                curr /= 1000;
                ++expo;
                if (expo >= symbol.Length) break;
            }
            string res = string.Format("{0:0.#}{1}", curr, symbol[expo]);
            return res;
        }
    }
}
