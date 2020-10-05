using DevicesApi.Domain;
using DevicesApi.TestsAuxiliaryTools;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DevicesApi.IntegrationTests
{
    public class ReadingsControllerIntegrationTests : IntegrationTests
    {
        [Fact]
        public async Task GetAll_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            var inserDeviceSqlString = "Insert into Devices (Name, Location) values('integrationTestGetAllName1', 'integrationTestGetAllLocation1')" +
                ", ('integrationTestGetAllName2', 'integrationTestGetAllLocation2')";
            ExecuteNonQuery(inserDeviceSqlString);

            var selectSqlStringDevice1 = "Select * from Devices WHERE Name = 'integrationTestGetAllName1'";
            var device1 = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device1);

            var selectSqlStringDevice2 = "Select * from Devices WHERE Name = 'integrationTestGetAllName2'";
            var device2 = ExecuteDevicesQuery(selectSqlStringDevice2);
            Assert.NotNull(device2);

            string insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device1.Device_id}, 1000, 'typeTest1', 10), ({device2.Device_id}, 1001, 'typeTest2', 20)";
            ExecuteNonQuery(insertReadingsSqlString);

            Reading reading1 = new Reading { Device_id = device1.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            Reading reading2 = new Reading { Device_id = device2.Device_id, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 };

            string getAllUri = $"http://api:80/api/Readings";

            // Act
            var response = await HttpClient.GetAsync(getAllUri);
            var readingList = await response.Content.ReadAsAsync<List<Reading>>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(readingList, reading1));
            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(readingList, reading2));

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id in ({device1.Device_id}, {device2.Device_id})";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings);

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id in ({device1.Device_id}, {device2.Device_id})";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            var insertDeviceSqlString = "Insert into Devices (Name, Location) values('integrationTestGetByIdName1', 'integrationTestGetByIdLocation1')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = "Select * from Devices WHERE Name = 'integrationTestGetByIdName1'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);
            Console.WriteLine("GetByDeviceId: Device introduzido: Id:" + device.Device_id);

            var insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device.Device_id}, 1000, 'typeTest1', 10), ({device.Device_id}, 1001, 'typeTest2', 20)";
            ExecuteNonQuery(insertReadingsSqlString);

            Reading reading1 = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            Reading reading2 = new Reading { Device_id = device.Device_id, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 };

            string getAllUri = $"http://api:80/api/Readings/{device.Device_id}";

            // Act
            var response = await HttpClient.GetAsync(getAllUri);
            var readingList = await response.Content.ReadAsAsync<List<Reading>>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(readingList, reading1));
            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(readingList, reading2));

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings);

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;
            string getById = $"http://api:80/api/Responses/{nonExistingDeviceId}";

            // Act
            var response = await HttpClient.GetAsync(getById);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            var insertDeviceSqlString = "Insert into Devices (Name, Location) values('integrationTestGetByIdName1', 'integrationTestGetByIdLocation1')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = "Select * from Devices WHERE Name = 'integrationTestGetByIdName1'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);
            Console.WriteLine("GetByDeviceId: Device introduzido: Id:" + device.Device_id);

            var insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device.Device_id}, 1000, 'typeTest1', 10), ({device.Device_id}, 1001, 'typeTest2', 20)";
            ExecuteNonQuery(insertReadingsSqlString);

            Reading reading1 = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            Reading reading2 = new Reading { Device_id = device.Device_id, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 };

            var startingDatetime = 1000;

            string getAllUri = $"http://api:80/api/Readings/{device.Device_id}/{startingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(getAllUri);
            var readingList = await response.Content.ReadAsAsync<List<Reading>>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(readingList, reading1));
            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(readingList, reading2));

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings);

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;
            var startingDatetime = 1000;

            string getById = $"http://api:80/api/Responses/{nonExistingDeviceId}/{startingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(getById);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            var insertDeviceSqlString = "Insert into Devices (Name, Location) values('integrationTestGetByIdName1', 'integrationTestGetByIdLocation1')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = "Select * from Devices WHERE Name = 'integrationTestGetByIdName1'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            var insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device.Device_id}, 1000, 'typeTest1', 10), ({device.Device_id}, 1001, 'typeTest2', 20)";
            ExecuteNonQuery(insertReadingsSqlString);

            Reading reading1 = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            Reading reading2 = new Reading { Device_id = device.Device_id, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 };

            var startingDatetime = 1001;
            var endingDatetime = 1001;

            string getAllUri = $"http://api:80/api/Readings/{device.Device_id}/{startingDatetime}/{endingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(getAllUri);
            var readingList = await response.Content.ReadAsAsync<List<Reading>>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(readingList, reading2));

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings);

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;
            var startingDatetime = 1000;
            var endingDatetime = 1001;

            string getById = $"http://api:80/api/Responses/{nonExistingDeviceId}/{startingDatetime}/{endingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(getById);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_StartingDateHigherThanEnding_ReturnsBadRequest()
        {
            //Arrange
            var insertDeviceSqlString = "Insert into Devices (Name, Location) values('integrationTestGetByIdName1', 'integrationTestGetByIdLocation1')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = "Select * from Devices WHERE Name = 'integrationTestGetByIdName1'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            var startingDatetime = 1005;
            var endingDatetime = 1000;

            string getAllUri = $"http://api:80/api/Readings/{device.Device_id}/{startingDatetime}/{endingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(getAllUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings);
        }

        [Fact]
        public async Task Create_ValidParameters_ReadingIsAdded()
        {
            //Arrange
            var insertDeviceSqlString = "Insert into Devices (Name, Location) values('integrationTestGetByIdName1', 'integrationTestGetByIdLocation1')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = "Select * from Devices WHERE Name = 'integrationTestGetByIdName1'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            Reading reading = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            var postPayload = JsonConvert.SerializeObject(reading);
            HttpContent postContent = new StringContent(postPayload, Encoding.UTF8, "application/json");

            string getAllUri = $"http://api:80/api/Readings/";

            // Act
            var response = await HttpClient.PostAsync(getAllUri, postContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings);

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices);
        }

        [Fact]
        public async Task Create_WithoutAnExistingDevice_ReturnsNotFound()
        {
            int nonExistingDeviceId = 99;

            Reading reading = new Reading { Device_id = nonExistingDeviceId, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            var postPayload = JsonConvert.SerializeObject(reading);
            HttpContent postContent = new StringContent(postPayload, Encoding.UTF8, "application/json");

            string getAllUri = $"http://api:80/api/Readings/";

            // Act
            var response = await HttpClient.PostAsync(getAllUri, postContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_AlreadyExistingDatetimeForTheDevice_ReturnsBadRequest()
        {
            //Arrange
            var insertDeviceSqlString = "Insert into Devices (Name, Location) values('integrationTestGetByIdName1', 'integrationTestGetByIdLocation1')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = "Select * from Devices WHERE Name = 'integrationTestGetByIdName1'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);
            Console.WriteLine("GetByDeviceId: Device introduzido: Id:" + device.Device_id);

            var insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device.Device_id}, 1000, 'typeTest1', 10), ({device.Device_id}, 1001, 'typeTest2', 20)";
            ExecuteNonQuery(insertReadingsSqlString);

            Reading reading = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            var postPayload = JsonConvert.SerializeObject(reading);
            HttpContent postContent = new StringContent(postPayload, Encoding.UTF8, "application/json");

            string getAllUri = $"http://api:80/api/Readings/";

            // Act
            var response = await HttpClient.PostAsync(getAllUri, postContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings);

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices);
        }
    }
}
