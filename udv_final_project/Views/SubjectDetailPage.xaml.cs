using Sdam.ViewModels;

namespace Sdam.Views;

public partial class SubjectDetailPage : ContentPage
{
	readonly SubjectDetailViewModel _vm;

	public SubjectDetailPage(SubjectDetailViewModel vm)
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
