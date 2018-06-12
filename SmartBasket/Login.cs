using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ArduinoSmartBasket;
using Microsoft.WindowsAzure.MobileServices;


namespace SmartBasket
{
    //(Label = "Login")
    [Activity(MainLauncher = true,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/AppTheme")]
    public class Login : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(SmartBasket.Resource.Layout.Login);
            CurrentPlatform.Init();
            EditText input = this.FindViewById<EditText>(SmartBasket.Resource.Id.input_id);
            Button sendButton = this.FindViewById<Button>(SmartBasket.Resource.Id.btn_login);
            sendButton.Click += async delegate
            {
                Toast.MakeText(this, "Login Succeeded", ToastLength.Short).Show();
                var m_activity = new Intent(this, typeof(ToDoActivity));
                this.StartActivity(m_activity);

                // Toast.MakeText(this, input.Text.ToString(), ToastLength.Short).Show();
                

            };
        }
    }
}