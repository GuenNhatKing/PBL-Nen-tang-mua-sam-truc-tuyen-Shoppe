using ShoppeWebApp.Data;

namespace ShoppeWebApp.Services
{
    public class PagingLoad
    {
        public static List<PagingInfo> GetPaging(int totalPage, int page)
        {
            List<PagingInfo> res = new List<PagingInfo>();
            if (page > 1)
            {
                res.Add(new PagingInfo
                {
                    Name = "Trang trước",
                    Value = page - 1
                });
            }
            int delta = Constants.PAGINATION_DELTA;
            for (int i = 1; i <= totalPage; ++i)
            {
                if (i <= 1 + delta || (i >= page - delta && i <= page + delta) || i >= totalPage - delta)
                {
                    res.Add(new PagingInfo
                    {
                        Name = i.ToString(),
                        Value = i
                    });
                }
                else
                {
                    if (i == 2 + delta || i == totalPage - delta - 1)
                    {
                        if ((i == 2 + delta && i == page - delta - 1) 
                        || (i == totalPage - delta - 1 && i == page + delta + 1))
                        {
                            res.Add(new PagingInfo
                            {
                                Name = i.ToString(),
                                Value = i
                            });
                        }
                        else
                        {
                            res.Add(new PagingInfo
                            {
                                Name = "...",
                                Value = null
                            });
                        }
                    }
                }
            }
            if (page < totalPage)
            {
                res.Add(new PagingInfo
                {
                    Name = "Trang sau",
                    Value = page + 1
                });
            }
            return res;
        }
        public class PagingInfo
        {
            public string Name { get; set; } = null!;
            public int? Value { get; set; }
        }
    }
}
