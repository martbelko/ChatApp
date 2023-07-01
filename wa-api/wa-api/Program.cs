using System.Net;
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

public class Program
{
	private static SecurityUtils CreateSecurityUtils(WebApplicationBuilder builder)
	{
		return new SecurityUtils(
				builder.Configuration["Jwt:RefreshTokenKey"]!,
				builder.Configuration["Jwt:AccessTokenKey"]!,
				int.Parse(builder.Configuration["Token:RefreshTokenDays"]!),
				int.Parse(builder.Configuration["Token:RefreshTokenDays"]!),
				builder.Configuration["Jwt:Issuer"]!,
				builder.Configuration["Jwt:Audience"]!
		);
	}

	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services
			.AddHttpContextAccessor()
			.AddCors()
			.AddTransient(provider => CreateSecurityUtils(builder))
			.AddPooledDbContextFactory<WaDbContext>((provider, options) =>
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
				var securityUtils = CreateSecurityUtils(builder);
				options.TokenValidationParameters = securityUtils.GenerateAccessTokenValidationParams();
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

		if (!builder.Environment.IsDevelopment())
		{
			builder.Services.AddHttpsRedirection(options =>
			{
				options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
				options.HttpsPort = 443;
			});
		}

		var app = builder.Build();

		app.UseCors(x => x
			.AllowAnyMethod()
			.AllowAnyHeader()
			.SetIsOriginAllowed((origin) => true)
			.AllowCredentials());

		app.UseHttpsRedirection();
		app.MapControllers();
		app.UseAuthentication();
		app.UseWebSockets();
		app.MapGraphQL();

		app.Run();
	}
}
