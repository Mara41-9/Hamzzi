//게임 전체 진행 상태, 세이브, 로드, 플레이어 데이터 등 관리하는 매니저
using System.IO;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    private PlayerModel _playerModel;

    public PlayerModel PlayerModel
    {
        get { return _playerModel; }
    }

    private string _saveFilePath;

    private void OnEnable()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        LoadPlayerData();
    }

    public void AddSeedCount(int amount)
    {
        _playerModel.SeedCount += amount;
        SavePlayerData();
    }

    private void LoadPlayerData()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            _playerModel = JsonUtility.FromJson<PlayerModel>(json);
        }
        else
        {
            _playerModel = new PlayerModel();
        }
    }

    private void SavePlayerData()
    {
        string json = JsonUtility.ToJson(_playerModel);
        File.WriteAllText(_saveFilePath, json);

#if UNITY_EDITOR
        Debug.Log("PlayerData 저장됨: " + json);
#endif
    }
}
