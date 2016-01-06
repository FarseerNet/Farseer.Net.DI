##Farseer.net是什么?
        由一批兴趣爱好者针对.Net平台下进行开发的一系列解决方案。

        并且它完全开源在GitHub中托管。
        

Farseer.Net是所有项目的基础组件，以FS命名空间开头


##Farseer.net有哪些功能？
* Farseer.Net.Sql：数据库ORM
  *  支持：Sqlserver/Oledb（Access/Execl）/Sqlite/MySql/Oracle 数据库。
  *  导出为一个html格式的文件，样式一点也不会丢失
    
* Farseer.Net.DI：IOC、DI、AOP
    * 支持无配置化
    * 支持MVC
    * 支持动态代理
    
* Farseer.Net.Redis：NoSql数据库
    * 支持对象映射
    
* Farseer.Net.Log：日志

## Farseer.Net.DI
1. Farseer.Net.DI是一个轻量级、高性能的IoC+Aop解决方案。
2. 基于.net Framework4开发。
3. 支持：无配置化依赖注入。
4. 支持：批量服务注册。
5. 支持：自动属性注入。
6. 支持：基于特性的异常，方法，参数和返回值拦截。
7. 支持：Mvc，Web Api，Wcf，Wpf，Winform。

### 获取

```cs

    通过nuget获取package：PM> Install-Package Farseer.Net.DI
    
```

### 开始使用

```cs
/// 初始化容器
IFarseerContainer container = new FarseerContainer();
     
///  创建注册器
IDependencyRegister register = container.CreateRegister();
     
///  注册类型
register.RegisterType<IUserRepository, UserRepository>();
     
///  创建解析器
using(IDependencyResolver resolver = container.CreateResolver())
{
     
    ///  解析类型
    IUserRepository repository = resolver.Resolve<IUserRepository>();
}
```
### 依赖注册
```cs
///  使用类型注册
register.RegisterType<IUserRepository, UserRepository>();
    
///  使用类型实例注册
register.RegisterInstance<IUserRepository>(new UserRepository());
    
//  使用委托注册
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
```
### 生命周期
```cs
///  每次解析创建一个新的实例
register.RegisterType<IUserRepository, UserRepository>().AsTransientLifetime();

///  在容器中为单例
register.RegisterType<IUserRepository, UserRepository>().AsSingletonLifetime();

///  在同一作用域中为单例
register.RegisterType<IUserRepository, UserRepository>().AsScopedLifetime();
``` 
### 依赖解析
```cs
///  解析实现特定接口的类型
IUserRepository repository = resolver.Resolve<IUserRepository>();

///  解析实现特定接口的所有类型
IEnumerable<IDependency> dependencys = resolver.ResolveAll<IDependency>();
```  
### 作用域
```cs
using (IDependencyResolver resolver = container.CreateResolver())
{
     ///  创建作用域解析器
     using (IScopedResolver scoped = resolver.CreateScopedResolver())
     {
         IUserRepository repository = scoped.Resolve<IUserRepository>();
     }
}
```

### 自动属性注入  
```cs
public class UserRepository : IUserRepository  
{  
    public ILogger Logger { get; set; }  
}  
  
///  作为自动注入的属性  
register.RegisterType<ILogger, Logger>().AsPropertyDependency();  

register.RegisterType<IUserRepository, UserRepository>();  

using (IDependencyResolver resolver = container.CreateResolver())  
{  
    ///  解析依赖，属性自动注入  
    IUserRepository repository = resolver.Resolve<IUserRepository>();  
}  
```

##申明与呼吁
 * Farseer.net 的初衷不是为了推广其知名度及祈求大家在自己项目上使用它，而是希望大家都参与到这个项目（哪怕仅仅是提供意见也是我非常需要的）。

* 一起研的开发过程，一起见证它的成长。并让大家从中学习到平时可能接触不到的其它知识。我们要的是这个学习氛围。

* 并且你可以完全免费运用到你的任何项目中，不必担心授权问题。

* 如果仅是为了得到源代码，而并没有真正掌握到里面的知识，我觉得这对你一点帮助都没有。成熟的框架太多了，没必要在这个框架上填坑。

* 所以我呼吁大家都能参与到这个项目，和我一起研究，和我一起思考。这就是Farseer.Net框架与其它框架不一样的地方。

##有问题反馈
在使用中有任何问题，欢迎反馈给我们，可以用以下联系方式跟我们交流

* QQ群: 116228666
* ORM教程：http://www.cnblogs.com/steden/
* DI教程：http://www.cnblogs.com/lhyEmpty/

##开源地址
[https://github.com/FarseerNet](https://github.com/FarseerNet "Farseer.net")

##关于作者
[steden](http://www.cnblogs.com/steden/)
[Leandro](http://www.cnblogs.com/lhyEmpty/)
