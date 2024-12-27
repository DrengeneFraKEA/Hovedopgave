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
            MODERATOR = 5,
            SUPERUSER = 8,
            SYSTEMADMIN = 9
        }

        public static Role GetRoleByName(string role)
        {
            return role.ToUpperInvariant() switch
            {
                "SYSTEMADMIN" => Role.SYSTEMADMIN,
                "SUPERUSER" => Role.SUPERUSER,
                "MODERATOR" => Role.MODERATOR,
                "CREATOR" => Role.CREATOR,
                "AFFILIATE" => Role.AFFILIATE,
                "USER" => Role.USER,
                "GUEST" => Role.GUEST,
                _ => Role.GUEST, // Default case
            };
        }

        public static bool CanChangeRole(Role currentUserRole, Role targetUserRole)
        {
            return currentUserRole > targetUserRole;
        }
    }
}
