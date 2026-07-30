namespace carwash.Data.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Cashier = "Cashier";
    public const string User = "User";

    /// <summary>Admin or Cashier — comma-separated for [Authorize(Roles = ...)].</summary>
    public const string Staff = Admin + "," + Cashier;

    public static readonly string[] All = [Admin, Cashier, User];
}
