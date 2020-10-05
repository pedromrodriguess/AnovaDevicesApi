using DevicesApi.Contracts.Requests;
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
            CreateDeviceRequest deviceRequest1 = new CreateDeviceRequest { Name = "testGetAllName1", Location = "testGetAllLocation1" };
            CreateDeviceRequest deviceRequest2 = new CreateDeviceRequest { Name = "testGetAllName2", Location = "testGetAllLocation2" };

            var inserDeviceSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest1.Name}', '{deviceRequest1.Location}')" +
                $", ('{deviceRequest2.Name}', '{deviceRequest2.Location}')";
            ExecuteNonQuery(inserDeviceSqlString);

            var selectSqlStringDevice1 = $"Select * from Devices WHERE Name = '{deviceRequest1.Name}'";
            var device1 = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device1);

            var selectSqlStringDevice2 = $"Select * from Devices WHERE Name = '{deviceRequest2.Name}'";
            var device2 = ExecuteDevicesQuery(selectSqlStringDevice2);
            Assert.NotNull(device2);

            Reading expectedReading1 = new Reading { Device_id = device1.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            Reading expectedReading2 = new Reading { Device_id = device2.Device_id, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 };

            string insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device1.Device_id}, {expectedReading1.Timestamp}, '{expectedReading1.Reading_type}', {expectedReading1.Raw_value})," +
                $" ({device2.Device_id}, {expectedReading2.Timestamp}, '{expectedReading2.Reading_type}', {expectedReading2.Raw_value})";

            ExecuteNonQuery(insertReadingsSqlString);

            string requestUri = "Readings";
            // Act
            var response = await HttpClient.GetAsync(requestUri);
            var receivedReadingList = await response.Content.ReadAsAsync<List<Reading>>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(receivedReadingList, expectedReading1), "The reading list received " +
                "does not contain the expected reading");
            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(receivedReadingList, expectedReading2), "The reading list received " +
                "does not contain the expected reading");

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id in ({device1.Device_id}, {device2.Device_id})";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings, "It was not possible to clean up the created readings database.");

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id in ({device1.Device_id}, {device2.Device_id})";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices, "It was not possible to clean up the created devices database.");
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "testGetByIdName1", Location = "testGetByIdLocation1" };

            var insertDeviceSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest.Name}', '{deviceRequest.Location}')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = $"Select * from Devices WHERE Name = '{deviceRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            Reading expectedReading1 = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            Reading expectedReading2 = new Reading { Device_id = device.Device_id, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 };

            var insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device.Device_id}, {expectedReading1.Timestamp}, '{expectedReading1.Reading_type}', {expectedReading1.Raw_value})," +
                $" ({device.Device_id}, {expectedReading2.Timestamp}, '{expectedReading2.Reading_type}', {expectedReading2.Raw_value})";

            ExecuteNonQuery(insertReadingsSqlString);

            string requestUri = $"Readings/{device.Device_id}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);
            var receivedReadingList = await response.Content.ReadAsAsync<List<Reading>>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(receivedReadingList, expectedReading1), "The reading list received " +
                "does not contain the expected reading");
            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(receivedReadingList, expectedReading2), "The reading list received " +
                "does not contain the expected reading");

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings, "It was not possible to clean up the created readings database.");

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices, "It was not possible to clean up the created devices database.");
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;
            string requestUri = $"Responses/{nonExistingDeviceId}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "testGetStartingDateName1", Location = "testGetStartingDateLocation1" };

            var insertDeviceSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest.Name}', '{deviceRequest.Location}')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = $"Select * from Devices WHERE Name = '{deviceRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            Reading reading1 = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            Reading reading2 = new Reading { Device_id = device.Device_id, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 };

            var insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device.Device_id}, {reading1.Timestamp}, '{reading1.Reading_type}', {reading1.Raw_value})," +
                $" ({device.Device_id}, {reading2.Timestamp}, '{reading2.Reading_type}', {reading2.Raw_value})";

            ExecuteNonQuery(insertReadingsSqlString);

            var startingDatetime = 1000;

            string requestUri = $"Readings/{device.Device_id}/{startingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);
            var receivedReadingList = await response.Content.ReadAsAsync<List<Reading>>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(receivedReadingList, reading1), "The reading list received " +
                "does not contain the expected reading");
            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(receivedReadingList, reading2), "The reading list received " +
                "does not contain the expected reading");

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings, "It was not possible to clean up the created readings database.");


            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices, "It was not possible to clean up the created devices database.");
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;
            var startingDatetime = 1000;

            string requestUri = $"Readings/{nonExistingDeviceId}/{startingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "testStartingEndDateName1", Location = "testStartingEndDateLocation1" };

            var insertDeviceSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest.Name}', '{deviceRequest.Location}')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = $"Select * from Devices WHERE Name = '{deviceRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            Reading reading1 = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            Reading reading2 = new Reading { Device_id = device.Device_id, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 };

            var insertReadingsSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device.Device_id}, {reading1.Timestamp}, '{reading1.Reading_type}', {reading1.Raw_value})," +
                $" ({device.Device_id}, {reading2.Timestamp}, '{reading2.Reading_type}', {reading2.Raw_value})";

            ExecuteNonQuery(insertReadingsSqlString);

            var startingDatetime = 1001;
            var endingDatetime = 1001;

            string requestUri = $"Readings/{device.Device_id}/{startingDatetime}/{endingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);
            var receivedReadingList = await response.Content.ReadAsAsync<List<Reading>>();

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(ReadingsComparer.DoesReadingsListContainSpecificReading(receivedReadingList, reading2), "The reading list received " +
                "does not contain the expected reading");

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings, "It was not possible to clean up the created readings database.");

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices, "It was not possible to clean up the created devices database.");
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            int nonExistingDeviceId = 99;
            var startingDatetime = 1000;
            var endingDatetime = 1001;

            string requestUri = $"Readings/{nonExistingDeviceId}/{startingDatetime}/{endingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_StartingDateHigherThanEnding_ReturnsBadRequest()
        {
            //Arrange
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "testInvalidStartingEndName1", Location = "testInvalidStartingEndLocation1" };

            var insertDeviceSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest.Name}', '{deviceRequest.Location}')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = $"Select * from Devices WHERE Name = '{deviceRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            var startingDatetime = 1005;
            var endingDatetime = 1000;

            string requestUri = $"Readings/{device.Device_id}/{startingDatetime}/{endingDatetime}";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings, "It was not possible to clean up the created readings database.");
        }

        [Fact]
        public async Task Create_ValidParameters_ReadingIsAdded()
        {
            //Arrange
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "testCreateName1", Location = "testCreateLocation1" };

            var insertDeviceSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest.Name}', '{deviceRequest.Location}')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = $"Select * from Devices WHERE Name = '{deviceRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            Reading reading = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            var postPayload = JsonConvert.SerializeObject(reading);
            HttpContent postContent = new StringContent(postPayload, Encoding.UTF8, "application/json");

            string requestUri = $"Readings/";

            // Act
            var response = await HttpClient.PostAsync(requestUri, postContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);
            Assert.True(cleanedUpReadings, "It was not possible to clean up the created readings database.");

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpDevices, "It was not possible to clean up the created devices database.");
        }

        [Fact]
        public async Task Create_WithoutAnExistingDevice_ReturnsNotFound()
        {
            int nonExistingDeviceId = 99;

            Reading reading = new Reading { Device_id = nonExistingDeviceId, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };
            var postPayload = JsonConvert.SerializeObject(reading);
            HttpContent postContent = new StringContent(postPayload, Encoding.UTF8, "application/json");

            string requestUri = $"Readings/";

            // Act
            var response = await HttpClient.PostAsync(requestUri, postContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_AlreadyExistingDatetimeForTheDevice_ReturnsBadRequest()
        {
            //Arrange
            CreateDeviceRequest deviceRequest = new CreateDeviceRequest { Name = "testBadRequestCreateName1", Location = "testBadRequestCreateLocation1" };

            var insertDeviceSqlString = $"Insert into Devices (Name, Location) values('{deviceRequest.Name}', '{deviceRequest.Location}')";
            ExecuteNonQuery(insertDeviceSqlString);

            var selectSqlStringDevice1 = $"Select * from Devices WHERE Name = '{deviceRequest.Name}'";
            var device = ExecuteDevicesQuery(selectSqlStringDevice1);
            Assert.NotNull(device);

            Reading reading = new Reading { Device_id = device.Device_id, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 };

            var insertReadingSqlString = "Insert into Readings (Device_id, Timestamp, Reading_type, Raw_value)" +
                $" values({device.Device_id}, {reading.Timestamp}, '{reading.Reading_type}', {reading.Raw_value})";

            ExecuteNonQuery(insertReadingSqlString);

            var postPayload = JsonConvert.SerializeObject(reading);
            HttpContent postContent = new StringContent(postPayload, Encoding.UTF8, "application/json");

            string requestUri = $"Readings/";

            // Act
            var response = await HttpClient.PostAsync(requestUri, postContent);

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            //Clean Up
            var cleanUpReadingsSqlString = $"DELETE From Readings WHERE Device_id = {device.Device_id}";
            var cleanedUpReadings = ExecuteNonQuery(cleanUpReadingsSqlString);

            var cleanUpDevicesSqlString = $"DELETE From Devices WHERE Device_id = {device.Device_id}";
            var cleanedUpDevices = ExecuteNonQuery(cleanUpDevicesSqlString);
            Assert.True(cleanedUpReadings, "It was not possible to clean up the created readings database.");
        }
    }
}
