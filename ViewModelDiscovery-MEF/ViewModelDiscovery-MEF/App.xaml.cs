using System.ComponentModel.Composition.Hosting;
using System.Windows;
using ViewModelDiscovery_MEF.ViewModel;

namespace ViewModelDiscovery_MEF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static readonly AggregateCatalog Catalog = new AggregateCatalog(new AssemblyCatalog(typeof(App).Assembly));

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			Locator.Dispose();
			Catalog.Dispose();
		}

		public static ViewModelLocator Locator
		{
			get
			{
				return (ViewModelLocator)Current.Resources["Locator"];
			}
		}

	}
}
