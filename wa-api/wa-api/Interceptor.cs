using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Protocols;
using HotChocolate.AspNetCore.Subscriptions.Protocols.Apollo;
using HotChocolate.Execution;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using wa_api.Security;

namespace wa_api
{
	public class SocketSessionInterceptor : DefaultSocketSessionInterceptor
	{
		private SecurityUtils _securityUtils;
		private static Dictionary<ISocketSession, string> _sockets = new Dictionary<ISocketSession, string>();

		public SocketSessionInterceptor(SecurityUtils securityUtils)
		{
			_securityUtils = securityUtils;
		}

		public override async ValueTask<ConnectionStatus> OnConnectAsync(ISocketSession session, IOperationMessagePayload imessage, CancellationToken cancellationToken)
		{
			var message = imessage.As<InitializeConnectionMessage>();
			if (message is null || message.Payload is null)
			{
				return await ValueTask.FromResult(ConnectionStatus.Reject("Payload is empty"));
			}

			JsonElement payload = message.Payload ?? default;
			if (!payload.TryGetProperty("authorization", out var token))
			{
				return await ValueTask.FromResult(ConnectionStatus.Reject("Token is empty"));
			}

			var tokenStr = token.ToString();
			if (tokenStr is null || !tokenStr.StartsWith("Bearer "))
			{
				return await ValueTask.FromResult(ConnectionStatus.Reject("Token is invalid"));
			}

			tokenStr = tokenStr.Substring("Bearer ".Length);
			var handler = new JwtSecurityTokenHandler();
			var result = await handler.ValidateTokenAsync(tokenStr, _securityUtils.GenerateAccessTokenValidationParams());
			if (!result.IsValid)
			{
				return ConnectionStatus.Reject("Token is invalid");
			}

			var username = result.Claims[ClaimTypes.NameIdentifier];
			if (username == null)
			{
				return ConnectionStatus.Reject("Token is invalid");
			}

			_sockets.Add(session, username.ToString()!);
			return await base.OnConnectAsync(session, message, cancellationToken);
		}

		public override ValueTask OnRequestAsync(ISocketSession session,
			string operationSessionId,
			IQueryRequestBuilder requestBuilder,
			CancellationToken cancellationToken)
		{
			if (!session.Connection.IsClosed)
			{
				session.Connection.HttpContext.Items.Add("username", _sockets[session]);
				_sockets.Remove(session);
			}

			return base.OnRequestAsync(session, operationSessionId, requestBuilder, cancellationToken);
		}

		public override ValueTask OnCloseAsync(ISocketSession session, CancellationToken cancellationToken)
		{
			if (_sockets.ContainsKey(session))
			{
				_sockets.Remove(session);
			}

			return base.OnCloseAsync(session, cancellationToken);
		}
	}
}
