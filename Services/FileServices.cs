namespace GraduationProject.Services
{
    static public class FileServices
    {
        static public void SaveFileLocal(IFormFile file,string DirectoryName)
        {
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), DirectoryName , fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }
    }
}
