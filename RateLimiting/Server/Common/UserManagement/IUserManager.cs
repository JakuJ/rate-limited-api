namespace Server.Common.UserManagement
{
    public interface IUserManager
    {
        int? RegisterUser(string username, string password);
        bool ValidateUser(int userId, string username, string password);
    }
}