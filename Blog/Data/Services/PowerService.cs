﻿using ImpSoft.OctopusEnergy.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Data.Services
{
    public class PowerService
    {
        private readonly BlogService _service;
        private IConfiguration Configuration { get; set; }

        private readonly string Key;
        private readonly IOctopusEnergyClient Client;

        public PowerService(IConfiguration configuration, BlogService BlogService, IOctopusEnergyClient client)
        {
            Configuration = configuration;
            _service = BlogService;
            Client = client;
            Key = Configuration.GetValue<string>("OctopusKey");
        }

        public async Task GetGas()
        {
            DateTimeOffset From = new(DateTime.UtcNow.AddDays(-30).AddHours(-1).AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            DateTimeOffset To = new(DateTime.UtcNow.AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            var consumption = await Client.GetGasConsumptionAsync(Key, Configuration.GetValue<string>("OctopusGasMPAN"), Configuration.GetValue<string>("OctopusGasSerial"), From, To, Interval.Hour);
            await CheckConsumption(14, consumption);
        }

        public async Task GetElec()
        {
            DateTimeOffset From = new(DateTime.UtcNow.AddDays(-30).AddHours(-1).AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            DateTimeOffset To = new(DateTime.UtcNow.AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            var consumption = await Client.GetElectricityConsumptionAsync(Key, Configuration.GetValue<string>("OctopusElecMPAN"), Configuration.GetValue<string>("OctopusElecSerial"), From, To, Interval.Hour);
            await CheckConsumption(15, consumption);
        }

        public async Task CheckConsumption(int Id, IEnumerable<Consumption> consumption)
        {
            var exist = await _service.Get(Id);
            foreach (var item in consumption)
            {
                if (exist.Any(x => x.Date.Value == item.Start.UtcDateTime.Date))
                {
                    await _service.Delete(Id, item.Start.UtcDateTime);
                }
                await _service.SaveData(item.Quantity, Id, item.Start.UtcDateTime);
            }
        }
    }
}
