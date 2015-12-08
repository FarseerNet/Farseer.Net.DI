namespace FS.DI.Tests.Infrastructure
{
    public class UserService : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;

        [IgnoreDependency]
        public ILogger Logger { get; set; }
        public UserService(IRepository<UserEntity> userRepository)
        {
            this._userRepository = userRepository;
            this.Logger = UnKownLogger.Instance;
        }


        public UserEntity GetById(int id)
        {
            Logger.Debug("UserService.GetById Invoke  /  id:" + id);
            return _userRepository.GetById(id);
        }
    }
}
