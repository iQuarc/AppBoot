using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace iQuarc.AppBoot.Unity
{
	internal interface IExtensionsFactory
	{
		IEnumerable<UnityContainerExtension> GetContainerExtensions();
		IEnumerable<UnityContainerExtension> GetChildExtensions();
	}

	internal class FuncExtensionsFactory : IExtensionsFactory
	{
		private readonly Func<IEnumerable<UnityContainerExtension>> containerFactory;
		private readonly Func<IEnumerable<UnityContainerExtension>> chiildFactory;

		public FuncExtensionsFactory(Func<UnityContainerExtension> factory)
		{
			this.containerFactory = () => new[] {factory()};
			this.chiildFactory = () => new[] { factory() };
		}

		public FuncExtensionsFactory(Func<IEnumerable<UnityContainerExtension>> factory)
			: this(factory, factory)
		{
		}

		public FuncExtensionsFactory(Func<IEnumerable<UnityContainerExtension>> containerFactory, Func<IEnumerable<UnityContainerExtension>> chiildFactory)
		{
			this.containerFactory = containerFactory;
			this.chiildFactory = chiildFactory;
		}

		public IEnumerable<UnityContainerExtension> GetContainerExtensions()
		{
			return containerFactory();
		}

		public IEnumerable<UnityContainerExtension> GetChildExtensions()
		{
			return chiildFactory();
		}
	}
	
	internal class EmptyExtensionsFactory : IExtensionsFactory
	{
		public IEnumerable<UnityContainerExtension> GetContainerExtensions()
		{
			yield break;
		}

		public IEnumerable<UnityContainerExtension> GetChildExtensions()
		{
			yield break;
		}
	}
}