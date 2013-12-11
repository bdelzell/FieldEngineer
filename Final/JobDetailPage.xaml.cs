using FieldEngineer.Common;
using FieldEngineer.DataModel;
using FieldEngineer.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
            // Custom
            // Fetch the job object and assign the values to be bound to the various controls
            job = await JobDataSource.GetDetailsAsync(e.NavigationParameter.ToString());
            this.DefaultViewModel["JobStockItems"] = await EquipmentDataSource.GetListAsync(job.EquipmentIds);
            this.DefaultViewModel["JobHistory"] = job.JobHistory;
            this.DefaultViewModel["JobSummaryItems"] = new ObservableCollection<Job>(new List<Job> { job });
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
        /// <see cref="FieldEngineer.Common.NavigationHelper.LoadState"/>
        /// and <see cref="FieldEngineer.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Call the corresponding method on navigation helper
            navigationHelper.OnNavigatedTo(e);

            //Get instance of DataTransfer Manager and add handler for DataRequested event
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataRequested;            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //Call the corresponding method on navigation helper
            navigationHelper.OnNavigatedFrom(e);

            //Get instance of DataTransfer Manager and remove handler for DataRequested event
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= DataRequested;
        }

        #endregion
              
        #region Feature - Share Charm
        /// <summary>
        /// This method contains the logic to prepare the data which would be shared.
        /// The Job Details would be prepared in the form of a table which can then be shared through email
        /// </summary>
        /// <param name="request"></param>
        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;            
            request.Data.Properties.Title = "Job Details for " + job.JobNumber + " - " + job.JobId;
            request.Data.Properties.Description = job.Title;

            //Create table format using the Job details
            StringBuilder htmlcontent = new StringBuilder("<p> <Table style=\"font-family:Segoe UI;\"");
            htmlcontent.Append("<tr> <td> <b> Job Number: </b>" + job.JobNumber + "  </td> </tr> ");
            htmlcontent.Append("<tr> <td> <b> Job ID: </b>" + job.JobId + " </td> </tr> ");
            htmlcontent.Append("<tr> <td> <b> Title: </b>" + job.Title + " </td> </tr> ");
            htmlcontent.Append("<tr> <td> <b> Scheduled For: </b>" + job.StartTime + " - " + job.EndTime +
                                " </td> </tr> ");
            htmlcontent.Append("<tr> <td> <b> Job Status: </b>" + job.Status + "</td> </tr> <tr> <td> </td> </tr>");
            htmlcontent.Append("<tr> <td> <b> Customer Address: </b> </td> </tr>");
            htmlcontent.Append("<tr> <td> " + job.Customer.Prefix + " " + job.Customer.FirstName + " " +
                                job.Customer.Surname + " </td> </tr>");
            htmlcontent.Append("<tr> <td> " + job.Customer.HouseNumberOrName + ", " + job.Customer.Street +
                                " </td> </tr>");
            htmlcontent.Append("<tr> <td> " + job.Customer.Town + " </td> </tr>");
            htmlcontent.Append("<tr> <td> " + job.Customer.County + " </td> </tr>");
            htmlcontent.Append("<tr> <td> " + job.Customer.Postcode + " </td> </tr> <tr> <td> </td> </tr>");
            htmlcontent.Append("<tr> <td> <b> Customer Contact: </b> </td> </tr>");
            htmlcontent.Append("<tr> <td> " + job.Customer.PrimaryContactNumber + " </td> </tr>");
            htmlcontent.Append("<tr> <td> " + job.Customer.AdditionalContactNumber + " </td> </tr>");
            htmlcontent.Append("</table> </p>");
            request.Data.SetHtmlFormat(HtmlFormatHelper.CreateHtmlFormat(htmlcontent.ToString()));
           
        }

        #endregion

        #region Feature - App Bar
        /// <summary>
        /// Handles the App Bar event - Pin to Start. This event is used to pin the current Job as a secondary tile 
        /// on the Start screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void PinToStart_Click(object sender, RoutedEventArgs e)
        {
            await SecondaryTileCreation(sender);
        }

        #endregion

        #region Feature - Secondary Tile

        /// <summary>
        /// This method checks if a secondary tile is already present for this job. If it is already present, 
        /// the job is unpinned, if not the job is pinned.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task SecondaryTileCreation(object sender)
        {
            string appbarTileId = job.JobId;
            if (!SecondaryTile.Exists(appbarTileId))
            {
                await PinSecondaryTile(sender, appbarTileId);
            }
            else
            {
                await UnpinSecondaryTile(sender, appbarTileId);
            }
        }

        /// <summary>
        /// This method pins the secondary tile. The secondary tile is created using the required parameters
        /// and pinned. The user is showna  message informing whether the tile is pinned successfully
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="appbarTileId">The appbar tile id.</param>
        /// <returns></returns>
        private async Task PinSecondaryTile(object sender, string appbarTileId)
        {
            // Prepare package images for use as the Tile Logo in our tile to be pinned
            Uri smallLogo = new Uri("ms-appx:///Assets/Tile_150X150.png");
            Uri wideLogo = new Uri("ms-appx:///Assets/Tile_310X150.png");

            // Create a Secondary tile
            string tileActivationArguments = appbarTileId;
            string subTitle = job.JobNumber + " - " + job.JobId;
            //Tile ID is set to current job Id. when user clicks on seconday job tile, the same  will be passed as LaunchActivatedEventArgs to 
            //OnLaunched method of App.XAML.cs
            SecondaryTile secondaryTile = new SecondaryTile(appbarTileId, "Field Engineer - Job " + job.JobNumber, subTitle,
                                                            tileActivationArguments,
                                                            TileOptions.ShowNameOnLogo | TileOptions.ShowNameOnWideLogo,
                                                            smallLogo, wideLogo);

            secondaryTile.ForegroundText = ForegroundText.Light;
            bool isPinned =
                await
                secondaryTile.RequestCreateForSelectionAsync(GetElementRect((FrameworkElement)sender),
                                                             Windows.UI.Popups.Placement.Above);

            if (isPinned)
            {
                MessageDialog dialog =
                    new MessageDialog("Job " + job.JobNumber + " (" + job.Title + ") successfully pinned.");
                await dialog.ShowAsync();
            }
            else
            {
                MessageDialog dialog = new MessageDialog("Job " + job.JobNumber + " (" + job.Title + ") not pinned.");
                await dialog.ShowAsync();
            }

            ToggleAppBarButton(!isPinned, sender as AppBarButton);
        }

        /// <summary>
        /// This method unpins the existing secondary tile. 
        /// The user is showna  message informing whether the tile is unpinned successfully
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="appbarTileId">The appbar tile id.</param>
        /// <returns></returns>
        private async Task UnpinSecondaryTile(object sender, string appbarTileId)
        {
            SecondaryTile secondaryTile = new SecondaryTile(appbarTileId);
            bool isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(GetElementRect((FrameworkElement)sender),
                                                                                 Windows.UI.Popups.Placement.Above);

            if (isUnpinned)
            {
                MessageDialog dialog =
                    new MessageDialog("Job " + job.JobNumber + " (" + job.Title + ") successfully unpinned.");
                await dialog.ShowAsync();
            }
            else
            {
                MessageDialog dialog = new MessageDialog("Job " + job.JobNumber + " (" + job.Title + ") not unpinned.");
                await dialog.ShowAsync();
            }

            ToggleAppBarButton(isUnpinned, sender as AppBarButton);
        }

        /// <summary>
        /// This method is called when the PinToStart button has completed loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PinToStart_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleAppBarButton(!SecondaryTile.Exists(job.JobId), sender as AppBarButton);
        }

        /// <summary>
        /// This method assigns the style to the app bar button.
        /// </summary>
        /// <param name="showPinButton">if set to <c>true</c> [show pin button].</param>
        private void ToggleAppBarButton(bool showPinButton, AppBarButton pinToStart)
        {
            if (pinToStart != null)
            {
                pinToStart.Icon = (showPinButton) ? new SymbolIcon(Symbol.Pin) :
                    new SymbolIcon(Symbol.UnPin);
                pinToStart.Label = (showPinButton) ? "Pin To Start" : "Unpin";
            }
        }
            
        /// <summary>
        /// This method creates the placeholder for the secondary tile and shows it above the 'Pin' button.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }



        #endregion


        
    }
}
