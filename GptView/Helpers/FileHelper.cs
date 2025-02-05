namespace GptView.Helpers
{
    public static class FileHelper
    {
        public static string CreateDirectory(this IHostEnvironment env, string directoryName)
        {
            var rootPath = env.ContentRootPath;
            return Directory.CreateDirectory(Path.Combine(rootPath, directoryName)).FullName;          
        } 
    }
}
