using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SilksongMultiplayer
{
    internal static class Utils
    {
        public static Texture2D LoadImage(string filename, int w, int h)
        {
            string folder = Path.Combine(Path.GetDirectoryName(Application.dataPath), "BepInEx", "plugins", "XvX");
            string imagePath = Path.Combine(folder, filename);
            byte[] fileData = File.ReadAllBytes(imagePath);

            Texture2D tex = new Texture2D(w, h);
            tex.LoadImage(fileData);

            return tex;
        }
    }
}
