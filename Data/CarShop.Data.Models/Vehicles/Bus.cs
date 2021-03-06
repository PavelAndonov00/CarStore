﻿using CarShop.Data.Models.Vehicles.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarShop.Data.Models.Vehicles
{
    public class Bus : Vehicle, IBus
    {
        [Required]
        public int SeatsCount { get; set; }
    }
}
