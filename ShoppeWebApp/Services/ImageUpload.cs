using System.Security.Cryptography;
using ShoppeWebApp.Data;

namespace QLTTDT.Services
{
    public class ImageUpload
    {
        private string[] allowImageTypes = new string[] { "image/jpg", "image/jpeg", "image/png", "image/webp" };
        private IWebHostEnvironment _webHost;
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public ImageUpload(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }
        public async Task<bool> SaveImageAs(IFormFile file, string[] Directories = null)
        {
            if (file == null || file.Length < 0)
            {
                return false;
            }
            if (!allowImageTypes.Contains(file.ContentType.ToLower()))
            {
                return false;
            }
            string uploadPath = _webHost.WebRootPath;
            FilePath = "";
            if(Directories != null)
            foreach(var str in Directories)
            {
                uploadPath = Path.Combine(uploadPath, str);
                FilePath = Path.Combine(FilePath, str);
            }
            
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            FileName = GenerateFileName() + Path.GetExtension(file.FileName);
            FilePath = Path.Combine(FilePath, FileName);
            var savePath = Path.Combine(uploadPath, FileName);
            try
            {
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
        public bool DeleteImage(string imgPath)
        {
            if (string.IsNullOrEmpty(imgPath)) return false;
            string path = Path.Combine(_webHost.WebRootPath, imgPath);
            if (Path.Exists(path))
            try
            {
                Console.WriteLine($"Delete file at: {path}");
                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }
        private string GenerateFileName()
        {
            byte[] bytes = new byte[Constants.RANDOM_SIZE];
            RandomNumberGenerator.Fill(bytes);
            long elapsed = (long)(DateTime.UtcNow - Constants.DAYS_USE_FOR_RANDOM).TotalMilliseconds;
            return elapsed.ToString() + bytesToBase62(bytes);
        }
        private static string bytesToBase62(byte[] bytes)
        {
            string result = "";
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            bool[] bools = new bool[bytes.Length * 8];
            bool[] boolRes = new bool[bytes.Length * 8];
            for (int i = 0; i < bytes.Length; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    bools[8 * i + j] = (bytes[i] & (1 << 7 - j)) != 0;
                }
            }
            bool[] bool62 = { false, false, false, true, true, true, true, true, false };
            while (true)
            {
                bool[] curr = new bool[9];
                for (int i = 0; i < bools.Length; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        curr[j] = curr[j + 1];
                    }
                    curr[8] = bools[i];
                    bool lowerThan = false;
                    for (int j = 0; j < 9; ++j)
                    {
                        if (!curr[j] && bool62[j])
                        {
                            lowerThan = true;
                        }
                        else if (curr[j] && !bool62[j])
                        {
                            break;
                        }
                    }
                    if (!lowerThan)
                    {
                        bool remember = false;
                        for (int j = 8; j >= 0; --j)
                        {
                            if ((curr[j] == bool62[j]) && remember)
                            {
                                curr[j] = true;
                            }
                            else if (!remember)
                            {
                                if (curr[j] == bool62[j]) curr[j] = false;
                                else
                                {
                                    curr[j] = true;
                                    remember = bool62[j];
                                }
                            }
                            else
                            {
                                curr[j] = false;
                                remember = bool62[j];
                            }
                        }
                        boolRes[i] = true;
                    }
                    else
                    {
                        boolRes[i] = false;
                    }
                }
                int idx = 0;
                int pow2 = 1;
                for (int i = 8; i >= 0; --i)
                {
                    if (curr[i]) idx += pow2;
                    pow2 <<= 1;
                }
                result += chars[idx];
                bool nextLoop = false;
                for (int i = 0; i < boolRes.Length; ++i)
                {
                    bools[i] = boolRes[i];
                    nextLoop = nextLoop || boolRes[i];
                }
                if (!nextLoop) break;
            }
            return result;
        }
    }
}