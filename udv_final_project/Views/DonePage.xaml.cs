using Sdam.ViewModels;

namespace Sdam.Views;

public partial class DonePage : ContentPage
{
	readonly DoneViewModel _vm;

	public DonePage(DoneViewModel vm)
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
