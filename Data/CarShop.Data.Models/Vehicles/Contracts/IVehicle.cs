﻿
using System;
using System.Collections.Generic;
using System.Text;

namespace CarShop.Data.Models.Vehicles
{
    public interface IVehicle
    {
        string EngineType { get; set; }

        int Power { get; set; }

        decimal Price { get; set; }

        DateTime ManufacturedOn { get; set; }

        string Brand { get; set; }

        string Model { get; set; }

        string Color { get; set; }

        int Run { get; set; }

        VehicleState State { get; set; }

        TransmissionType Transmission { get; set; }

        string Region { get; set; }

        string PopulatedPlace { get; set; }
    }
}