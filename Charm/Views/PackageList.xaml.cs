﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Arithmic;
using ConcurrentCollections;
using Tiger;
using Tiger.Schema;
using Tiger.Schema.Audio;
using Tiger.Schema.Entity;

namespace Charm;

/// <summary>
/// Interaction logic for PackageList.xaml
/// </summary>
public partial class PackageList : UserControl
{
    private ConcurrentBag<PackageItem> PackageItems;
    public event EventHandler<PackageItem> PackageItemChecked;

    /// PackageList for dummies (me, I'm the dummy):
    /// Step 1: Add element to xaml
    /// Step 2: Add "await PackageList.MakePackageItems<'SchemaType'>();" on usercontrol load
    /// Step 2.5: Add schema type to GetContentType
    /// Step 3: Add "PackageList.PackageItemChecked += async (s, item) =>{ await 'LoadFunction'(item); }; to constructor
    public PackageList()
    {
        InitializeComponent();
    }

    public async Task MakePackageItems<T>()
    {
        if (PackageItems != null)
            return;

        await Task.Run(() =>
        {
            PackageItems = new();
            ConcurrentDictionary<int, ConcurrentHashSet<FileHash>> packageIds = new();
            ConcurrentHashSet<FileHash> hashes = PackageResourcer.Get().GetAllHashes<T>();

            foreach (FileHash hash in hashes)
            {
                if (packageIds.ContainsKey(hash.PackageId))
                    packageIds[hash.PackageId].Add(hash);
                else
                    packageIds[hash.PackageId] = new() { hash };
            }

            Parallel.ForEach(packageIds, pkgId =>
            {
                if (pkgId.Value.Count == 0)
                    return;

                string name = string.Join('_', PackageResourcer.Get().GetPackage((ushort)pkgId.Key).GetPackageMetadata().Name.Split('_').Skip(1).SkipLast(1));
                PackageItems.Add(new PackageItem
                {
                    Name = name,
                    ID = pkgId.Key,
                    Count = pkgId.Value.Count,
                    Hashes = pkgId.Value,
                    Content = GetContentType<T>()
                });
            });
        });

        RefreshPackageList();
    }

    private void RefreshPackageList()
    {
        if (PackageItems == null)
            return;
        if (PackageItems.IsEmpty)
            return;

        string searchStr = SearchBox.Text;

        uint parsedHash = 0;
        bool isHash = Helpers.ParseHash(searchStr, out parsedHash);

        var displayItems = new ConcurrentBag<PackageItem>();
        Parallel.ForEach(PackageItems, pkg =>
        {
            if (isHash && pkg.Hashes.Any(x => x.Hash32 == parsedHash)) // hacky but eh
            {
                IEnumerable<FileHash> hashes = pkg.Hashes.Where(x => x.Hash32 == parsedHash);
                displayItems.Add(new PackageItem
                {
                    Name = pkg.Name,
                    ID = pkg.ID,
                    Count = hashes.Count(),
                    Hashes = new(hashes),
                    Content = pkg.Content
                });
            }
            else if (pkg.Name.Contains(searchStr, StringComparison.OrdinalIgnoreCase))
            {
                displayItems.Add(pkg);
            }
        });

        List<PackageItem> items = displayItems.OrderBy(x => x.Name).ToList();
        PackageListView.ItemsSource = items;
    }

    private void PackageItem_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton btn)
            return;

        if (btn.DataContext is PackageItem item)
        {
            Package pkg = PackageResourcer.Get().GetPackage((ushort)item.ID);
            if (pkg.GetPackageMetadata().Name.Contains("redacted") && !PackageResourcer.Get().Keys.ContainsKey(pkg.GetPackageMetadata().PackageGroup))
            {
                Log.Error($"No decryption key found for package {pkg.GetPackageMetadata().Name}, can not display content.");
                PopupBanner warn = new()
                {
                    Icon = "🔐",
                    Title = "ERROR",
                    Subtitle = "No decryption key found, can not display content.",
                    Description = "This item belongs to a redacted package, which means its content can not be shown.",
                    Style = PopupBanner.PopupStyle.Warning
                };
                warn.Show();

                btn.IsChecked = false;
                return;
            }
            PackageItemChecked?.Invoke(this, item);
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshPackageList();
    }

    private PackageItemContents GetContentType<T>()
    {
        return typeof(T) switch
        {
            Type t when t == typeof(Texture) => PackageItemContents.Textures,
            Type t when t == typeof(Wem) => PackageItemContents.Sounds,
            Type t when t == typeof(Entity) => PackageItemContents.Entities,
            _ => throw new NotImplementedException($"Type {typeof(T)} is not implemented. (implement it)")
        };
    }

    public class PackageItem
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int Count { get; set; }
        public ConcurrentHashSet<FileHash> Hashes { get; set; }
        public bool IsSelected { get; set; } = false;
        public PackageItemContents Content { get; set; }
    }

    /// <summary>
    /// Similar to <see cref="ETagListType"/>, just visually defines the content type of a package item
    /// </summary>
    public enum PackageItemContents
    {
        Textures,
        Sounds,
        Entities,
    }
}

