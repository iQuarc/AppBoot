using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Moq;
using Xunit;

namespace iQuarc.AppBoot.Unity.Tests
{
	public class DisposablesBuilderStrategyUnitTests
	{
		[Fact]
		public void PostBuildUp_MoreDisposablesBuilt_DisposablesDisposed()
		{
			DisposablesBuilderStrategy target = new DisposablesBuilderStrategy();

			Disposable disposable1 = new Disposable();
			var context1 = new BuilderContextDouble(target) {Existing = disposable1};

			Disposable disposable2 = new Disposable();
			var context2 = new BuilderContextDouble(target) {Existing = disposable2};


			target.PostBuildUp(context1);
			target.PostBuildUp(context2);
			target.Dispose();


			AssertIsDisposed(disposable1);
			AssertIsDisposed(disposable2);
		}

		[Fact]
		public void PostBuildUp_NotDisposableAndDisposableObjects_DisposablesDisposed()
		{
			DisposablesBuilderStrategy target = new DisposablesBuilderStrategy();
			var context1 = new BuilderContextDouble(target) {Existing = new object()};

			Disposable disposable = new Disposable();
			var context2 = new BuilderContextDouble(target) {Existing = disposable};


			target.PostBuildUp(context1);
			target.PostBuildUp(context2);
			target.Dispose();


			AssertIsDisposed(disposable);
		}

		[Fact]
		public void PostBuildUp_DisposableControlledByParent_NotDisposed()
		{
			DisposablesBuilderStrategy target = new DisposablesBuilderStrategy();
			Disposable disposable = new Disposable();

			Mock<IPolicyList> policyListStub = new Mock<IPolicyList>();
			IPolicyList otherList = new Mock<IPolicyList>().Object;
			policyListStub.Setup(p => p.Get(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>(), out otherList))
				.Returns(new ContainerControlledLifetimeManager());

			IBuilderContext context = new BuilderContextDouble(target, policyListStub.Object)
			{
				Existing = disposable,
			};


			target.PostBuildUp(context);
			target.Dispose();

			AssertIsNotDisposed(disposable);
		}

		private static void AssertIsDisposed(Disposable disposable)
		{
			Assert.True(disposable.IsDisposed, "Instance not disposed, but expected");
		}

		private void AssertIsNotDisposed(Disposable disposable)
		{
			Assert.False(disposable.IsDisposed, "Instance disposed, but NOT expected");
		}

		private class Disposable : IDisposable
		{
			public bool IsDisposed { get; private set; }

			public void Dispose()
			{
				IsDisposed = true;
			}
		}

		private class BuilderContextDouble : IBuilderContext
		{
			public BuilderContextDouble(IBuilderStrategy strategy)
				: this(strategy, GetEmptyPolicyList())
			{
			}

			public BuilderContextDouble(IBuilderStrategy strategy, IPolicyList policies)
			{
				Strategies = new StrategyChain {strategy};
				PersistentPolicies = policies;
				BuildKey = new NamedTypeBuildKey(GetType());
			}

			private static IPolicyList GetEmptyPolicyList()
			{
				Mock<IPolicyList> policiesStub = new Mock<IPolicyList>();
				IPolicyList policies = policiesStub.Object;

				IPolicyList source = policies;
				policiesStub.Setup(x => x.Get(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>(), out source));
				return policies;
			}

			public IStrategyChain Strategies { get; private set; }
			public ILifetimeContainer Lifetime { get; private set; }
			public NamedTypeBuildKey OriginalBuildKey { get; private set; }
			public NamedTypeBuildKey BuildKey { get; set; }
			public IPolicyList PersistentPolicies { get; private set; }
			public IPolicyList Policies { get; private set; }
			public IRecoveryStack RecoveryStack { get; private set; }
			public object Existing { get; set; }
			public bool BuildComplete { get; set; }
			public object CurrentOperation { get; set; }
			public IBuilderContext ChildContext { get; private set; }

			public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
			{
				throw new NotImplementedException();
			}

			public IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType)
			{
				throw new NotImplementedException();
			}

			public object NewBuildUp(NamedTypeBuildKey newBuildKey)
			{
				throw new NotImplementedException();
			}

			public object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock)
			{
				throw new NotImplementedException();
			}
		}
	}
}