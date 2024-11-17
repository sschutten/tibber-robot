var builder = DistributedApplication.CreateBuilder(args);

var dbserver = builder.AddPostgres("dbserver").WithPgAdmin();

var db = dbserver.AddDatabase("db");

builder.AddProject<Projects.Tibber_Robot_Api>("api")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
