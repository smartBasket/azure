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
               EditText pass = this.FindViewById<EditText>(SmartBasket.Resource.Id.input_password);
               Button sendButton = this.FindViewById<Button>(SmartBasket.Resource.Id.btn_login);
               var m_activity=new Intent(this, typeof(ToDoActivity));
            sendButton.Click += async delegate
               {
                   var list = await userTable.Where(user => user.userId == (input.Text)).ToListAsync();

                   int i = 0;
                   foreach (User current in list)
                   {
                       if (current.password.Equals(pass.Text))
                       {
                           Toast.MakeText(this, "Login Succeeded", ToastLength.Short).Show();
                           m_activity = new Intent(this, typeof(ToDoActivity));
                           this.StartActivity(m_activity);
                       }
                       else
                       {
                           Toast.MakeText(this, "Login Failed", ToastLength.Short).Show();

                       }
                       i++;
                   }
                   if (i == 0)
                   {
                       Toast.MakeText(this, "Login Failed", ToastLength.Short).Show();
                   }

                   // Toast.MakeText(this, input.Text.ToString(), ToastLength.Short).Show();
                   /*
                   var usr1 = new User();
                   usr1.userId = "1234";
                   usr1.password = "1234";
                   */
                   try
                   {
                       // Insert the new item into the local store.
                       //await userTable.InsertAsync(usr1);
 

                   }
                   catch (Exception e)
                   {
                       //CreateAndShowDialog(e, "Error");
                   }

               };
           }
                  
    }
}