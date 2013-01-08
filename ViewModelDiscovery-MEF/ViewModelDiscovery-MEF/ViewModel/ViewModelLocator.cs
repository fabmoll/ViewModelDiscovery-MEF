using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using GalaSoft.MvvmLight;

namespace ViewModelDiscovery_MEF.ViewModel
{
	public interface IViewModelLocator
	{
		T Locate<T>() where T : ViewModelBase;
	}

	public class ViewModelLocator : IViewModelLocator, IDisposable
	{
		private readonly ViewModelContainer _vmContainer;
		private readonly IList<ViewModelBase> _viewModels = new List<ViewModelBase>();


		public ViewModelLocator()
			: this(App.Catalog)
		{
		}

		public ViewModelLocator(ComposablePartCatalog catalog)
		{
			_vmContainer = new ViewModelContainer(catalog);
		}

		//Provide a indexer syntax for XAML([]).
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

		public T Locate<T>() where T : ViewModelBase
		{
			var vm = _vmContainer.Locate<T>();

			if (!_viewModels.Contains(vm))
				_viewModels.Add(vm);

			return vm;
		}

		//Call when the application is closing.
		public void Cleanup()
		{
			foreach (var viewModel in _viewModels)
				viewModel.Cleanup();

			Dispose();
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			_vmContainer.Dispose();
		}

		#endregion
	}



	internal class ViewModelContainer : IViewModelLocator, IDisposable
	{
		private readonly CompositionContainer _container;

		public ViewModelContainer(ComposablePartCatalog catalog)
		{
			if (catalog == null)
				throw new ArgumentException("ComposablePartCatalog parameter cannot be null");

			_container = new CompositionContainer(catalog);
			_container.ComposeParts(this);
		}

		[ImportMany(typeof(ViewModelBase), AllowRecomposition = true)]
		public IEnumerable<Lazy<ViewModelBase, IViewModelMetadata>> ViewModels { get; set; }

		internal ViewModelBase this[string viewModel]
		{
			get
			{
				return ViewModels.Single(v => v.Metadata.ViewModelType.Name.Equals(viewModel)).Value;
			}
		}

		public ExportProvider Container
		{
			get { return _container; }
		}

		#region IViewModelLocator Members

		public T Locate<T>() where T : ViewModelBase
		{
			var vm = _container.GetExport<T>();
			if (vm == null)
				throw new InvalidOperationException();

			return vm.Value;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			_container.Dispose();
		}

		#endregion
	}
}