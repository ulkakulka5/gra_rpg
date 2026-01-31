namespace gra_rpg;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rejestracja trasy, dzięki czemu Shell.Current.GoToAsync(nameof(Page1)) będzie działać
        Routing.RegisterRoute(nameof(Page1), typeof(Page1));
    }
}
