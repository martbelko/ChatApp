using Microsoft.EntityFrameworkCore;

using wa_api.Data;
using wa_api.GraphQL;
using wa_api.GraphQL.Types.Users;
using wa_api.GraphQL.Types.Messages;
using wa_api.GraphQL.Types.Conversations;
using EntityFramework.Exceptions.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddCors()
    .AddPooledDbContextFactory<WaDbContext>(options =>
    {
        options
            .UseNpgsql("Host=localhost:5432;Database=wa_dev;Username=postgres;Password=mysecretpassword")
            .UseExceptionProcessor();
    })
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddType<UserType>()
    .AddType<MessageType>()
    .AddType<ConversationType>()
    .AddFiltering()
    .AddSorting()
    .AddInMemorySubscriptions();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed((origin) => true)
    .AllowCredentials());

app.UseWebSockets();
app.MapGraphQL();
app.Run();
