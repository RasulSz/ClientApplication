using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using Microsoft.Win32;
using static System.Net.WebRequestMethods;
using System.IO;
using System.Threading;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public OpenFileDialog op = new OpenFileDialog();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                SelectedImage.Source = new BitmapImage(new Uri(op.FileName));
                FileInfoTxtb.Text = op.FileName;
            }
        }

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedImage.Source != null)
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ipAddress = IPAddress.Parse("192.168.1.48");
                var port = 80;
                var ep = new IPEndPoint(ipAddress, port);
                //var bytess = getJPGFromImageControl(SelectedImage.Source as BitmapImage);

                try
                {
                    socket.Connect(ep);

                    if (socket.Connected)
                    {
                        serverInfoLbl.Content = "Connected Server . . .";
                        var bytes = getJPGFromImageControl(SelectedImage.Source as BitmapImage);
                        socket.Send(bytes);
                    }
                }
                catch (Exception ex)
                {
                    serverInfoLbl.Content = ex.Message;
                }
            }
            else
            {
                MessageBox.Show("Please select a photo !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
