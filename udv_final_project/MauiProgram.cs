using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Sdam.Helpers;
using Sdam.Services;
using Sdam.ViewModels;
using Sdam.Views;

namespace Sdam;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		CultureInfo.DefaultThreadCurrentCulture = Ru.Culture;
		CultureInfo.DefaultThreadCurrentUICulture = Ru.Culture;

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		RemoveAndroidUnderlines();

		builder.Services.AddSingleton<Database>();

		builder.Services.AddTransient<DeadlinesViewModel>();
		builder.Services.AddTransient<SubjectsViewModel>();
		builder.Services.AddTransient<DoneViewModel>();
		builder.Services.AddTransient<AssignmentDetailViewModel>();
		builder.Services.AddTransient<AssignmentFormViewModel>();
		builder.Services.AddTransient<SubjectDetailViewModel>();
		builder.Services.AddTransient<SubjectFormViewModel>();

		builder.Services.AddTransient<DeadlinesPage>();
		builder.Services.AddTransient<SubjectsPage>();
		builder.Services.AddTransient<DonePage>();
		builder.Services.AddTransient<AssignmentDetailPage>();
		builder.Services.AddTransient<AssignmentFormPage>();
		builder.Services.AddTransient<SubjectDetailPage>();
		builder.Services.AddTransient<SubjectFormPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	// Поля обёрнуты в Border, поэтому системное подчёркивание Android убираем
	static void RemoveAndroidUnderlines()
	{
#if ANDROID
		var transparent = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
		EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, _) => h.PlatformView.BackgroundTintList = transparent);
		EditorHandler.Mapper.AppendToMapping("NoUnderline", (h, _) => h.PlatformView.BackgroundTintList = transparent);
		PickerHandler.Mapper.AppendToMapping("NoUnderline", (h, _) => h.PlatformView.BackgroundTintList = transparent);
		DatePickerHandler.Mapper.AppendToMapping("NoUnderline", (h, _) => h.PlatformView.BackgroundTintList = transparent);
		TimePickerHandler.Mapper.AppendToMapping("NoUnderline", (h, _) => h.PlatformView.BackgroundTintList = transparent);
#endif
	}
}
