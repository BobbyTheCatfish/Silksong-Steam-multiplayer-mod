using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DDOLFinder
{
    // Obtain the DDOL scenario
    public static Scene GetDDOLScene()
    {
        var tmp = new GameObject("__ddol_probe__");
        Object.DontDestroyOnLoad(tmp);
        var ddol = tmp.scene;
        Object.DestroyImmediate(tmp);
        return ddol;
    }

    // List the DDOL root nodes
    public static GameObject[] GetDDOLRoots()
    {
        var s = GetDDOLScene();
        return s.IsValid() && s.isLoaded ? s.GetRootGameObjects() : new GameObject[0];
    }

    // In the DDOL scenario, find objects by name (including inactive objects).
    public static List<GameObject> FindInDDOLByName(string name, bool exact = true)
    {
        var result = new List<GameObject>();
        foreach (var root in GetDDOLRoots())
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (!t) continue;
                if (IsMatch(t.name, name, exact))
                    result.Add(t.gameObject);
            }
        }
        return result;
    }

    // Find child objects within a given parent object (including inactive objects)
    public static GameObject? FindChildByName(GameObject parent, string childName, bool exact = true)
    {
        if (!parent) return null;
        foreach (var t in parent.GetComponentsInChildren<Transform>(true))
        {
            if (!t || t.gameObject == parent) continue;
            if (IsMatch(t.name, childName, exact))
                return t.gameObject;
        }
        return null;
    }

    // Search using a "hierarchical path" (e.g., "Canvas/Compass Icon")
    public static GameObject? FindChildByPath(GameObject parent, string path)
    {
        if (!parent || string.IsNullOrEmpty(path)) return null;
        var current = parent.transform;
        var segments = path.Split('/');
        foreach (var seg in segments)
        {
            Transform? next = null;
            foreach (var t in current.GetComponentsInChildren<Transform>(true))
            {
                if (t.parent == current && t.name == seg) { next = t; break; }
            }
            if (next == null) return null;
            current = next;
        }
        return current.gameObject;
    }

    // Print out the hierarchy of the DDOL scenario for easy comparison with the actual names/levels.
    public static void DumpDDOLTree()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== DDOL Tree ===");
        foreach (var root in GetDDOLRoots())
            DumpNode(root.transform, 0, sb);
        Debug.Log(sb.ToString());
    }

    private static void DumpNode(Transform tr, int depth, StringBuilder sb)
    {
        sb.Append(' ', depth * 2).Append("- ").Append(tr.name)
          .Append(" (activeSelf=").Append(tr.gameObject.activeSelf).Append(")")
          .AppendLine();
        foreach (Transform c in tr)
            DumpNode(c, depth + 1, sb);
    }

    private static bool IsMatch(string actual, string want, bool exact)
    {
        if (exact) return actual == want;
        // Fuzzy matching: compatible with "(Clone)" and prefixes/suffixes.
        return actual == want
            || actual.StartsWith(want)
            || actual.Contains(want);
    }
}
