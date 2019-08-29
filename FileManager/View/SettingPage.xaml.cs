﻿using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Services.Store;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;


namespace FileManager
{
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            Version.Text = string.Format("Version: {0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);

            for (int i = 1; i <= 10; i++)
            {
                SearchNum.Items.Add("前" + (i * 10) + "项结果");
            }

            if (ApplicationData.Current.LocalSettings.Values["SetSearchResultMaxNum"] is string MaxNum)
            {
                SearchNum.SelectedIndex = SearchNum.Items.IndexOf(SearchNum.Items.Where((Item) => Item.ToString().Contains(MaxNum)).FirstOrDefault());
            }
        }

        private void Like_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            LikeSymbol.Foreground = new SolidColorBrush(Colors.Yellow);
            LikeText.Foreground = new SolidColorBrush(Colors.Yellow);
        }

        private void Like_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            LikeSymbol.Foreground = new SolidColorBrush(Colors.White);
            LikeText.Foreground = new SolidColorBrush(Colors.White);
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            MainPage.ThisPage.Nav.Navigate(typeof(AboutMe), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private async void Like_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?productid=9N88QBQKF2RS"));
        }

        private void SearchNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["SetSearchResultMaxNum"] = ((SearchNum.SelectedIndex + 1) * 10).ToString();
        }

        private async void FlyoutContinue_Click(object sender, RoutedEventArgs e)
        {
            ConfirmFly.Hide();
            await SQLite.GetInstance().ClearSearchHistoryRecord();
            ContentDialog dialog = new ContentDialog
            {
                Title = "提示",
                Content = "搜索历史记录清理完成",
                CloseButtonText = "确定",
                Background = Application.Current.Resources["DialogAcrylicBrush"] as Brush
            };
            _ = await dialog.ShowAsync();
        }

        private void FlyoutCancel_Click(object sender, RoutedEventArgs e)
        {
            ConfirmFly.Hide();
        }

        private async void ClearUp_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "警告",
                Content = " 此操作将完全初始化RX文件管理器，包括：\r\r     • 清除全部数据存储\r\r     • 还原所有应用设置\r\r     • RX文件管理器将自动关闭\r\r 您需要按提示重新启动",
                CloseButtonText = "取消",
                PrimaryButtonText = "确认",
                Background = Application.Current.Resources["DialogAcrylicBrush"] as Brush
            };
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                SQLite.GetInstance().Dispose();
                await ApplicationData.Current.ClearAsync();
                ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(GenerateRestartToast().GetXml()));
                Application.Current.Exit();
            }
        }

        public static ToastContent GenerateRestartToast()
        {
            return new ToastContent()
            {
                Launch = "Restart",
                Scenario = ToastScenario.Alarm,

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                        new AdaptiveText()
                        {
                            Text = "需要重新启动RX文件管理器"
                        },

                        new AdaptiveText()
                        {
                            Text = "初始化已完成"
                        },

                        new AdaptiveText()
                        {
                            Text = "请点击以立即重新启动RX"
                        }
                        }
                    }
                },

                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButton("立即启动","Restart")
                        {
                            ActivationType =ToastActivationType.Foreground
                        },
                        new ToastButtonDismiss("稍后")
                    }
                }
            };
        }
    }
}