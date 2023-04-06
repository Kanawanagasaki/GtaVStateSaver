using GTA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaVStateSaver
{
    public class VehicleDoorState
    {
        public VehicleDoorIndex Index = VehicleDoorIndex.Hood;
        public bool IsOpen = false;
        public bool IsBroken = false;
        public float AngleRatio = 0;

        public VehicleDoorState() { }
        public VehicleDoorState(VehicleDoor door)
        {
            Index = door.Index;
            IsOpen = door.IsOpen;
            IsBroken = door.IsBroken;
            AngleRatio = door.AngleRatio;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((int)Index);
            writer.Write(IsOpen);
            writer.Write(IsBroken);
            writer.Write(AngleRatio);
        }

        public void Read(BinaryReader reader)
        {
            Index = (VehicleDoorIndex)reader.ReadInt32();
            IsOpen = reader.ReadBoolean();
            IsBroken = reader.ReadBoolean();
            AngleRatio = reader.ReadSingle();
        }
    }
}
