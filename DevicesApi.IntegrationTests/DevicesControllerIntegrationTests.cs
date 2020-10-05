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
            CreateDeviceRequest deviceRequest1 = new CreateDeviceRequest { Name = "integrationTestName1", Location = "integrationTestLocation1" };
            CreateDeviceRequest deviceRequest2 = new CreateDeviceRequest { Name = "integrationTestName2", Location = "integrationTestLocation2" };

            var sqlString = $"Insert into Devices (Name, Location) values('{deviceRequest1.Name}', '{deviceRequest1.Location}')" +
                $", ('{deviceRequest2.Name}', '{deviceRequest2.Location}')";
            
            ExecuteNonQuery(sqlString);

            string requestUri = "Devices";

            // Act
            var response = await HttpClient.GetAsync(requestUri);
            var receivedDevices = await response.Content.ReadAsAsync<List<Device>>();
            
            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(DevicesComparer.DoesDevicesListContainSpecificDevice(receivedDevices, deviceRequest1), "The requested device was not found in the" +
                " device list retrieved by the GetAll.");
            Assert.True(DevicesComparer.DoesDevicesListContainSpecificDevice(receivedDevices, deviceRequest2), "The requested device was not found in the" +
                " device list retrieved by the GetAll.");

            //Clean Up
            var cleanUpSqlString = $"DELETE From Devices WHERE Name IN ('{deviceRequest1.Name}', '{deviceRequest2.Name}')";
            var cleanedUp = ExecuteNonQuery(cleanUpSqlString);

            Assert.True(cleanedUp, "It was not possible to clean up the database.");
        }

        [Fact]
        public async Task GetById_WithDeviceAvailable_ReturnsDevice()
        {
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "integrationTestName1", Location = "integrationTestLocation1" };

            //Arrange
            var insertSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest.Name}', '{deviceRequest.Location}')";
            ExecuteNonQuery(insertSqlString);

            var selectSqlString = $"Select * from Devices WHERE Name = '{deviceRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlString);
            Assert.NotNull(device);

            string requestUri = $"Devices/{device.Device_id}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);
            var receivedDevice = await response.Content.ReadAsAsync<Device>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(DevicesComparer.CompareDeviceWithDeviceRequest(receivedDevice, deviceRequest), "The requested device was not found in the" +
                " device retrieved by the GetById.");

            //Clean Up
            var cleanUpSqlString = $"DELETE From Devices WHERE Name = '{deviceRequest.Name}'";
            var cleanedUp = ExecuteNonQuery(cleanUpSqlString);

            Assert.True(cleanedUp, "It was not possible to clean up the database.");
        }

        [Fact]
        public async Task GetById_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;
            string requestUri = $"Devices/{nonExistingDeviceId}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithValidParameters_DeviceWasUpdated()
        {
            //Arrange
            CreateDeviceRequest deviceCreateRequest = new CreateDeviceRequest { Name = "integrationTestName1", Location = "integrationTestLocation1" };

            var insertSqlString = $"Insert into Devices (Name, Location) values('{deviceCreateRequest.Name}', '{deviceCreateRequest.Location}')";
            ExecuteNonQuery(insertSqlString);

            var selectSqlString = $"Select * from Devices WHERE Name = '{deviceCreateRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlString);
            Assert.NotNull(device);

            UpdateDeviceRequest deviceUpdateRequest = new UpdateDeviceRequest { Name = "integrationTestNameUpdated1", Location = "integrationTestLocationUpdated1" };
            var updatePayload = JsonConvert.SerializeObject(deviceUpdateRequest);
            HttpContent updateContent = new StringContent(updatePayload, Encoding.UTF8, "application/json");

            string requestUri = $"Devices/{device.Device_id}";

            // Act
            var response = await HttpClient.PutAsync(requestUri, updateContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            selectSqlString = $"Select * from Devices WHERE Name = '{deviceUpdateRequest.Name}'";
            var receivedUpdatedDevice = ExecuteDevicesQuery(selectSqlString);
            Assert.NotNull(receivedUpdatedDevice);

            Assert.True(DevicesComparer.CompareDeviceWithDeviceRequest(receivedUpdatedDevice, deviceUpdateRequest), "The device was not updated.");

            //Clean Up
            var cleanUpSqlString = $"DELETE FROM Devices WHERE Name = '{deviceUpdateRequest.Name}'";
            var cleanedUp = ExecuteNonQuery(cleanUpSqlString);

            Assert.True(cleanedUp, "It was not possible to clean up the database.");
        }

        [Fact]
        public async Task Update_WithoutAnExistingDevice_ReturnNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;

            UpdateDeviceRequest deviceRequest = new UpdateDeviceRequest { Name = "integrationTestNameUpdated1", Location = "integrationTestLocationUpdated1" };
            var updatePayload = JsonConvert.SerializeObject(deviceRequest);
            HttpContent updateContent = new StringContent(updatePayload, Encoding.UTF8, "application/json");

            string requestUri = $"Devices/{nonExistingDeviceId}";

            // Act
            var response = await HttpClient.PutAsync(requestUri, updateContent);

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

            string requestUri = $"Devices/";

            // Act
            var response = await HttpClient.PostAsync(requestUri, postContent);

            Console.WriteLine("Status: " + response.StatusCode);
            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            //Clean Up
            var cleanUpSqlString = $"DELETE From Devices WHERE Name = '{deviceRequest.Name}'";
            var cleanedUp = ExecuteNonQuery(cleanUpSqlString);

            Assert.True(cleanedUp, "It was not possible to clean up the database.");
        }

        [Fact]
        public async Task Delete_WithValidParameters_DeviceWasDelete()
        {
            //Arrange
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "integrationTestName1", Location = "integrationTestLocation1" };

            var insertSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest.Name}', '{deviceRequest.Location}')";
            ExecuteNonQuery(insertSqlString);
            var selectSqlString = $"Select * from Devices WHERE Name = '{deviceRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlString);

            Assert.NotNull(device);

            string requestUri = $"Devices/{device.Device_id}";

            // Act
            var response = await HttpClient.DeleteAsync(requestUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Delete_WithoutAnExistingDevice_ReturnNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;

            string requestUri = $"Devices/{nonExistingDeviceId}";

            // Act
            var response = await HttpClient.DeleteAsync(requestUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
