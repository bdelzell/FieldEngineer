
namespace FieldEngineer.DataModel
{

    /// <summary>
    /// This class represents a Customer and contains all properties related to a Customer entity.
    /// </summary>
    public partial class Customer
    {
        /// <summary>
        /// Represents a unique ID for each customer.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Represents the prefix (Mr/ Miss/ Ms) for the customer.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Represents the first name of a customer.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Represents the last name of a customer.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Represnts the house number or house name of a customer.
        /// </summary>
        public string HouseNumberOrName { get; set; }

        /// <summary>
        /// Represents the street name in the customer's address.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Represents the town name in the customer's address.
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// Represents the county name in the customer's address.
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Represents the postcode in the customer's address.
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// Represents the primary contact number of the customer.
        /// </summary>
        public string PrimaryContactNumber { get; set; }

        /// <summary>
        /// Represents the additional contact number of the customer.
        /// </summary>
        public string AdditionalContactNumber { get; set; }
    }

}
