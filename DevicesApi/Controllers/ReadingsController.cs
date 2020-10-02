using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevicesApi.Data;
using DevicesApi.Domain;

namespace DevicesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReadingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all the readings in the system.
        /// </summary>
        /// <remarks>
        ///     **Route**: GET api/Readings
        ///     
        ///     **Example**:
        ///     
        ///             api/Readings
        /// </remarks>
        /// <response code="200"> Returns all the readings in the system.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reading>>> GetAll()
        {
            return Ok(await _context.Readings.ToListAsync());
        }

        /// <summary>
        /// Returns all the readings from the specified device.
        /// </summary>
        /// <remarks>
        ///     **Route**: GET: api/Readings/5
        ///     
        ///     **Example**:
        ///     
        ///             api/Readings/5
        /// </remarks>
        /// <response code="200"> Returns all the readings from the specified device.</response>
        /// <response code="404"> No device was found with the inserted id.</response>
        [HttpGet("{device_id}")]
        public async Task<ActionResult<Reading>> GetReadingsUsingDeviceId(int device_id)
        {
            var device = await _context.Devices.FindAsync(device_id);
            if (device == null)
                return NotFound($"The specified device_id {device_id} does not exist!");

            var readings = _context.Readings.Where(reading => reading.Device_id == device_id);
            return Ok(readings);
        }

        /// <summary>
        /// Returns all the readings registered since the given starting datetime from the specified device.
        /// </summary>
        /// <remarks>
        ///     **Route**: GET: api/Readings/id/startingDate
        ///     
        ///     **Example**:
        ///     
        ///             api/Readings/5/1601456548
        /// </remarks>
        /// <response code="200"> Returns all the readings, registered since the given starting datetime, from the specified device.</response>
        /// <response code="404"> No device was found with the inserted id.</response>
        [HttpGet("{device_id}/{startingDate}")]
        public async Task<ActionResult<Reading>> GetReadingsUsingDeviceId_Startingdate(int device_id, long startingDate)
        {
            var device = await _context.Devices.FindAsync(device_id);
            if (device == null)
                return NotFound($"The specified device_id {device_id} does not exist!");

            var readings = _context.Readings.Where(reading => reading.Device_id == device_id && reading.Timestamp >= startingDate);
            return Ok(readings);
        }

        // GET: api/Readings/5/date
        /// <summary>
        /// Returns all the readings, registered since the given starting datetime up until the provided ending datetime, from the specified device.
        /// </summary>
        /// <remarks>
        ///     **Route**: GET: api/Readings/id/startingDate/endingDate
        ///     
        ///     **Example**:
        ///     
        ///             api/Readings/5/1601456548/1601456606
        /// </remarks>
        /// <response code="200"> Returns all the readings registered since the given starting datetime from the specified device.</response>
        /// <response code="400"> The specified startingDate is more recent than the endingDate provided.</response>
        /// <response code="404"> No device was found with the inserted id.</response>
        [HttpGet("{device_id}/{startingDate}/{endingDate}")]
        public async Task<ActionResult<Reading>> GetReadingsUsingDeviceId_Startingdate_Endingdate(int device_id, long startingDate, long endingDate)
        {
            var device = await _context.Devices.FindAsync(device_id);
            if (device == null)
                return NotFound($"The specified device_id {device_id} does not exist!");

            if (startingDate > endingDate)
                return BadRequest("The specified startingDate is more recent than the endingDate provided!");

            var readings = _context.Readings.Where(reading => reading.Device_id == device_id && reading.Timestamp >= startingDate && reading.Timestamp <= endingDate);
            return Ok(readings);
        }

        /// <summary>
        /// Creates a new reading.
        /// </summary>
        /// <remarks>
        ///     **Route**: POST: api/Readings
        ///     
        ///     **Example**:
        ///     
        ///             api/Readings
        ///             {
        ///                 "timestamp": 1601456548,
        ///                 "reading_type": "battery",
        ///                 "raw_value": 80,
        ///                 "device_id": 1
        ///             }
        /// </remarks>
        /// <response code="201"> The specified reading was created.</response>
        /// <response code="400"> There is already a reading with the specified timestamp and device id combination.</response>
        [HttpPost]
        public async Task<ActionResult<Reading>> Create(Reading reading)
        {
            var device = await _context.Devices.FindAsync(reading.Device_id);
            if (device == null)
                return NotFound($"The device_id: {reading.Device_id} was not found!");

            if (_context.Readings.Any(existingReading => existingReading.Timestamp.Equals(reading.Timestamp) && existingReading.Device_id == reading.Device_id))
                return BadRequest($"There is already a reading with the specified timestamp: {reading.Timestamp} and device id {device.Device_id}.");

            await _context.Readings.AddAsync(reading);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReading", new { id = reading.Timestamp }, reading);
        }
    }
}
