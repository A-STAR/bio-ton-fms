using BioTonFMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTonFMS.Infrastructure.EF
{
    public static class DbInitializer
    {
        public static void Initialize(BioTonDBContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Trackers.Any())
            {
                return;   // DB has been seeded
            }

            var trackers = new Tracker[]
            {
                new Tracker{Name = "tracker 1", Imei = "12345678"},
                new Tracker{Name = "tracker 2", Imei = "22345679"}
            };
            var devices1 = new Device[]
            {
                new Device{Name = "D1"},
                new Device{Name = "D2"},
                new Device{Name = "D3"}
            };
            trackers[1].Devices = devices1;
            foreach (Tracker s in trackers)
            {
                context.Trackers.Add(s);
            }
            context.SaveChanges();

            var vehicles = new Vehicle[]
            {
                new Vehicle{Name = "vehicle 1", TrackerId = 1},
                new Vehicle{Name = "vehicle 2", TrackerId = 2},
            };
            foreach (Vehicle v in vehicles)
            {
                context.Vehicles.Add(v);
            }
            context.SaveChanges();
        }
    }
}