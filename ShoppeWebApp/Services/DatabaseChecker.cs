using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;

namespace ShoppeWebApp.Services
{
    public class DatabaseChecker
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseChecker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task CheckDatabase()
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ShoppeWebAppDbContext>();
            var danhSachDonHang = _context.Donhangs.Where(i => i.TrangThai == Constants.CHO_XAC_NHAN).ToList();
            TimeSpan time = TimeSpan.FromMinutes(1);
            foreach(var donHang in danhSachDonHang)
            {
                TimeSpan? difference = (DateTime.Now - donHang.ThoiGianTao)?.Duration();
                if (difference > time)
                {
                    donHang.TrangThai = Constants.HUY_DON_HANG;
                    _context.Donhangs.Update(donHang);
                    string IdLienHe = donHang.IdLienHe;
                    var ttlh = await _context.Thongtinlienhes.OrderBy(i => i.IdLienHe)
                        .FirstOrDefaultAsync(i => i.IdLienHe == IdLienHe);
                    string? IdNguoiDung = ttlh?.IdNguoiDung;
                    if(IdNguoiDung != null)
                    {
                        var nguoiDung = await _context.Nguoidungs.OrderBy(i => i.IdNguoiDung)
                            .FirstOrDefaultAsync(i => i.IdNguoiDung == IdNguoiDung);
                        if(nguoiDung != null)
                        {
                            nguoiDung.SoDu += donHang.TongTien;
                            _context.Nguoidungs.Update(nguoiDung);
                        }
                    }
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception) {}
        }
    }
}
