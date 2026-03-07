using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System;
using Microsoft.Maui.Graphics;
using System.Threading;
using Microsoft.Maui.Dispatching;


#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif


namespace gra_rpg;

public partial class Page2 : ContentPage
{
    double step = 10;
    double startX;
    double startY;
    bool isNavigating = false;

    List<Rect> clickableAreas = new List<Rect>();
    

    public Page2()
	{
		InitializeComponent();
        clickableAreas.Add(new Rect(1111, 392, 169, 195));

        

#if WINDOWS
        Loaded += OnLoaded;
#endif
    }

#if WINDOWS
    private void OnLoaded(object sender, EventArgs e)
    {
        var mauiWindow = Application.Current!.Windows[0];
        var winuiWindow = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

        if (winuiWindow != null)
        {
            var content = winuiWindow.Content as Microsoft.UI.Xaml.UIElement;
            if (content != null)
            {
                content.KeyDown += Content_KeyDown;
                content.PointerPressed += Content_PointerPressed;
                content.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            }
        }
    }

    private void Content_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        double newX = rozia.TranslationX;
        double newY = rozia.TranslationY;

        switch (e.Key)
        {
            case VirtualKey.Left:  newX -= step; break;
            case VirtualKey.Right: newX += step; break;
            case VirtualKey.Up:    newY -= step; break;
            case VirtualKey.Down:  newY += step; break;
        }

        
    }


    private void Content_PointerPressed(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((Microsoft.UI.Xaml.UIElement)sender);
        double mouseX = point.Position.X;
        double mouseY = point.Position.Y;

        Point mouse = new Point(mouseX, mouseY);

        foreach (var rect in clickableAreas)
        {
            if (rect.Contains(mouse))
            {
                ChestCode.IsVisible = true;
                Closed.IsVisible = true;
                CodeEntry.Text = "";
                CodeEntry.Focus();
            }
        }

    }

#endif



    protected override void OnAppearing()
    {
        base.OnAppearing();

        
        rozia.TranslationX = 549;
        rozia.TranslationY = 270;

    }
    private async  void OnCodeSubmit(object sender, EventArgs e)
    {
        string code = CodeEntry.Text;

        if (code == "158")
        {
            Closed.IsVisible = false;
            Opened.IsVisible = true;
            await Task.Delay(2000);
            ChestCode.IsVisible = false;
            Opened.IsVisible = false;
        }
        else
        {
            Opened.IsVisible = false;
            Closed.IsVisible = true;
            await Task.Delay(2000);
            ChestCode.IsVisible = false;
            Closed.IsVisible = false;
        }
    }









}
