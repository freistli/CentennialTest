using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using System;
using System.Reflection;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using Windows.UI.Core;

using Windows.Management.Deployment;
using Windows.UI.Xaml.Input;
using System.Net;
using Windows.ApplicationModel;

namespace wpfsavefile
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetCurrentPackagePath(ref int length, StringBuilder path);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        const uint APPMODEL_ERROR_NO_PACKAGE = 15700;
        const uint ERROR_INSUFFICIENT_BUFFER = 122;


        //check for an update on my server
        private async void CheckUpdate()
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://localhost:8082/wpfinstallpoint/version.txt");
            StreamReader reader = new StreamReader(stream);
            var newVersion = new Version(await reader.ReadToEndAsync());
            Package package = Package.Current;
            PackageVersion packageVersion = package.Id.Version;
            var currentVersion = new Version(string.Format("{0}.{1}.{2}.{3}", packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));
            
            //compare package versions
            if (newVersion.CompareTo(currentVersion) > 0)
            {
                string messageBoxText = "Do you want to update app?";
                string caption = "WPF Saver";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                // Display message box
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // User pressed Yes button
                        // ...
                        await CommandInvokedHandler();
                        break;
                    case MessageBoxResult.No:
                        // User pressed No button
                        // ...
                        break;
                    case MessageBoxResult.Cancel:
                        // User pressed Cancel button
                        // ...
                        break;
                }
            }
            else
            {
                string messageBoxText = "Didn't find update";
                string caption = "WPF Saver";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Information;
                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        private async Task CommandInvokedHandler()
        {
              PackageManager packagemanager = new PackageManager();
                await packagemanager.AddPackageAsync(
                    new Uri("http://localhost:8082/wpfinstallpoint/WPFPackage_1.0.27.0_x64_Debug_Test/WPFPackage_1.0.27.0_x64_Debug.msix"),
                    null,
                    DeploymentOptions.ForceApplicationShutdown
                );          
        }

        public MainWindow()
        {
            InitializeComponent();
            Package package = Package.Current;
            PackageVersion packageVersion = package.Id.Version;
            label.Content = new Version(string.Format("{0}.{1}.{2}.{3}", packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));
            try
            {
                var type = "ApplicationData";
                var prop = "Current";
                if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent(type, prop))
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("lastfilepath"))
                        label.Content = ApplicationData.Current.LocalSettings.Values["lastfilepath"];

                label.Content += "\n" + "~~~~~~~~~~~~Directory.GetCurrentDirectory\n";
                label.Content += Directory.GetCurrentDirectory();

            }
            catch (Exception ex)
            {
                label.Content += ex.GetType() + "\n" + ex.Message;
            }

        }

        private void saveToLocal_Click(object sender, RoutedEventArgs e)
        {
           

            string fileName = @"c:\output\SomeTextFile" + Guid.NewGuid().ToString() + ".txt";
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.Write("Writing Text ");
                    sw.WriteLine("Here we go.");
                    sw.WriteLine("-------------------");
                    sw.Write("Today is is: " + DateTime.Now);
                    sw.WriteLine("Done");
                    label.Content = fileName;
                    ApplicationData.Current.LocalSettings.Values["lastfilepath"] = label.Content;

                    string toastStr =
                                    "<toast>" +
                                    "   <visual>" +
                                    "       <binding template = 'ToastGeneric'>" +
                                    "       <image src='ms-appx:///Assets/SplashScreen.png' alt='Splash'/>" +
                                    "          <text>Cool Notification</text>" +
                                    "          <text>Select snooze time.</text>" +
                                    "          </binding>" +
                                    "   </visual>" +
                                    "   <actions>" +
                                    "       <input id = 'snoozeTime' type = 'selection' defaultSelection = '10' >" +
                                    "           <selection id = '1' content = '1 minute' />" +
                                    "           <selection id = '5' content = '5 minutes' />" +
                                    "           <selection id = '10' content = '10 minutes' />" +
                                    "           <selection id = '30' content = '30 minutes' />" +
                                    "           <selection id = '60' content = '1 hour' />" +
                                    "       </input>" +
                                    "       <action activationType = 'system' arguments = 'snooze' hint-inputId = 'snoozeTime' content = '' />" +
                                    "       <action activationType = 'system' arguments = 'dismiss' content = '' />" +
                                    "   </actions>" +
                                    "</toast>";

                    XmlDocument toastXml = new XmlDocument();
                    toastXml.LoadXml(toastStr);

                    ToastNotification toast = new ToastNotification(toastXml);

                    ToastNotificationManager.CreateToastNotifier().Show(toast);

                }
            }
            catch (Exception ex)
            {
                label.Content = ex.Message;
            }

        }

        private void saveToUserLocal_Click(object sender, RoutedEventArgs e)
        {
            //var pathWithEnv = @"%USERPROFILE%\AppData\Local\wpfsavefileApp";

            MessageBox.Show("Paused");

            var pathWithEnv = @"c:\users\freistli\AppData\Local\wpfsavefileApp";

            //var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);


            var filePath = pathWithEnv;

            // If directory does not exist, create it. 
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var fileName = filePath + @"\newtest" + Guid.NewGuid().ToString() + ".txt";
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.Write("Writing Text ");
                    sw.WriteLine("Here we go.");
                    sw.WriteLine("-------------------");
                    sw.Write("Today is is: " + DateTime.Now);
                    sw.WriteLine("Done");
                    label.Content = fileName;

                    ApplicationData.Current.LocalSettings.Values["lastfilepath"] = label.Content;

                }
            }
            catch (Exception ex)
            {
                label.Content = ex.Message;

            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder localCacheFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder;

            Task.Run(async () => localCacheFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.ReplaceExisting)).Wait();

            label.Content = localCacheFolder.Path;


        }

        private void GetCurrentDirectory_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            label.Content = currentDirectory;


        }

        private async void CheckIdentity_Click(object sender, RoutedEventArgs e)
        {
            int length = 0;
            StringBuilder name = new StringBuilder();
            int rc = GetCurrentPackageFullName(ref length, name);

            if (rc != ERROR_INSUFFICIENT_BUFFER)
            {
                if (rc == APPMODEL_ERROR_NO_PACKAGE)
                    label.Content = "Process has no package identity [15700]\n";

            }
            else
            {
                label.Content = "kernel32!GetCurrentPackageFullName got length: " + "\n" + length;
                name = new StringBuilder(length);
                GetCurrentPackageFullName(ref length, name);
                label.Content += "\n" + name;
            }

        }

        private void CheckIdentity_V2_Click(object sender, RoutedEventArgs e)
        {
            int length = 0;
            StringBuilder path = new StringBuilder();
            uint rc = GetCurrentPackagePath(ref length, path);

            if (rc != ERROR_INSUFFICIENT_BUFFER)
            {
                if (rc == APPMODEL_ERROR_NO_PACKAGE)
                    label.Content = "Process has no package identity [15700]\n";

            }
            else
            {
                label.Content = "kernel32!GetCurrentPackagePath got length: " + "\n" + length;
                path = new StringBuilder(length);
                GetCurrentPackagePath(ref length, path);
                label.Content += "\n" + path;
            }
        }

        private async void CheckIdentity_V3_Click(object sender, RoutedEventArgs e)
        {
            int length = 0;
            String path = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

            label.Content += "\n" + "~~~~~~~~~~~~Windows.ApplicationModel.Package.Current.InstalledLocation.Path";
            label.Content += "\n" + path;

            StorageFile sfi = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/StoreLogo.png"));

            label.Content += "\n" + "~~~~~~~~~~~~StorageFile.GetFileFromApplicationUriAsync";
            label.Content += "\n" + sfi?.Path;

            label.Content += "\n" + "~~~~~~~~~~~~System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location";
            label.Content += "\n" + System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            label.Content += "\n" + "~~~~~~~~~~~~Environment.SpecialFolder";
            var sfDict = new SortedDictionary<string, Environment.SpecialFolder>();
            foreach (var sf in Enum.GetValues(typeof(Environment.SpecialFolder)).AsQueryable())
            {
                if (!sfDict.ContainsKey(sf.ToString()))
                    sfDict.Add(sf.ToString(), (Environment.SpecialFolder)sf);
            }
            // look up special folders and print them on screen
            foreach (var item in sfDict)
            {
                var name = $"Environment.SpecialFolder.{item.Key}";
                var temppath = Environment.GetFolderPath(item.Value);
                label.Content += "\n" + temppath;
            }

        }

        private void GetTmpFolder_Click(object sender, RoutedEventArgs e)
        {
            label.Content += "\n" + "ApplicationData.Current.TemporaryFolder.Path";
            label.Content += "\n" + ApplicationData.Current.TemporaryFolder.Path;
            CopyToMem(label.Content);
        }

        private void CopyToMem(object str)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(str.ToString());
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }

        private void SaveToCurrent_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("log.txt", DateTime.Now.ToString());
        }

        private void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            CheckUpdate();
        }
    }

}
