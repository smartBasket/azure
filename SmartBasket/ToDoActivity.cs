/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=717898
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using SmartBasket;
using ZXing.Mobile;
using Android.Content;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
#endif

namespace ArduinoSmartBasket
{
    /* MainLauncher = true was removed by salma 
     *        (MainLauncher = true,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/AppTheme")
    */
    [Activity(Label = "ToDoActivity")]
    public class ToDoActivity : Activity
    {

        // Client reference.
        private MobileServiceClient client;
        public static int sum = 0;

#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<ToDoItem> todoTable;

        const string localDbFilename = "localstore.db";
#else
        private IMobileServiceTable<ToDoItem> todoTable;
        private IMobileServiceTable<Item> itemTable;
       // private IMobileServiceTable<User> userTable;
#endif

        // Adapter to map the items list to the view
        private ToDoItemAdapter adapter;

        // EditText containing the "New ToDo" text
        private EditText textNewToDo;

        // URL of the mobile app backend.
       
        const string applicationURL = @"https://smartbasketarduino.azurewebsites.net";
        Button buttonDoScan;
        Button buttonBluetooth;
        MobileBarcodeScanner scanner;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            scanner = new MobileBarcodeScanner(this);

            //removed 
            // Set our view from the "main" layout resource
            // SetContentView(SmartBasket.Resource.Layout.Activity_To_Do);
            // CurrentPlatform.Init();
            //edit
            SetContentView(SmartBasket.Resource.Layout.Activity_To_Do);
            //done 

            // Create the client instance, using the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);
#if OFFLINE_SYNC_ENABLED
            await InitLocalStoreAsync();

            // Get the sync table instance to use to store TodoItem rows.
            todoTable = client.GetSyncTable<ToDoItem>();
#else
            todoTable = client.GetTable<ToDoItem>();
            itemTable = client.GetTable<Item>();
#endif
            TextView sumView = FindViewById<TextView>(SmartBasket.Resource.Id.sum);
            sumView.Text = "Total: " + sum;
            textNewToDo = FindViewById<EditText>(SmartBasket.Resource.Id.textNewToDo);

            // Create an adapter to bind the items with the view
            adapter = new ToDoItemAdapter(this, SmartBasket.Resource.Layout.Row_List_To_Do);
            var listViewToDo = FindViewById<ListView>(SmartBasket.Resource.Id.listViewToDo);
            listViewToDo.Adapter = adapter;

            // Load the items from the mobile app backend.
            OnRefreshItemsSelected();

    /*
            buttonDoScan = this.FindViewById<Button>(SmartBasket.Resource.Id.scanner);
            buttonDoScan.Click += async delegate
            {
                scanner.UseCustomOverlay = false;
                scanner.TopText = "Scanning for barcode";
                var result = await scanner.Scan();
                DisplayResult(result);
            };
            */

            buttonBluetooth = this.FindViewById<Button>(SmartBasket.Resource.Id.bluetooth);
            buttonBluetooth.Click += async delegate
            {
                scanner.UseCustomOverlay = false;
                scanner.TopText = "moving to new activity";
                var m_activity =  new Intent(this, typeof(BluetoothA));



                this.StartActivity(m_activity);
            };


           

        }
        void DisplayResult(ZXing.Result result)
        {
            string message = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
                message = "Barcode: " + result.Text;
            else
                message = "Could not scan.";

            this.RunOnUiThread(() => Toast.MakeText(this, message, ToastLength.Short).Show());
        }

#if OFFLINE_SYNC_ENABLED
        private async Task InitLocalStoreAsync()
        {
            var store = new MobileServiceSQLiteStore(localDbFilename);
            store.DefineTable<ToDoItem>();

            // Uses the default conflict handler, which fails on conflict
            // To use a different conflict handler, pass a parameter to InitializeAsync.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=521416
            await client.SyncContext.InitializeAsync(store);
        }

        private async Task SyncAsync(bool pullData = false)
        {
            try {
                await client.SyncContext.PushAsync();

                if (pullData) {
                    await todoTable.PullAsync("allTodoItems", todoTable.CreateQuery()); // query ID is used for incremental sync
                }
            }
            catch (Java.Net.MalformedURLException) {
                CreateAndShowDialog(new Exception("There was an error creating the Mobile Service. Verify the URL"), "Error");
            }
            catch (Exception e) {
                CreateAndShowDialog(e, "Error");
            }
        }
#endif

        //Initializes the activity menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(SmartBasket.Resource.Menu.activity_main, menu);
            return true;
        }

        //Select an option from the menu
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == SmartBasket.Resource.Id.menu_refresh)
            {
                item.SetEnabled(false);

                OnRefreshItemsSelected();

                item.SetEnabled(true);
            }
            return true;
        }

        // Called when the refresh menu option is selected.
        private async void OnRefreshItemsSelected()
        {
#if OFFLINE_SYNC_ENABLED
			// Get changes from the mobile app backend.
            await SyncAsync(pullData: true);
#endif
            // refresh view using local store.
            await RefreshItemsFromTableAsync();
        }

        //Refresh the list with the items in the local store.
        private async Task RefreshItemsFromTableAsync()
        {
            try
            {
                // Get the items that weren't marked as completed and add them in the adapter
                var list = await todoTable.Where(item => item.Complete == false).ToListAsync();

                adapter.Clear();

                foreach (ToDoItem current in list)
                    adapter.Add(current);

            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        public async Task CheckItem(ToDoItem item)
        {
            if (client == null)
            {
                return;
            }

            TextView sumView = FindViewById<TextView>(SmartBasket.Resource.Id.sum);
            sumView.Text = "Total: " + sum;

            // Set the item as completed and update it in the table
            item.Complete = true;
            try
            {
                // Update the new item in the local store.
                //await todoTable.UpdateAsync(item);
#if OFFLINE_SYNC_ENABLED
                // Send changes to the mobile app backend.
				await SyncAsync();
#endif

                if (item.Complete)
                    adapter.Remove(item);

            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        [Java.Interop.Export()]
        public async void AddItem(View view)
        {
            if (client == null || string.IsNullOrWhiteSpace(textNewToDo.Text))
            {
            //    return;
            }
            
            scanner.UseCustomOverlay = false;
            scanner.TopText = "Scanning for barcode";
            var result = await scanner.Scan();
            textNewToDo.Text = result.Text;
           System.Collections.Generic.List<Item> list=null;

           

            try
            {
               
                list = await itemTable.Where(it => (it.itemId) == (result.Text)).ToListAsync();
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }

            Item item2 = null;
            foreach (Item current in list)
            {
                item2 = current;
               
            }
            // Create a new item
            var itemmm = new ToDoItem
            {
                Text = item2.name + " " + item2.price + "NIS",
                price=item2.price,
                Complete = false
            };
            sum += item2.price;
            TextView sumView = FindViewById<TextView>(SmartBasket.Resource.Id.sum);
            sumView.Text = "Total: " + sum;
            try
            {
                // Insert the new item into the local store.
              //  await todoTable.InsertAsync(item);
#if OFFLINE_SYNC_ENABLED
                // Send changes to the mobile app backend.
				await SyncAsync();
#endif

                if (!itemmm.Complete)
                {
                    adapter.Add(itemmm);
                }
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }

            textNewToDo.Text = "";
        }

        private void CreateAndShowDialog(Exception exception, String title)
        {
            CreateAndShowDialog(exception.Message, title);
        }

        private void CreateAndShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
    }
}


