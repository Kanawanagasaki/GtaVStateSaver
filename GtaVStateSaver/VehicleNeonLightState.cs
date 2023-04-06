using GTA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaVStateSaver
{
    public class VehicleNeonLightState
    {
        public VehicleNeonLight NeonLight = VehicleNeonLight.Front;
        public bool HasNeonLigh = false;
        public bool IsOn = false;

        public VehicleNeonLightState() { }

        public void Write(BinaryWriter writer)
        {
            writer.Write((int)NeonLight);
            writer.Write(HasNeonLigh);
            writer.Write(IsOn);
        }

        public void Read(BinaryReader reader)
        {
            NeonLight = (VehicleNeonLight)reader.ReadInt32();
            HasNeonLigh = reader.ReadBoolean();
            IsOn = reader.ReadBoolean();
        }
    }
}
