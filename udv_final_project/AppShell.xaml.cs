using Sdam.Helpers;
using Sdam.Views;

namespace Sdam;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Внутренние экраны открываются поверх вкладок
		Routing.RegisterRoute(Routes.Assignment, typeof(AssignmentDetailPage));
		Routing.RegisterRoute(Routes.AssignmentForm, typeof(AssignmentFormPage));
		Routing.RegisterRoute(Routes.Subject, typeof(SubjectDetailPage));
		Routing.RegisterRoute(Routes.SubjectForm, typeof(SubjectFormPage));
	}
}
