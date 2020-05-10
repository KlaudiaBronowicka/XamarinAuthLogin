using System;
using AuthLogin.Models;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using Xamarin.Auth.XamarinForms;
using Xamarin.Forms;

namespace AuthLogin.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        void LinkedIn_Button_Clicked(object sender, EventArgs e)
        {
            var authenticator = new OAuth2Authenticator(
                clientId: "772sgat14hsdye",
                clientSecret: "vWD595X5OBRZhF0N",
                scope: "r_liteprofile",
                authorizeUrl: new Uri("https://www.linkedin.com/uas/oauth2/authorization"),
                redirectUrl: new Uri("https://indiespring.com/"),
                accessTokenUrl: new Uri("https://www.linkedin.com/uas/oauth2/accessToken"),
                null,
                false);

            var page = new AuthenticatorPage(authenticator);

            authenticator.Completed += OnLinkedInAuthenticationComplete;

            Application.Current.MainPage.Navigation.PushModalAsync(page);
        }

        private async void OnLinkedInAuthenticationComplete(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (!e.IsAuthenticated)
            {
                //Show error message to the user

                return;
            }
            try
            {
                // request user data from LinkedIn

                var request = new OAuth2Request(
                    "GET",
                    new Uri("https://api.linkedin.com/v2/me?projection=(id,localizedFirstName,localizedLastName,profilePicture(displayImage~:playableStreams))"),
                    null,
                    e.Account);

                request.AccessTokenParameterName = "oauth2_access_token";

                var linkedInResponse = await request.GetResponseAsync();

                var json = linkedInResponse.GetResponseText();

                var user = GetUserFromLinkedInJson(json);

                if (user == null) return;

                ContinueToNextPage(user);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        private User GetUserFromLinkedInJson(string json)
        {
            try
            {
                var userData = JObject.Parse(json);

                var firstName = userData["localizedFirstName"];
                var lastName = userData["localizedLastName"];
                var id = userData["id"];

                var imageData = userData["profilePicture"];
                var displayImageData = imageData["displayImage~"];
                var elements = displayImageData["elements"];
                var image = elements.Last.Last.First[0];
                var url = image["identifier"];

                return new User
                {
                    Id = id.ToString(),
                    Name = $"{firstName} {lastName}",
                    ImageURL = url.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing LinkedIn data: {ex.Message}");

                return null;
            }
        }

        private void ContinueToNextPage(User user)
        {
            App.Current.MainPage = new NavigationPage(new ProfilePage(user)); 
        }
    }
}
