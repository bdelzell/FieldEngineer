using FieldEngineer.Common;
using FieldEngineer.DataModel;
using FieldEngineer.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FieldEngineer
{
    /// <summary>
    /// Represents a class for the Job Details Page. 
    /// This page is the default page for the application and displays a list 
    /// of job cards grouped by job status i.e. Current, Pending, Completed.
    /// </summary>
    public sealed partial class JobDetailPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Job job;

        /// <summary>
        /// Represents the NavigationHelper instance for the page. It is used on 
        /// each page to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Represents the default View Model for the page. It is used for binding 
        /// data to XAML viem and can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public JobDetailPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // Fetch the job object and assign the values to be bound to the various controls
            job = await JobDataSource.GetDetailsAsync(e.NavigationParameter.ToString());            
            this.DefaultViewModel["JobHistory"] = job.JobHistory;
            this.DefaultViewModel["JobSummaryItems"] = new ObservableCollection<Job>(new List<Job> { job });
            this.DefaultViewModel["JobStockItems"] = await EquipmentDataSource.GetListAsync_Dummy(job.EquipmentIds);
        }
            
        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((Equipment)e.ClickedItem).EquipmentId;
            this.Frame.Navigate(typeof(EquipmentDetails), itemId);
        }

        /// <summary>
        /// Handles the back button click event. Takes the user to main job list page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(JobListPage), null);
        }


        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region Feature - Share

        #endregion

        #region Feature - App Bar
        
        #endregion

        #region Feature - Secondary Tile            

        #endregion
    }
}
