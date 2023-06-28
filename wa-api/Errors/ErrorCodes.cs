namespace wa_api.Errors
{
	public enum ErrorCode
	{
		UnknownError,
		ValidationError
	}

	public static class ErrorCodesExtensions
	{
		public static string ToString(this ErrorCode errorCode)
		{
			switch (errorCode)
			{
				case ErrorCode.ValidationError:
					return "VALIDATION_FAILED";
				case ErrorCode.UnknownError:
				default:
					return "Unknown error";
			}
		}
	}
}
