namespace FileFlows.Gotify;

public class Plugin : FileFlows.Plugin.IPlugin
{
    public Guid Uid => new Guid("3d8e13f2-819f-437f-b177-be40147c6e2b");
    public string Name => "Gotify Nodes";
    public string MinimumVersion => "0.6.3.1000";

    public void Init()
    {
    }
}
