using System.ComponentModel.Composition;
using GalaSoft.MvvmLight;

namespace ViewModelDiscovery_MEF.ViewModel
{
	[Export(typeof(CustomerViewModel))]
	[ExportViewModel(typeof(CustomerViewModel))]
	public class CustomerViewModel : ViewModelBase
	{
		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				RaisePropertyChanged("Name");
			}
		}
	}
}