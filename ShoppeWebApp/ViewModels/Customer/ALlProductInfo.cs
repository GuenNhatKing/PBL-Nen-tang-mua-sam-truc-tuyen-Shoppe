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
    }
}
