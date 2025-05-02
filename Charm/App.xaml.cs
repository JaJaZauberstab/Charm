﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Win32;
using VersionChecker;

namespace Charm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ApplicationVersion CurrentVersion = new ApplicationVersion("2.6.4");

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Idk why for some people Charm is looking at system32 instead of the exe location...
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (!IsVcRedistInstalled())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Charm requires Visual C++ Redistributables to function properly, would you like to install these now?",
                    "Missing Dependency",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#latest-microsoft-visual-c-redistributable-version",
                        UseShellExecute = true
                    });
                    Environment.Exit(0);
                }
            }

            var args = e.Args;
            if (args.Length > 0)
            {
                uint apiHash = 0;
                int c = 0;
                while (c < args.Length)
                {
                    if (args[c] == "--api")
                    {
                        apiHash = Convert.ToUInt32(args[c + 1]);
                        break;
                    }
                    c++;
                }
                if (apiHash != 0)
                {
                    return; // todo fix api
                    // to check if we need to update caches
                    // PackageHandler.Initialise();
                    //
                    // // Initialise FNV handler -- must be first bc my code is shit
                    // FnvHandler.Initialise();
                    //
                    // // Get all hash64 -- must be before InvestmentHandler
                    // TagHash64Handler.Initialise();
                    //
                    // // Initialise investment
                    // InvestmentHandler.Initialise();
                    //
                    // // InvestmentHandler.DebugAllInvestmentEntities();
                    // // InvestmentHandler.DebugAPIRequestAllInfo();
                    // // InvestmentHandler.DebugAPIRenderMetadata();
                    //
                    // FbxHandler fbxHandler = new FbxHandler();
                    //
                    // TigerHash hash = new TigerHash(apiHash);
                    //
                    // var entities = InvestmentHandler.GetEntitiesFromHash(hash);
                    // string meshName = hash;
                    // string savePath = ConfigSubsystem.GetExportSavePath() + $"/API_{meshName}";
                    // Directory.CreateDirectory(savePath);
                    //
                    // foreach (var entity in entities)
                    // {
                    //     var dynamicParts = entity.Load(ExportDetailLevel.MostDetailed);
                    //     fbxHandler.AddEntityToScene(entity, dynamicParts, ExportDetailLevel.MostDetailed);
                    //     entity.SaveMaterialsFromParts(savePath, dynamicParts, ConfigSubsystem.GetUnrealInteropEnabled() || ConfigSubsystem.GetS2ShaderExportEnabled());
                    //     entity.SaveTexturePlates(savePath);
                    // }
                    //
                    // fbxHandler.InfoHandler.SetMeshName(meshName);
                    // if (ConfigSubsystem.GetUnrealInteropEnabled())
                    // {
                    //     fbxHandler.InfoHandler.SetUnrealInteropPath(ConfigSubsystem.GetUnrealInteropPath());
                    //     AutomatedExporter.SaveInteropUnrealPythonFile(savePath, meshName, AutomatedExporter.EImportType.Entity, ConfigSubsystem.GetOutputTextureFormat());
                    //     //AutomatedExporter.SaveInteropBlenderPythonFile(savePath, meshName, AutomatedExporter.ImportType.Entity, ConfigSubsystem.GetOutputTextureFormat());
                    // }
                    // fbxHandler.ExportScene($"{savePath}/{meshName}.fbx");
                    // Console.WriteLine($"[Charm] Saved all data to {savePath}.");
                    // //Shutdown();
                }
            }
        }

        bool IsVcRedistInstalled()
        {
            // Key for VC++ 2015-2022 Redistributable (x64)
            const string keyPath = @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
            {
                if (key != null)
                {
                    object installed = key.GetValue("Installed");
                    if (installed is int value && value == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    // Idk where else to put this, I don't want to make a whole new file
    public static class StyleHelper
    {
        // BorderThickness attached property
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.RegisterAttached(
                "BorderThickness",
                typeof(Thickness),
                typeof(StyleHelper),
                new PropertyMetadata(new Thickness(1))); // Default thickness

        public static void SetBorderThickness(UIElement element, Thickness value)
        {
            element.SetValue(BorderThicknessProperty, value);
        }

        public static Thickness GetBorderThickness(UIElement element)
        {
            return (Thickness)element.GetValue(BorderThicknessProperty);
        }

        // BackgroundColor attached property
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.RegisterAttached(
                "BackgroundColor",
                typeof(Brush),
                typeof(StyleHelper),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#433C3C41")))); // Default background color

        public static void SetBackgroundColor(UIElement element, Brush value)
        {
            element.SetValue(BackgroundColorProperty, value);
        }

        public static Brush GetBackgroundColor(UIElement element)
        {
            return (Brush)element.GetValue(BackgroundColorProperty);
        }
    }

    public static class UIHelper
    {
        public static void AnimateFadeIn(dynamic obj, float seconds, float to = 1, float from = 0)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                DoubleAnimation fadeInAnimation = new DoubleAnimation();
                fadeInAnimation.From = from;
                fadeInAnimation.To = to;
                fadeInAnimation.Duration = TimeSpan.FromSeconds(seconds);
                obj.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            }), DispatcherPriority.Background);
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                {
                    return t;
                }
                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}

