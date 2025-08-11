using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using GORI.ViewModels;

namespace GORI.Services
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();
        public Bootstrapper() => Initialize();

        protected override void Configure()
        {
            _container.Instance(_container);
            // Đăng ký services
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();
            // Đăng ký ViewModels
            GetType().Assembly.GetTypes()
                 .Where(type => type.IsClass)
                 .Where(type => type.Name.EndsWith("ViewModel"))
                 .ToList()
                 .ForEach(viewModelType => _container.RegisterPerRequest(
                     viewModelType, viewModelType.ToString(), viewModelType
                     ));
            //ViewLocator.NameTransformer.AddRule(@"IMGProcess\.ViewModels\.SettingChildViewModels\.(.*)ViewModel", @"IMGProcess.Views.SettingChildViews.${1}View");
            //ViewLocator.NameTransformer.AddRule(@"IMGProcess\.ViewModels\.AutoChildViewModels\.(.*)ViewModel", @"IMGProcess.Views.AutoChildViews.${1}View");
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
        //    if (LicenseChecker.IsLicenseValid())
        //    {
                base.OnStartup(sender, e);
                DisplayRootViewForAsync<MainViewModel>();
            //}
            //else
            //{
            //    base.OnStartup(sender, e);
            //    DisplayRootViewForAsync<LicenseViewModel>();
            //}
        }

        protected override object GetInstance(Type service, string key)
            => _container.GetInstance(service, key);
    }
}
