using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace directordemo2
{
    [DependsOn(typeof(directordemo2CoreModule), typeof(AbpAutoMapperModule))]
    public class directordemo2ApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                cfg.AddMaps(typeof(directordemo2ApplicationModule).GetAssembly());
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(directordemo2ApplicationModule).GetAssembly());
        }
    }
}
