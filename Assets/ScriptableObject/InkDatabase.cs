using UnityEngine;
using System.Collections.Generic;
using Ink.UnityIntegration;

[CreateAssetMenu(fileName = "InkDatabase", menuName = "Scriptable Objects/InkDatabase")]
public class InkDatabase : ScriptableObject
{
    public List<InkFile> inkFiles;

    public InkFile GetInkFile(string inkName)
    {
        return inkFiles.Find(ink => ink.name == inkName);
    }
}
