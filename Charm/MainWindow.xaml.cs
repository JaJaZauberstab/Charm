﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Tiger;
using Tiger.Schema;
using VersionChecker;

namespace Charm;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public static ProgressView Progress = null;
    private static TabItem _newestTab = null;
    private static LogView _logView = null;
    private static TabItem _logTab = null;
    private bool _bHasInitialised = false;
    public FileVersionInfo GameInfo = null;

    private void OnControlLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
        if (MainMenuTab.Visibility == Visibility.Visible)
        {
            Task.Run(InitialiseHandlers);
            _bHasInitialised = true;
        }

        Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
        CharmIcon.Source = GetBitmapSource(appIcon);
    }

    public MainWindow()
    {
        InitializeComponent();

        Progress = ProgressView;

        int numSingletons = InitialiseStrategistSingletons();

        Strategy.BeforeStrategyEvent += args => { Progress.SetProgressStages(Enumerable.Range(1, numSingletons).Select(num => $"Initialising game version {args.Strategy}: {num}/{numSingletons}").ToList()); };
        Strategy.DuringStrategyEvent += _ => { Progress.CompleteStage(); };
        Strategy.OnStrategyChangedEvent += args =>
        {
            Dispatcher.Invoke(() =>
            {
                // remove all tabs marked with .Tag == 1 as this means we added it manually
                MainTabControl.Items.SourceCollection
                    .Cast<TabItem>()
                    .Where(t => t.Tag is 1 && !t.Header.ToString().Contains("configuration", StringComparison.InvariantCultureIgnoreCase))
                    .ToList()
                    .ForEach(t => MainTabControl.Items.Remove(t));
                CurrentStrategyText.Text = args.Strategy.ToString().Split(".").Last();
            });
        };

        InitialiseSubsystems();

        _logView = new LogView();
        LogHandler.Initialise(_logView);

        // Hide tab by default
        HideMainMenu();

        // Check if packages path exists in config
        // ConfigSubsystem.CheckPackagesPathIsValid();
        ConfigSubsystem config = CharmInstance.GetSubsystem<ConfigSubsystem>();
        if (config.GetPackagesPath(Strategy.CurrentStrategy) != "" && config.GetExportSavePath() != "")
        {
            MainMenuTab.Visibility = Visibility.Visible;

            // Check version
            CheckVersion();

            // Log game version
            CheckGameVersion();
        }
        else
        {
            MakeNewTab("Configuration", new ConfigView());
            SetNewestTabSelected();
        }

        Strategy.AfterStrategyEvent += delegate (StrategyEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                if (Commandlet.RunCommandlet())
                {
                    // Environment.Exit(0);
                }
            });
        };

        if (!ConfigSubsystem.Get().GetAcceptedAgreement())
        {
            PopupBanner warn = new();
            warn.DarkenBackground = true;
            warn.Icon = "⚠️";
            warn.Title = "ATTENTION";
            warn.Subtitle = "Charm is NOT a datamining tool!";
            warn.Description = $"Charm's main purpose is focused towards 3D artists, content preservation and learning how the game works!" +
                $"\n\nBy using Charm, you agree to:" +
                $"\n• Not use this to leak content." +
                $"\n• Not use this to spread spoilers." +
                $"\n\nSeeing leaks come from here makes public releases and updates less and less likely.\nDon't ruin the experience for yourself and others. Uncover things the way they were intended!";

            warn.Style = PopupBanner.PopupStyle.Warning;
            warn.UserInput = "Accept";
            warn.HoldDuration = 4000;
            warn.Progress = true;
            warn.OnProgressComplete += () => ConfigSubsystem.Get().SetAcceptedAgreement(true);
            warn.Show();
        }
    }


    private int InitialiseStrategistSingletons()
    {
        HashSet<Type> lazyStrategistSingletons = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Select(t => t.GetNonGenericParent(typeof(Strategy.LazyStrategistSingleton<>)))
            .Where(t => t is { ContainsGenericParameters: false })
            .Select(t => t.GetNonGenericParent(typeof(Strategy.StrategistSingleton<>)))
            .ToHashSet();

        // Get all classes that inherit from StrategistSingleton<>
        // Then call RegisterEvents() on each of them
        HashSet<Type> allStrategistSingletons = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Select(t => t.GetNonGenericParent(typeof(Strategy.StrategistSingleton<>)))
            .Where(t => t is { ContainsGenericParameters: false })
            .ToHashSet();

        allStrategistSingletons.ExceptWith(lazyStrategistSingletons);

        // order dependencies from InitializesAfterAttribute
        List<Type> strategistSingletons = SortByInitializationOrder(allStrategistSingletons.ToList()).ToList();

        foreach (Type strategistSingleton in strategistSingletons)
        {
            strategistSingleton.GetMethod("RegisterEvents", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        }

        return strategistSingletons.Count;
    }

    private static IEnumerable<Type> SortByInitializationOrder(IEnumerable<Type> types)
    {
        var dependencyMap = new Dictionary<Type, List<Type>>();
        var dependencyCount = new Dictionary<Type, int>();

        // Build dependency map and count dependencies
        foreach (var type in types)
        {
            var attributes = type.GenericTypeArguments[0].GetCustomAttributes(typeof(InitializeAfterAttribute), true);
            foreach (InitializeAfterAttribute attribute in attributes)
            {
                var dependentType = attribute.TypeToInitializeAfter.GetNonGenericParent(
                    typeof(Strategy.StrategistSingleton<>));
                if (!dependencyMap.ContainsKey(dependentType))
                {
                    dependencyMap[dependentType] = new List<Type>();
                    dependencyCount[dependentType] = 0;
                }
                dependencyMap[dependentType].Add(type);
                dependencyCount[type] = dependencyCount.ContainsKey(type) ? dependencyCount[type] + 1 : 1;
            }
        }

        // Perform topological sorting
        var sortedTypes = types.Where(t => !dependencyCount.ContainsKey(t)).ToList();
        var queue = new Queue<Type>(dependencyMap.Keys.Where(k => dependencyCount[k] == 0));
        while (queue.Count > 0)
        {
            var type = queue.Dequeue();
            sortedTypes.Add(type);

            if (dependencyMap.ContainsKey(type))
            {
                foreach (var dependentType in dependencyMap[type])
                {
                    dependencyCount[dependentType]--;
                    if (dependencyCount[dependentType] == 0)
                    {
                        queue.Enqueue(dependentType);
                    }
                }
            }
        }

        if (sortedTypes.Count < types.Count())
        {
            throw new InvalidOperationException("Circular dependency detected.");
        }

        return sortedTypes;
    }

    private void InitialiseSubsystems()
    {
        Arithmic.Log.Info("Initialising Charm subsystems");
        string[] args = Environment.GetCommandLineArgs();
        CharmInstance.Args = new CharmArgs(args);
        CharmInstance.InitialiseSubsystems();
        Arithmic.Log.Info("Initialised Charm subsystems");

    }

    private void CheckGameVersion()
    {
        try
        {
            ConfigSubsystem config = CharmInstance.GetSubsystem<ConfigSubsystem>();
            var path = config.GetPackagesPath(Strategy.CurrentStrategy).Split("packages")[0] + "destiny2.exe";
            var versionInfo = FileVersionInfo.GetVersionInfo(path);
            string version = versionInfo.FileVersion;
            GameInfo = versionInfo;
            Arithmic.Log.Info("Game version: " + version);
        }
        catch (Exception e)
        {
            Arithmic.Log.Error($"Could not get game version error {e}.");
        }
    }

    private async void CheckVersion()
    {
        Arithmic.Log.Info($"Charm Version: {App.CurrentVersion.Id}");
        var versionChecker = new ApplicationVersionChecker("https://github.com/MontagueM/Charm/raw/delta/TFS", App.CurrentVersion);
        versionChecker.LatestVersionName = "version";
        try
        {
            var latestVersion = await versionChecker.GetLatestVersion();
            var latestID = int.Parse(latestVersion.Id.Replace(".", ""));
            var currentID = int.Parse(App.CurrentVersion.Id.Replace(".", ""));

            bool upToDate = currentID >= latestID;
            if (!upToDate)
            {
                //MessageBox.Show($"New version available on GitHub! (local {versionChecker.CurrentVersion.Id} vs ext {versionChecker.LatestVersion.Id})");
                Arithmic.Log.Info($"Version is not up-to-date (local {versionChecker.CurrentVersion.Id} vs ext {latestVersion.Id}).");

                PopupBanner update = new();
                update.DarkenBackground = true;
                update.Icon = "";
                update.Title = "UPDATE AVAILABLE";
                update.Subtitle = "A new Charm version is available!";
                update.Description =
                    $"Current Version: v{App.CurrentVersion.Id}\n" +
                    $"Latest Version: v{latestVersion.Id}";
                update.UserInput = "Update";
                update.UserInputSecondary = "Dismiss";

                update.MouseLeftButtonDown += OpenLatestRelease;
                update.MouseRightButtonDown += update.WarningBanner_MouseDown;

                update.Style = PopupBanner.PopupStyle.Information;
                update.Show();
            }
            else
            {
                Arithmic.Log.Info($"Version is up to date (v{versionChecker.CurrentVersion.Id}, Github v{latestVersion.Id}).");
            }
        }
        catch (Exception e)
        {
            // Could not get or parse version file
#if !DEBUG
            MessageBox.Show("Could not get version.");
#endif
            Arithmic.Log.Error($"Could not get version error {e}.");
        }
    }

    private void OpenLatestRelease(object sender, MouseButtonEventArgs e)
    {
        Process.Start(new ProcessStartInfo { FileName = $"https://github.com/MontagueM/Charm/releases/latest", UseShellExecute = true });
    }

    private async void InitialiseHandlers()
    {
        // Set texture format
        ConfigSubsystem config = CharmInstance.GetSubsystem<ConfigSubsystem>();
        TextureExtractor.SetTextureFormat(config.GetOutputTextureFormat());
    }

    private void OpenConfigPanel_OnClick(object sender, RoutedEventArgs e)
    {
        MakeNewTab("Configuration", new ConfigView());
        SetNewestTabSelected();
    }

    private void OpenLogPanel_OnClick(object sender, RoutedEventArgs e)
    {
        MakeNewTab("Log", _logView);
        SetNewestTabSelected();
    }

    public void HideMainMenu()
    {
        MainMenuTab.Visibility = Visibility.Collapsed;
    }

    public void ShowMainMenu()
    {
        MainMenuTab.Visibility = Visibility.Visible;
        // MainTabControl.SelectedItem = MainMenuTab;
        if (_bHasInitialised == false)
        {
            Task.Run(InitialiseHandlers);
            _bHasInitialised = true;
        }
    }

    public void SetNewestTabSelected()
    {
        MainTabControl.SelectedItem = _newestTab;
    }

    public void SetLoggerSelected()
    {
        MainTabControl.SelectedItem = _logTab;
    }

    public void SetNewestTabName(string newName)
    {
        _newestTab.Header = newName.Replace('_', '.');
    }

    public bool SetCurrentTab(string name)
    {
        // Testing making it all caps
        name = name.ToUpper();
        name = name.Replace('_', '.');
        // Check if the name already exists, if so set newest tab to that
        var items = MainTabControl.Items;
        foreach (TabItem item in items)
        {
            if (name == (string)item.Header)
            {
                _newestTab = item;
                SetNewestTabSelected();
                return true;
            }
        }
        return false;
    }

    public void MakeNewTab(string name, UserControl content)
    {
        // Testing making it all caps
        name = name.ToUpper();
        name = name.Replace('_', '.');
        // Check if the name already exists, if so set newest tab to that
        var items = MainTabControl.Items;
        foreach (TabItem item in items)
        {
            if (name == (string)item.Header)
            {
                _newestTab = item;
                return;
            }
        }

        _newestTab = new TabItem();
        _newestTab.Content = content;
        _newestTab.Tag = 1;
        _newestTab.MouseDown += MenuTab_OnMouseDown;
        _newestTab.HorizontalAlignment = HorizontalAlignment.Left;
        MainTabControl.Items.Add(_newestTab);
        SetNewestTabName(name);
    }

    private void MenuTab_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle && e.Source is TabItem)
        {
            TabItem tab = (TabItem)sender;
            MainTabControl.Items.Remove(tab);
            dynamic content = tab.Content;
            if (content is ActivityView av)
            {
                av.Dispose();
            }
        }
    }

    private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.D && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            MakeNewTab("Dev", new DevView());
            SetNewestTabSelected();
        }
        else if (e.Key == Key.C
                 && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
                 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                 && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        {
            throw new ExternalException("Crash induced. I don't know why you did that but good job.");
        }
        else if (e.Key == Key.Escape)
        {
            var tab = (TabItem)MainTabControl.Items[MainTabControl.SelectedIndex];
            dynamic content = tab.Content;
            if (content is APIItemView || content is CategoryView)
                MainTabControl.Items.Remove(tab);
        }
        else if (e.Key == Key.W
            && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
            && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        {
            PopupBanner test = new();
            test.DarkenBackground = false;
            test.Icon = "ℹ️";
            test.Title = "INFORMATION";
            test.Subtitle = "Test Information Popup Subtitle";
            test.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

            test.Style = PopupBanner.PopupStyle.Information;

            var rootPanel = Application.Current.MainWindow?.Content as Panel;
            rootPanel.Children.Add(test);
        }
        else if (e.Key == Key.E
            && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
            && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        {
            PopupBanner test = new();
            test.DarkenBackground = false;
            test.Icon = "⚠️";
            test.Title = "ERROR";
            test.Subtitle = "Test Error Popup Subtitle";
            test.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\n\nError code: Valumptious";

            test.Style = PopupBanner.PopupStyle.Warning;
            test.UserInput = "Hold To Accept";
            test.HoldDuration = 1000;
            test.Progress = true;

            var rootPanel = Application.Current.MainWindow?.Content as Panel;
            rootPanel.Children.Add(test);
        }
        else if (e.Key == Key.Q
            && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
            && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        {
            PopupBanner test = new();
            test.DarkenBackground = false;
            test.Icon = "💬";
            test.Title = "GENERAL";
            test.Subtitle = "Test General Popup Subtitle";
            test.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            test.UserInput = "Ok";
            test.Style = PopupBanner.PopupStyle.Generic;

            var rootPanel = Application.Current.MainWindow?.Content as Panel;
            rootPanel.Children.Add(test);
        }
    }

    public static BitmapSource GetBitmapSource(Icon icon)
    {
        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                 icon.Handle,
                 new Int32Rect(0, 0, icon.Width, icon.Height),
                 BitmapSizeOptions.FromEmptyOptions());
    }
}
