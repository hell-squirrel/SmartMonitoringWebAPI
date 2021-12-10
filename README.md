# Introduction 

This repo contains the SmartMonitoringWebAPI application.

# Service Description
Service to keep track of all the services that are running on the cluster and get the information that is needed to deploy.

Applitation provide CRUD based REST API.
For storage embedded database is used(SQLite). Can be swapped by implementing interface contract <code>IAssignmentRepository</code>.

##### Fields:

* `name`: The service name, must be unique (should be a name from 4 to 30 characters).
* `port`: Which port the service should run on (should be a valid port).
* `maintainer`: The person that is responsible for the service (should be a valid email).
* `labels`: Can contain multiple labels.

### Endpoint overview

* **Get all Monitoring Assignments**
  `GET` HTTP with optional fields and pagination
  *example*: `?label=test&pageSize=100&pageNumber=1`
  * label - Search by assignment label
  * pageSize - Page size
  * pageNumber - Page Number

<br/><br/>
*  **Create new Assignment with labels if required**
   `POST` HTTP with body:
    >  {     
            "name": "string",
            "port": 0,
            "maintainer": "string",
            "labels": ["string"] 
    >  }

    *example*: /services

<br/><br/>
* **Get Assignment by name**
  `GET` HTTP 
  *example*: `?services/assignmentName`
  * assignmentName - Name of assignment

<br/><br/>
* **Update Assignment by name**
  `PUT` HTTP with body
    >{
        "port": 0,
        "maintainer": "string",
        "labels": ["string"]
    >}

  *example*: `?services/assignmentName`
  * assignmentName - Name of assignment

<br/><br/>
* **Delete Assignment by name**
  `DELETE` HTTP 
  *example*: `?services/assignmentName`
  * assignmentName - Name of assignment


### Generic response model

>{
    "id": 0,
    "name": "string",
    "port": 0,
    "maintainer": "string",
    "labels": ["string"]
>}

## Docker Image Build
- Pre-requisites
  - Docker (Linux Container)

No extra dependencies.

To build the container locally:

- Open Powershell and navigate to the /src directory
- Make sure Docker is running
- Run this Powershell command:

```powershell
docker build -t smartmonitoringwebapi .
```
- Wait for the container to build the image

In case of debug update Dockerfile with appropriate parameter (`/p:Configuration=Debug`)

- Run this Powershell command To run the Service:

```powershell
docker run smartmonitoringwebapi --name SmartMonitoringWebAPI
```

## Libraries 
* Serilog.AspNetCore 4.1.0 - Serilog support for ASP.NET Core logging.
* Serilog.Settings.Configuration 3.3.0 - Microsoft.Extensions.Configuration (appsettings.json) support for Serilog.
* Serilog.Sinks.File 5.0.0 - Write Serilog events to text files in plain or JSON format.
* Swashbuckle.AspNetCore 6.2.3 - Swagger tools for documenting APIs built on ASP.NET Core.
* Microsoft.AspNetCore.Http.Abstractions 2.2.0 - ASP.NET Core HTTP object model for HTTP requests and responses and also common extension methods for registering middleware in an IApplicationBuilder.
* Microsoft.Data.Sqlite 6.0.0 - Microsoft.Data.Sqlite is a lightweight ADO.NET provider for SQLite.
* Microsoft.Data.Sqlite.Core 6.0.0 - Microsoft.Data.Sqlite is a lightweight ADO.NET provider for SQLite. This package does not include a copy of the native SQLite library.
* Dapper 2.0.123 - A high performance Micro-ORM supporting SQL Server, MySQL, Sqlite, SqlCE, Firebird etc..
* Microsoft.Extensions.Logging.Abstractions 6.0.0 - Logging abstractions for Microsoft.Extensions.Logging.

For test:

* xunit - 2.4.1 - xUnit.net is a developer testing framework, built to support Test Driven Development, with a design goal of extreme simplicity and alignment with framework features.

## Future improvements
* Caching can be added for `GET` request but lifetime is needed to be discussed.
* Fluentvalidation can be added to provied more complex validation of existing assignments.