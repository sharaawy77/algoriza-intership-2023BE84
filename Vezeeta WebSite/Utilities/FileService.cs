namespace Vezeeta_WebSite.Utilities
{
    public class FileService:IFileService
    {
        private readonly IWebHostEnvironment env;

        public FileService(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public bool DeleteImage(string fileName)
        {
            try
            {
                var wwwPath = env.WebRootPath;
                var path = Path.Combine(wwwPath, "Uploads\\", fileName);
                if (File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public Tuple<int, string> SaveImage(IFormFile file)
        {
            try
            {
                var contentpath = env.
                    WebRootPath;
                var path = Path.Combine(contentpath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var ext = Path.GetExtension(file.FileName);
                var allowedExtentios = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtentios.Contains(ext))
                {
                    string msg = string.Format("only {0} extentions are allowed", string.Join(",", allowedExtentios));
                    return new Tuple<int, string>(0, msg);

                }
                string UniqueString = Guid.NewGuid().ToString();
                var newfileName = UniqueString + ext;
                var filepath = Path.Combine(path, newfileName);
                var stream = new FileStream(filepath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
                return new Tuple<int, string>(1, newfileName);

            }
            catch (Exception)
            {

                return new Tuple<int, string>(0, "Error has Occured");
            }
        }
    }

}

