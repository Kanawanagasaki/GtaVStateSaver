using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace GtaVStateSaver
{
    public class State
    {
        public Weather Weather = Weather.Clear;
        public Weather NextWeather = Weather.Clear;

        public DateTime CurrentDate = DateTime.Now;
        public TimeSpan CurrentTimeOfDay = TimeSpan.FromHours(12);

        public bool IsWaypointSet = false;
        public Vector3 WaypointPosition = Vector3.Zero;

        public int Money = 0;

        public Vector3 WantedCenterPosition = Vector3.Zero;
        public int WantedLevel = 0;

        public float RelativeHeading = 0;
        public float RelativePitch = 0;

        public VehicleState[] Vehicles = Array.Empty<VehicleState>();

        public PedState PlayerPed = new PedState();

        public PedState[] Peds = Array.Empty<PedState>();

        public void Save()
        {
            var player = Game.Player;
            var playerPed = player.Character;
            var playerVehicle = playerPed.CurrentVehicle;

            Weather = World.Weather;
            NextWeather = World.NextWeather;
            CurrentDate = World.CurrentDate;
            CurrentTimeOfDay = World.CurrentTimeOfDay;
            IsWaypointSet = World.WaypointBlip != null;
            WaypointPosition = World.WaypointPosition;

            Money = player.Money;
            WantedCenterPosition = player.WantedCenterPosition;
            WantedLevel = player.WantedLevel;

            RelativeHeading = GameplayCamera.RelativeHeading;
            RelativePitch = GameplayCamera.RelativePitch;

            var vehicles = World.GetAllVehicles()
                .OrderBy
                (x =>
                    x == playerVehicle ? 0
                    : x.PopulationType == EntityPopulationType.Unknown
                        || x.PopulationType == EntityPopulationType.Mission
                        || x.PopulationType == EntityPopulationType.Permanent
                        || x.PopulationType == EntityPopulationType.RandomPermanent
                        || x.PopulationType == EntityPopulationType.RandomScenario ? 1
                    : x.AttachedBlip != null ? 2
                    : Vector3.DistanceSquared2D(x.Position, playerPed.Position)
                )
                .Take(20)
                .ToArray();
            Vehicles = new VehicleState[vehicles.Length];
            for (int i = 0; i < vehicles.Length; i++)
                Vehicles[i] = new VehicleState(vehicles[i]);

            PlayerPed = new PedState(this, player.Character);

            var peds = vehicles
                .Select(x => x.Occupants)
                .SelectMany(x => x)
                .Concat(World.GetAllPeds()
                        .OrderBy(x =>
                            x.AttachedBlip != null ? 1
                            : x.PopulationType == EntityPopulationType.Unknown
                                || x.PopulationType == EntityPopulationType.Mission
                                || x.PopulationType == EntityPopulationType.Permanent
                                || x.PopulationType == EntityPopulationType.RandomPermanent
                                || x.PopulationType == EntityPopulationType.RandomScenario ? 1
                            : Vector3.DistanceSquared2D(x.Position, playerPed.Position))
                        .Take(30))
                .Where(x => x != playerPed)
                .Distinct()
                .ToArray();
            Peds = new PedState[peds.Length];
            for (int i = 0; i < peds.Length; i++)
                Peds[i] = new PedState(this, peds[i]);
        }

        public void Load()
        {
            var player = Game.Player;
            var playerPed = player.Character;

            World.Weather = Weather;
            World.NextWeather = NextWeather;
            World.CurrentDate = CurrentDate;
            World.CurrentTimeOfDay = CurrentTimeOfDay;
            World.WaypointPosition = WaypointPosition;
            if (!IsWaypointSet)
                World.RemoveWaypoint();

            player.Money = Money;
            player.WantedCenterPosition = WantedCenterPosition;
            player.WantedLevel = WantedLevel;

            GameplayCamera.RelativeHeading = RelativeHeading;
            GameplayCamera.RelativePitch = RelativePitch;

            foreach (var vehicleState in Vehicles)
                vehicleState.Load();

            PlayerPed.Load(this, playerPed);

            foreach (var pedState in Peds)
                pedState.Load(this);

            foreach (var ped in World.GetAllPeds())
                if (!Peds.Any(x => x.Reference == ped)
                    && ped.PopulationType != EntityPopulationType.Unknown
                    && ped.PopulationType != EntityPopulationType.Mission
                    && ped.PopulationType != EntityPopulationType.Permanent
                    && ped.PopulationType != EntityPopulationType.RandomPermanent
                    && ped.PopulationType != EntityPopulationType.RandomScenario)
                    ped.Delete();

            foreach (var vehicle in World.GetAllVehicles())
                if (!Vehicles.Any(x => x.Reference == vehicle)
                    && vehicle.PopulationType != EntityPopulationType.Unknown
                    && vehicle.PopulationType != EntityPopulationType.Mission
                    && vehicle.PopulationType != EntityPopulationType.Permanent
                    && vehicle.PopulationType != EntityPopulationType.RandomPermanent
                    && vehicle.PopulationType != EntityPopulationType.RandomScenario)
                    vehicle.Delete();

            Script.Yield();
            foreach (var vehicleState in Vehicles)
            {
                if (vehicleState.ShouldMarkAsNoLongerNeeded)
                {
                    if (vehicleState.Reference != null)
                        vehicleState.Reference.MarkAsNoLongerNeeded();

                    vehicleState.ShouldMarkAsNoLongerNeeded = false;
                }

                if (vehicleState.ShouldRestorePopulationType)
                {
                    if (vehicleState.Reference != null)
                        vehicleState.Reference.PopulationType = vehicleState.PopulationTypeToRestore;

                    vehicleState.ShouldRestorePopulationType = false;
                }
            }

            Script.Yield();
            foreach (var pedState in Peds)
            {
                if (pedState.ShouldMarkAsNoLongerNeeded)
                {
                    if (pedState.Reference != null)
                        pedState.Reference.MarkAsNoLongerNeeded();

                    pedState.ShouldMarkAsNoLongerNeeded = false;
                }

                if (pedState.ShouldRestorePopulationType)
                {
                    if (pedState.Reference != null)
                        pedState.Reference.PopulationType = pedState.PopulationTypeToRestore;

                    pedState.ShouldRestorePopulationType = false;
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((int)Weather);
            writer.Write((int)NextWeather);
            writer.Write(CurrentDate.ToBinary());
            writer.Write(CurrentTimeOfDay.TotalMilliseconds);
            writer.Write(IsWaypointSet);
            writer.Write(WaypointPosition.X);
            writer.Write(WaypointPosition.Y);
            writer.Write(WaypointPosition.Z);
            writer.Write(Money);
            writer.Write(WantedCenterPosition.X);
            writer.Write(WantedCenterPosition.Y);
            writer.Write(WantedCenterPosition.Z);
            writer.Write(WantedLevel);
            writer.Write(RelativeHeading);
            writer.Write(RelativePitch);

            writer.Write(Vehicles.Length);
            foreach (var vehicle in Vehicles)
                vehicle.Write(writer);

            PlayerPed.Write(writer);

            writer.Write(Peds.Length);
            foreach (var ped in Peds)
                ped.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            Weather = (Weather)reader.ReadInt32();
            NextWeather = (Weather)reader.ReadInt32();
            CurrentDate = DateTime.FromBinary(reader.ReadInt64());
            CurrentTimeOfDay = TimeSpan.FromMilliseconds(reader.ReadDouble());
            IsWaypointSet = reader.ReadBoolean();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            WaypointPosition = new Vector3(x, y, z);
            Money = reader.ReadInt32();
            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            WantedCenterPosition = new Vector3(x, y, z);
            WantedLevel = reader.ReadInt32();
            RelativeHeading = reader.ReadSingle();
            RelativePitch = reader.ReadSingle();

            var vehiclesAmount = reader.ReadInt32();
            Vehicles = new VehicleState[vehiclesAmount];
            for (int i = 0; i < vehiclesAmount; i++)
            {
                var vehicle = new VehicleState();
                vehicle.Read(reader);
                Vehicles[i] = vehicle;
            }

            PlayerPed = new PedState();
            PlayerPed.Read(reader);

            var pedsAmount = reader.ReadInt32();
            Peds = new PedState[pedsAmount];
            for (int i = 0; i < pedsAmount; i++)
            {
                var ped = new PedState();
                ped.Read(reader);
                Peds[i] = ped;
            }
        }
    }
}
