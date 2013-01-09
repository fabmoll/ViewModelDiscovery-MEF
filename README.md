ViewModel Discovery
===================

ViewModelDiscovery-MEF project contains a sample to decouple discovery of ViewModel using [MEF](http://mef.codeplex.com/) and [MVVMLight](http://mvvmlight.codeplex.com/).

I'm using it for another project and I wanted to share it (no fireworks here, just a really simple sample).

App.xaml.cs
-----------

In this file, you'll find two things:

- A static property to return the ViewModelLocator instance

```c#
public static ViewModelLocator Locator
{
  get
	{
		return (ViewModelLocator)Current.Resources["Locator"];
	}
}
```

- A static variable that will contain an [AggregateCatalog](http://msdn.microsoft.com/en-us/library/system.componentmodel.composition.hosting.aggregatecatalog.aspx)

```c#
public static readonly AggregateCatalog Catalog = new AggregateCatalog(new AssemblyCatalog(typeof(App).Assembly));
```

ExportViewModelAttribute.cs
---------------------------

This file contains:

- An ExportAttribute that receives a ViewModelBase type in the constructor (The ViewModelType property will be used within the indexer of the ViewModelContainer).

```c#
[MetadataAttribute]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ExportViewModelAttribute : ExportAttribute
{
	public Type ViewModelType { get; private set; }
	public ExportViewModelAttribute(Type viewModelType)
		: base(typeof(ViewModelBase))
	{
		ViewModelType = viewModelType;
	}
}
```

- An interface (IViewModelMetadata) used as metadata in the ViewModels Lazy Collection property (property defined in the ViewModelContainer)

```c#
public interface IViewModelMetadata
{
	Type ViewModelType { get; }
}
```

ViewModelLocator.cs
-------------------

The important thing in this file is the indexer, used to provide a syntax for XAML:

```c#
//e.g.: DataContext="{Binding Source={StaticResource Locator}, Path=[MainViewModel]}"
public ViewModelBase this[string viewModel]
{
	get
	{
		var vm = _vmContainer[viewModel];

		if (!_viewModels.Contains(vm))
			_viewModels.Add(vm);

		return vm;
	}
}
```

The Locate method availabe to get an instance of a specific ViewModel type in our code (in a method, in code-behind,...)

```c#
public T Locate<T>() where T : ViewModelBase
{
	var vm = _vmContainer.Locate<T>();

	if (!_viewModels.Contains(vm))
		_viewModels.Add(vm);

	return vm;
}
}
```

And in the ViewModelContainer, there is a Collection that will contain all exported ViewModel:

```c#
[ImportMany(typeof(ViewModelBase), AllowRecomposition = true)]
public IEnumerable<Lazy<ViewModelBase, IViewModelMetadata>> ViewModels { get; set; }
```

CustomerViewModel.cs
--------------------

In the CustomerViewModel class, I defined a Name property to display a string in my XAML page.

The Export Attribute used by MEF and the ExportViewModel Attribute used by MEF that contains the ViewModelType property.


The sample
----------

For the sample, you'll find two way to use the ViewModelLocator:

- In the XAML (to bind the DataContext):

```c#
﻿<Window x:Class="ViewModelDiscovery_MEF.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="MainWindow" Height="350" Width="525" DataContext="{Binding Source={StaticResource Locator}, Path=[CustomerViewModel]}">
    <Grid>
        <TextBlock Text="{Binding Name}"></TextBlock>
    </Grid>
</Window>
```

- And in code (to get an instance of our ViewModel):

```c#
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		var vm = App.Locator.Locate<CustomerViewModel>();

		vm.Name = "Hello";
	}
}
```
