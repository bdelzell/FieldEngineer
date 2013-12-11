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
    public sealed class EquipmentDataSource
    {

        private static EquipmentDataSource _equipmentDataSource = new EquipmentDataSource();

        private List<Equipment> _allEquipments = null;

        public List<Equipment> AllEquipments
        {
            get { return this._allEquipments; }

        }

        /// <summary>
        /// This method is used to create dummy equipments list.
        /// </summary>
        private async Task PrepareDummyData()
        {
            try
            {
                //If job list already loaded, skip generating it again. 
                if (_equipmentDataSource.AllEquipments != null)
                    return;

                _allEquipments = new List<Equipment>();
                for (int i = 0; i < 7; i++)
                {
                    Equipment equipment = new Equipment()
                    {
                        Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                        EquipmentId = i.ToString(),
                        EquipmentNumber = "ABC123" + i.ToString(),
                        FullImage = "/Data/EquipmentImages/placeholder.jpg",
                        Name = "Equipment "+ i.ToString(),
                        ThumbImage = "/Data/EquipmentImages/placeholder.jpg"
                    };

                    List<EquipmentSpecification> equipmentSpecifications = new List<EquipmentSpecification>();
                    for (int j = 0; j < 7; j++)
                    {
                        equipmentSpecifications.Add(new EquipmentSpecification()
                        {
                            Name = "Specification " + j.ToString(),
                            Value = "Value " + j.ToString()
                        });
                    }
                    equipment.EquipmentSpecifications = equipmentSpecifications.ToArray();

                    _allEquipments.Add(equipment);

                }
            }
            catch (Exception ex)
            {
                _allEquipments = null;
            }
        }

        /// <summary>
        /// This method returns the complete details about a Equipment using the Equipment ID.
        /// </summary>
        /// <param name="EquipmentId">The Equipment id.</param>
        /// <returns>Equipment object</returns>
        public static async Task<Equipment> GetDetailsAsync_Dummy(string EquipmentId)
        {
            await _equipmentDataSource.PrepareDummyData();
            var matches = _equipmentDataSource.AllEquipments.Where((item) => item.EquipmentId.Equals(EquipmentId));
            if (matches != null && matches.Count() == 1) return matches.First();
            return null;
        }

        /// <summary>
        /// This method gets all the Equipments using the equipement ids supplied.
        /// </summary>
        /// <returns>List of Equipment objects</returns>
        public static async Task<List<Equipment>> GetListAsync_Dummy(List<string> equipmentIds)
        {
            await _equipmentDataSource.PrepareDummyData();
            var matches = _equipmentDataSource.AllEquipments.Where((item) => equipmentIds.Contains(item.EquipmentId)).ToList();
            return matches;
        }

    }
}
