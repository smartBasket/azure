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

        // Client reference.
        private MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<ToDoItem> todoTable;

        const string localDbFilename = "localstore.db";
#else
        
        private IMobileServiceTable<User> userTable;
#endif

        // Adapter to map the items list to the view
        //private ToDoItemAdapter adapter;

        // EditText containing the "New ToDo" text
        //private EditText textNewToDo;

        // URL of the mobile app backend.
        
        const string applicationURL = @"https://smartbasketarduino.azurewebsites.net";
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(SmartBasket.Resource.Layout.Login);
            // Create the client instance, using the mobile app backend URL.
               client = new MobileServiceClient(applicationURL);
   #if OFFLINE_SYNC_ENABLED
               await InitLocalStoreAsync();

               // Get the sync table instance to use to store TodoItem rows.
               todoTable = client.GetSyncTable<ToDoItem>();
   #else

               userTable = client.GetTable<User>();
   #endif
               CurrentPlatform.Init();
               EditText input = this.FindViewById<EditText>(SmartBasket.Resource.Id.input_id);
               Button sendButton = this.FindViewById<Button>(SmartBasket.Resource.Id.btn_login);
               sendButton.Click += async delegate
               {
                   Toast.MakeText(this, "Login Succeeded", ToastLength.Short).Show();
                   var m_activity = new Intent(this, typeof(ToDoActivity));
                   this.StartActivity(m_activity);

                   // Toast.MakeText(this, input.Text.ToString(), ToastLength.Short).Show();
                   var usr = new User();
                   usr.userId = "123";
                   usr.password = "123";
                   try
                   {
                       // Insert the new item into the local store.
                       await userTable.InsertAsync(usr);

                   }
                   catch (Exception e)
                   {
                       //CreateAndShowDialog(e, "Error");
                   }

               };
           }
           [Java.Interop.Export()]
           public async void AddUsr(View view)
           {
               if (client == null )
               {
                   //    return;
               }


               // Create a new item
               var usr = new User();
               usr.userId = "123";
               usr.password = "123";
               try
               {
                   // Insert the new item into the local store.
                   await userTable.InsertAsync(usr);
   #if OFFLINE_SYNC_ENABLED
                   // Send changes to the mobile app backend.
                   await SyncAsync();
   #endif


               }
               catch (Exception e)
               {
                   //CreateAndShowDialog(e, "Error");
               }

               //textNewToDo.Text = "";
           }
        
    }
}