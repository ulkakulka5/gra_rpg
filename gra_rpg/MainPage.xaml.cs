namespace gra_rpg;

using Plugin.Maui.Audio;

/// @class MainPage
/// @brief G³ówna strona startowa gry RPG.
/// @details Odpowiada za odtwarzanie muzyki w tle oraz obs³ugê interakcji u¿ytkownika
/// takich jak najechanie kursorem i rozpoczêcie nowej gry.
public partial class MainPage : ContentPage
{
    /// @brief Flaga informuj¹ca czy przycisk jest aktualnie najechany kursorem.
    bool ishovered = false;

    /// @brief Odtwarzacz audio wykorzystywany do muzyki i efektów dŸwiêkowych.
    IAudioPlayer? player;

    /// @brief Konstruktor strony g³ównej.
    /// @details Inicjalizuje komponenty oraz uruchamia muzykê w tle.
    public MainPage()
    {
        InitializeComponent();
        PlayMusic();
    }

    /// @brief Odtwarza muzykê w tle.
    /// @async
    /// @details £aduje plik "music.mp3" z zasobów aplikacji i ustawia zapêtlanie.
    async void PlayMusic()
    {
        var audioManager = AudioManager.Current;
        var stream = await FileSystem.OpenAppPackageFileAsync("music.mp3");

        player = audioManager.CreatePlayer(stream);
        player.Loop = true;
        player.Play();
    }

    /// @brief Obs³uga zdarzenia najechania kursorem na przycisk.
    /// @param sender Obiekt wywo³uj¹cy zdarzenie.
    /// @param e Argumenty zdarzenia.
    /// @async
    /// @event PointerEntered
    /// @details Odtwarza dŸwiêk klikniêcia i powiêksza przycisk.
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

    /// @brief Obs³uga opuszczenia kursora z przycisku.
    /// @param sender Obiekt wywo³uj¹cy zdarzenie.
    /// @param e Argumenty zdarzenia.
    /// @async
    /// @event PointerExited
    /// @details Przywraca normalny rozmiar przycisku.
    private async void OnPointerExited(object sender, EventArgs e)
    {
        ishovered = false;
        await newGameButton.ScaleTo(1.0, 120, Easing.CubicIn);
    }

    /// @brief Obs³uga klikniêcia przycisku "Nowa Gra".
    /// @param sender Obiekt wywo³uj¹cy zdarzenie.
    /// @param e Argumenty zdarzenia.
    /// @async
    /// @details Przechodzi do strony Page1.
    async void OnNewGameClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Page1));
    }
}