using Galatasaray.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace Galatasaray
{
    public partial class MainPage : ContentPage
    {
        private readonly SensorViewModel vm;
        public MainPage(SensorViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            vm.OnDisappearing();
        }
    }
}
