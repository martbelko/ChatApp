using FluentValidation;
using wa_api.Errors;
using wa_api.Models;

namespace wa_api.GraphQL
{
	public class GraphQLErrorFilter : IErrorFilter
	{
		public IError OnError(IError error)
		{
			// TODO: Log error

			switch (error.Exception)
			{
				case ValidationException ex:
					var errors = ex.Errors.ToList();
					var message = errors.Count == 0 ? ex.Message : errors[0].ToString();

					return ErrorBuilder.New()
						.SetMessage(message)
						.SetCode(ErrorCode.ValidationError.ToString())
						.SetExtension("timestamp", DateTime.UtcNow)
						.Build();
			}

			return ErrorBuilder.New()
				.SetMessage(error.Message)
				.SetCode(ErrorCode.UnknownError.ToString())
				.SetExtension("timestamp", DateTime.UtcNow)
				.Build();
		}
	}
}
