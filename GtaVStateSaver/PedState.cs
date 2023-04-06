using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaVStateSaver
{
    public class PedState
    {
        public Ped Reference = null;

        public PedHash Model = PedHash.Jesus01;

        public Vector3 Position = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 RotationVelocity = Vector3.Zero;

        public bool IsInVehicle = false;
        public Guid VehicleId = Guid.Empty;
        public VehicleSeat VehicleSeat = 0;

        public int Accuracy = 0;
        public int Armor = 0;
        public bool CanBeTargetted = true;
        public bool CanFlyThroughWindscreen = true;
        public bool CanRagdoll = true;
        public bool CanSufferCriticalHits = true;
        public bool CanWrithe = true;
        public bool DropsEquippedWeaponOnDeath = true;
        public float FatalInjuryHealthThreshold = 0;
        public FiringPattern FiringPattern = FiringPattern.Default;
        public int Health = 1;
        public float HearingRange = 0;
        public float InjuryHealthThreshold = 0;
        public bool IsDucking = false;
        public bool IsInvincible = false;
        public bool IsOnlyDamagedByPlayer = false;
        public int Money = 0;
        public EntityPopulationType PopulationType = EntityPopulationType.RandomAmbient;
        public float SeeingRange = 0;
        public float Sweat = 0;
        public float VisualFieldCenterAngle = 0;
        public float VisualFieldMaxAngle = 0;
        public float VisualFieldMaxElevationAngle = 0;
        public float VisualFieldMinAngle = 0;
        public float VisualFieldMinElevationAngle = 0;
        public float VisualFieldPeripheralRange = 0;

        public PedComponentState[] Components = Array.Empty<PedComponentState>();

        public List<WeaponState> Weapons = new List<WeaponState>();

        public bool ShouldMarkAsNoLongerNeeded = false;
        public bool ShouldRestorePopulationType = false;
        public EntityPopulationType PopulationTypeToRestore = EntityPopulationType.RandomAmbient;

        public PedState() { }
        public PedState(State state, Ped ped)
        {
            Reference = ped;

            Model = ped.Model;

            Position = ped.Position;
            Velocity = ped.Velocity;
            Rotation = ped.Rotation;
            RotationVelocity = ped.RotationVelocity;

            IsInVehicle = ped.IsInVehicle();
            if (IsInVehicle)
            {
                var vehicleState = state.Vehicles.FirstOrDefault(x => x.Reference == ped.CurrentVehicle);
                if (vehicleState == null)
                    IsInVehicle = false;
                else
                {
                    VehicleId = vehicleState.Id;
                    VehicleSeat = ped.SeatIndex;
                }
            }

            Accuracy = ped.Accuracy;
            Armor = ped.Armor;
            CanBeTargetted = ped.CanBeTargetted;
            CanFlyThroughWindscreen = ped.CanFlyThroughWindscreen;
            CanRagdoll = ped.CanRagdoll;
            CanSufferCriticalHits = ped.CanSufferCriticalHits;
            CanWrithe = ped.CanWrithe;
            DropsEquippedWeaponOnDeath = ped.DropsEquippedWeaponOnDeath;
            FatalInjuryHealthThreshold = ped.FatalInjuryHealthThreshold;
            FiringPattern = ped.FiringPattern;
            Health = ped.Health;
            HearingRange = ped.HearingRange;
            InjuryHealthThreshold = ped.InjuryHealthThreshold;
            IsDucking = ped.IsDucking;
            IsInvincible = ped.IsInvincible;
            IsOnlyDamagedByPlayer = ped.IsOnlyDamagedByPlayer;
            Money = ped.Money;
            PopulationType = ped.PopulationType;
            SeeingRange = ped.SeeingRange;
            Sweat = ped.Sweat;
            VisualFieldCenterAngle = ped.VisualFieldCenterAngle;
            VisualFieldMaxAngle = ped.VisualFieldMaxAngle;
            VisualFieldMaxElevationAngle = ped.VisualFieldMaxElevationAngle;
            VisualFieldMinAngle = ped.VisualFieldMinAngle;
            VisualFieldMinElevationAngle = ped.VisualFieldMinElevationAngle;
            VisualFieldPeripheralRange = ped.VisualFieldPeripheralRange;

            var components = ped.Style.GetAllComponents();
            Components = new PedComponentState[components.Length];
            for (int i = 0; i < components.Length; i++)
                Components[i] = new PedComponentState(components[i]);

            foreach (WeaponHash weaponHash in Enum.GetValues(typeof(WeaponHash)))
            {
                if (ped.Weapons.HasWeapon(weaponHash))
                {
                    var weapon = ped.Weapons[weaponHash];
                    Weapons.Add(new WeaponState(weapon)
                    {
                        IsCurrentWeapon = ped.Weapons.Current.Hash == weapon.Hash
                    });
                }
            }
        }

        public void Load(State state, Ped overridePed = null)
        {
            if (overridePed != null)
                Reference = overridePed;

            if (Reference != null)
            {
                if (!Reference.Exists())
                    Reference = null;
                else if (Reference.Model != Model)
                {
                    Reference.Delete();
                    Reference = null;
                }
            }

            VehicleState vehicleState = null;
            if (IsInVehicle)
                vehicleState = state.Vehicles.FirstOrDefault(x => x.Id == VehicleId && x.Reference != null);

            bool isPedCreatedOnSeat = false;

            if (Reference == null)
            {
                if (vehicleState == null)
                    Reference = World.CreatePed(Model, Position);
                else
                {
                    if (!vehicleState.Reference.IsSeatFree(VehicleSeat))
                        return;

                    Reference = vehicleState.Reference.CreatePedOnSeat(VehicleSeat, Model);
                    isPedCreatedOnSeat = true;
                }
                if (Reference == null)
                    return;
                else
                {
                    ShouldMarkAsNoLongerNeeded = true;
                    Reference.Task.Wait(0);
                }
            }
            else
            {
                ShouldRestorePopulationType = true;
                PopulationTypeToRestore = Reference.PopulationType;
                Reference.PopulationType = EntityPopulationType.Permanent;
            }

            if (vehicleState != null)
            {
                if (VehicleSeat == VehicleSeat.Driver)
                {
                    if (!isPedCreatedOnSeat)
                        Reference.SetIntoVehicle(vehicleState.Reference, VehicleSeat);
                    Reference.Task.Wait(0);
                }
                else if (!isPedCreatedOnSeat)
                    Reference.Task.WarpIntoVehicle(vehicleState.Reference, VehicleSeat);
            }
            else
            {
                Reference.PositionNoOffset = Position;
                Reference.Velocity = Velocity;
                Reference.Rotation = Rotation;
                Reference.RotationVelocity = RotationVelocity;
            }

            Reference.Accuracy = Accuracy;
            Reference.Armor = Armor;
            Reference.CanBeTargetted = CanBeTargetted;
            Reference.CanFlyThroughWindscreen = CanFlyThroughWindscreen;
            Reference.CanRagdoll = CanRagdoll;
            Reference.CanSufferCriticalHits = CanSufferCriticalHits;
            Reference.CanWrithe = CanWrithe;
            Reference.DropsEquippedWeaponOnDeath = DropsEquippedWeaponOnDeath;
            Reference.FatalInjuryHealthThreshold = FatalInjuryHealthThreshold;
            Reference.FiringPattern = FiringPattern;
            Reference.Health = Health;
            Reference.HearingRange = HearingRange;
            Reference.InjuryHealthThreshold = InjuryHealthThreshold;
            Reference.IsDucking = IsDucking;
            Reference.IsInvincible = IsInvincible;
            Reference.IsOnlyDamagedByPlayer = IsOnlyDamagedByPlayer;
            Reference.Money = Money;
            Reference.PopulationType = PopulationType;
            Reference.SeeingRange = SeeingRange;
            Reference.Sweat = Sweat;
            Reference.VisualFieldCenterAngle = VisualFieldCenterAngle;
            Reference.VisualFieldMaxAngle = VisualFieldMaxAngle;
            Reference.VisualFieldMaxElevationAngle = VisualFieldMaxElevationAngle;
            Reference.VisualFieldMinAngle = VisualFieldMinAngle;
            Reference.VisualFieldMinElevationAngle = VisualFieldMinElevationAngle;
            Reference.VisualFieldPeripheralRange = VisualFieldPeripheralRange;

            var join = Reference.Style.GetAllComponents().Join(Components, x => x.Type, x => x.Type, (a, b) => new { Component = a, State = b });
            foreach (var item in join)
            {
                item.Component.Index = item.State.Index;
                item.Component.TextureIndex = item.State.TextureIndex;
            }

            Reference.Weapons.RemoveAll();
            foreach (var weaponState in Weapons)
            {
                var weapon = Reference.Weapons.Give(weaponState.Hash, weaponState.Ammo, weaponState.IsCurrentWeapon, true);
                weapon.AmmoInClip = weaponState.AmmoInClip;
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((uint)Model);

            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);

            writer.Write(Velocity.X);
            writer.Write(Velocity.Y);
            writer.Write(Velocity.Z);

            writer.Write(Rotation.X);
            writer.Write(Rotation.Y);
            writer.Write(Rotation.Z);

            writer.Write(RotationVelocity.X);
            writer.Write(RotationVelocity.Y);
            writer.Write(RotationVelocity.Z);

            writer.Write(IsInVehicle);
            writer.Write(VehicleId.ToByteArray());
            writer.Write((int)VehicleSeat);

            writer.Write(Accuracy);
            writer.Write(Armor);
            writer.Write(CanBeTargetted);
            writer.Write(CanFlyThroughWindscreen);
            writer.Write(CanRagdoll);
            writer.Write(CanSufferCriticalHits);
            writer.Write(CanWrithe);
            writer.Write(DropsEquippedWeaponOnDeath);
            writer.Write(FatalInjuryHealthThreshold);
            writer.Write((uint)FiringPattern);
            writer.Write(Health);
            writer.Write(HearingRange);
            writer.Write(InjuryHealthThreshold);
            writer.Write(IsDucking);
            writer.Write(IsInvincible);
            writer.Write(IsOnlyDamagedByPlayer);
            writer.Write(Money);
            writer.Write((int)PopulationType);
            writer.Write(SeeingRange);
            writer.Write(Sweat);
            writer.Write(VisualFieldCenterAngle);
            writer.Write(VisualFieldMaxAngle);
            writer.Write(VisualFieldMaxElevationAngle);
            writer.Write(VisualFieldMinAngle);
            writer.Write(VisualFieldMinElevationAngle);
            writer.Write(VisualFieldPeripheralRange);

            writer.Write(Components.Length);
            foreach (var component in Components)
                component.Write(writer);

            writer.Write(Weapons.Count);
            foreach (var weapon in Weapons)
                weapon.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            Model = (PedHash)reader.ReadUInt32();

            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            Position = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            Velocity = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            Rotation = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            RotationVelocity = new Vector3(x, y, z);

            IsInVehicle = reader.ReadBoolean();
            VehicleId = new Guid(reader.ReadBytes(16));
            VehicleSeat = (VehicleSeat)reader.ReadInt32();

            Accuracy = reader.ReadInt32();
            Armor = reader.ReadInt32();
            CanBeTargetted = reader.ReadBoolean();
            CanFlyThroughWindscreen = reader.ReadBoolean();
            CanRagdoll = reader.ReadBoolean();
            CanSufferCriticalHits = reader.ReadBoolean();
            CanWrithe = reader.ReadBoolean();
            DropsEquippedWeaponOnDeath = reader.ReadBoolean();
            FatalInjuryHealthThreshold = reader.ReadInt32();
            FiringPattern = (FiringPattern)reader.ReadUInt32();
            Health = reader.ReadInt32();
            HearingRange = reader.ReadInt32();
            InjuryHealthThreshold = reader.ReadInt32();
            IsDucking = reader.ReadBoolean();
            IsInvincible = reader.ReadBoolean();
            IsOnlyDamagedByPlayer = reader.ReadBoolean();
            Money = reader.ReadInt32();
            PopulationType = (EntityPopulationType)reader.ReadInt32();
            SeeingRange = reader.ReadInt32();
            Sweat = reader.ReadInt32();
            VisualFieldCenterAngle = reader.ReadInt32();
            VisualFieldMaxAngle = reader.ReadInt32();
            VisualFieldMaxElevationAngle = reader.ReadInt32();
            VisualFieldMinAngle = reader.ReadInt32();
            VisualFieldMinElevationAngle = reader.ReadInt32();
            VisualFieldPeripheralRange = reader.ReadInt32();

            var componentsAmount = reader.ReadInt32();
            Components = new PedComponentState[componentsAmount];
            for (int i = 0; i < componentsAmount; i++)
            {
                Components[i] = new PedComponentState();
                Components[i].Read(reader);
            }

            var weaponsAmount = reader.ReadInt32();
            Weapons.Clear();
            for (int i = 0; i < weaponsAmount; i++)
            {
                var weaponState = new WeaponState();
                weaponState.Read(reader);
                Weapons.Add(weaponState);
            }
        }
    }
}
