using Windows.Security.Credentials;

namespace RumarApp.Configurations
{
    public static class SecretStorage
    {
        private const string RESOURCE_NAME = "MyWinUIApp";

        public static void SaveToken(string userName, string token)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(RESOURCE_NAME, userName, token));
        }

        public static string? GetToken(string userName)
        {
            try
            {
                var vault = new PasswordVault();
                var credential = vault.Retrieve(RESOURCE_NAME, userName);
                credential.RetrievePassword();
                return credential.Password;
            }
            catch
            {
                return null;
            }
        }

        public static void ClearToken(string userName)
        {
            var vault = new PasswordVault();
            var credential = vault.Retrieve(RESOURCE_NAME, userName);
            vault.Remove(credential);
        }
    }
}
