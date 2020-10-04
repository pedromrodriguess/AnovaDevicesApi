using DevicesApi.Contracts.Requests;
using DevicesApi.Controllers;
using DevicesApi.Domain;
using DevicesApi.TestsAuxiliaryTools;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DevicesApi.UnitTests
{
    public class DevicesControllerUnitTests
    {
        [Fact]
        public async Task GetAll_WithDevicesAvailable_ReturnsDevices()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetAll_WithDevicesAvailable_ReturnsDevices));
            var devicesController = new DevicesController(dbContext);
            var expectedDevices = new List<Device>
            {
                new Device() { Device_id = 1, Name = "testName1", Location = "testLocation1" },
                new Device() { Device_id = 2, Name = "testName2", Location = "testLocation2" },
                new Device() { Device_id = 3, Name = "testName3", Location = "testLocation3" }
            };

            //Act
            var response = await devicesController.GetAll();
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var devicesReceived = values.As<List<Device>>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            Assert.True(DevicesComparer.CompareDevicesLists(devicesReceived, expectedDevices));
        }

        [Fact]
        public async Task GetById_WithDeviceAvailable_ReturnsDevice()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetById_WithDeviceAvailable_ReturnsDevice));
            var devicesController = new DevicesController(dbContext);
            var expectedDevice = new Device
            { 
                Device_id = 1, Name = "testName1", Location = "testLocation1" 
            };

            //Act
            var response = await devicesController.GetById(1);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var deviceReceived = values.As<Device>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            Assert.True(DevicesComparer.CompareDevices(deviceReceived, expectedDevice));
        }

        [Fact]
        public async Task GetById_WithoutAnExistingDevice_ReturnsNotFound()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(GetById_WithoutAnExistingDevice_ReturnsNotFound));
            var devicesController = new DevicesController(dbContext);

            //Act
            var response = await devicesController.GetById(4);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var deviceReceived = values.As<Device>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public void Update_WithValidParameters_DeviceIsUpdated()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(Update_WithValidParameters_DeviceIsUpdated));
            var devicesController = new DevicesController(dbContext);
            var updateDeviceRequest = new UpdateDeviceRequest
            {
                Name = "updatedName",
                Location = "updatedLocation"
            };
            var expectedDevice = new Device
            {
                Device_id = 1,
                Name = "updatedName",
                Location = "updatedLocation"
            };

            //Act
            var response = devicesController.Update(1, updateDeviceRequest);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var deviceReceived = values.As<Device>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            Assert.True(DevicesComparer.CompareDevices(deviceReceived, expectedDevice));
        }

        [Fact]
        public void Update_WithoutAnExistingDevice_ReturnNotFound()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(Update_WithoutAnExistingDevice_ReturnNotFound));
            var devicesController = new DevicesController(dbContext);
            var updateDeviceRequest = new UpdateDeviceRequest
            {
                Name = "updatedName",
                Location = "updatedLocation"
            };

            //Act
            var response = devicesController.Update(4, updateDeviceRequest);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var deviceReceived = values.As<Device>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_WithValidParameters_DeviceIsAdded()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(Create_WithValidParameters_DeviceIsAdded));
            var devicesController = new DevicesController(dbContext);
            var createDeviceRequest = new CreateDeviceRequest
            {
                Name = "testName1",
                Location = "testLocation1"
            };
            var expectedDevice = new Device
            {
                Device_id = 4,
                Name = "testName1",
                Location = "testLocation1"
            };

            //Act
            var response = await devicesController.Create(createDeviceRequest);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var deviceReceived = values.As<Device>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
            Assert.True(DevicesComparer.CompareDevices(deviceReceived, expectedDevice));
        }

        [Fact]
        public async Task Delete_WithValidParameters_DeviceIsDeleted()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(Delete_WithValidParameters_DeviceIsDeleted));
            var devicesController = new DevicesController(dbContext);

            //Act
            var response = await devicesController.Delete(1);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var deviceReceived = values.As<Device>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task Delete_WithoutAnExistingDevice_ReturnNotFound()
        {
            //Arrange
            var dbContext = DbContextMocker.GetDbContext(nameof(Delete_WithoutAnExistingDevice_ReturnNotFound));
            var devicesController = new DevicesController(dbContext);

            //Act
            var response = await devicesController.Delete(4);
            var result = (ObjectResult)response.Result;
            var values = result.Value;
            var deviceReceived = values.As<Device>();

            dbContext.Dispose();

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

    }
}
