using DevicesApi.Data;
using DevicesApi.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesApi.UnitTests
{
    public static class DbContextExtensions
    {
        public static void Seed (this ApplicationDbContext dbContext)
        {
            dbContext.Add(new Device() { Device_id = 1, Name = "testName1", Location = "testLocation1" });
            dbContext.Add(new Device() { Device_id = 2, Name = "testName2", Location = "testLocation2" });
            dbContext.Add(new Device() { Device_id = 3, Name = "testName3", Location = "testLocation3" });

            dbContext.Add(new Reading() { Device_id = 1, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 });
            dbContext.Add(new Reading() { Device_id = 1, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 });
            dbContext.Add(new Reading() { Device_id = 1, Timestamp = 1002, Reading_type = "typeTest3", Raw_value = 30 });
            dbContext.Add(new Reading() { Device_id = 2, Timestamp = 1000, Reading_type = "typeTest4", Raw_value = 40 });

            dbContext.SaveChanges();
        }
    }
}
