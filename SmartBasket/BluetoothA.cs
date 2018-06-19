using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace SmartBasket
{
    [Activity(Label = "bluetoothA")]
    public class BluetoothA : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            App.blue1 = BlueTooth.Pair(BlueTooth.Blut(), "SmartStove");
            App.socket1 = App.blue1.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            // Create your application here
            Asynch();

            SetContentView(SmartBasket.Resource.Layout.BluetoothA);
            string num = "";

            void A()
            {
                Thread.Sleep(5000);
                num = "2";
                App.send_to_ard = num;
                //Toast.MakeText(this, App.send_to_ard, ToastLength.Short).Show();
                BlueTooth.Send_to_arduino(null, null);
                //Toast.MakeText(this, "open", ToastLength.Short).Show();

                Console.WriteLine('A');
            }
            Thread thread1 = new Thread(new ThreadStart(A));
            thread1.Start();
       
            //EditText num= this.FindViewById<EditText>(SmartBasket.Resource.Id.num);
            Button sendButton = this.FindViewById<Button>(SmartBasket.Resource.Id.close);
            sendButton.Click += async delegate
            {
               
                       
                    num = "9";
                
                // App.send_to_ard = num.Text.ToString();
                App.send_to_ard = num;
                Toast.MakeText(this, App.send_to_ard, ToastLength.Short).Show();
                
                BlueTooth.Send_to_arduino(null, null);
                Toast.MakeText(this, "close", ToastLength.Short).Show();
            };

        }
        private async void Asynch()
        {
            await App.socket1.ConnectAsync();
        }
    }
}