using Plugin.Maui.Audio;
namespace gra_rpg;

public partial class Page6 : ContentPage
{

	int nutki = 0;
    List<IAudioPlayer> players = new();
    public Page6()
	{
		InitializeComponent();
        LoadSounds();
    }


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
            await Shell.Current.GoToAsync(nameof(Page_7));
            await Shell.Current.GoToAsync(nameof(Page_7));
        }
    }

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
