using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.BusinessLayer
{
    public interface IUserBL
    {
        User InsertUser(User user);
        User UpdateUser(User user);
        IEnumerable<User> GetUsers();
        bool DeleteUser(int userId);
        User GetUser(int Id);
        IQueryable<User> QueryUsers();
        User GetUserByEmailandPassword(string email,string password);

        IEnumerable<User> GetOperator(int customer_id);
        UserResponseDTO ResetPassword(string email, string newPassword);
        User ForgotPassword(string email);
        bool ValidatePassword(string email, string currentPassword);
        User GetOperatorByUniqueId(string userId);
        User GetUserByUserId(string userId);
        User GetOperatorByCustomerId(int customerId, string operatorId);
        int GetUserDbIdByEmail(string email);
    }
}
