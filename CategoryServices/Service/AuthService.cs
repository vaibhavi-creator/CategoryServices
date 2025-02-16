using CategoryServices.Data;
using CategoryServices.Models;
using CategoryServices.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CategoryServices.Service
{
    public class AuthService: IAuthRepository
    {
        private readonly AppDbContext _appContext;
        private readonly IConfiguration _configuration; 

        public AuthService(AppDbContext appContext, IConfiguration configuration)
        {
            _appContext = appContext;
            _configuration = configuration;
        }

        public Role AddRole(Role role)
        {
            var addedRole = _appContext.Roles.Add(role);
            _appContext.SaveChanges();
            return addedRole.Entity;
        }

        public User AddUser(User user)
        {
            var addedUser = _appContext.Users.Add(user);
            _appContext.SaveChanges();
            return addedUser.Entity;
        }

        public bool AssignRoleToUser(AddUserRole obj)
        {
            try
            {
                var addRoles = new List<UserRole>();
                var user = _appContext.Users.FirstOrDefault(x => x.Id == obj.UserId);
                if (user == null) { throw new Exception("User is not vallid"); }
                foreach (var role in obj.RoleIds)
                {
                    var userRole = new UserRole();
                    userRole.RoleId = role;
                    userRole.UserId = user.Id;
                    addRoles.Add(userRole);
                }
                _appContext.UserRoles.AddRange(addRoles);
                _appContext.SaveChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
            
        }

        public string Login(LoginRequest loginRequest)
        {
            if (loginRequest.UserName != null && loginRequest.Password != null)
            {
                var user = _appContext.Users.SingleOrDefault(
                    s => s.UserName == loginRequest.UserName
                    && s.Password == loginRequest.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim("Id",user.Id.ToString()),
                        new Claim("UserName",user.UserName),
                    };
                    var userRoles = _appContext.UserRoles.Where(u => u.UserId == user.Id).ToList();
                    var roleIds = userRoles.Select(s => s.RoleId).ToList();
                    var roles = _appContext.Roles.Where(r => roleIds.Contains(r.Id)).ToList();
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                         _configuration["Jwt:Audience"],
                         claims,
                         expires: DateTime.UtcNow.AddMinutes(10),
                         signingCredentials: signIn);
                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return jwtToken;
                }
                else
                {
                    throw new Exception("User is not valid");
                }
            }
            else {
                throw new Exception("Credential are not valid");
            }

        }
    }
}
