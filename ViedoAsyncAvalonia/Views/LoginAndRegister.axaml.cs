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
        // �ӵ�ǰ�ؼ���ȡ TopLevel�����ߣ���Ҳ����ʹ�� Window ���á�
        var topLevel = TopLevel.GetTopLevel(this);
        // �����첽�����Դ򿪶Ի���
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


            //// �򿪵�һ���ļ��Ķ�ȡ����
            //await using var stream = await files[0].OpenReadAsync();
            //using var streamReader = new StreamReader(stream);
            //// ���ļ�������������Ϊ�ı���ȡ��
            //var fileContent = await streamReader.ReadToEndAsync();
        }
    }
}