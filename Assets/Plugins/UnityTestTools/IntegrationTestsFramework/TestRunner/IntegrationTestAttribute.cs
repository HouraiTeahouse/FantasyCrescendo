using System;
using System.IO;

[AttributeUsage(AttributeTargets.Class)]
public class IntegrationTestAttribute : Attribute {

    private readonly string m_Path;

    public IntegrationTestAttribute(string path) {
        if (path.EndsWith(".unity"))
            path = path.Substring(0, path.Length - ".unity".Length);
        m_Path = path;
    }

    public bool IncludeOnScene(string scenePath) {
        if (scenePath == m_Path)
            return true;
        string fileName = Path.GetFileNameWithoutExtension(scenePath);
        return fileName == m_Path;
    }

}