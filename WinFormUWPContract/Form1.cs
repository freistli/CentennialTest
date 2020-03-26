using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace WinFormUWPContract
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       async Task ShowNotification(string path)
        {
            string title = "The current time is";
            string timeString = $"{DateTime.Now:HH:mm:ss}";
            string thomasImage = "https://unsplash.it/360/202?image=883";

            string toastXmlString =
            $@"<toast><visual>
            <binding template='ToastGeneric'>
            <text>{title}</text>
            <text>{timeString}</text>
            <image src='{thomasImage}'/>
            </binding>
        </visual></toast>";

            string filepath = path;
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await folder.GetFileAsync(filepath); // error here
            toastXmlString = await Windows.Storage.FileIO.ReadTextAsync(file);

            var xmlDoc = new XmlDocument();
            
            xmlDoc.LoadXml(toastXmlString);

            var toastNotification = new ToastNotification(xmlDoc);

            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toastNotification);
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            
            Windows.Storage.StorageFolder localCacheFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder;

            StorageFile sf =  await localCacheFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.ReplaceExisting);

            label.Text = sf.Path;

            var locator = new Windows.Devices.Geolocation.Geolocator();
            var location = await locator.GetGeopositionAsync();
            var position = location.Coordinate.Point.Position;
            var latlong = string.Format("lat:{0}, long:{1}", position.Latitude, position.Longitude);
            label2.Text = latlong;

            webView1.Navigate("https://www.bing.com");
            webView1.Update();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowNotification(@"Assets\toast.xml");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowNotification(@"Assets\toast2.xml");
        }
    }
}
