﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Charm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
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
                    //     entity.SaveMaterialsFromParts(savePath, dynamicParts, ConfigSubsystem.GetUnrealInteropEnabled() || ConfigSubsystem.GetSBoxShaderExportEnabled());
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
    }
}
