namespace Hovedopgave.Server.Models
{
    public class Roles
    {
        public enum Role 
        {
            GUEST = 0,
            USER = 1,
            AFFILIATE = 2,
            CREATOR = 3,
            MODERATOR = 4,
            SUPERUSER = 5,
            SYSTEMADMIN = 6
        }

        public enum RoleDB
        {
            user,
            admin
        }

        public static Role GetRoleByName(string role) 
        {
            // Should be changed ofcourse.
            if (role == "admin") 
            {
                return Role.SYSTEMADMIN;
            }
            else 
            {
                return Role.GUEST;
            }
        }
    }
}
