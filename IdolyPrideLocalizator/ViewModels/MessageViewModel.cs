using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdolyPrideLocalizator.ViewModels
{
    public partial class MessageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _originalText;
        [ObservableProperty]
        private string _originalName;

        [ObservableProperty]
        private string _translatedText;
        [ObservableProperty]
        private string _translatedName;
    }
}
