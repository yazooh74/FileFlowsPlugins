﻿namespace FileFlows.EmailNodes;

public class Plugin : IPlugin
{
    public Guid Uid => new Guid("b5077522-4a31-4faa-b9a7-b409ecb9c01e");
    public string Name => "Email";
    public string MinimumVersion => "0.6.3.1000";

    public void Init()
    {
    }
}
