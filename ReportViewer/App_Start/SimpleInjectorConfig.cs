using BAL.Contracts.Repository;
using BAL.Data.Contracts;
using BAL.Data.Repository;
using BAL.Services;
using BAL.Services.Contracts;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ReportViewer
{


    public static class SimpleInjectorConfig
    {
        public static void Register()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.Options.ResolveUnregisteredConcreteTypes = true;


            // Register services
            container.Register<INotificationSenderService, NotificationSenderService>(Lifestyle.Scoped);
             

            // Verify the container
            container.Verify();

            // Set the dependency resolver
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}