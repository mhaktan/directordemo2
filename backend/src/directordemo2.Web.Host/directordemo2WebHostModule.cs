using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using directordemo2.EntityFrameworkCore;

namespace directordemo2.Web.Host
{
    [DependsOn(typeof(directordemo2ApplicationModule), typeof(directordemo2EntityFrameworkCoreModule), typeof(AbpAspNetCoreModule))]
    public class directordemo2WebHostModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Expose all AppServices as dynamic API controllers
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(directordemo2ApplicationModule).GetAssembly(),
                    moduleName: "app",
                    useConventionalHttpVerbs: true
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(directordemo2WebHostModule).GetAssembly());
        }
    }
}
