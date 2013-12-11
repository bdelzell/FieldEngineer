
using System.Collections.Generic;
namespace FieldEngineer.DataModel
{

    /// <summary>
    /// This class represents a Job card and contains all properties related to a Job entity.
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Represents the customer for whim the job has been created.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Represents the job history or the actions taken on the job so far.
        /// </summary>
        public List<JobHistoryItem> JobHistory { get; set; }

        /// <summary>
        /// Represents the job number used to indicate its position in the list of jobs.
        /// </summary>
        public string JobNumber { get; set; }

        /// <summary>
        /// Represents an unique ID used to distinguish each job.
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Represents the starting time of the job.
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Represents the ending time of the job.
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Represents whether an estimated time of arrival has been submitted for the engineer working on the job.
        /// </summary>
        public string EtaSubmitted { get; set; }

        /// <summary>
        /// Represents the estimated time of arrival for the engineer working on the job.
        /// </summary>
        public string EtaTime { get; set; }

        /// <summary>
        /// Represents the status (On Site/ Completed/ Not Started) of the job.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Represents the job title as a one-line description of the job.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Represents equipmenents involved in this job.
        /// </summary>
        public List<string> EquipmentIds { get; set; }

    }

}
