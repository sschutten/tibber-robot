# Tibber Robot 🤖

The Tibber Robot service simulates a robot moving in an office space
and cleans the places this robot visits. The path of the robot's movement is
described by the starting coordinates and move commands. After the cleaning has
been done, the robot reports the number of unique places cleaned. The service stores
the results into the database and returns the created record in JSON format. The
service listens to the HTTP protocol on port 5000.

## Getting started 🚀

### Prerequisites 🛠️

The following tools are required to run the service:
- Microsoft .NET 8.0 SDK
- Docker

### .NET Aspire 👩‍💻

The easiest way to run the service is by launching the .NET Aspire App Host.

1. Run the following command from the root of the repository:
`dotnet run --project ./Tibber.Robot.AppHost/`
2. Click on the link in the console to login to the dashboard

The dashboard provides an overview of the running services and their status. You can view their health, logs, traces and metrics.

- Click on the endpoint of the service named `api` to get to the API documentation page. From here you can test the service by sending requests to the API.
- Click on the endpoint of the service named `dbserver-pgadmin` to get to the pgAdmin dashboard. From here you can easily access the database and view the stored records.

### Docker compose 🐳

The service can be run using Docker compose. The following command will build the service and run it in a container:

- `docker build -f "Tibber.Robot.Api/Dockerfile" -t tibberrobotapi .`
- `docker compose up`

## Tests 🧪

The service has unit tests and integration tests or can be tested manually using the API documentation page or any HTTP client.

### Unit tests

The unit tests are to test the logic of the robot. The tests are located in the `Tibber.Robot.UnitTests` project. To run the tests, execute the following command from the root of the repository: `dotnet test ./Tibber.Robot.UnitTests/`

### Integration tests

The integration tests are to test the API endpoint. It ensures the API returns the execution result from the API and that the result is stored in the database.

The tests are located in the `Tibber.Robot.IntegrationTests` project. To run the tests, execute the following command from the root of the repository: `dotnet test ./Tibber.Robot.IntegrationTests/`

### Manual testing

The API documentation page can be used to test the service manually or any HTTP client can be used to send requests to the service.

There's a sample HTTP request to test the service in at the following location: `Tibber.Robot.Api/Tibber.Robot.Api.http`. The file can be opened in Visual Studio or Visual Studio Code with the REST Client extension installed.

## Considerations 🤔

Here are some considerations that were made during the development of the service:
- The API project contains the API endpoints as well as the database models and the database context. This is to keep the project simple and easy to understand.
- The API response uses the same model as the database model. Since both models would otherwise be the same, it's easier to use the same model for both. In a real-world scenario, the models would probably be (slightly) different and would it make sense to map between the two models.
- The database initialization is part of the startup sequence of the API. This is again to keep things simple and works fine because we have just one instance of the API. In a real-world scenario, where multiple instances of the API may start, the database should be initialized separately from the API start-up.
- As per the notes in the case documentation, all input is considered to be well formed, syntactically correct and the robot will never be sent outside the bounds of the office space. The API does not validate the input and assumes it's correct.
- There's a small typo in the example request in the case documentation. The example request uses `commmands` instead of `commands`. The API uses the correct spelling.