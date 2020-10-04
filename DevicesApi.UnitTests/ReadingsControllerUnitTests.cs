using DevicesApi.Controllers;
using DevicesApi.Domain;
using DevicesApi.TestsAuxiliaryTools;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DevicesApi.UnitTests
{
    public class ReadingsControllerUnitTests
    {
        [Fact]
        public async Task GetAll_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetAll_WithReadingsAvailable_ReturnsReadings));
            var readingsController = new ReadingsController(dbContext);
            var expectedReadings = new List<Reading> { };
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 });
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 });
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1002, Reading_type = "typeTest3", Raw_value = 30 });
            expectedReadings.Add(new Reading() { Device_id = 2, Timestamp = 1000, Reading_type = "typeTest4", Raw_value = 40 });

            //Act
            var response = await readingsController.GetAll();
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingsReceived = values.As<List<Reading>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            Assert.True(ReadingsComparer.CompareReadingsLists(readingsReceived, expectedReadings));
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetReadingsUsingDeviceId_WithReadingsAvailable_ReturnsReadings));
            var readingsController = new ReadingsController(dbContext);
            var expectedReadings = new List<Reading> { };
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1000, Reading_type = "typeTest1", Raw_value = 10 });
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 });
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1002, Reading_type = "typeTest3", Raw_value = 30 });

            //Act
            var response = await readingsController.GetReadingsUsingDeviceId(1);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingsReceived = values.As<List<Reading>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            Assert.True(ReadingsComparer.CompareReadingsLists(readingsReceived, expectedReadings));
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetReadingsUsingDeviceId_WithoutAnExistingDevice_ReturnsNotFound));
            var readingsController = new ReadingsController(dbContext);

            //Act
            var response = await readingsController.GetReadingsUsingDeviceId(5);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingsReceived = values.As<List<Reading>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetReadingsUsingDeviceId_Startingdate_WithReadingsAvailable_ReturnsReadings));
            var readingsController = new ReadingsController(dbContext);
            var expectedReadings = new List<Reading> { };
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 });
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1002, Reading_type = "typeTest3", Raw_value = 30 });

            //Act
            var response = await readingsController.GetReadingsUsingDeviceId_Startingdate(1, 1001);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingsReceived = values.As<List<Reading>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            Assert.True(ReadingsComparer.CompareReadingsLists(readingsReceived, expectedReadings));
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetReadingsUsingDeviceId_Startingdate_WithoutAnExistingDevice_ReturnsNotFound));
            var readingsController = new ReadingsController(dbContext);

            //Act
            var response = await readingsController.GetReadingsUsingDeviceId_Startingdate(5, 1001);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingsReceived = values.As<List<Reading>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_WithReadingsAvailable_ReturnsReadings()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetReadingsUsingDeviceId_Startingdate_Endingdate_WithReadingsAvailable_ReturnsReadings));
            var readingsController = new ReadingsController(dbContext);
            var expectedReadings = new List<Reading> { };
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1001, Reading_type = "typeTest2", Raw_value = 20 });
            expectedReadings.Add(new Reading() { Device_id = 1, Timestamp = 1002, Reading_type = "typeTest3", Raw_value = 30 });

            //Act
            var response = await readingsController.GetReadingsUsingDeviceId_Startingdate_Endingdate(1, 1001, 1002);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingsReceived = values.As<List<Reading>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            Assert.True(ReadingsComparer.CompareReadingsLists(readingsReceived, expectedReadings));
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetReadingsUsingDeviceId_Startingdate_Endingdate_WithoutAnExistingDevice_ReturnsNotFound));
            var readingsController = new ReadingsController(dbContext);

            //Act
            var response = await readingsController.GetReadingsUsingDeviceId_Startingdate_Endingdate(5, 1001, 1002);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingsReceived = values.As<List<Reading>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReadingsUsingDeviceId_Startingdate_Endingdate_StartingDateHigherThanEnding_ReturnsBadRequest()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetReadingsUsingDeviceId_Startingdate_Endingdate_StartingDateHigherThanEnding_ReturnsBadRequest));
            var readingsController = new ReadingsController(dbContext);

            //Act
            var response = await readingsController.GetReadingsUsingDeviceId_Startingdate_Endingdate(1, 1002, 1000);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingsReceived = values.As<List<Reading>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_ValidParameters_ReadingIsAdded()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(Create_ValidParameters_ReadingIsAdded));
            var readingsController = new ReadingsController(dbContext);
            var expectedReading = new Reading { Device_id = 2, Timestamp = 1010, Reading_type = "typeTest10", Raw_value = 60 };

            //Act
            var response = await readingsController.Create(expectedReading);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingReceived = values.As<Reading>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
            Assert.True(ReadingsComparer.CompareReadings(readingReceived, expectedReading));
        }

        [Fact]
        public async Task Create_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(Create_WithoutAnExistingDevice_ReturnsNotFound));
            var readingsController = new ReadingsController(dbContext);
            var expectedReading = new Reading { Device_id = 5, Timestamp = 1010, Reading_type = "typeTest10", Raw_value = 60 };

            //Act
            var response = await readingsController.Create(expectedReading);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingReceived = values.As<Reading>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_AlreadyExistingDatetimeForTheDevice_ReturnsBadRequest()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(Create_AlreadyExistingDatetimeForTheDevice_ReturnsBadRequest));
            var readingsController = new ReadingsController(dbContext);
            var expectedReading = new Reading { Device_id = 1, Timestamp = 1000, Reading_type = "typeTest10", Raw_value = 60 };

            //Act
            var response = await readingsController.Create(expectedReading);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var readingReceived = values.As<Reading>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
