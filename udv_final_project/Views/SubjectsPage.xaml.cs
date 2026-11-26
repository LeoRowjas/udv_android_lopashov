using Sdam.ViewModels;

namespace Sdam.Views;

public partial class SubjectsPage : ContentPage
{
	readonly SubjectsViewModel _vm;

	public SubjectsPage(SubjectsViewModel vm)
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
