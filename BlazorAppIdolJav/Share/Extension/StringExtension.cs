using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameManagement.Share.Extension
{
    public class StringExtension
    {
        public string GetCharacterHash(string code)
        {
            var codeHash = BCrypt.Net.BCrypt.HashPassword(code);
            return codeHash;
        }
    }
}
