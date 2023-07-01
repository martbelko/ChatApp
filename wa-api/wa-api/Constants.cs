namespace wa_api
{
	public static class Constants
	{
		public static readonly (int from, int to) USERNAME_LENGTH_RANGE = (4, 30);
		public static readonly (int from, int to) PASSWORD_LENGTH_RANGE = (8, 128);

		// _@_; 320 characters is maximum lenghth of email according to RFC
		public static readonly (int from, int to) EMAIL_LENGTH_RANGE = (3, 320);
	}
}
