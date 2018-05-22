using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth.LE;
using Android.Util;
using Android.Media;

public static class App
{
    public static Java.IO.File _file;
    public static Java.IO.File _dir;
    public static Android.Graphics.Bitmap bitmap;
    public static Button but;
    public static BluetoothDevice blue1;
    public static BluetoothSocket socket1;
    public static BluetoothDevice Gun_blue;
    public static BluetoothSocket Gun_socket;
    public static string send_to_ard;
    public static string get_from_gun;
    public static int[] cords = new int[12];
    public static byte[] buf = new byte[32];
    public static string RecRes;
    public static MediaPlayer _player;
    public static Vibrator vibrator;
    public static bool wait;
    public static int count;
    public static System.Diagnostics.Stopwatch stopwatch;

}
namespace SmartBasket
{
    using System.IO;
    using Android.Bluetooth;
    using Android.Graphics;
    using Java.Util;

    public static class BlueTooth

    {
        public static BluetoothAdapter Blut()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                throw new Exception("No Bluetooth adapter found.");

            if (!adapter.IsEnabled)
            {
                throw new Exception("Bluetooth adapter is not enabled.");
            }
            return adapter;
        }
        public static BluetoothDevice Pair(BluetoothAdapter adapter, String name)
        {
            //Android.Bluetooth.LE.ScanCallback x = Android.Bluetooth.LE.ScanCallback();
            //adapter.BluetoothLeScanner.StartScan(x);
            ICollection<BluetoothDevice> pairedDevices = adapter.BondedDevices;
            BluetoothDevice devicee = null;
            if (pairedDevices.Count > 0)
            {
                // There are paired devices. Get the name and address of each paired device.
                foreach (BluetoothDevice devic in pairedDevices)
                {
                    if (devic.Name.Contains(name))
                    {
                        devicee = devic;
                        break;
                    }

                }

            }

            return devicee;
        }
        public static async System.Threading.Tasks.Task<BluetoothSocket> SocAsync(BluetoothDevice device)
        {
            BluetoothSocket _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            await _socket.ConnectAsync();
            return _socket;
        }
        public static async System.Threading.Tasks.Task<byte[]> ReeadAsync(BluetoothSocket _socket)
        {
            byte[] buffer = new byte[32];
            await _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);
            App.get_from_gun = buffer.ToString();
            return buffer;
        }
        public static async System.Threading.Tasks.Task<int> WriteeAsync(BluetoothSocket _socket, byte[] buffer)
        {

            await _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);

            return 0;
        }
        public static async void Send_to_arduino(object sender, EventArgs eventArgs)
        {
            BluetoothSocket _socket = App.socket1;

            if (_socket.IsConnected)
            {
                App.but.Text = "connected";


                byte[] bytes_c = System.Text.Encoding.ASCII.GetBytes(App.send_to_ard);
                await BlueTooth.WriteeAsync(_socket, bytes_c);

                App.but.Text = "sent";
                return;


            }
        }



    }
}