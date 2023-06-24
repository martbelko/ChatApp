using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Messages;
using HotChocolate.Execution;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using wa_api.Security;

namespace wa_api
{
	public class SocketSessionInterceptor : DefaultSocketSessionInterceptor
	{
		private static Dictionary<ISocketConnection, string> _sockets = new Dictionary<ISocketConnection, string>();

		public async override ValueTask<ConnectionStatus> OnConnectAsync(
			ISocketConnection connection, InitializeConnectionMessage message,
			CancellationToken cancellationToken)
		{
			if (message.Payload == null)
			{
				return ConnectionStatus.Reject("Payload is empty");
			}

			if (!message.Payload.ContainsKey("authorization"))
			{
				return ConnectionStatus.Reject("Invalid payload");
			}

			var token = message.Payload["authorization"];
			if (token == null)
			{
				return ConnectionStatus.Reject("Token is empty");
			}

			var tokenStr = token.ToString();
			if (tokenStr == null || !tokenStr.StartsWith("Bearer "))
			{
				return ConnectionStatus.Reject("Token is invalid");
			}

			tokenStr = tokenStr.Substring("Bearer ".Length);
			var handler = new JwtSecurityTokenHandler();
			var result = await handler.ValidateTokenAsync(tokenStr, SecurityUtils.GenerateAccessTokenValidationParams());
			if (!result.IsValid)
			{
				return ConnectionStatus.Reject("Token is invalid");
			}

			var username = result.Claims[ClaimTypes.NameIdentifier];
			if (username == null)
			{
				return ConnectionStatus.Reject("Token is invalid");
			}

			_sockets.Add(connection, username.ToString()!);
			return await base.OnConnectAsync(connection, message, cancellationToken);
		}

		public override ValueTask OnRequestAsync(ISocketConnection connection,
			IQueryRequestBuilder requestBuilder,
			CancellationToken cancellationToken)
		{
			if (!connection.Closed)
			{
				connection.HttpContext.Items.Add("username", _sockets[connection]);
				_sockets.Remove(connection);
			}

			return base.OnRequestAsync(connection, requestBuilder, cancellationToken);
		}

		public override ValueTask OnCloseAsync(ISocketConnection connection,
			CancellationToken cancellationToken)
		{
			if (_sockets.ContainsKey(connection))
			{
				_sockets.Remove(connection);
			}

			return base.OnCloseAsync(connection, cancellationToken);
		}
	}
}
