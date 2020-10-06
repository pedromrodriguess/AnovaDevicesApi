# AnovaDevicesApi

# Getting started

## How to start the api and database server

This will run the api at http://localhost:8000/

```
docker-compose up --build
```

## Using Swagger
This Api uses the "Swagger" tool to better describe its structure. Access http://localhost:8000/swagger for a better user experience.

# API overview

The following section provides a high level description of the API. 

## Application endpoints

### Devices Controller endpoints

|      | Path             | Method | Description                       			   |  Status codes  |
| ---- | ---------------- | ------ | --------------------------------------------  |  ------------- |
| 1    | /api/Devices/    | GET    | Returns all the devices in the system.		   |  200           |		
| 2    | /api/Devices/id  | GET    | Returns the devices with the specified id.	   |  200, 404	    |
| 3    | /api/Devices/id  | PUT    | Updates the specified device.		           |  200, 404	    |
| 4    | /api/Devices     | POST   | Creates a new device.						   |  201           |
| 5    | /api/Devices/id  | DELETE | Deletes the specified device.                 |  200, 404      |


### Readings Controller endpoints

|      | Path             | Method | Description                       			   |  Status codes  |
| ---- | ---------------- | ------ | --------------------------------------------  |  ------------- |
| 1    | /api/Readings/   | GET    | Returns all the readings in the system.	   |  200           |	
| 2    | /api/Readings/id | GET    | Returns all the readings from the specified   |  200, 404	    |
|	   |				  |        | device.									   |				|
| 3    | /api/Readings/id | GET    | Returns all the readings registered since     |  200, 404	    |
|	   | /startingDate    |        | the given starting datetime from the 		   |				|
|	   |				  |		   | specified device.							   |				|
| 4    | /api/Readings/id | GET    | Returns all the readings registered since     |  200, 404	    |
|	   | /startingDate    |        | the given starting datetime up until the 	   |				|
|	   | /endingDate	  |		   | provided ending datetime, from the specified  |				|
|	   |				  |		   | device.									   |				|
| 5    | /api/Readings	  | POST   | Creates a new reading.						   |  201, 400, 404 |

## Return codes

The following table lists all the used HTTP status codes by the application:

| Code | Description         |
| ---- | ------------------- |
| 200  | OK                  |
| 201  | Created             |
| 400  | Bad Request         |
| 404  | Not Found           |
| 409  | Conflict            |
| 500  | Server Error        |


## Testing

### Tests Information

The api contains unit and integration tests that validate the api endpoints

The unit tests use a in-memory database to better isolate the methods' logic testing.

The integration test populate the containerized database, perform the tests and clean up all the created data.

As recommended in the Docker documentation (https://docs.docker.com/compose/startup-order/), the integration tests
use the "wait-for-it" script to guarantee that the api and db services are fully operational before running.
If you are using a linux or macOS, you have to give the "wait-for-it" file permission for execution.

```
chmod +x "wait-for-it.sh"
```

### Run the tests

```
docker-compose -f docker-compose.tests.yml up --build
```

















