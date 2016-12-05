using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

namespace StateTriggersSample.ViewModels
{
    public class ListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MainPageViewModel : ViewModelBase
    {
        private INavigationService navigationService;
        private IDeviceGestureService deviceGestureService;
        private ISessionStateService sessionStateService;

        private string itemInfoText;
        public string ItemInfoText
        {
            get { return this.itemInfoText; }
            set { this.SetProperty(ref this.itemInfoText, value); }
        }

        private ObservableCollection<ListItem> itemsList;
        public ObservableCollection<ListItem> ItemsList
        {
            get { return this.itemsList; }
            set
            {
                this.SetProperty(ref this.itemsList, value);
                OnPropertyChanged("HasItemsListData");
            }
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return this.selectedIndex; }
            set
            {
                this.SetProperty(ref this.selectedIndex, value);
                OnPropertyChanged("IsItemsListSelected");
            }
        }

        public bool IsItemsListSelected
        {
            get { return this.SelectedIndex >= 0; }
        }

        public bool HasItemsListData
        {
            get { return this.ItemsList.Count > 0; }
        }

        public DelegateCommand SelectionChangedCommand { get; private set; }

        public MainPageViewModel(
            INavigationService navigationService,
            IDeviceGestureService deviceGestureService,
            ISessionStateService sessionStateService)
        {
            this.navigationService = navigationService;
            this.deviceGestureService = deviceGestureService;
            this.sessionStateService = sessionStateService;

            this.ItemInfoText = String.Empty;

            this.ItemsList = new ObservableCollection<ListItem>();
            for (int i = 0;i < 10;i++)
            {
                this.ItemsList.Add(new ListItem() { Id = i, Name = String.Format("くまー#{0}", i) });
            }
            this.SelectedIndex = -1;

            this.SelectionChangedCommand = new DelegateCommand(() => SelectionChanged());
        }

        private void SelectionChanged()
        {
            this.ItemInfoText = String.Format("くまー#{0}の説明文", this.SelectedIndex);
            return;
        }

        private void DeviceGestureService_GoBackRequested(object sender, DeviceGestureEventArgs e)
        {
            // FIXME: VisualStateGroupの状態を取得する良い方法が見つからないため、ViewModelで値を取得している
            var view = Windows.UI.ViewManagement.UIViewSettings.GetForCurrentView();
            if (view.UserInteractionMode == Windows.UI.ViewManagement.UserInteractionMode.Mouse)
            {
                // PCもしくはContinuumの場合、アプリを終了する
                e.Handled = false;
                e.Cancel = false;
            }
            else
            {
                // 電話で、アイテムが選択されている画面の場合、リスト画面へと戻る
                if (this.IsItemsListSelected)
                {
                    this.SelectedIndex = -1;
                    e.Handled = true;
                    e.Cancel = true;
                }
                else
                {
                    e.Handled = false;
                    e.Cancel = false;
                }
            }
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatingFrom(e, viewModelState, suspending);
            this.deviceGestureService.GoBackRequested -= DeviceGestureService_GoBackRequested;
            return;
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            this.deviceGestureService.GoBackRequested += DeviceGestureService_GoBackRequested;
            return;
        }
    }
}
