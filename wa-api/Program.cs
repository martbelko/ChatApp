using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using wa_api.Data;
using wa_api.GraphQL;
using wa_api.GraphQL.Types;

var builder = WebApplication.CreateBuilder(args);
builder.Services
	.AddPooledDbContextFactory<WaDbContext>(options =>
	{
		options.UseNpgsql("Host=localhost:5432;Database=wa_dev;Username=postgres;Password=mysecretpassword");
	})
	.AddGraphQLServer()
	.AddQueryType<Query>()
	.AddType<UserType>()
	.AddType<MessageType>()
	.AddType<ConversationType>()
	.AddFiltering()
	.AddSorting();

var app = builder.Build();
app.MapGraphQL();
app.Run();
