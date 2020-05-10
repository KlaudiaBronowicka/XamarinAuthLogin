using AuthLogin.Models;
using Xamarin.Forms;

namespace AuthLogin.Views
{
    public partial class ProfilePage : ContentPage
    {
        public User User { get; set; }

        public ProfilePage(User user)
        {
            InitializeComponent();

            User = user;
            BindingContext = User;
        }
    }

}
