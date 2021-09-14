using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xavor.SD.WebAPI.Helper
{
    public static class ConfigurationManager
    {
        public static string GetSecretValue(string secretName, string vaultName)
        {
            //var client = new SecretClient(new Uri("https://" + vaultName + ".vault.azure.net/"), new ManagedIdentityCredential());

            //KeyVaultSecret keyVaultSecret = client.GetSecret(secretName);

            //string secretValue = keyVaultSecret.Value;

            return "";
        }
    }
}
