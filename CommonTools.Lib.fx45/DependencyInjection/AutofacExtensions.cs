﻿using Autofac;
using Autofac.Builder;
using Autofac.Core;
using CommonTools.Lib.ns11.ExceptionTools;
using CommonTools.Lib.ns11.StringTools;
using System.Windows;

namespace CommonTools.Lib.fx45.DependencyInjection
{
    public static class AutofacExtensions
    {
        public static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> Solo<T>(this ContainerBuilder buildr)
            => buildr.RegisterType<T>().AsSelf().SingleInstance();

        public static IRegistrationBuilder<TConcrete, ConcreteReflectionActivatorData, SingleRegistrationStyle> Solo<TInterface, TConcrete>(this ContainerBuilder buildr) where TConcrete : TInterface
            => buildr.RegisterType<TConcrete>().As<TInterface>().SingleInstance();

        public static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> Multi<T>(this ContainerBuilder buildr)
            => buildr.RegisterType<T>().AsSelf();

        public static IRegistrationBuilder<TConcrete, ConcreteReflectionActivatorData, SingleRegistrationStyle> Multi<TInterface, TConcrete>(this ContainerBuilder buildr) where TConcrete : TInterface
            => buildr.RegisterType<TConcrete>().As<TInterface>();


        public static bool TryResolveOrAlert<T>(this ILifetimeScope scope, out T component)
        {
            if (scope == null) goto ReturnFalse;
            try
            {
                component = scope.Resolve<T>();
                return true;
            }
            catch (DependencyResolutionException ex)
            {
                MessageBox.Show(ex.GetMessage(), "Failed to Resolve Dependencies", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ReturnFalse:
                component = default(T);
                return false;
        }



        public static string GetMessage(this DependencyResolutionException ex)
        {
            if (ex.InnerException == null)
                return ex.Message;

            if (ex.InnerException.InnerException == null)
                return ex.InnerException.Message;

            var msg = ex.InnerException.InnerException.Message;

            if (msg.Contains("DefaultConstructorFinder"))
            {
                var resolving = msg.Between("DefaultConstructorFinder' on type '", "'");
                var argTyp = msg.Between("Cannot resolve parameter '", " ");
                var argNme = msg.Between(argTyp + " ", "'");
                return $"Check constructor of :{L.f}‹{resolving}›{L.F}"
                     + $"Can't resolve argument “{argNme}” of type :{L.f}‹{argTyp}›";
            }
            else
            {
                return ex.InnerException.InnerException.Info(false, true);
            }
        }
    }
}
