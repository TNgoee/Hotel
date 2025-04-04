using KhachSan.Models;

namespace KhachSan.Services
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRole();
        Task<Role?> GetRoleById(string id);
        Task CreateRole(Role role);
        Task<bool> UpdateRole(string id, Role role);
        Task<bool> DeleteRole(string id);
    }
}