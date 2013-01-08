using System.Windows;
using ViewModelDiscovery_MEF.ViewModel;

namespace ViewModelDiscovery_MEF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var vm = App.Locator.Locate<CustomerViewModel>();

			vm.Name = "Hello";
		}
	}
}
