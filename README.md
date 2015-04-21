#AppBoot

Generic .Net application boostrapper

##Overview

A lightweight library that handles the startup of an application and abstracts the composite application as concept. It gives a reusable implementation of the *Separate Configuration and Construction from Use* principle.

The two most important steps performed at application startup are: 

 - configure the Dependency Injection Container (DIC), and 
 - initialize the Composite Application

##Dependency Injection Support

AppBoot abstracts and hides the underlying Dependency Injection Container (DIC).

By doing this it enforces consistency on how dependency injection and service location are done in the entire application. It favors constructor dependency injection and encourages programming against interfaces.

It provides annotation based and convention based mechanisms for declaring interface implementations. It is extensible with new custom mechanisms for configuring the DIC.

####Annotation Based Configuration
**`ServiceAttribute`** is used to decorate a type which should be registered into the DIC as implementation to the specified interface (or contract).

Declares `PriceCalculator` as 'default' implementation for `IPriceCalculator`:
```csharp
[Service(typeof (IPriceCalculator), Lifetime.Instance)]
public class PriceCalculator : IPriceCalculator
{
	...
}

public interface IPriceCalculator
{
	...
}
```

Declares more implementations for an interface, with different contract names (named implementations):
```csharp
[Service("Banned Customer Approval", typeof(IApprovalService))]
class BannedCustomer : IApprovalService
{
	public bool Approve(ApproveRequest approveRequest)
	{
		if (IsBanned(approveRequest.Customer))
            return false;
		return true;
	}
}

[Service("Price for Customer Approval", typeof(IApprovalService))]
class PriceForCustomer : IApprovalService
{
	public bool Approve(ApproveRequest approveRequest)
	{
		// check if the order price is to high for the trust we have in this customer
	}
}	

// default implementation for IApprovalService which gets through DI all the others named implementations declared for it
[Service(typeof(IApprovalService))]
class CompositeApprovalService : IApprovalService
{
	private readonly IApprovalService[] approvals;
    public CompositeApprovalService(IApprovalService[] approvals)
    {
		this.approvals = approvals;
	}
}
```
By using the annotations, you can specify on the implementation if its instances should be *Singleton* or a new instance should be created each time. It can be done by using **`Lifetime`** enum on the `ServiceAttribute`. 
```csharp
public enum Lifetime
{
	/// <summary>
	///     New instances are created each time a new object graph is created.
	///     During the scope of build-up of one object graph the created instances are reused.
	/// </summary>
	Instance,

	/// <summary>
	///     Always creates a new instance of this class when it is injected as a dependency.
	/// </summary>
	AlwaysNew,

	/// <summary>
	///     Lives on the application as a singleton instance
	/// </summary>
	Application
}
```
By doing this declaration close to the implementation class, it makes you more careful not to make stateful singletons, or if you do, to synchronize the access in multi-thread environments.

####Convention Based Configuration
Configurations can also be done through conventions.

Registers `UnitOfWork` as 'default' implementation of `IUnitOfWork` interface:
```csharp
conventions.ForType<UnitOfWork>().Export(x => x.AsContractType<IUnitOfWork>());
```

Registers all the types that inherit `Repository` class as implementations for the interfaces they implement, and which name ends with *Repository*:
``` csharp
conventions	.ForTypesDerivedFrom<Repository>()
			.ExportInterfaces(i => i.Name.EndsWith("Repository"));
```
Registers all the types that implement `IApprovalService` and are not named with *Composite*, as named implementations of this interface.
```csharp
conventions	.ForTypesMatching(t =>  t.Implements<IApprovalService>() && 
									!t.Name.Contains("Composite"))
	                   .Export(x => x.AsContractType<IApprovalService>()
	                                 .AsContractName(x.FromType.FullName));
```

##Composite Application Support

AppBoot provides a simple definition of a modular application.

```csharp
internal sealed class Application
{
	private readonly IEnumerable<IModule> modules;
	public Application(IModule[] modules)
	{
		this.modules = modules.OrderByPriority();
	}

	public void Initialize()
	{
		foreach (IModule module in Modules)
			module.Initialize();
	}
}

public interface IModule
{
	void Initialize();
}
```
At application startup all the `IModule` implementations that were registered into the DIC are initialized.

This mechanism, even if it is very simple, it gives a great flexibility in deployment by configuring which modules are available. Multiple logical modules may be defined by implementing `IModule`, and based on the DIC configuration they are initialized at the application startup.

##Getting Started

 - Remove any references to a Dependency Injection framework and use the AppBoot instead. Currently AppBoot uses Unity Container, but it may be configured to work with any other DIC
 - Use attribute annotations, convention based or a custom mechanism to define register interface implementation into the underlying DIC
 - Define logical modules that you want to be initialized at application startup