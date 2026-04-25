namespace gra_rpg;

using Plugin.Maui.Audio;

/// @class MainPage
/// @brief Główna strona startowa gry RPG.
/// @details Odpowiada za odtwarzanie muzyki w tle oraz obsługę interakcji użytkownika
/// takich jak najechanie kursorem i rozpoczęcie nowej gry.
public partial class Page_7 : ContentPage
{
    /// @brief Flaga informująca czy przycisk jest aktualnie najechany kursorem.
    bool ishovered = false;

    /// @brief Odtwarzacz audio wykorzystywany do muzyki i efektów dźwiękowych.
    IAudioPlayer? player;

    /// @brief Konstruktor strony głównej.
    /// @details Inicjalizuje komponenty oraz uruchamia muzykę w tle.
	public Page_7()
	{
		InitializeComponent();
        PlayMusic();
    }

    /// @brief Odtwarza muzykę w tle.
    /// @async
    /// @details Ładuje plik "music.mp3" z zasobów aplikacji i ustawia zapętlanie.
    async void PlayMusic()
    {
        var audioManager = AudioManager.Current;
        var stream = await FileSystem.OpenAppPackageFileAsync("music.mp3");

        player = audioManager.CreatePlayer(stream);
        player.Loop = true;
        player.Play();
    }

    /// @brief Obsługa zdarzenia najechania kursorem na przycisk.
    /// @param sender Obiekt wywołujący zdarzenie.
    /// @param e Argumenty zdarzenia.
    /// @async
    /// @event PointerEntered
    /// @details Odtwarza dźwięk kliknięcia i powiększa przycisk.
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

    /// @brief Obsługa opuszczenia kursora z przycisku.
    /// @param sender Obiekt wywołujący zdarzenie.
    /// @param e Argumenty zdarzenia.
    /// @async
    /// @event PointerExited
    /// @details Przywraca normalny rozmiar przycisku.
    private async void OnPointerExited(object sender, EventArgs e)
    {
        ishovered = false;
        await newGameButton.ScaleTo(1.0, 120, Easing.CubicIn);
    }

    /// @brief Obsługa kliknięcia przycisku "Nowa Gra".
    /// @param sender Obiekt wywołujący zdarzenie.
    /// @param e Argumenty zdarzenia.
    /// @async
    /// @details Przechodzi do strony Page1.
    async void newGameButton_Clicked(object sender, EventArgs e)
    {
        Levels.Level = 0;
        await Shell.Current.GoToAsync(nameof(Page1));
    }
}