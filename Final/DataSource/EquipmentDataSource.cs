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
        /// This method is used to populate all the Equipments from the file Equipments.xml. 
        /// It reads the xml file and serializes the data into a list of Equipments.
        /// </summary>
        private async Task ReadXmlDataFromLocalStorageAsync()
        {
            try
            {
                //If eqipmenets list already loaded from XML file, skip reloading it again. 
                if (_equipmentDataSource.AllEquipments != null)
                    return;

                var dataFolder = await Package.Current.InstalledLocation.GetFolderAsync(Constants.DataFilesFolder);
                StorageFile sessionFile = await dataFolder.GetFileAsync(Constants.EqipmentsFile);

                using (
                    IRandomAccessStreamWithContentType sessionInputStream =
                        await sessionFile.OpenReadAsync())
                {

                    var sessionSerializer = new DataContractSerializer(typeof(List<Equipment>));
                    var restoredData = sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());
                    _allEquipments = (List<Equipment>)restoredData;
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
        public static async Task<Equipment> GetDetailsAsync(string EquipmentId)
        {
            await _equipmentDataSource.ReadXmlDataFromLocalStorageAsync();
            var matches = _equipmentDataSource.AllEquipments.Where((item) => item.EquipmentId.Equals(EquipmentId));
            if (matches != null && matches.Count() == 1) return matches.First();
            return null;
        }

        /// <summary>
        /// This method gets all the Equipments using the equipement ids supplied.
        /// </summary>
        /// <returns>List of Equipment objects</returns>
        public static async Task<List<Equipment>> GetListAsync(List<string> equipmentIds)
        {
            await _equipmentDataSource.ReadXmlDataFromLocalStorageAsync();
            var matches = _equipmentDataSource.AllEquipments.Where((item) => equipmentIds.Contains(item.EquipmentId)).ToList();
            return matches;
        }
    }
}
