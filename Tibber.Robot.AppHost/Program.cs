var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("db").WithPgAdmin();

builder.AddProject<Projects.Tibber_Robot_Api>("api")
    .WithReference(db);

builder.Build().Run();
