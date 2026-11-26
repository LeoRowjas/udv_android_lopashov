using Sdam.ViewModels;

namespace Sdam.Views;

public partial class SubjectFormPage : ContentPage
{
	readonly SubjectFormViewModel _vm;

	public SubjectFormPage(SubjectFormViewModel vm)
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
