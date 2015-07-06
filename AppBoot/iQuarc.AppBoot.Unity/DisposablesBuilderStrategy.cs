using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace iQuarc.AppBoot.Unity
{
	internal class DisposablesBuilderStrategy : BuilderStrategy, IDisposable
	{
		private readonly DisposablesBag disposables = new DisposablesBag();

		public override void PostBuildUp(IBuilderContext context)
		{
			if (context != null)
				RecordDisposableInstance(context);

			base.PostBuildUp(context);
		}

		private void RecordDisposableInstance(IBuilderContext context)
		{
			IDisposable instance = context.Existing as IDisposable;
			if (instance != null && IsNotParentContainerControlled(context))
			{
				disposables.Add(instance);
			}
		}

		private bool IsNotParentContainerControlled(IBuilderContext context)
		{
			IPolicyList policySource;
			ILifetimePolicy lifetime = context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey, out policySource);

			bool isParentContainerControlled = lifetime is ContainerControlledLifetimeManager
											   &&
											   !ReferenceEquals(policySource, context.PersistentPolicies);
			return !isParentContainerControlled;
		}

		public void Dispose()
		{
			disposables.Dispose();
		}
	}
}