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
using Java.Util;

namespace SmartBasket
{
    [Activity(Label = "bluetoothA")]
    public class BluetoothA : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            App.blue1 = BlueTooth.Pair(BlueTooth.Blut(), "Mi Phone");
            App.socket1 = App.blue1.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            // Create your application here
            Asynch();

            SetContentView(SmartBasket.Resource.Layout.BluetoothA);
            EditText num= this.FindViewById<EditText>(SmartBasket.Resource.Id.num);
            Button sendButton = this.FindViewById<Button>(SmartBasket.Resource.Id.send);
            sendButton.Click += async delegate
            {
                App.send_to_ard = num.Text.ToString();
                Toast.MakeText(this, App.send_to_ard, ToastLength.Short).Show();
                
                BlueTooth.Send_to_arduino(null, null);
            };

        }
        private async void Asynch()
        {
            await App.socket1.ConnectAsync();
        }
    }
}