using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileExtensions 
{
    public static string GetStreamingAssetsPath()
    {
        return Application.streamingAssetsPath;
// #if UNITY_ANDROID && !UNITY_EDITOR
//             return Path.Combine("jar:file://" + Application.dataPath + "/!assets/", fileName);
// #elif !UNITY_IOS && !UNITY_EDITOR
//             return Application.dataPath + "/Raw/" + fileName;
// #else
//         return Path.Combine(Application.dataPath, "StreamingAssets", fileName);
// #endif
    }

    public static string GetFilePathAsUrl(string filePath)
    {
        var uri = new System.Uri("file://");
        uri = new Uri(uri, filePath);
        // .Replace(" ", "%20")
        return uri.AbsoluteUri;
    }

    public static string ToRelativePath(string absolutePath)
    {
        if (string.IsNullOrEmpty(absolutePath))
        {
            return "";
        }

        if (Path.GetFullPath(absolutePath).StartsWith(Path.GetFullPath(Application.dataPath)))
        {
            return "Assets" + absolutePath.Substring(Application.dataPath.Length);
        }

        return absolutePath;
    }

    public static string RemoveExtention(this string path)
    {
        string ext = System.IO.Path.GetExtension(path);

        if (string.IsNullOrEmpty(ext) == false)
        {
            return path.Remove(path.Length - ext.Length, ext.Length);
        }

        return path;
    }

    public static string ToFileSize(this byte[] file)
    {
        return ToFileSize(file.LongLength);
    }

    public static string ToFileSize(this int bytes)
    {
        return ((long) bytes).ToFileSize();
    }

    private static readonly string[] sizes = {"B", "KB", "MB", "GB", "TB"};

    public static string ToFileSize(this long byteCount)
    {
        if (byteCount == 0)
            return "0" + sizes[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + sizes[place];
    }
}
