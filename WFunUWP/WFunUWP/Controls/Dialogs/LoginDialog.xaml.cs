using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WFunUWP.Controls.Dialogs
{
    public sealed partial class LoginDialog : ContentDialog
    {
        private string Mail;
        private string Link;

        public LoginDialog()
        {
            InitializeComponent();
            ResourceLoader loader = ResourceLoader.GetForCurrentView("LoginDialog");
            InfoBar.Title = loader.GetString("FirstStep");
            Closing += OnClosing;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as FrameworkElement).Tag.ToString())
            {
                case "GetLink":
                    GetLink();
                    break;
            }
        }

        private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary)
            {
                UIHelper.ShowProgressBar();
                if (!Utils.AwaitByTaskCompleteSource(GetToken))
                {
                    args.Cancel = true;
                    ResourceLoader loader = ResourceLoader.GetForCurrentView("LoginDialog");
                    InfoBar.IsOpen = true;
                    InfoBar.Severity = InfoBarSeverity.Error;
                    InfoBar.Title = loader.GetString("BadAuthLink");
                }
                UIHelper.HideProgressBar();
            }
        }

        private async void GetLink()
        {
            UIHelper.ShowProgressBar();
            using (StringContent email = new StringContent($"{{\"email\":\"{Mail}\"}}"))
            {
                email.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                (bool isSucceed, JToken result) results = await RequestHelper.PostDataAsync(UriHelper.GetUri(UriType.Login), email);
                if (!results.isSucceed)
                {
                    if (results.result != null)
                    {
                        JObject token = (JObject)results.result;
                        if (token.TryGetValue("message", out JToken message))
                        {
                            InfoBar.IsOpen = true;
                            InfoBar.Severity = InfoBarSeverity.Error;
                            InfoBar.Title = message.ToString();
                        }
                    }
                    else
                    {
                        ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
                        InfoBar.IsOpen = true;
                        InfoBar.Severity = InfoBarSeverity.Error;
                        InfoBar.Title = loader.GetString("NetworkError");
                    }
                }
                else if (results.result != null)
                {
                    JObject token = (JObject)results.result;
                    if (token.TryGetValue("result", out JToken v1))
                    {
                        JObject result = (JObject)v1;
                        if (result.TryGetValue("username", out JToken username))
                        {
                            ResourceLoader loader = ResourceLoader.GetForCurrentView("LoginDialog");
                            InfoBar.IsOpen = true;
                            InfoBar.Severity = InfoBarSeverity.Success;
                            InfoBar.Title = string.Format(loader.GetString("WelcomeBack"), username.ToString());
                        }
                    }
                }
            }
            UIHelper.HideProgressBar();
        }

        private async Task<bool> GetToken()
        {
            Uri uri = Link.ValidateAndGetUri();
            if (uri == null) { return false; }
            HttpClientHandler clientHandler = new HttpClientHandler { AllowAutoRedirect = false };
            clientHandler.BeforeGetOrPost(NetworkHelper.GetWFunCookies(uri), uri);
            using (HttpClient Client = new HttpClient(clientHandler))
            {
                HttpResponseMessage response = await Client.GetAsync(uri);
                if (response.StatusCode == HttpStatusCode.Found)
                {
                    if (response.Headers.Contains("Set-Cookie"))
                    {
                        Regex authRegex = new Regex(@"auth=(.*?);");
                        string cookie = response.Headers.GetValues("Set-Cookie").Where((x) => x.Contains("auth")).FirstOrDefault();
                        string auth = authRegex.Match(cookie).Groups[1].Value;
                        return await SettingsHelper.LoginIn(auth);
                    }
                }
            }
            return false;
        }
    }
}
