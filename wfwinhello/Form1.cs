using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Data.Json;
using Windows.Networking.Connectivity;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;

namespace wfwinhello
{
    public partial class Form1 : Form
    {
        private string userId;
        private string publicKeyHint;


       
        public Form1()
        {
            InitializeComponent();
        }

        private async Task<IBuffer> CreatePassportKeyCredentialAsync()
        {
            // Create a new KeyCredential for the user on the device.
            KeyCredentialRetrievalResult keyCreationResult = await KeyCredentialManager.RequestCreateAsync(userId, KeyCredentialCreationOption.ReplaceExisting);
            if (keyCreationResult.Status == KeyCredentialStatus.Success)
            {
                // User has autheniticated with Windows Hello and the key credential is created.
                KeyCredential userKey = keyCreationResult.Credential;
                return userKey.RetrievePublicKey();
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.NotFound)
            {
               MessageBox.Show("To proceed, Windows Hello needs to be configured in Windows Settings (Accounts -> Sign-in options)");
               return null;
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.UnknownError)
            {
                MessageBox.Show("The key credential could not be created. Please try again.");
                 
                return null;
            }

            return null;
        }

        // Register the user and device with the server
        private async Task<bool> RegisterPassportCredentialWithServerAsync(IBuffer publicKey)
        {
            // Include the name of the current device for the benefit of the user.
            // The server could support a Web interface that shows the user all the devices they
            // have signed in from and revoke access from devices they have lost.

            var hostNames = NetworkInformation.GetHostNames();
            var localName = hostNames.FirstOrDefault(name => name.DisplayName.Contains(".local"));
            string computerName = localName.DisplayName.Replace(".local", "");

            JsonValue jsonResult = await JsonRequest.Create()
                .AddString("userId", userId)
                .AddString("publicKey", CryptographicBuffer.EncodeToBase64String(publicKey))
                .AddString("deviceName", computerName)
                .PostAsync("api/Authentication/Register");
            bool result = (jsonResult != null) && jsonResult.GetBoolean();

            return result;
        }
        private async    Task StartUsingWindowsHello()
        {
            
            // Create the key credential with Passport APIs
            IBuffer publicKey = await CreatePassportKeyCredentialAsync();
            if (publicKey != null)
            {
                // Register the public key and attestation of the key credential with the server
                // In a real-world scenario, this would likely also include:
                // - Certificate chain for attestation endorsement if available
                // - Status code of the Key Attestation result : Included / retrieved later / retry type
                if (await RegisterPassportCredentialWithServerAsync(publicKey))
                {
                    // Remember that this is the user whose credentials have been registered
                    // with the server.
                    ApplicationData.Current.LocalSettings.Values["userId"] = userId;

                    // When communicating with the server in the future, we pass a hash of the
                    // public key in order to identify which key the server should use to verify the challenge.
                    HashAlgorithmProvider hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
                    IBuffer publicKeyHash = hashProvider.HashData(publicKey);
                    ApplicationData.Current.LocalSettings.Values["publicKeyHint"] = CryptographicBuffer.EncodeToBase64String(publicKeyHash);

                    MessageBox.Show("Windows Hello Registered for " + userId);
                }
                else
                {
                    // Delete the failed credentials from the device.
                    await Util.TryDeleteCredentialAccountAsync(userId);

                   MessageBox.Show("Failed to register with the server.");
                    
                }
            }
            
        }


        private async Task<bool> SignInWithHelloAsync()
        {
            LookupUser();

            // Open the existing user key credential.
            KeyCredentialRetrievalResult retrieveResult = await KeyCredentialManager.OpenAsync(userId);

            if (retrieveResult.Status != KeyCredentialStatus.Success)
            {
                return false;
            }

            KeyCredential userCredential = retrieveResult.Credential;

            // Request a challenge from the server.
            JsonValue jsonChallenge = await JsonRequest.Create()
                .AddString("userId", userId)
                .AddString("publicKeyHint", publicKeyHint)
                .PostAsync("api/Authentication/RequestChallenge");

            if (jsonChallenge == null)
            {
                return false;
            }

            // Sign the challenge using the user's KeyCredential.
            IBuffer challengeBuffer = CryptographicBuffer.DecodeFromBase64String(jsonChallenge.GetString());
            KeyCredentialOperationResult opResult = await userCredential.RequestSignAsync(challengeBuffer);

            if (opResult.Status != KeyCredentialStatus.Success)
            {
                return false;
            }

            // Get the signature.
            IBuffer signatureBuffer = opResult.Result;

            // Send the signature back to the server to confirm our identity.
            // The publicKeyHint tells the server which public key to use to verify the signature.
            JsonValue jsonResult = await JsonRequest.Create()
                .AddString("userId", userId)
                .AddString("publicKeyHint", publicKeyHint)
                .AddString("signature", CryptographicBuffer.EncodeToBase64String(signatureBuffer))
                .PostAsync("api/Authentication/SubmitResponse");
            if (jsonResult == null)
            {
                return false;
            }

            return jsonResult.GetBoolean();
        }
        private async Task SignInWithHello()
        {
            

            bool result = await SignInWithHelloAsync();

           
            if (result)
            {
                MessageBox.Show("Signed in with Windows Hello");
            }
            else
            {
                MessageBox.Show("Signed In Failed with Windows Hello");
            }
        }

        void LookupUser()
        {
            userId = UserName.Text;
            publicKeyHint = ApplicationData.Current.LocalSettings.Values["publicKeyHint"] as string ?? string.Empty;
        }
        private async Task Unregister()
        {
            LookupUser();

            // Remove the credential from the key credential manager, to free up space on the device.
            await Util.TryDeleteCredentialAccountAsync(userId);

            // Remove the credential from the server, to free up space on the server.
            JsonValue jsonResult = await JsonRequest.Create()
                .AddString("userId", userId)
                .AddString("publicKeyHint", publicKeyHint)
                .PostAsync("api/Authentication/RemoveRegisteredKey");
            if (jsonResult == null || !jsonResult.GetBoolean())
            {
                // Could not delete from the server. Nothing we can do; just ignore the error.
            }

            // Remove our app's knowledge of the user.
            ApplicationData.Current.LocalSettings.Values.Remove("userId");
            ApplicationData.Current.LocalSettings.Values.Remove("publicKeyHint");
            userId = string.Empty;
            
        }
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            userId = UserName.Text;
            if (await KeyCredentialManager.IsSupportedAsync())
            {
                // Navigate to Enable Hello Page, passing the account ID (username) as a parameter
                MessageBox.Show("Support Windows Hello");
                await StartUsingWindowsHello();
            }
            else
            {
              
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await SignInWithHello();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await Unregister();
        }
     
        private async void button4_Click(object sender, EventArgs e)
        {
           /* AccountsSettingsPane.Show();
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += OnAccountCommandsRequested;

            bool checkID = IdentityChecker.SampleIdentityConfigurationCorrect(AzureActiveDirectoryClientId);
            */
            Uri uri = new Uri("uwpbase://");

            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
