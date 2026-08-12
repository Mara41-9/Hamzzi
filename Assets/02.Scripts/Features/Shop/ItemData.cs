using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string Description;
    public string Category;
    public string SubCategory;
    public string IconPath;
    public string PrefabPath;

    public int SizeX;
    public int SizeY;
    public Vector2Int Size
    {
        get
        {
            return new Vector2Int(SizeX, SizeY);
        }
    }
}