using IdolyPrideLocalizator.ContentDialogs;
using IdolyPrideLocalizator.DataTypes;
using IdolyPrideLocalizator.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace IdolyPrideLocalizator;

public sealed partial class MainWindow : Window
{
    private string _currentFileContent;
    private readonly ObservableCollection<MessageViewModel> _viewModels = [];

    public MainWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon("Icon.ico");
        SystemBackdrop = new MicaBackdrop();
        ExtendsContentIntoTitleBar = true;
        IrMain.ItemsSource = _viewModels;
        SetTitleBar(TitleBar);
    }

    private async void OnOpenRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        if (_currentFileContent != null)
        {
            var dialog = Content.GenerateMessageDialog("Warning", "You have unsaved changes.\nDo you want to continue?", "Yes", "No");
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Secondary) return;
        }

        var fileOpenPicker = new FileOpenPicker();

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hWnd);

        fileOpenPicker.ViewMode = PickerViewMode.List;
        fileOpenPicker.FileTypeFilter.Add(".txt");

        // Open the picker for the user to pick a file
        var file = await fileOpenPicker.PickSingleFileAsync();
        if (file == null) return;

        await OpenFileAsync(file.Path);
    }

    private async Task OpenFileAsync(string filePath)
    {
        _viewModels.Clear();

        var content = File.ReadAllText(filePath);
        if (!content.Contains("\n[title title="))
        {
            await Content.ShowMessageDialogAsync("Error", "File format error!");
            return;
        }

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        TitleBar.Subtitle = fileName;


        var lines = content.Replace("\r", string.Empty).Split('\n');
        var title = lines.FirstOrDefault(x => x.StartsWith("[title title=")).Replace("[title title=", string.Empty).Replace("]", string.Empty);
        TbxTitle.Text = title;
        TbxTranslatedTitle.Text = title;

        // Message Example: 
        // [message text= 本戦は、十六グループによるトーナメント形式によって name = 司会者 thumbnial = img_mob_adv_host_host - 00 clip =\{ "_startTime":0.8,"_duration":3.7333333333333336,"_easeInDuration":0.0,"_easeOutDuration":0.0,"_blendInDuration":-1.0,"_blendOutDuration":-1.0,"_mixInEaseType":1,"_mixOutEaseType":1,"_timeScale":1.0\}]
        foreach (var line in lines)
        {
            if (line.StartsWith("[message text="))
            {
                var message = new MessageViewModel();
                var text = line.Replace("[message text=", string.Empty).Split(" name=")[0];
                var name = line.Split(" name=")[1].Split(" thumbnial=")[0];
                message.OriginalText = text;
                message.OriginalName = name;
                message.TranslatedText = text;
                message.TranslatedName = name;
                _viewModels.Add(message);
            }
        }

        TbxSearch.IsEnabled = true;
        FrMain.IsEnabled = true;
        MfiSave.IsEnabled = true;
        MfiReplaceAll.IsEnabled = true;
        _currentFileContent = content;
    }

    private async void OnSaveRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        if (_currentFileContent == null) return;

        var fileSavePicker = new FileSavePicker();

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WinRT.Interop.InitializeWithWindow.Initialize(fileSavePicker, hWnd);

        fileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        fileSavePicker.FileTypeChoices.Add("Text File", [".txt"]);
        fileSavePicker.SuggestedFileName = $"[Translated] {TitleBar.Subtitle}";

        var file = await fileSavePicker.PickSaveFileAsync();
        if (file == null) return;

        var content = _currentFileContent;
        var lines = content.Replace("\r", string.Empty).Split('\n');

        var viewModelQueue = new Queue<MessageViewModel>(_viewModels);
        var newLines = new List<string>();
        var newMessageLines = new List<string>();
        foreach(var line in lines)
        {
            var changedLine = line;
            if(changedLine.StartsWith("[message text="))
            {
                var viewModel = viewModelQueue.Dequeue();
                changedLine = changedLine.Replace(viewModel.OriginalText, viewModel.TranslatedText);
                changedLine = changedLine.Replace(viewModel.OriginalName, viewModel.TranslatedName);
                newMessageLines.Add($"[message text={viewModel.TranslatedText} name={viewModel.TranslatedName}]");
            }
            else if(changedLine.StartsWith("[title title="))
            {
                changedLine = $"[title title={TbxTranslatedTitle.Text.Trim()}]";
                newMessageLines.Add(changedLine);
            }
            newLines.Add(changedLine);
        }

        var newContent = string.Join('\n', newLines);
        var newMessageOnlyContent = string.Join('\n', newMessageLines);
        File.WriteAllText(file.Path, newContent);
        File.WriteAllText(file.Path.Replace(".txt", "_message.txt"), newMessageOnlyContent);
    }

    private void OnExitRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => Environment.Exit(0);

    private async void OnReplaceRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        var replaceContentDialog = new ReplaceContentDialog { XamlRoot = Content.XamlRoot };
        replaceContentDialog.ReplaceAll += OnReplaceAll;
        await replaceContentDialog.ShowAsync();
    }

    private void OnReplaceAll(object sender, ReplaceAllEventArgs e)
    {
        if (_currentFileContent == null) return;

        foreach (var viewModel in _viewModels)
        {
            viewModel.TranslatedText = viewModel.TranslatedText.Replace(e.OriginalText, e.NewText);
            viewModel.TranslatedName = viewModel.TranslatedName.Replace(e.OriginalText, e.NewText);
        }
    }

    private void OnSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (string.IsNullOrEmpty(textBox.Text)) IrMain.ItemsSource = _viewModels;
        else IrMain.ItemsSource = _viewModels.Where(x => x.OriginalText.Contains(textBox.Text, 
            StringComparison.OrdinalIgnoreCase) || x.OriginalName.Contains(textBox.Text, StringComparison.OrdinalIgnoreCase)
            || x.TranslatedText.Contains(textBox.Text, StringComparison.OrdinalIgnoreCase) || x.TranslatedName.Contains(textBox.Text, StringComparison.OrdinalIgnoreCase));
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 1)
            {
                var item = items.First();
                if (_currentFileContent != null)
                {
                    var dialog = Content.GenerateMessageDialog("Warning", "You have unsaved changes.\nDo you want to continue?", "Yes", "No");
                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Secondary) return;
                }

                var filePath = item.Path;
                var extension = Path.GetExtension(filePath);
                if (extension != ".txt")
                {
                    await Content.ShowMessageDialogAsync("Error", "File format error!");
                    return;
                }

                await OpenFileAsync(filePath);
            }
            else
            {
                await Content.ShowMessageDialogAsync("Error", "Please drop only one file!");
                return;
            }
        }
    }

    private async void OnClosed(object sender, WindowEventArgs args)
    {
        args.Handled = true;

        if (_currentFileContent != null)
        {
            var dialog = Content.GenerateMessageDialog("Warning", "You have unsaved changes.\nDo you want to continue?", "Yes", "No");
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Secondary) return;
        }

        Environment.Exit(0);
    }
}
