using Sdam.ViewModels;

namespace Sdam.Views;

public partial class AssignmentDetailPage : ContentPage
{
	readonly AssignmentDetailViewModel _vm;

	public AssignmentDetailPage(AssignmentDetailViewModel vm)
	{
		InitializeComponent();
		BindingContext = _vm = vm;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _vm.LoadAsync();
	}
}
