using IdolyPrideLocalizator.DataTypes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace IdolyPrideLocalizator.ContentDialogs;

public sealed partial class ReplaceContentDialog : ContentDialog
{
    public event EventHandler<ReplaceAllEventArgs> ReplaceAll;
    public ReplaceContentDialog()
    {
        InitializeComponent();
    }

    private void OnReplaceAllButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ReplaceAll?.Invoke(this, new ReplaceAllEventArgs(TbxFind.Text, TbxReplace.Text));
        ReplaceAll = null;
    }

    private void OnCancelButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args) => ReplaceAll = null;
}
