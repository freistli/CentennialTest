using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using uwpsavefile.Classes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace uwpsavefile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
       
        public MainPage()
        {
            this.InitializeComponent();                   
        }

        async Task SaveJsonToFile(FileHelper.StorageStrategies storageStratety)
        {
            try
            {
                FileService fileService = new FileService();
                fileService.storageStrategy = storageStratety;
                string fileName = "time.txt";
                List<string> items = new List<string>();
                items.Add(System.DateTime.Now.ToLocalTime().ToString());
                items.Add(storageStratety.ToString());
                await fileService.WriteAsync(fileName, items);
                Windows.Storage.StorageFile storageFile = await fileService.GetFile(fileName);
                Status.Text = storageFile.Path;
            }
            catch (Exception ex)
            {
                Status.Text = ex.Message;
                Status.Text += ex.StackTrace;
            }
        }
        private async void SavetoLocal_Click(object sender, RoutedEventArgs e)
        {
            await SaveJsonToFile(FileHelper.StorageStrategies.Local);
        }

        private async void SavetoRoaming_Click(object sender, RoutedEventArgs e)
        {
            await SaveJsonToFile(FileHelper.StorageStrategies.Roaming);
        }

        private async void SavetoLocalCache_Click(object sender, RoutedEventArgs e)
        {
            await SaveJsonToFile(FileHelper.StorageStrategies.LocalCache);
        }

        private async void SavetoTemporary_Click(object sender, RoutedEventArgs e)
        {
            await SaveJsonToFile(FileHelper.StorageStrategies.Temporary);
        }

        private async void SavetoSharedLocalFolder_Click(object sender, RoutedEventArgs e)
        {
            await SaveJsonToFile(FileHelper.StorageStrategies.SharedLocalFolder);
        }

        private async void SavetoDrive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = @"c:\data\defaultaccount\appdata\local\SomeTextFile" + Guid.NewGuid().ToString() + ".txt";
                using (StreamWriter Destination = File.CreateText(fileName))
                {
                    char[] buffer = new char[0x1000];
                    await Destination.WriteAsync(buffer, 0, 0x1000);
                }

            }
            catch (Exception ex)
            {
                Status.Text = ex.Message;
                Status.Text += ex.StackTrace;
            }

        }

        private async void SavetoDriveAsync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // string fileName = @"c:\output\SomeTextFile" + Guid.NewGuid().ToString() + ".txt";
                Uri uri = new Uri("ms-appdata:///local/time.txt");
                //Uri uri = new Uri(fileName);
                var thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(uri);
                StorageFile file = await Windows.Storage.StorageFile.CreateStreamedFileFromUriAsync("time.txt",uri,thumbnail);
                Status.Text = uri.AbsolutePath;
            }
            catch (Exception ex)
            {
                Status.Text = ex.Message;
                Status.Text += ex.StackTrace;
            }
        }

        private async void SavetoDocuments_Click(object sender, RoutedEventArgs e)
        {
            await SaveJsonToFile(FileHelper.StorageStrategies.DocumentLibrary);
        }

        private async void SavetoPublisherCacheFolder_Click(object sender, RoutedEventArgs e)
        {
            await SaveJsonToFile(FileHelper.StorageStrategies.PublisherCacheFolder);
        }

        void SaveCredential(string resource, string username, string password)
        {
            PasswordVault vault = new PasswordVault();
            PasswordCredential cred = new PasswordCredential(resource, username, password);
            vault.Add(cred);
        }
        IReadOnlyList<PasswordCredential> RetrieveCredential(string resource)
        {
            PasswordVault vault = new PasswordVault();
            return vault.FindAllByResource(resource);
        }

        private void SavetoCredential_Click(object sender, RoutedEventArgs e)
        {
            string resource = "MyAppResource";
            SaveCredential(resource, "freistli", "uwpdemov2");
            IReadOnlyList<PasswordCredential> locker = RetrieveCredential(resource);
            PasswordCredential passwordCredential = locker.First();
            Status.Text = passwordCredential.UserName + " ";
            passwordCredential.RetrievePassword();
            Status.Text += passwordCredential.Password;
        }

        private async void OpenFilePicker_Click(object sender, RoutedEventArgs e)
        {
            //Create the picker object
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            // Users expect to have a filtered view of their folders 
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            // Open the picker for the user to pick a file
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                Status.Text = file.Path;
            } 

        }
    }
}
