using DevicesApi.Domain;
using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System;
using System.Text;
using DevicesApi.Contracts.Requests;
using Newtonsoft.Json;
using DevicesApi.TestsAuxiliaryTools;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace DevicesApi.IntegrationTests
{
    public class DevicesControllerIntegrationTests : IntegrationTests
    {
        [Fact]
        public async Task GetAll_WithValidParameters_ReturnDevices()
        {
            //Arrange
            var sqlString = "Insert into Devices (Name, Location) values('integrationTestName1', 'integrationTestLocation1')" +
                ", ('integrationTestName2', 'integrationTestLocation2')";
            ExecuteNonQuery(sqlString);

            CreateDeviceRequest deviceRequest1 = new CreateDeviceRequest { Name = "integrationTestName1", Location = "integrationTestLocation1" };
            CreateDeviceRequest deviceRequest2 = new CreateDeviceRequest { Name = "integrationTestName2", Location = "integrationTestLocation2" };

            string getAllUri = "http://api:80/api/Devices";

            // Act
            var response = await HttpClient.GetAsync(getAllUri);
            var devicesList = await response.Content.ReadAsAsync<List<Device>>();
            
            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(DevicesComparer.DoesDevicesListContainSpecificDevice(devicesList, deviceRequest1));
            Assert.True(DevicesComparer.DoesDevicesListContainSpecificDevice(devicesList, deviceRequest2));

            //Clean Up
            var cleanUpSqlString = "DELETE From Devices WHERE Name IN ('integrationTestName1', 'integrationTestName2')";
            var cleanedUp = ExecuteNonQuery(cleanUpSqlString);

            Assert.True(cleanedUp);
        }
        
        [Fact]
        public async Task GetById_WithDeviceAvailable_ReturnsDevice()
        {
            //Arrange
            var insertSqlString = "Insert into Devices (Name, Location) values('integrationTestName1', 'integrationTestLocation1')";
            ExecuteNonQuery(insertSqlString);

            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "integrationTestName1", Location = "integrationTestLocation1" };

            var selectSqlString = "Select * from Devices WHERE Name = 'integrationTestName1'";
            var device = ExecuteDevicesQuery(selectSqlString);
            Assert.NotNull(device);

            string getById = $"http://api:80/api/Devices/{device.Device_id}";

            // Act
            var response = await HttpClient.GetAsync(getById);
            var deviceRead = await response.Content.ReadAsAsync<Device>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(DevicesComparer.CompareDeviceWithDeviceRequest(deviceRead, deviceRequest));

            //Clean Up
            var cleanUpSqlString = "DELETE From Devices WHERE Name = 'integrationTestName1' or Name = 'intergrationTestName2'";
            var cleanedUp = ExecuteNonQuery(cleanUpSqlString);

            Assert.True(cleanedUp);
        }

        [Fact]
        public async Task GetById_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;
            string getById = $"http://api:80/api/Devices/{nonExistingDeviceId}";

            // Act
            var response = await HttpClient.GetAsync(getById);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithValidParameters_DeviceWasUpdated()
        {
            //Arrange
            var insertSqlString = "Insert into Devices (Name, Location) values('integrationTestName1', 'integrationTestLocation1')";
            ExecuteNonQuery(insertSqlString);

            var selectSqlString = "Select * from Devices WHERE Name = 'integrationTestName1'";
            var device = ExecuteDevicesQuery(selectSqlString);
            Assert.NotNull(device);

            UpdateDeviceRequest deviceRequest = new UpdateDeviceRequest { Name = "integrationTestNameUpdated1", Location = "integrationTestLocationUpdated1" };
            var updatePayload = JsonConvert.SerializeObject(deviceRequest);
            HttpContent updateContent = new StringContent(updatePayload, Encoding.UTF8, "application/json");

            string getAllUri = $"http://api:80/api/Devices/{device.Device_id}";

            // Act
            var response = await HttpClient.PutAsync(getAllUri, updateContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            selectSqlString = "Select * from Devices WHERE Name = 'integrationTestNameUpdated1'";
            var updatedDevice = ExecuteDevicesQuery(selectSqlString);
            Assert.NotNull(updatedDevice);

            Assert.True(DevicesComparer.CompareDeviceWithDeviceRequest(updatedDevice, deviceRequest));

            //Clean Up
            var cleanUpSqlString = $"DELETE FROM Devices WHERE Name = '{deviceRequest.Name}'";
            var cleanedUp = ExecuteNonQuery(cleanUpSqlString);

            Assert.True(cleanedUp);
        }

        [Fact]
        public async Task Update_WithoutAnExistingDevice_ReturnNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;

            UpdateDeviceRequest deviceRequest = new UpdateDeviceRequest { Name = "integrationTestNameUpdated1", Location = "integrationTestLocationUpdated1" };
            var updatePayload = JsonConvert.SerializeObject(deviceRequest);
            HttpContent updateContent = new StringContent(updatePayload, Encoding.UTF8, "application/json");

            string getById = $"http://api:80/api/Devices/{nonExistingDeviceId}";

            // Act
            var response = await HttpClient.PutAsync(getById, updateContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_WithValidParameters_DeviceIsCreated()
        {
            //Arrange
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "integrationTestName1", Location = "integrationTestLocation1" };
            var postPayload = JsonConvert.SerializeObject(deviceRequest);
            HttpContent postContent = new StringContent(postPayload, Encoding.UTF8, "application/json");
            
            string getAllUri = "http://api:80/api/Devices";

            // Act
            var response = await HttpClient.PostAsync(getAllUri, postContent);

            Console.WriteLine("Status: " + response.StatusCode);
            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            //Clean Up
            var cleanUpSqlString = "DELETE From Devices WHERE Name = 'integrationTestName1'";
            var cleanedUp = ExecuteNonQuery(cleanUpSqlString);

            Assert.True(cleanedUp);
        }

        [Fact]
        public async Task Delete_WithValidParameters_DeviceWasDelete()
        {
            //Arrange
            var insertSqlString = "Insert into Devices (Name, Location) values('integrationTestName1', 'integrationTestLocation1')";
            ExecuteNonQuery(insertSqlString);
            var selectSqlString = "Select * from Devices WHERE Name = 'integrationTestName1'";
            var device = ExecuteDevicesQuery(selectSqlString);

            Assert.NotNull(device);

            string getAllUri = $"http://api:80/api/Devices/{device.Device_id}";

            // Act
            var response = await HttpClient.DeleteAsync(getAllUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Delete_WithoutAnExistingDevice_ReturnNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;

            string getById = $"http://api:80/api/Devices/{nonExistingDeviceId}";

            // Act
            var response = await HttpClient.DeleteAsync(getById);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
