﻿using Arithmic;
using Tiger.Schema;

namespace Tiger.Commandlets;

public class ExportStaticMeshCommandlet : ICommandlet
{
    public void Run(TigerArgs args)
    {
        string hash;
        if (!args.GetArgValue("hash", out hash))
        {
            Log.Error("No hash argument provided");
        }

        StaticMesh mesh = FileResourcer.Get().GetFile<StaticMesh>(hash);
    }
}
