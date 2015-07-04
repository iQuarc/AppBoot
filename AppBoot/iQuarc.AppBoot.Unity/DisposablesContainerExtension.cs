using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace iQuarc.AppBoot.Unity
{
	internal class DisposablesContainerExtension : UnityContainerExtension, IDisposable
	{
		private DisposablesBuilderStrategy strategy;

		protected override void Initialize()
		{
			strategy = new DisposablesBuilderStrategy();
			Context.Strategies.Add(strategy, UnityBuildStage.TypeMapping);
		}

		public void Dispose()
		{
			strategy.Dispose();
			strategy = null;
		}
	}
}