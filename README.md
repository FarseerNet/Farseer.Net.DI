官方地址：http://www.cnblogs.com/steden/
官方QQ群：116228666 (Farseer.net开源框架交流) 请注明：博客园

Farseer.Net.DI
-
Farseer.Net.DI是一个轻量级、高性能的IoC容器，用于解耦和管理类之间的依赖关系。

######使用
     /// 初始化容器
     IFarseerContainer container = new FarseerContainer();
     
     ///  创建注册器
     IDependencyRegister register = container.CreateRegister();
     
     ///  注册类型
     register.RegisterType<IUserRepository, UserRepository>();
     
     ///  创建解析器
     using (IDependencyResolver resolver = container.CreateResolver())
     {
     
         ///  解析类型
         IUserRepository repository = resolver.Resolve<IUserRepository>();
     }

######依赖注册

    ///  使用类型注册
    register.RegisterType<IUserRepository, UserRepository>();
    
    ///  使用类型实例注册
    register.RegisterInstance<IUserRepository>(new UserRepository());
    
    ///  使用委托注册
    register.RegisterDelegate<IUserRepository, UserRepository>(
        resolver =>
        {
            return new UserRepository();
        });
        
    ///  注册指定程序集包含的所有类型
    register.RegisterAssembly(Assembly.GetExecutingAssembly());
    
    ///  注册指定程序集中实现特定接口的所有类型
    register.RegisterAssembly<IDependency>(Assembly.GetExecutingAssembly());
    
    ///  注册指定程序集中遵循命名约定的所有类型
    register.RegisterAssembly(Assembly.GetExecutingAssembly(), "Service");
    
    ///  注册程序集中所有符合过滤条件的类型
    register.RegisterAssembly(Assembly.GetExecutingAssembly(), type => type.IsClass);
    
######生命周期

    ///  每次解析创建一个新的实例
    register.RegisterType<IUserRepository, UserRepository>().AsTransientLifetime();

    ///  在容器中为单例
    register.RegisterType<IUserRepository, UserRepository>().AsSingletonLifetime();

    ///  在同一作用域中为单例
    register.RegisterType<IUserRepository, UserRepository>().AsScopedLifetime();
    
######依赖解析

    ///  解析实现特定接口的类型
    IUserRepository repository = resolver.Resolve<IUserRepository>();

    ///  解析实现特定接口的所有类型
    IEnumerable<IDependency> dependencys = resolver.ResolveAll<IDependency>();
    
######作用域

    using (IDependencyResolver resolver = container.CreateResolver())
    {
        ///  创建作用域解析器
        using (IScopedResolver scoped = resolver.CreateScopedResolver())
        {
            IUserRepository repository = scoped.Resolve<IUserRepository>();
        }
    }

######自动属性注入

    public class UserRepository : IUserRepository
    {
        public ILogger Logger { get; set; }
    }
    
    ///  作为自动注入的属性
    register.RegisterType<ILogger, Logger>().AsPropertyInjection();
    
    register.RegisterType<IUserRepository, UserRepository>();
    using (IDependencyResolver resolver = container.CreateResolver())
    {
        ///  解析依赖，属性自动注入
        IUserRepository repository = resolver.Resolve<IUserRepository>();
    }
######忽略自动属性

     
     public class UserRepository : IUserRepository
     {
        ///IgnoreDependency标记的属性将不会被自动注入
        [IgnoreDependency]
        public ILogger Logger { get; set; }
     }
    
