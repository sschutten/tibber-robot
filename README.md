# Tibber Robot

The Tibber Robot service simulates a robot moving in an office space
and cleans the places this robot visits. The path of the robot's movement is
described by the starting coordinates and move commands. After the cleaning has
been done, the robot reports the number of unique places cleaned. The service stores
the results into the database and returns the created record in JSON format. The
service listens to the HTTP protocol on port 5000.

## Getting started

### Prerequisites

The following tools are required to run the service:
- Microsoft .NET 8.0 SDK
- Docker

### .NET Aspire

The easiest way to run the service is by launching the .NET Aspire App Host.

1. Run the following command from the root of the repository:
`dotnet run --project ./Tibber.Robot.AppHost/`
2. Click on the link in the console to login to the dashboard

The dashboard provides an overview of the running services and their status. You can view their health, logs, traces and metrics.

- Click on the endpoint of the service named `api` to get to the API documentation page. From here you can test the service by sending requests to the API.
- Click on the endpoint of the service named `dbserver-pgadmin` to get to the pgAdmin dashboard. From here you can easily access the database and view the stored records.

### Docker compose

The service can be run using Docker compose. The following command will build the service and run it in a container:

- `docker build -f "Tibber.Robot.Api/Dockerfile" -t tibberrobotapi .`
- `docker compose up`

## Tests

The service has unit tests and integration tests or can be tested manually using the API documentation page or any HTTP client.

### Unit tests

The unit tests are to test the logic of the robot. The tests are located in the `Tibber.Robot.Tests` project. To run the tests, execute the following command from the root of the repository: `dotnet test ./Tibber.Robot.Tests/`

### Integration tests

The integration tests are to test the API endpoint. It ensures the API returns the exection result from the API and that the result is stored in the database.

The tests are located in the `Tibber.Robot.IntegrationTests` project. To run the tests, execute the following command from the root of the repository: `dotnet test ./Tibber.Robot.IntegrationTests/`

### Manual testing

The API documentation page can be used to test the service manually or any HTTP client can be used to send requests to the service.

There's a sample HTTP request to test the service at the following location: `Tibber.Robot.Api/Tibber.Robot.Api.http`. The file can be opened in Visual Studio or Visual Studio Code with the REST Client extension installed.
