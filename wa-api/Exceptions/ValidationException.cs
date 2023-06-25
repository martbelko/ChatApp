using wa_api.Models;

namespace wa_api.Exceptions
{
	class ValidationException : Exception
	{
		public ValidationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Don't display call stack as it's irrelevant
		/// </summary>
		public override string StackTrace
		{
			get
			{
				return "";
			}
		}
	}
}
