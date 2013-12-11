using FieldEngineer.Common;
using FieldEngineer.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace FieldEngineer.DataSource
{
    /// <summary> 
    /// This class acts as a facade for fetching data. It contains all the methods used to fetch data from
    /// the xml files, filter them as required and return the data to the xaml pages.
    /// </summary>
    public sealed class JobDataSource
    {
        private static JobDataSource _jobDataSource = new JobDataSource();

        private List<Job> _alljobs = null;

        public List<Job> AllJobs
        {
            get { return this._alljobs; }

        }

        /// <summary>
        /// This method is used to populate all the jobs from the file Jobs.xml. 
        /// It reads the xml file and serializes the data into a list of jobs.
        /// </summary>
        private async Task ReadXmlDataFromLocalStorageAsync()
        {
            try
            {
                //If job list already loaded from XML file, skip reloading it again. 
                if (_jobDataSource.AllJobs != null)
                    return;

                var dataFolder = await Package.Current.InstalledLocation.GetFolderAsync(Constants.DataFilesFolder);
                StorageFile sessionFile = await dataFolder.GetFileAsync(Constants.FieldEngineerFile);

                using (
                    IRandomAccessStreamWithContentType sessionInputStream =
                       await sessionFile.OpenReadAsync())
                {

                    var sessionSerializer = new DataContractSerializer(typeof(List<Job>));
                    var restoredData = sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());
                    _alljobs = (List<Job>)restoredData;
                }

            }
            catch (Exception ex)
            {
                _alljobs = null;
            }
        }

        /// <summary>
        /// This method returns the complete details about a job using the Job ID.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns>Job object</returns>
        public static async Task<Job> GetDetailsAsync(string jobId)
        {
            await _jobDataSource.ReadXmlDataFromLocalStorageAsync();
            var matches = _jobDataSource.AllJobs.Where((item) => item.JobId.Equals(jobId));
            if (matches != null && matches.Count() == 1) return matches.First();
            return null;
        }

        /// <summary>
        /// This method gets all the jobs which are still pending (jobs with status 'Not Started').
        /// </summary>
        /// <returns>List of Pending Jobs</returns>
        public static async Task<List<Job>> GetPendingJobsAsync()
        {
            await _jobDataSource.ReadXmlDataFromLocalStorageAsync();
            var matches = _jobDataSource.AllJobs.Where((item) => item.Status == Constants.PendingStatus).ToList();
            return matches;
        }

        /// <summary>
        /// This method groups all the jobs by the Job Status (On-Site/ Completed/ Not Started) 
        /// and returns the groups.
        /// </summary>
        /// <returns>Groups of Jobs grouped by Status</returns>
        public static async Task<List<JobGroup>> GetJobGroupsAsync()
        {
            await _jobDataSource.ReadXmlDataFromLocalStorageAsync();
            var groupCollection = new List<JobGroup>
                {
                    new JobGroup()
                        {
                            Title = Constants.CurrentStatus,
                            Jobs = _jobDataSource.AllJobs.Where(job => job.Status.Equals(Constants.CurrentStatus)).ToList()
                        },
                    new JobGroup()
                        {
                            Title = Constants.PendingStatus,
                            Jobs = _jobDataSource.AllJobs.Where(job => job.Status.Equals(Constants.PendingStatus)).ToList()
                        },
                    new JobGroup()
                        {
                            Title = Constants.CompletedStatus,
                            Jobs = _jobDataSource.AllJobs.Where(job => job.Status.Equals(Constants.CompletedStatus)).ToList()
                        }
                };

            return groupCollection;
        }

        /// <summary>
        /// This method searches the jobs by search text. The search text can be a part of the 
        /// Job ID, Job Title or Customer Name
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>Search results</returns>
        public static async Task<List<Job>> SearchJobsBySearchTextAsync(string searchText)
        {
            await _jobDataSource.ReadXmlDataFromLocalStorageAsync();
            var filteredList =
                _jobDataSource.AllJobs.Where(
                    item =>
                    item.JobId.Contains(searchText) || item.Title.ToUpper().Contains(searchText.ToUpper()) ||
                    item.Customer.Surname.ToUpper().Contains(searchText.ToUpper()) ||
                    item.Customer.FirstName.ToUpper().Contains(searchText.ToUpper()));
            return filteredList.ToList();
        }
    }
}
