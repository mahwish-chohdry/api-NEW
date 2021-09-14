using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.BusinessLayer
{
    public class UserBL : IUserBL
    {
        private readonly IUnitOfWork _uow;
        private SmartFanDbContext context;
        private IRepository<User> repository;
        public UserBL()
        {
            context = new SmartFanDbContext();
            _uow = new UnitOfWork<SmartFanDbContext>(context);
            repository = _uow.GetRepository<User>();
        }

  

        public bool DeleteUser(int userId)
        {
            try
            {
                repository.Delete(userId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public User ForgotPassword(string email)
        {
            var user = repository.Queryable().Where(x => x.EmailAddress == email).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public User GetOperatorByCustomerId(int customerId, string operatorId)
        {
            var opUser = GetUsers().Where(x => x.UserId == operatorId && x.CustomerId == customerId).FirstOrDefault();
            return opUser;
        }

        public User GetOperatorByUniqueId(string userId)
        {
            var _operator = QueryUsers().Where(x => x.UserId == userId).FirstOrDefault();
            return _operator;
        }

        public User GetUserByUserId(string userId)
        {
            var _operator = QueryUsers().Where(x => x.UserId == userId).FirstOrDefault();
            return _operator;
        }

        public User GetUser(int Id)
        {
            try
            {
                if (Id <= default(int))
                    throw new ArgumentException("Invalid id");
                return repository.Find(Id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public User GetUserByEmailandPassword(string email, string password)
        {

            User ResponseUser = repository.Queryable().Where(user => user.EmailAddress.ToLower() == email.ToLower() && user.Password == password).FirstOrDefault();
            return ResponseUser;
            
        }

        public int GetUserDbIdByEmail(string email)
        {
            return repository.Queryable().Where(x => x.EmailAddress == email).Select(x => x.Id).FirstOrDefault();
        }

        public IEnumerable<User> GetUsers()
        {
            try
            {
                return repository.GetList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User InsertUser(User user)
        {
            try
            {
                //var customer = _uow.GetRepository<Customer>().Find((int)user.CustomerId);
                //user.CreatedBy = customer.Name;
                repository.Add(user);
                _uow.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<User> QueryUsers()
        {
            try
            {
                return repository.Queryable();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public UserResponseDTO ResetPassword(string email, string newPassword)
        {
            var user = QueryUsers().Where(x => x.EmailAddress == email).FirstOrDefault();
            UserResponseDTO res = new UserResponseDTO();
            res.success = false;
            if (user != null)
            {
                user.Password = newPassword;
                var UpdatedUser = UpdateUser(user);
                if (UpdatedUser != null)
                {
                    res.statusCode = 200;
                    res.success = true;
                    res.user_id = user.Id;
                    res.message = "Password Updated";
                }
                else
                {
                    res.statusCode = 200;
                    res.success = false;
                    res.user_id = user.Id;
                    res.message = "Internal Server Error";
                }
                
            }

            return res;
            
        }

        public User UpdateUser(User user)
        {
            try
            {
                repository.Update(user);
                _uow.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidatePassword(string email, string currentPassword)
        {
            var res = QueryUsers().Where(x => x.EmailAddress == email && x.Password == currentPassword).FirstOrDefault();
            if (res != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        IEnumerable<User> IUserBL.GetOperator(int customer_id)
        {
            var user = repository.Queryable().Where(x => x.CustomerId == customer_id);
            var role = _uow.GetRepository<Userrole>().Queryable().Where(x => x.RoleId == 3);
            var result = from u in user join r in role on u.Id equals r.UserId select(u);
            return result;
        }
    }
}
