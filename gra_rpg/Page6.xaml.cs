using Plugin.Maui.Audio;
namespace gra_rpg;

/// @brief Strona mini-gry polegającej na zbieraniu i odtwarzaniu nutek
/// @details
/// Gracz klika kolejne nutki, które odtwarzają fragmenty melodii.
/// Po zebraniu wszystkich nut uruchamiana jest cutscenka,
/// a następnie następuje przejście do kolejnego poziomu gry.
public partial class Page6 : ContentPage
{
    /// @brief Licznik zebranych nutek
	int nutki = 0;

    /// @brief Lista odtwarzaczy dźwięków dla nut
    List<IAudioPlayer> players = new();

    /// @brief Konstruktor strony, inicjalizuje komponenty i ładuje dźwięki
    public Page6()
    {
        InitializeComponent();
        LoadSounds();
    }

    /// @brief Obsługuje kliknięcie nutki przez użytkownika
    /// @param sender Źródło zdarzenia (kliknięty przycisk)
    /// @param e Argumenty zdarzenia
    /// @async
    /// @details
    /// Ukrywa klikniętą nutkę, zwiększa licznik, odtwarza odpowiedni dźwięk
    /// oraz po zebraniu wszystkich nut uruchamia cutscenkę i przechodzi do kolejnej strony.
    private async void nutka1_Clicked(object sender, EventArgs e)
    {
        var btn = sender as ImageButton;

        if (btn != null)
            btn.IsVisible = false;

        nutki++;

        if (nutki <= players.Count)
        {
            foreach (var p in players)
                if (p.IsPlaying)
                    p.Stop();

            players[nutki - 1].Play();
        }

        if (nutki == players.Count)
        {
            cutscenka.IsVisible = true;
            Levels.Level = 5;
            cutscenka.Source = "cutscenka.mp4";
            await Task.Delay(10000);
            await Shell.Current.GoToAsync(nameof(Page7));
            await Shell.Current.GoToAsync(nameof(Page7));
        }
    }

    /// @brief Ładuje pliki dźwiękowe i tworzy odtwarzacze
    /// @async
    /// @details
    /// Wczytuje pliki audio z zasobów aplikacji i przypisuje je do listy odtwarzaczy.
    async void LoadSounds()
    {
        var audioManager = AudioManager.Current;

        string[] files =
        {
            "C.mp3","C.mp3","G.mp3","G.mp3",
            "A.mp3","A.mp3","G.mp3",
            "F.mp3","F.mp3","E.mp3","E.mp3",
            "D.mp3","D.mp3","C.mp3"
        };

        foreach (var file in files)
        {
            var stream = await FileSystem.OpenAppPackageFileAsync(file);
            var p = audioManager.CreatePlayer(stream);
            p.Loop = false;
            players.Add(p);
        }
    }
}