using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using ViedoAsyncAvalonia.ViewModels;

namespace ViedoAsyncAvalonia.Views;

public partial class LoginAndRegister : UserControl
{
    public LoginAndRegister()
    {
        InitializeComponent();
    }


    private async void OpenAvatarButton_Clicked(object sender, RoutedEventArgs args)
    {
        // 从当前控件获取 TopLevel。或者，您也可以使用 Window 引用。
        var topLevel = TopLevel.GetTopLevel(this);
        // 启动异步操作以打开对话框。
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false,
            
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        });

        if (files.Count >= 1)
        {
            Uri path = files[0].Path;
            if (DataContext is LoginAndRegisterViewModel loginAndRegisterViewModel)
            {
                loginAndRegisterViewModel.SetSelfAvatar(path);
            }


            //// 打开第一个文件的读取流。
            //await using var stream = await files[0].OpenReadAsync();
            //using var streamReader = new StreamReader(stream);
            //// 将文件的所有内容作为文本读取。
            //var fileContent = await streamReader.ReadToEndAsync();
        }
    }
}