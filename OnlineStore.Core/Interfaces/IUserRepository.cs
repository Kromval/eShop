using OnlineStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role);
    }
}