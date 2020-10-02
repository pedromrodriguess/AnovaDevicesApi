using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevicesApi.Data;
using DevicesApi.Domain;
using DevicesApi.Contracts.Requests;

namespace DevicesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all the devices in the system.
        /// </summary>
        /// <remarks>
        ///     **Route**: GET api/Devices
        ///     
        ///     **Example**:
        ///     
        ///             api/Devices
        /// </remarks>
        /// <response code="200"> Returns all the devices in the system.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetAll()
        {
            return Ok(await _context.Devices.ToListAsync());
        }

        /// <summary>
        /// Returns the specified device.
        /// </summary>
        /// <remarks>
        ///     **Route**: GET api/Devices/id
        ///     
        ///     **Example**:
        ///     
        ///             api/Devices/1
        /// </remarks>
        /// <response code="200"> Returns the device with specified id.</response>
        /// <response code="404"> No device was found with the inserted id.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Device>> GetById(int id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
                return NotFound("No device was found with the inserted device_id: " + id);

            return Ok(device);
        }

        /// <summary>
        /// Updates the specified device.
        /// </summary>
        /// <remarks>
        ///     **Route**: PUT: api/Devices/id 
        ///     
        ///     **Example**:
        ///     
        ///             api/Devices/5
        ///             {
        ///                 "name": "gas tank bp",
        ///                 "location": "porto"
        ///             }
        /// </remarks>
        /// <response code="200"> The specified device was updated.</response>
        /// <response code="404"> No device was found with the inserted id.</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateDeviceRequest updateDeviceRequest)
        {
            var currentDevice = _context.Devices.SingleOrDefault(device => device.Device_id == id);
            if (currentDevice == null)
                return NotFound("No device was found with the inserted device_id: " + id);

            currentDevice.Name = updateDeviceRequest.Name;
            currentDevice.Location = updateDeviceRequest.Location;

            _context.Devices.Update(currentDevice);
            
            var numerOfUpdates = await _context.SaveChangesAsync();
            if(numerOfUpdates > 0)
                return Ok(currentDevice);

            return NotFound("No device was found with the inserted device_id: " + id);
        }

        /// <summary>
        /// Creates a new device.
        /// </summary>
        /// <remarks>
        ///     **Route**: POST: api/Devices
        ///     
        ///     **Example**:
        ///     
        ///             api/Devices
        ///             {
        ///                 "name": "gas tank bp",
        ///                 "location": "porto"
        ///             }
        /// </remarks>
        /// <response code="201"> The specified device was created.</response>
        [HttpPost]
        public async Task<ActionResult<Device>> Create(CreateDeviceRequest createDeviceRequest)
        {
            var device = new Device
            {
                Name = createDeviceRequest.Name,
                Location = createDeviceRequest.Location
            };
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.Device_id }, device);
        }

        /// <summary>
        /// Deletes the specified device.
        /// </summary>
        /// <remarks>
        ///     **Route**: DELETE: api/Devices/id
        ///     
        ///     **Example**:
        ///     
        ///             api/Devices/5
        /// </remarks>
        /// <response code="404"> No device was found with the inserted id.</response>
        /// <response code="200"> The specified device and all its readings were deleted.</response>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Device>> Delete(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
                return NotFound("No device was found with the inserted device_id: " + id);

            var readings = _context.Readings.Where(reading => reading.Device_id == id);

            foreach (var reading in readings)
            {
                _context.Readings.Remove(reading);
            }

            _context.Devices.Remove(device);

            var numberOfDeletes = await _context.SaveChangesAsync();

            if(numberOfDeletes > 0)
                return Ok($"The device with device_id {id} was deleted!");

            return NotFound();
        }
    }
}
