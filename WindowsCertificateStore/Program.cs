using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using static System.Formats.Asn1.AsnWriter;

namespace WindowsCertificateStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("\r\nExists Certs Name and Location");
            Console.WriteLine("------ ----- -------------------------");

            foreach (StoreLocation storeLocation in (StoreLocation[]) Enum.GetValues(typeof(StoreLocation)))
            {
                foreach (StoreName storeName in (StoreName[]) Enum.GetValues(typeof(StoreName)))
                {
                    X509Store store = new X509Store(storeName, storeLocation);
                    try
                    {
                        store.Open(OpenFlags.OpenExistingOnly);
                        Console.WriteLine("Yes    {0,4}  {1}, {2}", store.Certificates.Count, store.Name, store.Location);
                    }
                    catch (CryptographicException)
                    {
                        Console.WriteLine("No           {0}, {1}", store.Name, store.Location);
                    }
                }
                Console.WriteLine();
            }
            // Create the windows certificate store instance. Certificates will be accessed for personal certs
            // from the current user.
            X509Store myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            // Open the store for reading and writing
            myStore.Open(OpenFlags.ReadWrite);

            // Cng creation parameters to configure the cng key
            CngKeyCreationParameters keyParams = new CngKeyCreationParameters
            {
                ExportPolicy = CngExportPolicies.None, // private key not exportable
                Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider, // we will use the microsoft software key storage provider to create and store the key.
                KeyUsage = CngKeyUsages.Signing | CngKeyUsages.Decryption, // key will be used for signing and decryption
                KeyCreationOptions = CngKeyCreationOptions.OverwriteExistingKey // Overwrite existing key for testing
            };

            // Add a property of length 3072 so the OS knows how big the key will be.
            //keyParams.Parameters.Add(new CngProperty("Length", BitConverter.GetBytes(3072), CngPropertyOptions.None));

            // Create the key within the OS.
            //using CngKey cngKey = CngKey.Create(CngAlgorithm.Rsa, "private_key_gabeamv", keyParams);
            
            //using RSA rsa = new RSACng(cngKey);
            //Console.WriteLine(rsa.ExportSubjectPublicKeyInfoPem());

            /*
            CertificateRequest certRequest = new CertificateRequest("CN=SecureNotes-gabeamv", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            X509Certificate2 cert = certRequest.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10));
            Console.WriteLine(cert.HasPrivateKey);
            myStore.Add(cert);
            myStore.Close();
            */
            var cert = myStore.Certificates
                .Find(X509FindType.FindBySubjectName, "SecureNotes-gabeamv", false)
                .FirstOrDefault();
            RSA rsa2 = cert.GetRSAPrivateKey();
            Console.WriteLine(rsa2.ExportSubjectPublicKeyInfoPem());
            RSA rsa = new RSACng(CngKey.Open("private_key_gabeamv"));
            Console.WriteLine(rsa.ExportSubjectPublicKeyInfoPem());


        }
    }
}
