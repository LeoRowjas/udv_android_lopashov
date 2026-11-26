using Sdam.ViewModels;

namespace Sdam.Views;

public partial class DeadlinesPage : ContentPage
{
	readonly DeadlinesViewModel _vm;

	public DeadlinesPage(DeadlinesViewModel vm)
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
