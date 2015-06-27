using DemoSalesforceMobileSDK.Models;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Exceptions;
using Salesforce.SDK.Native;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoSalesforceMobileSDK.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : NativeMainPage
    {
        private ObservableCollection<JObject> _sobjects = new ObservableCollection<JObject>();
        public ObservableCollection<JObject> Sobjects
        {
            get { return _sobjects; }
        }


        private ObservableCollection<Contact> _contacts = new ObservableCollection<Contact>();

        public ObservableCollection<Contact> Contacts
        {
            get { return this._contacts; }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var color = new Config().LoginBackgroundColor;
            if (color != null)
            {
                grid.Background = new SolidColorBrush(color.Value);
            }
            Account account = AccountManager.GetAccount();
            if (account != null)
            {
                try
                {
                    account = await OAuth2.RefreshAuthToken(account);
                    _sobjects = await SendRequest("SELECT Id,FirstName,LastName FROM Contact");
                    contactList.DataContext = _sobjects;
                }
                catch (OAuthException ex)
                {
                    SDKManager.GlobalClientManager.Logout();
                }

            }
            else
            {
                base.OnNavigatedTo(e);
            }
        }

        private void SwitchAccount(object sender, RoutedEventArgs e)
        {
            AccountManager.SwitchAccount();
        }

        private async void Logout(object sender, RoutedEventArgs e)
        {

            if (SDKManager.GlobalClientManager != null)
            {
                await SDKManager.GlobalClientManager.Logout();
            }
            AccountManager.SwitchAccount();
        }

        private async Task<ObservableCollection<JObject>> SendRequest(string soql)
        {
            var restRequest = RestRequest.GetRequestForQuery(ApiVersionStrings.VersionNumber, soql);
            var client = SDKManager.GlobalClientManager.GetRestClient() ??
                         new RestClient(AccountManager.GetAccount().InstanceUrl);
            var response = await client.SendAsync(restRequest);
            if (!response.Success)
            {
                return null;
            }
            var records = response.AsJObject.GetValue("records").ToObject<JArray>();
            foreach (var item in records.ToList())
            {
                _sobjects.Add(item.ToObject<JObject>());
            }
            return _sobjects;
        }

        public async Task<bool> UpdateOnServer(String objectType, String objectId, Dictionary<String, Object> fields)
        {
            var request = RestRequest.GetRequestForUpdate(ApiVersionStrings.VersionNumber, objectType, objectId, fields);
            var client = SDKManager.GlobalClientManager.GetRestClient() ??
                        new RestClient(AccountManager.GetAccount().InstanceUrl);
            var response = await client.SendAsync(request);

            return response.Success;
        }

        private void contactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            popup.DataContext = e.AddedItems.First();
            popup.IsOpen = true;

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            JObject data = (JObject)popup.DataContext;
            Dictionary<String, Object> fields = new Dictionary<string, object>();
            fields.Add("FirstName", first.Text);
            fields.Add("LastName", last.Text);
            var id = data["Id"].ToString();
            var res = await UpdateOnServer("Contact",id , fields);
            popup.IsOpen = false;
        }

    }
}
