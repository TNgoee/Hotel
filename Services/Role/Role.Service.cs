using KhachSan.Models;
using MongoDB.Driver;

namespace KhachSan.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMongoCollection<Role> _role;
        public RoleService(MongoDBService mongoDBService)
        {
            _role = mongoDBService.GetCollection<Role>("Role");
        }

        public async Task CreateRole(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.Id = null;
            await _role.InsertOneAsync(role);
        }

        public async Task<bool> DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _role.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Role>> GetAllRole()
        {
            return await _role.Find(_ => true).ToListAsync();
        }

        public async Task<Role?> GetRoleById(string id)
        {
            return await _role.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateRole(string id, Role role)
        {
            if (string.IsNullOrEmpty(id) || role == null)
            {
                return false;
            }
            var result = await _role.ReplaceOneAsync(s => s.Id == id, role);
            return result.ModifiedCount > 0;
        }
    }
}