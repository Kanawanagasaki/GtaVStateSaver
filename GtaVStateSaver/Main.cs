using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtaVStateSaver
{
    public class Main : Script
    {
        private const string SAVE_FILE = "scripts/GtaVStateSaver.bin";

        public Main()
        {
            KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F10)
                return;

            var player = Game.Player;
            var playerPed = player.Character;
            var playerVehicle = playerPed.CurrentVehicle;

            if (e.Shift)
            {
                using (var fileStream = File.OpenWrite(SAVE_FILE))
                using (var writer = new BinaryWriter(fileStream))
                {
                    WriteWorld(writer);
                    WritePlayer(writer, player);

                    writer.Write(playerVehicle != null);
                    if (playerVehicle != null)
                    {
                        writer.Write((int)playerPed.SeatIndex);
                        WriteVehicle(writer, playerVehicle);
                    }
                    else
                        WritePed(writer, playerPed);

                    var vehicles = World.GetAllVehicles()
                        .Where(x => x != playerVehicle)
                        .OrderBy(x => x.IsOnScreen ? 0 : Vector3.DistanceSquared2D(x.Position, playerPed.Position))
                        .Take(20)
                        .ToArray();
                    writer.Write(vehicles.Length);
                    foreach (var vehicle in vehicles)
                        WriteVehicle(writer, vehicle);

                    var peds = World.GetAllPeds()
                        .Where(x => x != playerPed && !x.IsInVehicle())
                        .OrderBy(x => x.IsOnScreen ? 0 : Vector3.DistanceSquared2D(x.Position, playerPed.Position))
                        .Take(30)
                        .ToArray();
                    writer.Write(peds.Length);
                    foreach (var ped in peds)
                        WritePed(writer, ped);
                }
            }
            else
            {
                if (!File.Exists(SAVE_FILE))
                    return;

                foreach (var ped in World.GetAllPeds())
                    ped.Delete();
                foreach (var vehicle in World.GetAllVehicles())
                    vehicle.Delete();

                using (var fileStream = File.OpenRead(SAVE_FILE))
                using (var reader = new BinaryReader(fileStream))
                {
                    ReadWorld(reader);
                    ReadPlayer(reader, player);

                    var isPlayerInVehicle = reader.ReadBoolean();
                    if (isPlayerInVehicle)
                    {
                        var seatIndex = reader.ReadInt32();
                        var vehicle = SpawnVehicle(reader, (playerPed, seatIndex), false);
                        if (vehicle != null)
                            playerPed.SetIntoVehicle(vehicle, (VehicleSeat)seatIndex);
                    }
                    else
                        ReadPed(reader, playerPed);

                    var vehiclesAmount = reader.ReadInt32();
                    for (int i = 0; i < vehiclesAmount; i++)
                        SpawnVehicle(reader);

                    var pedsAmount = reader.ReadInt32();
                    for (int i = 0; i < pedsAmount; i++)
                    {
                        var ped = SpawnPed(reader);
                        if (ped != null)
                            ped.MarkAsNoLongerNeeded();
                    }
                }
            }
        }

        private void WriteWorld(BinaryWriter writer)
        {
            writer.Write((int)World.Weather);
            writer.Write((int)World.NextWeather);
            writer.Write(World.CurrentDate.ToBinary());
            writer.Write(World.CurrentTimeOfDay.TotalMilliseconds);
            writer.Write(World.WaypointBlip != null);
            writer.Write(World.WaypointPosition.X);
            writer.Write(World.WaypointPosition.Y);
            writer.Write(World.WaypointPosition.Z);
        }

        private void ReadWorld(BinaryReader reader)
        {
            World.Weather = (Weather)reader.ReadInt32();
            World.NextWeather = (Weather)reader.ReadInt32();
            World.CurrentDate = DateTime.FromBinary(reader.ReadInt64());
            World.CurrentTimeOfDay = TimeSpan.FromMilliseconds(reader.ReadDouble());
            var isWaypointSet = reader.ReadBoolean();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            if (isWaypointSet)
                World.WaypointPosition = new Vector3(x, y, z);
            else
                World.RemoveWaypoint();
        }

        private void WritePlayer(BinaryWriter writer, Player player)
        {
            writer.Write(player.Money);
            writer.Write(player.WantedCenterPosition.X);
            writer.Write(player.WantedCenterPosition.Y);
            writer.Write(player.WantedCenterPosition.Z);
            writer.Write(player.WantedLevel);

            writer.Write(GameplayCamera.RelativeHeading);
            writer.Write(GameplayCamera.RelativePitch);
        }

        private void ReadPlayer(BinaryReader reader, Player player)
        {
            player.Money = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            player.WantedCenterPosition = new Vector3(x, y, z);
            player.WantedLevel = reader.ReadInt32();

            GameplayCamera.RelativeHeading = reader.ReadSingle();
            GameplayCamera.RelativePitch = reader.ReadSingle();
        }

        private void WritePed(BinaryWriter writer, Ped ped)
        {
            writer.Write(ped.Model.Hash);

            writer.Write(ped.Position.X);
            writer.Write(ped.Position.Y);
            writer.Write(ped.Position.Z);

            writer.Write(ped.Velocity.X);
            writer.Write(ped.Velocity.Y);
            writer.Write(ped.Velocity.Z);

            writer.Write(ped.Rotation.X);
            writer.Write(ped.Rotation.Y);
            writer.Write(ped.Rotation.Z);

            writer.Write(ped.RotationVelocity.X);
            writer.Write(ped.RotationVelocity.Y);
            writer.Write(ped.RotationVelocity.Z);

            writer.Write(ped.Accuracy);
            writer.Write(ped.ArmorFloat);
            writer.Write(ped.CanBeTargetted);
            writer.Write(ped.CanFlyThroughWindscreen);
            writer.Write(ped.CanRagdoll);
            writer.Write(ped.CanSufferCriticalHits);
            writer.Write(ped.CanWrithe);
            writer.Write(ped.DropsEquippedWeaponOnDeath);
            writer.Write(ped.FatalInjuryHealthThreshold);
            writer.Write((uint)ped.FiringPattern);
            writer.Write(ped.HealthFloat);
            writer.Write(ped.HearingRange);
            writer.Write(ped.InjuryHealthThreshold);
            writer.Write(ped.IsDucking);
            writer.Write(ped.IsInvincible);
            writer.Write(ped.IsOnlyDamagedByPlayer);
            writer.Write(ped.MaxHealthFloat);
            writer.Write(ped.Money);
            writer.Write((int)ped.PopulationType);
            writer.Write(ped.SeeingRange);
            writer.Write(ped.Sweat);
            writer.Write(ped.VisualFieldCenterAngle);
            writer.Write(ped.VisualFieldMaxAngle);
            writer.Write(ped.VisualFieldMaxElevationAngle);
            writer.Write(ped.VisualFieldMinAngle);
            writer.Write(ped.VisualFieldMinElevationAngle);
            writer.Write(ped.VisualFieldPeripheralRange);

            var components = ped.Style.GetAllComponents();
            writer.Write(components.Length);
            foreach (var component in components)
            {
                writer.Write(component.Index);
                writer.Write(component.TextureIndex);
            }

            WritePedWeapons(writer, ped);
        }

        private Ped SpawnPed(BinaryReader reader)
        {
            var model = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var ped = World.CreatePed(model, new Vector3(x, y, z));

            if (ped == null)
            {
                reader.BaseStream.Seek(48, SeekOrigin.Current);
                SeekPedWeapons(reader);
                return null;
            }

            ped.PositionNoOffset = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            ped.Velocity = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            ped.Rotation = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            ped.RotationVelocity = new Vector3(x, y, z);

            ped.Accuracy = reader.ReadInt32();
            ped.ArmorFloat = reader.ReadInt32();
            ped.CanBeTargetted = reader.ReadBoolean();
            ped.CanFlyThroughWindscreen = reader.ReadBoolean();
            ped.CanRagdoll = reader.ReadBoolean();
            ped.CanSufferCriticalHits = reader.ReadBoolean();
            ped.CanWrithe = reader.ReadBoolean();
            ped.DropsEquippedWeaponOnDeath = reader.ReadBoolean();
            ped.FatalInjuryHealthThreshold = reader.ReadInt32();
            ped.FiringPattern = (FiringPattern)reader.ReadUInt32();
            ped.HealthFloat = reader.ReadInt32();
            ped.HearingRange = reader.ReadInt32();
            ped.InjuryHealthThreshold = reader.ReadInt32();
            ped.IsDucking = reader.ReadBoolean();
            ped.IsInvincible = reader.ReadBoolean();
            ped.IsOnlyDamagedByPlayer = reader.ReadBoolean();
            ped.MaxHealthFloat = reader.ReadInt32();
            ped.Money = reader.ReadInt32();
            ped.PopulationType = (EntityPopulationType)reader.ReadInt32();
            ped.SeeingRange = reader.ReadInt32();
            ped.Sweat = reader.ReadInt32();
            ped.VisualFieldCenterAngle = reader.ReadInt32();
            ped.VisualFieldMaxAngle = reader.ReadInt32();
            ped.VisualFieldMaxElevationAngle = reader.ReadInt32();
            ped.VisualFieldMinAngle = reader.ReadInt32();
            ped.VisualFieldMinElevationAngle = reader.ReadInt32();
            ped.VisualFieldPeripheralRange = reader.ReadInt32();

            var componentsAmount = reader.ReadInt32();
            var components = ped.Style.GetAllComponents();
            for (int i = 0; i < componentsAmount && i < components.Length; i++)
            {
                var index = reader.ReadInt32();
                var textureIndex = reader.ReadInt32();
                components[i].SetVariation(index, textureIndex);
            }

            ped.Task.Wait(0);

            ReadPedWeapons(reader, ped);

            return ped;
        }

        private void ReadPed(BinaryReader reader, Ped ped)
        {
            var _ = reader.ReadInt32();

            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            ped.PositionNoOffset = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            ped.Velocity = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            ped.Rotation = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            ped.RotationVelocity = new Vector3(x, y, z);

            ped.Accuracy = reader.ReadInt32();
            ped.ArmorFloat = reader.ReadInt32();
            ped.CanBeTargetted = reader.ReadBoolean();
            ped.CanFlyThroughWindscreen = reader.ReadBoolean();
            ped.CanRagdoll = reader.ReadBoolean();
            ped.CanSufferCriticalHits = reader.ReadBoolean();
            ped.CanWrithe = reader.ReadBoolean();
            ped.DropsEquippedWeaponOnDeath = reader.ReadBoolean();
            ped.FatalInjuryHealthThreshold = reader.ReadInt32();
            ped.FiringPattern = (FiringPattern)reader.ReadUInt32();
            ped.HealthFloat = reader.ReadInt32();
            ped.HearingRange = reader.ReadInt32();
            ped.InjuryHealthThreshold = reader.ReadInt32();
            ped.IsDucking = reader.ReadBoolean();
            ped.IsInvincible = reader.ReadBoolean();
            ped.IsOnlyDamagedByPlayer = reader.ReadBoolean();
            ped.MaxHealthFloat = reader.ReadInt32();
            ped.Money = reader.ReadInt32();
            ped.PopulationType = (EntityPopulationType)reader.ReadInt32();
            ped.SeeingRange = reader.ReadInt32();
            ped.Sweat = reader.ReadInt32();
            ped.VisualFieldCenterAngle = reader.ReadInt32();
            ped.VisualFieldMaxAngle = reader.ReadInt32();
            ped.VisualFieldMaxElevationAngle = reader.ReadInt32();
            ped.VisualFieldMinAngle = reader.ReadInt32();
            ped.VisualFieldMinElevationAngle = reader.ReadInt32();
            ped.VisualFieldPeripheralRange = reader.ReadInt32();

            var componentsAmount = reader.ReadInt32();
            var components = ped.Style.GetAllComponents();
            for (int i = 0; i < componentsAmount; i++)
            {
                var index = reader.ReadInt32();
                var textureIndex = reader.ReadInt32();
                if (i < components.Length)
                    components[i].SetVariation(index, textureIndex);
            }

            ReadPedWeapons(reader, ped);
        }

        private void SeekPed(BinaryReader reader)
        {
            reader.BaseStream.Seek(133, SeekOrigin.Current);
            var componentsAmount = reader.ReadInt32();
            reader.BaseStream.Seek(componentsAmount * 8, SeekOrigin.Current);
            SeekPedWeapons(reader);
        }

        private void WritePedWeapons(BinaryWriter writer, Ped ped)
        {
            int weaponCount = 0;
            foreach (WeaponHash weaponHash in Enum.GetValues(typeof(WeaponHash)))
                if (ped.Weapons.HasWeapon(weaponHash))
                    weaponCount++;
            writer.Write(weaponCount);
            foreach (WeaponHash weaponHash in Enum.GetValues(typeof(WeaponHash)))
            {
                if (ped.Weapons.HasWeapon(weaponHash))
                {
                    var weapon = ped.Weapons[weaponHash];
                    writer.Write((uint)weapon.Hash);
                    writer.Write(weapon.Ammo);
                    writer.Write(weapon.AmmoInClip);
                    writer.Write(ped.Weapons.Current.Hash == weapon.Hash);
                }
            }
        }

        private void ReadPedWeapons(BinaryReader reader, Ped ped)
        {
            int weaponsAmount = reader.ReadInt32();
            for (int i = 0; i < weaponsAmount; i++)
            {
                var hash = (WeaponHash)reader.ReadUInt32();
                var ammo = reader.ReadInt32();
                var ammoInClip = reader.ReadInt32();
                var isEquipped = reader.ReadBoolean();
                if (ped.Weapons.HasWeapon(hash))
                {
                    ped.Weapons[hash].Ammo = ammo;
                    ped.Weapons[hash].AmmoInClip = ammoInClip;
                    if (isEquipped)
                        ped.Weapons.Select(hash, true);
                }
                else
                {
                    ped.Weapons.Give(hash, ammo, isEquipped, true);
                    ped.Weapons[hash].AmmoInClip = ammoInClip;
                }
            }
        }

        private void SeekPedWeapons(BinaryReader reader)
        {
            int weaponsAmount = reader.ReadInt32();
            reader.BaseStream.Seek(weaponsAmount * 13, SeekOrigin.Current);
        }

        private void WriteVehicle(BinaryWriter writer, Vehicle vehicle)
        {
            // HEADER

            writer.Write(vehicle.Model.Hash);

            writer.Write(vehicle.Position.X);
            writer.Write(vehicle.Position.Y);
            writer.Write(vehicle.Position.Z);

            writer.Write(vehicle.Heading);

            var doors = vehicle.Doors.ToArray();
            writer.Write(doors.Length);
            var occupants = vehicle.Occupants;
            writer.Write(occupants.Length);

            var licensePlate = vehicle.Mods.LicensePlate;
            var lincensePlateUTF8 = Encoding.UTF8.GetBytes(licensePlate);
            writer.Write(lincensePlateUTF8.Length);
            writer.Write(lincensePlateUTF8);

            // BODY

            writer.Write(vehicle.Velocity.X);
            writer.Write(vehicle.Velocity.Y);
            writer.Write(vehicle.Velocity.Z);

            writer.Write(vehicle.Rotation.X);
            writer.Write(vehicle.Rotation.Y);
            writer.Write(vehicle.Rotation.Z);

            writer.Write(vehicle.RotationVelocity.X);
            writer.Write(vehicle.RotationVelocity.Y);
            writer.Write(vehicle.RotationVelocity.Z);

            writer.Write(vehicle.AlarmTimeLeft);
            writer.Write(vehicle.AreHighBeamsOn);
            writer.Write(vehicle.AreLightsOn);
            writer.Write(vehicle.BodyHealth);
            writer.Write(vehicle.Clutch);
            writer.Write(vehicle.CurrentGear);
            writer.Write(vehicle.DirtLevel);
            writer.Write(vehicle.EngineHealth);
            writer.Write(vehicle.FuelLevel);
            writer.Write(vehicle.HealthFloat);
            writer.Write(vehicle.HeliBladesSpeed);
            writer.Write(vehicle.HeliEngineHealth);
            writer.Write(vehicle.HeliMainRotorHealth);
            writer.Write(vehicle.HeliTailRotorHealth);
            writer.Write(vehicle.HighGear);
            writer.Write(vehicle.IsConsideredDestroyed);
            writer.Write(vehicle.IsDriveable);
            writer.Write(vehicle.IsEngineRunning);
            writer.Write(vehicle.IsLeftHeadLightBroken);
            writer.Write(vehicle.IsRightHeadLightBroken);
            writer.Write(vehicle.IsRocketBoostActive);
            writer.Write(vehicle.IsSearchLightOn);
            writer.Write(vehicle.IsSirenActive);
            writer.Write(vehicle.IsStolen);
            writer.Write(vehicle.IsTaxiLightOn);
            writer.Write((int)vehicle.LandingGearState);
            writer.Write((int)vehicle.LockStatus);
            writer.Write(vehicle.NeedsToBeHotwired);
            writer.Write(vehicle.NextGear);
            writer.Write(vehicle.OilLevel);
            writer.Write(vehicle.PetrolTankHealth);
            writer.Write(vehicle.PreviouslyOwnedByPlayer);
            writer.Write((int)vehicle.RoofState);
            writer.Write(vehicle.SteeringAngle);
            writer.Write(vehicle.SteeringScale);

            writer.Write(vehicle.Mods.ColorCombination);
            writer.Write(vehicle.Mods.CustomPrimaryColor.ToArgb());
            writer.Write(vehicle.Mods.CustomSecondaryColor.ToArgb());
            writer.Write((int)vehicle.Mods.DashboardColor);
            writer.Write((int)vehicle.Mods.LicensePlateStyle);
            writer.Write(vehicle.Mods.Livery);
            writer.Write(vehicle.Mods.NeonLightsColor.ToArgb());
            writer.Write((int)vehicle.Mods.PearlescentColor);
            writer.Write((int)vehicle.Mods.PrimaryColor);
            writer.Write((int)vehicle.Mods.RimColor);
            writer.Write((int)vehicle.Mods.SecondaryColor);
            writer.Write(vehicle.Mods.TireSmokeColor.ToArgb());
            writer.Write((int)vehicle.Mods.TrimColor);
            writer.Write((int)vehicle.Mods.WheelType);
            writer.Write((int)vehicle.Mods.WindowTint);

            foreach (VehicleNeonLight neonLight in Enum.GetValues(typeof(VehicleNeonLight)))
            {
                writer.Write(vehicle.Mods.HasNeonLight(neonLight));
                writer.Write(vehicle.Mods.IsNeonLightsOn(neonLight));
            }

            foreach (VehicleModType modType in Enum.GetValues(typeof(VehicleModType)))
            {
                writer.Write(vehicle.Mods[modType].Variation);
                writer.Write(vehicle.Mods[modType].Index);
            }

            foreach (var door in doors)
            {
                writer.Write((int)door.Index);
                writer.Write(door.IsOpen);
                writer.Write(door.IsBroken);
                writer.Write(door.AngleRatio);
            }

            foreach (var ped in occupants)
            {
                writer.Write((int)ped.SeatIndex);
                WritePed(writer, ped);
            }
        }

        private Vehicle SpawnVehicle(BinaryReader reader, (Ped ped, int index)? playerSeat = null, bool markOccupants = true)
        {
            var model = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var heading = reader.ReadSingle();
            var vehicle = World.CreateVehicle(model, new Vector3(x, y, z), heading);

            var doorsAmount = reader.ReadInt32();
            var occupantsAmount = reader.ReadInt32();
            var licensePlateLength = reader.ReadInt32();
            var lincensePlateUTF8 = reader.ReadBytes(licensePlateLength);
            var lincensePlate = Encoding.UTF8.GetString(lincensePlateUTF8);

            if (vehicle == null)
            {
                reader.BaseStream.Seek(194, SeekOrigin.Current);
                reader.BaseStream.Seek(Enum.GetValues(typeof(VehicleNeonLight)).Length * 2, SeekOrigin.Current);
                reader.BaseStream.Seek(Enum.GetValues(typeof(VehicleModType)).Length * 5, SeekOrigin.Current);
                reader.BaseStream.Seek(doorsAmount * 10, SeekOrigin.Current);
                for (int i = 0; i < occupantsAmount; i++)
                {
                    reader.BaseStream.Seek(4, SeekOrigin.Current);
                    SeekPed(reader);
                }
                return null;
            }

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            vehicle.Velocity = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            vehicle.Rotation = new Vector3(x, y, z);

            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            vehicle.RotationVelocity = new Vector3(x, y, z);

            vehicle.AlarmTimeLeft = reader.ReadInt32();
            vehicle.AreHighBeamsOn = reader.ReadBoolean();
            vehicle.AreLightsOn = reader.ReadBoolean();
            vehicle.BodyHealth = reader.ReadSingle();
            vehicle.Clutch = reader.ReadSingle();
            vehicle.CurrentGear = reader.ReadInt32();
            vehicle.DirtLevel = reader.ReadSingle();
            vehicle.EngineHealth = reader.ReadSingle();
            vehicle.FuelLevel = reader.ReadSingle();
            vehicle.HealthFloat = reader.ReadSingle();
            vehicle.HeliBladesSpeed = reader.ReadSingle();
            vehicle.HeliEngineHealth = reader.ReadSingle();
            vehicle.HeliMainRotorHealth = reader.ReadSingle();
            vehicle.HeliTailRotorHealth = reader.ReadSingle();
            vehicle.HighGear = reader.ReadInt32();
            vehicle.IsConsideredDestroyed = reader.ReadBoolean();
            vehicle.IsDriveable = reader.ReadBoolean();
            vehicle.IsEngineRunning = reader.ReadBoolean();
            vehicle.IsLeftHeadLightBroken = reader.ReadBoolean();
            vehicle.IsRightHeadLightBroken = reader.ReadBoolean();
            vehicle.IsRocketBoostActive = reader.ReadBoolean();
            vehicle.IsSearchLightOn = reader.ReadBoolean();
            vehicle.IsSirenActive = reader.ReadBoolean();
            vehicle.IsStolen = reader.ReadBoolean();
            vehicle.IsTaxiLightOn = reader.ReadBoolean();
            vehicle.LandingGearState = (VehicleLandingGearState)reader.ReadInt32();
            vehicle.LockStatus = (VehicleLockStatus)reader.ReadInt32();
            vehicle.NeedsToBeHotwired = reader.ReadBoolean();
            vehicle.NextGear = reader.ReadInt32();
            vehicle.OilLevel = reader.ReadSingle();
            vehicle.PetrolTankHealth = reader.ReadSingle();
            vehicle.PreviouslyOwnedByPlayer = reader.ReadBoolean();
            vehicle.RoofState = (VehicleRoofState)reader.ReadInt32();
            vehicle.SteeringAngle = reader.ReadSingle();
            vehicle.SteeringScale = reader.ReadSingle();

            vehicle.Mods.ColorCombination = reader.ReadInt32();
            vehicle.Mods.CustomPrimaryColor = Color.FromArgb(reader.ReadInt32());
            vehicle.Mods.CustomSecondaryColor = Color.FromArgb(reader.ReadInt32());
            vehicle.Mods.DashboardColor = (VehicleColor)reader.ReadInt32();
            vehicle.Mods.LicensePlate = lincensePlate;
            vehicle.Mods.LicensePlateStyle = (LicensePlateStyle)reader.ReadInt32();
            vehicle.Mods.Livery = reader.ReadInt32();
            vehicle.Mods.NeonLightsColor = Color.FromArgb(reader.ReadInt32());
            vehicle.Mods.PearlescentColor = (VehicleColor)reader.ReadInt32();
            vehicle.Mods.PrimaryColor = (VehicleColor)reader.ReadInt32();
            vehicle.Mods.RimColor = (VehicleColor)reader.ReadInt32();
            vehicle.Mods.SecondaryColor = (VehicleColor)reader.ReadInt32();
            vehicle.Mods.TireSmokeColor = Color.FromArgb(reader.ReadInt32());
            vehicle.Mods.TrimColor = (VehicleColor)reader.ReadInt32();
            vehicle.Mods.WheelType = (VehicleWheelType)reader.ReadInt32();
            vehicle.Mods.WindowTint = (VehicleWindowTint)reader.ReadInt32();

            vehicle.Mods.InstallModKit();
            foreach (VehicleNeonLight neonLight in Enum.GetValues(typeof(VehicleNeonLight)))
            {
                var hasNeonLight = reader.ReadBoolean();
                var isNeonLightOn = reader.ReadBoolean();
                if (hasNeonLight && isNeonLightOn)
                    vehicle.Mods.SetNeonLightsOn(neonLight, true);
            }

            foreach (VehicleModType modType in Enum.GetValues(typeof(VehicleModType)))
            {
                vehicle.Mods[modType].Variation = reader.ReadBoolean();
                vehicle.Mods[modType].Index = reader.ReadInt32();
            }

            for (int i = 0; i < doorsAmount; i++)
            {
                var doorIndex = reader.ReadInt32();
                var isDoorOpen = reader.ReadBoolean();
                var isDoorBroken = reader.ReadBoolean();
                var doorAngleRatio = reader.ReadSingle();

                var door = vehicle.Doors[(VehicleDoorIndex)doorIndex];
                if (isDoorBroken)
                    door.Break(false);
                else if (isDoorOpen)
                {
                    door.Open(false, true);
                    door.AngleRatio = doorAngleRatio;
                }
            }

            for (int i = 0; i < occupantsAmount; i++)
            {
                var seatIndex = reader.ReadInt32();
                Ped ped;
                if (playerSeat.HasValue && playerSeat.Value.index == seatIndex)
                {
                    ped = playerSeat.Value.ped;
                    ReadPed(reader, ped);
                }
                else
                    ped = SpawnPed(reader);
                if (ped != null)
                {
                    ped.SetIntoVehicle(vehicle, (VehicleSeat)seatIndex);
                    if (markOccupants)
                        ped.MarkAsNoLongerNeeded();
                }
            }

            return vehicle;
        }
    }
}
