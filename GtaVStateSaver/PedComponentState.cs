using GTA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaVStateSaver
{
    public class PedComponentState
    {
        public PedComponentType Type;
        public int Index = 0;
        public int TextureIndex = 0;

        public PedComponentState() { }
        public PedComponentState(PedComponent pedComponent)
        {
            Type = pedComponent.Type;
            Index = pedComponent.Index;
            TextureIndex = pedComponent.TextureIndex;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((int)Type);
            writer.Write(Index);
            writer.Write(TextureIndex);
        }

        public void Read(BinaryReader reader)
        {
            Type = (PedComponentType)reader.ReadInt32();
            Index = reader.ReadInt32();
            TextureIndex = reader.ReadInt32();
        }
    }
}
