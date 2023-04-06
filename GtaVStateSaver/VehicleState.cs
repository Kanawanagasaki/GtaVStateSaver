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
    public class VehicleState
    {
        public Guid Id = Guid.NewGuid();

        public Vehicle Reference = null;

        public VehicleHash Model = VehicleHash.Dilettante;

        public float Heading = 0;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 RotationVelocity = Vector3.Zero;

        public int AlarmTimeLeft = 0;
        public bool AreHighBeamsOn = false;
        public bool AreLightsOn = false;
        public float BodyHealth = 0;
        public float Clutch = 0;
        public int CurrentGear = 0;
        public float DirtLevel = 0;
        public float EngineHealth = 0;
        public float FuelLevel = 0;
        public float HealthFloat = 0;
        public float HeliBladesSpeed = 0;
        public float HeliEngineHealth = 0;
        public float HeliMainRotorHealth = 0;
        public float HeliTailRotorHealth = 0;
        public int HighGear = 0;
        public bool IsConsideredDestroyed = false;
        public bool IsDriveable = true;
        public bool IsEngineRunning = false;
        public bool IsLeftHeadLightBroken = false;
        public bool IsRightHeadLightBroken = false;
        public bool IsRocketBoostActive = false;
        public bool IsSearchLightOn = false;
        public bool IsSirenActive = false;
        public bool IsStolen = false;
        public bool IsTaxiLightOn = false;
        public VehicleLandingGearState LandingGearState = VehicleLandingGearState.Deployed;
        public VehicleLockStatus LockStatus = VehicleLockStatus.None;
        public bool NeedsToBeHotwired = false;
        public int NextGear = 0;
        public float OilLevel = 0;
        public float PetrolTankHealth = 0;
        public bool PreviouslyOwnedByPlayer = false;
        public VehicleRoofState RoofState = VehicleRoofState.Closed;
        public float SteeringAngle = 0;
        public float SteeringScale = 0;

        public int DamageAmount = 0;

        public int ModColorCombination = 0;
        public Color ModCustomPrimaryColor = Color.Pink;
        public Color ModCustomSecondaryColor = Color.Pink;
        public VehicleColor ModDashboardColor = VehicleColor.HotPink;
        public string ModLicensePlate = "Hello";
        public LicensePlateStyle ModLicensePlateStyle = 0;
        public int ModLivery = 0;
        public Color ModNeonLightsColor = Color.Pink;
        public VehicleColor ModPearlescentColor = VehicleColor.HotPink;
        public VehicleColor ModPrimaryColor = VehicleColor.HotPink;
        public VehicleColor ModRimColor = VehicleColor.HotPink;
        public VehicleColor ModSecondaryColor = VehicleColor.HotPink;
        public Color ModTireSmokeColor = Color.Pink;
        public VehicleColor ModTrimColor = VehicleColor.HotPink;
        public VehicleWheelType ModWheelType = VehicleWheelType.SUV;
        public VehicleWindowTint ModWindowTint = VehicleWindowTint.None;

        public VehicleNeonLightState[] NeonLights = Array.Empty<VehicleNeonLightState>();

        public VehicleModState[] Mods = Array.Empty<VehicleModState>();

        public List<VehicleDoorState> Doors = new List<VehicleDoorState>();

        public bool ShouldMarkAsNoLongerNeeded = false;
        public bool ShouldRestorePopulationType = false;
        public EntityPopulationType PopulationTypeToRestore = EntityPopulationType.RandomAmbient;

        private Random _rng = new Random();

        public VehicleState() { }
        public VehicleState(Vehicle vehicle)
        {
            Reference = vehicle;

            Model = vehicle.Model;

            Heading = vehicle.Heading;
            Position = vehicle.Position;
            Velocity = vehicle.Velocity;
            Rotation = vehicle.Rotation;
            RotationVelocity = vehicle.RotationVelocity;

            AlarmTimeLeft = vehicle.AlarmTimeLeft;
            AreHighBeamsOn = vehicle.AreHighBeamsOn;
            AreLightsOn = vehicle.AreLightsOn;
            BodyHealth = vehicle.BodyHealth;
            Clutch = vehicle.Clutch;
            CurrentGear = vehicle.CurrentGear;
            DirtLevel = vehicle.DirtLevel;
            EngineHealth = vehicle.EngineHealth;
            FuelLevel = vehicle.FuelLevel;
            HealthFloat = vehicle.HealthFloat;
            HeliBladesSpeed = vehicle.HeliBladesSpeed;
            HeliEngineHealth = vehicle.HeliEngineHealth;
            HeliMainRotorHealth = vehicle.HeliMainRotorHealth;
            HeliTailRotorHealth = vehicle.HeliTailRotorHealth;
            HighGear = vehicle.HighGear;
            IsConsideredDestroyed = vehicle.IsConsideredDestroyed;
            IsDriveable = vehicle.IsDriveable;
            IsEngineRunning = vehicle.IsEngineRunning;
            IsLeftHeadLightBroken = vehicle.IsLeftHeadLightBroken;
            IsRightHeadLightBroken = vehicle.IsRightHeadLightBroken;
            IsRocketBoostActive = vehicle.IsRocketBoostActive;
            IsSearchLightOn = vehicle.IsSearchLightOn;
            IsSirenActive = vehicle.IsSirenActive;
            IsStolen = vehicle.IsStolen;
            IsTaxiLightOn = vehicle.IsTaxiLightOn;
            LandingGearState = vehicle.LandingGearState;
            LockStatus = vehicle.LockStatus;
            NeedsToBeHotwired = vehicle.NeedsToBeHotwired;
            NextGear = vehicle.NextGear;
            OilLevel = vehicle.OilLevel;
            PetrolTankHealth = vehicle.PetrolTankHealth;
            PreviouslyOwnedByPlayer = vehicle.PreviouslyOwnedByPlayer;
            RoofState = vehicle.RoofState;
            SteeringAngle = vehicle.SteeringAngle;
            SteeringScale = vehicle.SteeringScale;

            ModColorCombination = vehicle.Mods.ColorCombination;
            ModCustomPrimaryColor = vehicle.Mods.CustomPrimaryColor;
            ModCustomSecondaryColor = vehicle.Mods.CustomSecondaryColor;
            ModDashboardColor = vehicle.Mods.DashboardColor;
            ModLicensePlate = vehicle.Mods.LicensePlate;
            ModLicensePlateStyle = vehicle.Mods.LicensePlateStyle;
            ModLivery = vehicle.Mods.Livery;
            ModNeonLightsColor = vehicle.Mods.NeonLightsColor;
            ModPearlescentColor = vehicle.Mods.PearlescentColor;
            ModPrimaryColor = vehicle.Mods.PrimaryColor;
            ModRimColor = vehicle.Mods.RimColor;
            ModSecondaryColor = vehicle.Mods.SecondaryColor;
            ModTireSmokeColor = vehicle.Mods.TireSmokeColor;
            ModTrimColor = vehicle.Mods.TrimColor;
            ModWheelType = vehicle.Mods.WheelType;
            ModWindowTint = vehicle.Mods.WindowTint;

            DamageAmount = vehicle.DamageRecords.ToArray().Length;

            var neonLightsValues = Enum.GetValues(typeof(VehicleNeonLight)).Cast<VehicleNeonLight>().ToArray();
            NeonLights = new VehicleNeonLightState[neonLightsValues.Length];
            for (int i = 0; i < neonLightsValues.Length; i++)
            {
                NeonLights[i] = new VehicleNeonLightState
                {
                    NeonLight = neonLightsValues[i],
                    HasNeonLigh = vehicle.Mods.HasNeonLight(neonLightsValues[i]),
                    IsOn = vehicle.Mods.IsNeonLightsOn(neonLightsValues[i])
                };
            }

            var modTypes = Enum.GetValues(typeof(VehicleModType)).Cast<VehicleModType>().ToArray();
            Mods = new VehicleModState[modTypes.Length];
            for (int i = 0; i < modTypes.Length; i++)
            {
                Mods[i] = new VehicleModState()
                {
                    Type = modTypes[i],
                    Variation = vehicle.Mods[modTypes[i]].Variation,
                    Index = vehicle.Mods[modTypes[i]].Index
                };
            }

            foreach (var door in vehicle.Doors)
                Doors.Add(new VehicleDoorState(door));
        }

        public void Load()
        {
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

            if (Reference == null)
            {
                Reference = World.CreateVehicle(Model, Position, Heading);
                if (Reference == null)
                    return;
                else
                {
                    Reference.Mods.InstallModKit();
                    ShouldMarkAsNoLongerNeeded = true;
                }
            }
            else
            {
                ShouldRestorePopulationType = true;
                PopulationTypeToRestore = Reference.PopulationType;
                Reference.PopulationType = EntityPopulationType.Permanent;
            }

            Reference.Heading = Heading;
            Reference.Position = Position;
            Reference.Velocity = Velocity;
            Reference.Rotation = Rotation;
            Reference.RotationVelocity = RotationVelocity;

            Reference.AlarmTimeLeft = AlarmTimeLeft;
            Reference.AreHighBeamsOn = AreHighBeamsOn;
            Reference.AreLightsOn = AreLightsOn;
            Reference.BodyHealth = BodyHealth;
            Reference.Clutch = Clutch;
            Reference.CurrentGear = CurrentGear;
            Reference.DirtLevel = DirtLevel;
            Reference.EngineHealth = EngineHealth;
            Reference.FuelLevel = FuelLevel;
            Reference.HealthFloat = HealthFloat;
            Reference.HeliBladesSpeed = HeliBladesSpeed;
            Reference.HeliEngineHealth = HeliEngineHealth;
            Reference.HeliMainRotorHealth = HeliMainRotorHealth;
            Reference.HeliTailRotorHealth = HeliTailRotorHealth;
            Reference.HighGear = HighGear;
            Reference.IsConsideredDestroyed = IsConsideredDestroyed;
            Reference.IsDriveable = IsDriveable;
            Reference.IsEngineRunning = IsEngineRunning;
            Reference.IsLeftHeadLightBroken = IsLeftHeadLightBroken;
            Reference.IsRightHeadLightBroken = IsRightHeadLightBroken;
            Reference.IsRocketBoostActive = IsRocketBoostActive;
            Reference.IsSearchLightOn = IsSearchLightOn;
            Reference.IsSirenActive = IsSirenActive;
            Reference.IsStolen = IsStolen;
            Reference.IsTaxiLightOn = IsTaxiLightOn;
            Reference.LandingGearState = LandingGearState;
            Reference.LockStatus = LockStatus;
            Reference.NeedsToBeHotwired = NeedsToBeHotwired;
            Reference.NextGear = NextGear;
            Reference.OilLevel = OilLevel;
            Reference.PetrolTankHealth = PetrolTankHealth;
            Reference.PreviouslyOwnedByPlayer = PreviouslyOwnedByPlayer;
            Reference.RoofState = RoofState;
            Reference.SteeringAngle = SteeringAngle;
            Reference.SteeringScale = SteeringScale;

            Reference.Mods.ColorCombination = ModColorCombination;
            Reference.Mods.CustomPrimaryColor = ModCustomPrimaryColor;
            Reference.Mods.CustomSecondaryColor = ModCustomSecondaryColor;
            Reference.Mods.DashboardColor = ModDashboardColor;
            Reference.Mods.LicensePlate = ModLicensePlate;
            Reference.Mods.LicensePlateStyle = ModLicensePlateStyle;
            Reference.Mods.Livery = ModLivery;
            Reference.Mods.NeonLightsColor = ModNeonLightsColor;
            Reference.Mods.PearlescentColor = ModPearlescentColor;
            Reference.Mods.PrimaryColor = ModPrimaryColor;
            Reference.Mods.RimColor = ModRimColor;
            Reference.Mods.SecondaryColor = ModSecondaryColor;
            Reference.Mods.TireSmokeColor = ModTireSmokeColor;
            Reference.Mods.TrimColor = ModTrimColor;
            Reference.Mods.WheelType = ModWheelType;
            Reference.Mods.WindowTint = ModWindowTint;

            Reference.Repair();

            foreach (var neon in NeonLights)
                if (neon.HasNeonLigh && neon.IsOn)
                    Reference.Mods.SetNeonLightsOn(neon.NeonLight, true);

            foreach (var mod in Mods)
            {
                Reference.Mods[mod.Type].Variation = mod.Variation;
                Reference.Mods[mod.Type].Index = mod.Index;
            }

            foreach (var door in Doors)
            {
                if (door.IsBroken)
                    Reference.Doors[door.Index].Break(false);
                else if (door.IsOpen)
                {
                    Reference.Doors[door.Index].Open(false, true);
                    Reference.Doors[door.Index].AngleRatio = door.AngleRatio;
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Id.ToByteArray());

            writer.Write((uint)Model);

            writer.Write(Heading);

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

            writer.Write(AlarmTimeLeft);
            writer.Write(AreHighBeamsOn);
            writer.Write(AreLightsOn);
            writer.Write(BodyHealth);
            writer.Write(Clutch);
            writer.Write(CurrentGear);
            writer.Write(DirtLevel);
            writer.Write(EngineHealth);
            writer.Write(FuelLevel);
            writer.Write(HealthFloat);
            writer.Write(HeliBladesSpeed);
            writer.Write(HeliEngineHealth);
            writer.Write(HeliMainRotorHealth);
            writer.Write(HeliTailRotorHealth);
            writer.Write(HighGear);
            writer.Write(IsConsideredDestroyed);
            writer.Write(IsDriveable);
            writer.Write(IsEngineRunning);
            writer.Write(IsLeftHeadLightBroken);
            writer.Write(IsRightHeadLightBroken);
            writer.Write(IsRocketBoostActive);
            writer.Write(IsSearchLightOn);
            writer.Write(IsSirenActive);
            writer.Write(IsStolen);
            writer.Write(IsTaxiLightOn);
            writer.Write((int)LandingGearState);
            writer.Write((int)LockStatus);
            writer.Write(NeedsToBeHotwired);
            writer.Write(NextGear);
            writer.Write(OilLevel);
            writer.Write(PetrolTankHealth);
            writer.Write(PreviouslyOwnedByPlayer);
            writer.Write((int)RoofState);
            writer.Write(SteeringAngle);
            writer.Write(SteeringScale);

            writer.Write(ModColorCombination);
            writer.Write(ModCustomPrimaryColor.ToArgb());
            writer.Write(ModCustomSecondaryColor.ToArgb());
            writer.Write((int)ModDashboardColor);
            writer.Write(ModLicensePlate);
            writer.Write((int)ModLicensePlateStyle);
            writer.Write(ModLivery);
            writer.Write(ModNeonLightsColor.ToArgb());
            writer.Write((int)ModPearlescentColor);
            writer.Write((int)ModPrimaryColor);
            writer.Write((int)ModRimColor);
            writer.Write((int)ModSecondaryColor);
            writer.Write(ModTireSmokeColor.ToArgb());
            writer.Write((int)ModTrimColor);
            writer.Write((int)ModWheelType);
            writer.Write((int)ModWindowTint);

            writer.Write(DamageAmount);

            writer.Write(NeonLights.Length);
            foreach (var neon in NeonLights)
                neon.Write(writer);

            writer.Write(Mods.Length);
            foreach (var mod in Mods)
                mod.Write(writer);

            writer.Write(Doors.Count);
            foreach (var door in Doors)
                door.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            Id = new Guid(reader.ReadBytes(16));

            Model = (VehicleHash)reader.ReadUInt32();

            Heading = reader.ReadUInt32();

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

            AlarmTimeLeft = reader.ReadInt32();
            AreHighBeamsOn = reader.ReadBoolean();
            AreLightsOn = reader.ReadBoolean();
            BodyHealth = reader.ReadSingle();
            Clutch = reader.ReadSingle();
            CurrentGear = reader.ReadInt32();
            DirtLevel = reader.ReadSingle();
            EngineHealth = reader.ReadSingle();
            FuelLevel = reader.ReadSingle();
            HealthFloat = reader.ReadSingle();
            HeliBladesSpeed = reader.ReadSingle();
            HeliEngineHealth = reader.ReadSingle();
            HeliMainRotorHealth = reader.ReadSingle();
            HeliTailRotorHealth = reader.ReadSingle();
            HighGear = reader.ReadInt32();
            IsConsideredDestroyed = reader.ReadBoolean();
            IsDriveable = reader.ReadBoolean();
            IsEngineRunning = reader.ReadBoolean();
            IsLeftHeadLightBroken = reader.ReadBoolean();
            IsRightHeadLightBroken = reader.ReadBoolean();
            IsRocketBoostActive = reader.ReadBoolean();
            IsSearchLightOn = reader.ReadBoolean();
            IsSirenActive = reader.ReadBoolean();
            IsStolen = reader.ReadBoolean();
            IsTaxiLightOn = reader.ReadBoolean();
            LandingGearState = (VehicleLandingGearState)reader.ReadInt32();
            LockStatus = (VehicleLockStatus)reader.ReadInt32();
            NeedsToBeHotwired = reader.ReadBoolean();
            NextGear = reader.ReadInt32();
            OilLevel = reader.ReadSingle();
            PetrolTankHealth = reader.ReadSingle();
            PreviouslyOwnedByPlayer = reader.ReadBoolean();
            RoofState = (VehicleRoofState)reader.ReadInt32();
            SteeringAngle = reader.ReadSingle();
            SteeringScale = reader.ReadSingle();

            ModColorCombination = reader.ReadInt32();
            ModCustomPrimaryColor = Color.FromArgb(reader.ReadInt32());
            ModCustomSecondaryColor = Color.FromArgb(reader.ReadInt32());
            ModDashboardColor = (VehicleColor)reader.ReadInt32();
            ModLicensePlate = reader.ReadString();
            ModLicensePlateStyle = (LicensePlateStyle)reader.ReadInt32();
            ModLivery = reader.ReadInt32();
            ModNeonLightsColor = Color.FromArgb(reader.ReadInt32());
            ModPearlescentColor = (VehicleColor)reader.ReadInt32();
            ModPrimaryColor = (VehicleColor)reader.ReadInt32();
            ModRimColor = (VehicleColor)reader.ReadInt32();
            ModSecondaryColor = (VehicleColor)reader.ReadInt32();
            ModTireSmokeColor = Color.FromArgb(reader.ReadInt32());
            ModTrimColor = (VehicleColor)reader.ReadInt32();
            ModWheelType = (VehicleWheelType)reader.ReadInt32();
            ModWindowTint = (VehicleWindowTint)reader.ReadInt32();

            DamageAmount = reader.ReadInt32();

            var neonLightsAmount = reader.ReadInt32();
            NeonLights = new VehicleNeonLightState[neonLightsAmount];
            for (int i = 0; i < neonLightsAmount; i++)
            {
                NeonLights[i] = new VehicleNeonLightState();
                NeonLights[i].Read(reader);
            }

            var modsAmount = reader.ReadInt32();
            Mods = new VehicleModState[modsAmount];
            for (int i = 0; i < modsAmount; i++)
            {
                Mods[i] = new VehicleModState();
                Mods[i].Read(reader);
            }

            var doorsAmount = reader.ReadInt32();
            Doors.Clear();
            for (int i = 0; i < doorsAmount; i++)
            {
                var door = new VehicleDoorState();
                door.Read(reader);
                Doors.Add(door);
            }
        }
    }
}
