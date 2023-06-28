using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EntityFramework.Exceptions.PostgreSQL;
using StackExchange.Redis;
using wa_api.Data;
using wa_api.GraphQL;
using wa_api.GraphQL.Types.Users;
using wa_api.GraphQL.Types.Messages;
using wa_api.GraphQL.Types.Conversations;
using wa_api.Security;
using wa_api;
using wa_api.GraphQL.Middlewares.Validate;

var builder = WebApplication.CreateBuilder(args);
SecurityUtils.Init(builder);

builder.Services
	.AddHttpContextAccessor()
	.AddCors()
	.AddPooledDbContextFactory<WaDbContext>(options =>
	{
		options
			.UseNpgsql("Host=localhost:5432;Database=wa_dev;Username=postgres;Password=mysecretpassword") // TODO: Use configuration file instead of raw string
			.UseExceptionProcessor();
	})
	.AddGraphQLServer()
	.AddErrorFilter<GraphQLErrorFilter>()
	.UseValidation()
	.AddSocketSessionInterceptor<SocketSessionInterceptor>()
	.AddAuthorization()
	.AddQueryType<Query>()
	.AddMutationType<Mutation>()
	.AddSubscriptionType<Subscription>()
	.AddType<GenericUserType>()
	.AddType<UserType>()
	.AddType<MessageType>()
	.AddType<ConversationType>()
	.AddInMemorySubscriptions()
	.AddFiltering()
	.AddSorting()
	.ModifyRequestOptions(o =>
	{
		o.Complexity.Enable = true;
		o.Complexity.MaximumAllowed = 800;
		o.Complexity.DefaultComplexity = 1;
	});

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = SecurityUtils.GenerateAccessTokenValidationParams();
	});

var redisConf = new ConfigurationOptions
{
	EndPoints = { builder.Configuration["Redis:EndPoint"]! },
	Password = builder.Configuration["Redis:Password"]
};

builder.Services
	.AddStackExchangeRedisCache(options =>
	{
		options.ConfigurationOptions = redisConf;
		options.InstanceName = SecurityUtils.REDIS_INSTANCE_NAME;
	});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(x => x
	.AllowAnyMethod()
	.AllowAnyHeader()
	.SetIsOriginAllowed((origin) => true)
	.AllowCredentials());

app.MapControllers();
app.UseAuthentication();
app.UseWebSockets();
app.MapGraphQL();

app.Run();
