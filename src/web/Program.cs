using web;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

var app = builder.Build();

await app.RunAsync();
