using System.Security.Claims;

namespace wa_api
{
    /*
	 * This class is used as indicator that the property is being
	 * considered as personal, thus not shared for other users
	 * other than the user who owns this.
	*/
    public class Personal : Attribute
    {
    }

    /*
	 * Similar to Personal attribute, but only for internal
     * purposes, e.g. not to share with ANYONE
	*/
    public class Internal : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Authorize : Attribute
    {
        public Authorize(ClaimsPrincipal claims)
        {
        }
    }
}
