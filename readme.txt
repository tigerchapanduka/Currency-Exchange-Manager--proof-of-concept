Currency Exchange Manager API(CEMAPI) is an API for converting currencies base on rates retrived from a currency exchange API.

The implementation is currently limited to open licence single base currency conversion for USD.

The rate conversion end point first checks for rates from the cache and retrieves latest rates from third party service if none exists. The caching is performed per currency code in demand
to eliminate the need for string rates that are not going to be used which lightens the resources required per call.

Cache Key expiry removes the key from database and call to this event is documented to be an unreliable requiring that the update be done on the conversion call where it is guaranteed.

Future enhancements could include saving history per currency code and date. The current Historical implementation retrieves all historical records and can be enhanced to include currency code and date.

The historical rates are saved through a background service that runs in a separate thread on the hosted service. This approach is limited to the host being awake. Host services thread goes to sleep if no requests are made in a given time period. This could be moved into a standalone windows service that runs external of the host.

The project is made up of the components below

CEM.Cache   : Implementation for the Redis cache,
CEM.Data    : Implementation for database persistence with Entity Framework ORM.
CEM.Model   : Contains plain c# objects that represent subsets of Domain
CEM.Service : Implements generic or utility functions that are used across the application
CEMAPI      : The web api application with controller classes. 
              Cache,Data and Model.components are referenced by this component and injected into controller classes where required.



----------------------------------------------------------
Technologies and tools.
------------------------------------------------------
visual studio 2022
Windows 11 Pro machine
Microsoft Dotnet core framework 
Dotnet 8
MySQL
Redis7
Docker
Entity Framework Core


--------------------------------------------------
Docker
--------------------------------------------------
Install the latest version of Docker Desktop.
Docker must be switched to Linux version to run Redis.



---------------------------------------------------
MySQL
---------------------------------------------------
MySQL instance was installed locally.
Running CEMAPI in a Docker container will require that the User connecting to the Database be enbled for remote access.
More investigation will need to be carried to determine the solution connecting CEMAPI to MySQL database when running in Docker.
The issue maybe related to reassignment of network ports but literature also points to security feature on the DBMS
not allowing port switching and needing the database user to be configured.
The database tables were created manually using MySQL work bench.

Find the table scrip further below that must be executed.

CEMAPI connects to currencyexchangemanager schema with
username = root
pswd = root






----------------------------------------------------
Entity Framework
----------------------------------------------------
CEMAPI uses Pomelo.EntityFrameworkCore.MySql version 7 for database Object relation mapping.

Install package via visual studio nuget package manager.





-----------------------------------------------------
Redis
-----------------------------------------------------
Redis requires Docker.
Redis 7.0 was used for the application cache.
The version was installed via Docker desktop from Docker hub.
Search for Redis in Docker desktop and select version 7.
Latest version of Redis may require additional Docker configuration as CEMAPI
could not connect to it on both the Windows and Linux containers.
Docker must be switched to Linux container



-----------------------------------------------------
Logging
-----------------------------------------------------
Both Default Microsoft.Extensions.logging injected via object constructors 
and  Seri 3.1.1 custom logging injected via application configuration are implemented in different components of th project.



-----------------------------------------------------
Database Tables Scripts
-----------------------------------------------------

CREATE TABLE `spotrate` (
  `idspotrate` bigint NOT NULL AUTO_INCREMENT,
  `disclaimer` varchar(250) NOT NULL,
  `license` varchar(450) NOT NULL,
  `timestamp` bigint NOT NULL,
  `rates` json DEFAULT NULL,
  PRIMARY KEY (`idspotrate`),
  UNIQUE KEY `iddailyrate_UNIQUE` (`idspotrate`)
) ENGINE=InnoDB AUTO_INCREMENT=533 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;







