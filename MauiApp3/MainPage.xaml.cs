namespace MauiApp3
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnNavigateNewPage1ButtonClicked(object sender, EventArgs e)
        { 
            await Shell.Current.GoToAsync(nameof(NewPage1));
        }
        private async void OnNavigateNewPage2ButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(NewPage2));
        }
        private async void OnNavigateNewPage3ButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(NewPage3));
        }
        private async void OnNavigateNewPage4ButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(NewPage4));
        }

    }
}
