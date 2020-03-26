using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using uwpsavefile.Classes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace uwpSerializer
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
                string fileName = "serailize.txt";
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
        private void test()
        {
            Status.Text = "invoked";
        }
        private async void StartSerialize_Click(object sender, RoutedEventArgs e)
        {
            Test t = new Test();
            await t.CreatePO("serialize.txt");
            FileService fileService = new FileService();
            Windows.Storage.StorageFile storageFile = await fileService.GetFile("serialize.txt");
            Status.Text = storageFile.Path;
            
        }
    }
    /* The XmlRootAttribute allows you to set an alternate name 
   (PurchaseOrder) of the XML element, the element namespace; by 
   default, the XmlSerializer uses the class name. The attribute 
   also allows you to set the XML namespace for the element.  Lastly,
   the attribute sets the IsNullable property, which specifies whether 
   the xsi:null attribute appears if the class instance is set to 
   a null reference. */
    [XmlRootAttribute("PurchaseOrder", Namespace = "http://www.cpandl.com",
    IsNullable = false)]
    public class PurchaseOrder
    {
        public Address ShipTo;
        public string OrderDate;
        /* The XmlArrayAttribute changes the XML element name
         from the default of "OrderedItems" to "Items". */
        [XmlArrayAttribute("Items")]
        public OrderedItem[] OrderedItems;
        public decimal SubTotal;
        public decimal ShipCost;
        public decimal TotalCost;
    }

    public class Address
    {
        /* The XmlAttribute instructs the XmlSerializer to serialize the Name
           field as an XML attribute instead of an XML element (the default
           behavior). */
        [XmlAttribute]
        public string Name;
        public string Line1;

        /* Setting the IsNullable property to false instructs the 
           XmlSerializer that the XML attribute will not appear if 
           the City field is set to a null reference. */
        [XmlElementAttribute(IsNullable = false)]
        public string City;
        public string State;
        public string Zip;
    }

    public class OrderedItem
    {
        public string ItemName;
        public string Description;
        public decimal UnitPrice;
        public int Quantity;
        public decimal LineTotal;

        /* Calculate is a custom method that calculates the price per item,
           and stores the value in a field. */
        public void Calculate()
        {
            LineTotal = UnitPrice * Quantity;
        }
    }

    public class Test
    {
     
        public async Task CreatePO(string filename)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to serialize.
            XmlSerializer serializer =
            new XmlSerializer(typeof(PurchaseOrder));

            PurchaseOrder po = new PurchaseOrder();

            // Create an address to ship and bill to.
            Address billAddress = new Address();
            billAddress.Name = "Teresa Atkinson";
            billAddress.Line1 = "1 Main St.";
            billAddress.City = "AnyTown";
            billAddress.State = "WA";
            billAddress.Zip = "00000";
            // Set ShipTo and BillTo to the same addressee.
            po.ShipTo = billAddress;
            po.OrderDate = System.DateTime.Now.ToLocalTime().ToString();

            // Create an OrderedItem object.
            OrderedItem i1 = new OrderedItem();
            i1.ItemName = "Widget S";
            i1.Description = "Small widget";
            i1.UnitPrice = (decimal)5.23;
            i1.Quantity = 3;
            i1.Calculate();

            // Insert the item into the array.
            OrderedItem[] items = { i1 };
            po.OrderedItems = items;
            // Calculate the total cost.
            decimal subTotal = new decimal();
            foreach (OrderedItem oi in items)
            {
                subTotal += oi.LineTotal;
            }
            po.SubTotal = subTotal;
            po.ShipCost = (decimal)12.51;
            po.TotalCost = po.SubTotal + po.ShipCost;

            // create file
            var _File = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
       
            try
            {
                var file = await _File.OpenAsync(FileAccessMode.ReadWrite);
                serializer.Serialize(file.AsStreamForWrite(), po);
                file.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw ex;
            }           
            
        }

        protected void ReadPO(string filename)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(PurchaseOrder));
            
            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filename, FileMode.Open);
            // Declare an object variable of the type to be deserialized.
            PurchaseOrder po;
            /* Use the Deserialize method to restore the object's state with
            data from the XML document. */
            po = (PurchaseOrder)serializer.Deserialize(fs);           
           
        }

      

       
    }

}
