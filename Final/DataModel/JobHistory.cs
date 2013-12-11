
namespace FieldEngineer.DataModel
{
    /// <summary>
    /// This class represents a history of all the actions taken for a job. The history is represented as a
    /// list of actions taken at a certain time.
    /// </summary>
    public partial class JobHistoryItem
    {
        /// <summary>
        /// Represents the engineer who performed the action.
        /// </summary>
        public string ActionBy { get; set; }

        /// <summary>
        /// Represents the timestamp when action was performed.
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// Represents the comments regarding the action.
        /// </summary>
        public string Comments { get; set; }
    }

}
