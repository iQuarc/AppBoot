using System;
using System.Linq;
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
			if (instance != null && IsNotInherited(context) && IsNotParentContainerControlled(context))
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

		private bool IsNotInherited(IBuilderContext builderContext)
		{
			// unity container puts the parent container strategies before child strategies when it builds the chain
			IBuilderStrategy lastStrategy = builderContext.Strategies.LastOrDefault(s => s is DisposablesBuilderStrategy);
			return ReferenceEquals(this, lastStrategy);
		}

		public void Dispose()
		{
			disposables.Dispose();
		}
	}
}