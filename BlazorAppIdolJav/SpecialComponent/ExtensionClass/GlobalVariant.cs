namespace GameManagement.SpecialComponent.ExtensionClass
{
    public class GlobalVariant
    {
        private static string _fileVersion;
        public static void InitFileVersion()
        {
            _fileVersion = DateTime.Now.Ticks.ToString();
        }
        public static string FileVersion => _fileVersion;
        public static string UploadFolder;
        public static string UploadFolderResource = "Upload";
        public static string TempFolder = "TempFolder";
        public static string TempFolderResource = "Temp";
        public static long MaxFileSize = 1024 * 1024 * 20;


    }
}
