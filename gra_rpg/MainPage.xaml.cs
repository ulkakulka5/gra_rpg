namespace gra_rpg;  

public partial class MainPage : ContentPage
{
    bool ishovered = false;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnPointerEntered(object sender, EventArgs e)
    {
        if (ishovered) return;
        ishovered = true;

        await newGameButton.ScaleTo(1.08, 120, Easing.CubicOut);
    }

    private async void OnPointerExited(object sender, EventArgs e)
    {
        ishovered = false;

        await newGameButton.ScaleTo(1.0, 120, Easing.CubicIn);
    }

    async void OnNewGameClicked(object sender, EventArgs e)
    {
        // start
        await Shell.Current.GoToAsync(nameof(Page1));

    }
}
