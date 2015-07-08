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

	internal class ExtensionsFactory : IExtensionsFactory
	{
		private readonly Func<IEnumerable<UnityContainerExtension>> containerFactory;
		private readonly Func<IEnumerable<UnityContainerExtension>> chiildFactory;

		public ExtensionsFactory(Func<IEnumerable<UnityContainerExtension>> factory)
			: this(factory, factory)
		{
		}

		public ExtensionsFactory(Func<IEnumerable<UnityContainerExtension>> containerFactory, Func<IEnumerable<UnityContainerExtension>> chiildFactory)
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