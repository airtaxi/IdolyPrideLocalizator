﻿using Microsoft.UI.Xaml;

namespace IdolyPrideLocalizator;

public partial class App : Application
{
    public App()
    {
        DetoursCustomDPI.Handler.OverrideDefaltDpi(192f);
        InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var window = new MainWindow();
        window.Activate();
    }
}
