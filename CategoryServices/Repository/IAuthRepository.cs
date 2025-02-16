using CategoryServices.Models;

namespace CategoryServices.Repository
{
    public interface IAuthRepository
    {
        User AddUser (User user);
        string Login(LoginRequest loginRequest);

        Role AddRole(Role role);

        bool AssignRoleToUser(AddUserRole obj);
    }
}
