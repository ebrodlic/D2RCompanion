using System;
using System.IO;

namespace D2RCompanion.UI.AppCore;

public class AppPaths
{
    public string Root { get; }
    public string Logs { get; }
    public string Cache { get; }

    public string Screenshots { get; }
    public string Masks { get; }
    public string Tooltips { get; }
    public string Lines { get; }

    public AppPaths(AppInfo appInfo)
    {
        Root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            appInfo.Id);

        Logs = Path.Combine(Root, "Logs");
        Cache = Path.Combine(Root, "Cache");

        Screenshots = Path.Combine(Cache, "Screenshots");
        Masks = Path.Combine(Cache, "Masks");
        Tooltips = Path.Combine(Cache, "Tooltips");
        Lines = Path.Combine(Cache, "Lines");
    }

    public void EnsureCreated()
    {
        Directory.CreateDirectory(Root);
        Directory.CreateDirectory(Logs);
        Directory.CreateDirectory(Cache);

        Directory.CreateDirectory(Screenshots);
        Directory.CreateDirectory(Masks);
        Directory.CreateDirectory(Tooltips);
        Directory.CreateDirectory(Lines);
    }
}
