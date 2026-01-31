#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif

namespace gra_rpg;

public partial class Page1 : ContentPage
{
    double step = 10;
    double startX;
    double startY;

    public Page1()
    {
        InitializeComponent();

#if WINDOWS
        Loaded += OnLoaded;
#endif
    }
    //test
#if WINDOWS
    void OnLoaded(object? sender, EventArgs e)
    {
        var mauiWindow =
            Microsoft.Maui.Controls.Application.Current!.Windows[0];

        var winuiWindow =
            (mauiWindow.Handler!.PlatformView as Microsoft.UI.Xaml.Window)!;

        if (winuiWindow.Content is UIElement root)
        {
            root.KeyDown += OnKeyDown;
            root.Focus(FocusState.Programmatic);
        }
    }

    void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Left:
                rozia.TranslationX -= step;
                break;

            case VirtualKey.Right:
                rozia.TranslationX += step;
                break;

            case VirtualKey.Up:
                rozia.TranslationY -= step;
                break;

            case VirtualKey.Down:
                rozia.TranslationY += step;
                break;
        }
    }
#endif

    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        var obrazek = (Image)sender;

        if (e.StatusType == GestureStatus.Started)
        {
            
            startX = obrazek.TranslationX;
            startY = obrazek.TranslationY;
        }

        if (e.StatusType == GestureStatus.Running)
        {
         
            obrazek.TranslationX = startX + e.TotalX;
            obrazek.TranslationY = startY + e.TotalY;
        }

      
    }
}
