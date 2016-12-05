using Prism.Windows.Mvvm;

namespace StateTriggersSample.Views
{
    public sealed partial class MainPage : SessionStateAwarePage
    {
        public ViewModels.MainPageViewModel ViewModel => (ViewModels.MainPageViewModel)DataContext;

        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}
