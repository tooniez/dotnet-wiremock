using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

class Program
{
static void Main(string[] args)
{
    int port;
    if (args.Length == 0 || !int.TryParse(args[0], out port))
        port = 8080;

    var server = WireMockServer.Start(port);
    Console.WriteLine("WireMockServer running at {0}", string.Join(",", server.Ports));

    // Order of rules matters. First matching is taken.
    server
        .Given(Request.Create().WithPath(u => u.Contains("x")).UsingGet())
        .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""result"": ""/x with FUNC 200""}"));

    server
        .Given(Request.Create().WithPath("/*").UsingGet())
        .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""msg"": ""Hello world!""}")
        );

    server
        .Given(Request.Create().WithPath("/data").UsingPost().WithBody(b => b != null && b.Contains("e")))
        .RespondWith(Response.Create()
            .WithStatusCode(201)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""result"": ""data posted with FUNC 201""}"));

    server
        .Given(Request.Create().WithPath("/data").UsingPost())
        .RespondWith(Response.Create()
            .WithStatusCode(201)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""result"": ""data posted with 201""}"));

    server
        .Given(Request.Create().WithPath("/data").UsingDelete())
        .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""result"": ""data deleted with 200""}"));

    Console.WriteLine("Press any key to stop the server");
    Console.ReadKey();

    Console.WriteLine("Displaying all requests");
    var allRequests = server.LogEntries;
    Console.WriteLine(JsonConvert.SerializeObject(allRequests, Formatting.Indented));

    Console.WriteLine("Press any key to quit");
    Console.ReadKey();
}
}