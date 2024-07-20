using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdolyPrideLocalizator;
public static class DialogHelper
{
    public static ContentDialog GenerateMessageDialog(this UIElement element, string title, string content, string primaryButtonText = "OK", string secondaryButtonText = null)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            PrimaryButtonText = primaryButtonText
        };
        if (secondaryButtonText != null) dialog.SecondaryButtonText = secondaryButtonText;
        dialog.XamlRoot = element.XamlRoot;
        return dialog;
    }

    public static async Task<ContentDialogResult> ShowMessageDialogAsync(this UIElement element, string title, string content, string primaryButtonText = "OK", string secondaryButtonText = null)
    {
        var dialog = element.GenerateMessageDialog(title, content, primaryButtonText, secondaryButtonText);
        return await dialog.ShowAsync();
    }
}
