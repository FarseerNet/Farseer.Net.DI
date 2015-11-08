using FS.DI.Tests.DynamicProxy;

namespace FS.DI.Tests.Infrastructure
{
    public class UserRepository : IUserRepository, IRepository<UserEntity>
    {
        public ILogger Logger { get; set; }

        public UserRepository()
        {
            this.Logger = UnKownLogger.Instance;
        }

        [MethodTimer]
        public virtual UserEntity GetById(int id)
        {
            Logger.Debug("UserRepository.GetById Invoke  /  id:" + id);
            return new UserEntity { Id = id, Name = "user" + id };
        }
    }
}
