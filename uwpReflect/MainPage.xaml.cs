using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Input;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace uwpReflect
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

    }
    public sealed partial class MainPage : Page
    {
        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);
        static POINT pt;
        public MainPage()
        {
            this.InitializeComponent();
            MouseDevice mouseDevice = Windows.Devices.Input.MouseDevice.GetForCurrentView();
            mouseDevice.MouseMoved += OnMouseMoved;
        }

        private void test1()
        {
            Status.Text += "\ntest1 method is invoked!";
        }

        private void test2()
        {
            Status.Text += "\ntest2 method is invoked!";
        }
        private void button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                Type mytype = this.GetType();
                mytype.GetTypeInfo().GetDeclaredMethod("test1").Invoke(this, null);

                Type type = this.GetType();
                //BindingFlags.Instance表示是实例方法，也就是不是static方法
                MethodInfo Haha = type.GetMethod("test2", BindingFlags.NonPublic | BindingFlags.Instance);
                Haha.Invoke(this, null);

                Status.Text += "\n" + Directory.GetCurrentDirectory();
               
            }
            catch (Exception ex)
            {
                Status.Text += "\n" + ex.Message;
                Status.Text += "\n" + ex.StackTrace;
            }

        }

        //PInvoke Method to get cursor
        private void MousePosition_Click(object sender, RoutedEventArgs e)
        {
            POINT pt = new POINT();
            GetCursorPos(out pt);
            Status.Text += "\nMouse X:" + pt.X.ToString();
            Status.Text += "\nMouse Y:" + pt.Y.ToString();

            
            //MouseDevice md = new Windows.Devices.Input.MouseDevice();
            
        }
        

    private void OnMouseMoved(MouseDevice sender, MouseEventArgs args)
        {
          
                pt.X += args.MouseDelta.X;
                pt.Y += args.MouseDelta.Y;

            Status.Text = "\nMouse delta X:" + pt.X.ToString();
            Status.Text += "\nMouse delta Y:" + pt.Y.ToString();

        }

        private void touchRectangle_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pp = e.GetCurrentPoint(touchRectangle);
            Status.Text = "\nToucch X:" + pp.Position.X.ToString();
            Status.Text += "\nTouch Y:" + pp.Position.Y.ToString();

        }

        private void touchRectangle_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pp = e.GetCurrentPoint(touchRectangle);
            Status.Text = "\nTouch X:" + pp.Position.X.ToString();
            Status.Text += "\nTouch Y:" + pp.Position.Y.ToString();
        }
    }
}
