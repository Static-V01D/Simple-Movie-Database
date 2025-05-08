namespace SMDB;

public class Roles
{
    public static readonly string ADMIN = "Admin";
    public static readonly string PLUS = "Plus";

    public static readonly string[] ROLES = [ ADMIN, PLUS ];

    public static bool Check(string? role)
    {
        return ROLES.Contains(role);
    }

}