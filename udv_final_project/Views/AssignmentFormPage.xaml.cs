using Sdam.ViewModels;

namespace Sdam.Views;

public partial class AssignmentFormPage : ContentPage
{
	readonly AssignmentFormViewModel _vm;

	public AssignmentFormPage(AssignmentFormViewModel vm)
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
