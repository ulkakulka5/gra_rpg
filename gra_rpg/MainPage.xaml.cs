namespace gra_rpg;

using Plugin.Maui.Audio;// POBRAC!
public partial class MainPage : ContentPage
{
    bool ishovered = false;
    IAudioPlayer? player;
    public MainPage()
    {
        InitializeComponent();
        PlayMusic();
    }


    async void PlayMusic()
    {
        var audioManager = AudioManager.Current;

        var stream = await FileSystem.OpenAppPackageFileAsync("music.mp3");

        player = audioManager.CreatePlayer(stream);

        player.Loop = true;   
        player.Play();
    }

    private async void OnPointerEntered(object sender, EventArgs e)
    {
        var audioManager = AudioManager.Current;

        var stream = await FileSystem.OpenAppPackageFileAsync("click.mp3");

        player = audioManager.CreatePlayer(stream);

        player.Loop = false;
        player.Play();
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
