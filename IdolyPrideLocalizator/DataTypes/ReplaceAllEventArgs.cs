using System;

namespace IdolyPrideLocalizator.DataTypes;

public class ReplaceAllEventArgs : EventArgs
{
    public string OriginalText { get; set; }
    public string NewText { get; set; }

    public ReplaceAllEventArgs(string originalText, string newText)
    {
        OriginalText = originalText;
        NewText = newText;
    }
}
