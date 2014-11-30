using System;
using System.Collections.Generic;
using System.Linq;
using iQuarc.xUnitEx;
using Microsoft.Practices.Unity;
using Xunit;

namespace iQuarc.AppBoot.Unity.ExplorationTests
{
	public class RegisterTests
	{
		private readonly InjectionMember[] emptyInjectionMembers;

		public RegisterTests()
		{
			emptyInjectionMembers = new InjectionMember[] {};
		}

		[Fact]
		public void RegisterType_FromIsNull_RegistrationMadeWithToType()
		{
			UnityContainer container = new UnityContainer();

			Type to = typeof (SomeBaseClass);
			container.RegisterType(null, to, (string) null, emptyInjectionMembers);

			AssertRegistrationsContain(container, to, to, null);
		}

		[Fact]
		public void RegisterType_ToIsNull_ArgumentNullException()
		{
			UnityContainer container = new UnityContainer();

			Type @from = typeof (SomeBaseClass);
			Action act = () => container.RegisterType(@from, null, (string) null, emptyInjectionMembers);

			act.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void RegisterType_FromIsAnInterfaceOfTo_RegistrationMade()
		{
			UnityContainer container = new UnityContainer();

			container.RegisterType(typeof (ISomeInterface), typeof (SomeInterfaceImp), (string) null, emptyInjectionMembers);

			AssertRegistrationsContain(container, typeof (ISomeInterface), typeof (SomeInterfaceImp), null);
		}

		[Fact]
		public void RegisterType_ToDoesNotInheritFrom_ExceptionExpected()
		{
			UnityContainer container = new UnityContainer();

			Action act = () =>
				container.RegisterType(typeof (SomeInterfaceImp), typeof (SomeBaseClass), "", emptyInjectionMembers);

			act.ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void RegisterType_FromIsNotAbstractButToInheritsIt_RegistrationIsCorrect()
		{
			UnityContainer container = new UnityContainer();

			container.RegisterType(typeof (SomeBaseClass), typeof (SomeSubClass), "", emptyInjectionMembers);

			AssertRegistrationsContain(container, typeof (SomeBaseClass), typeof (SomeSubClass), null);
		}

		[Fact]
		public void RegisterType_TwoNamedContracts_FirstIsOverwritten()
		{

			IUnityContainer container = new UnityContainer()
				.RegisterType<ISomeInterface, SomeInterfaceImp>("MyName")
				.RegisterType<ISomeInterface, SomeInterfaceSecondImp>("MyName");

			List<ISomeInterface> instances = container.ResolveAll<ISomeInterface>().ToList();
			Assert.Equal(1, instances.Count);
		}

		private static void AssertRegistrationsContain(UnityContainer container, Type from, Type to, string name)
		{
			Assert.True(container.Registrations.Any(r =>
				r.RegisteredType == from &&
				r.MappedToType == to &&
				r.Name == name
				),
				"Registrations do not contain expected type registration");
		}

		private interface ISomeInterface
		{
		}

		private class SomeInterfaceImp : ISomeInterface
		{
		}

		public class SomeInterfaceSecondImp : ISomeInterface
		{
		}

		private class SomeBaseClass
		{
		}

		private class SomeSubClass : SomeBaseClass
		{
		}
	}
}