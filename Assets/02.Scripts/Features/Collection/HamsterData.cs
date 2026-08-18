using UnityEngine;

[System.Serializable]
public class HamsterData : GameDataBase
{
    public string Name;
    public string Description;
    public HamsterTier HamsterTier;
    public float CollectSpeed;
    public string IconPath;
}

[System.Serializable]
public class HamsterSave
{
    public int HamsterUID;
    public int UserUID;
    public string HamsterId;
    public string FaceId;
}