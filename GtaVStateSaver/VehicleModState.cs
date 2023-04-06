using GTA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaVStateSaver
{
    public class VehicleModState
    {
        public VehicleModType Type = VehicleModType.Armor;
        public bool Variation = false;
        public int Index = 0;

        public VehicleModState() { }

        public void Write(BinaryWriter writer)
        {
            writer.Write((int)Type);
            writer.Write(Variation);
            writer.Write(Index);
        }

        public void Read(BinaryReader reader)
        {
            Type = (VehicleModType)reader.ReadInt32();
            Variation = reader.ReadBoolean();
            Index = reader.ReadInt32();
        }
    }
}
