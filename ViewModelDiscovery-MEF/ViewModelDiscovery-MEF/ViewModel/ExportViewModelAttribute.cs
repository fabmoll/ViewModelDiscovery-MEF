using System;
using System.ComponentModel.Composition;
using GalaSoft.MvvmLight;

namespace ViewModelDiscovery_MEF.ViewModel
{
	public interface IViewModelMetadata
	{
		Type ViewModelType { get; }
	}

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
}