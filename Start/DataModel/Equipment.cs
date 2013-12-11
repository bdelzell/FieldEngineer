using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldEngineer.DataModel
{
    public class Equipment
    {
        public string EquipmentId { get; set; }

        public string EquipmentNumber { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ThumbImage { get; set; }

        public string FullImage { get; set; }

        public EquipmentSpecification[] EquipmentSpecifications { get; set; }
    }
}
