using GTA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaVStateSaver
{
    public class WeaponState
    {
        public WeaponHash Hash = WeaponHash.Pistol;
        public int Ammo = 0;
        public int AmmoInClip = 0;
        public bool IsCurrentWeapon = false;

        public WeaponState() { }
        public WeaponState(Weapon weapon)
        {
            Hash = weapon.Hash;
            Ammo = weapon.Ammo;
            AmmoInClip = weapon.AmmoInClip;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((uint)Hash);
            writer.Write(Ammo);
            writer.Write(AmmoInClip);
            writer.Write(IsCurrentWeapon);
        }

        public void Read(BinaryReader reader)
        {
            Hash = (WeaponHash)reader.ReadUInt32();
            Ammo = reader.ReadInt32();
            AmmoInClip = reader.ReadInt32();
            IsCurrentWeapon = reader.ReadBoolean();
        }
    }
}
