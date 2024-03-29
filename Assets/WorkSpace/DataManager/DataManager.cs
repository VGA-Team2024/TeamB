using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    static DataManager _instance;
    public static DataManager Instance { get => _instance; }

    [SerializeField] SaveLoadManager _saveLoadManager;
    [SerializeField] FacilityDataBase _facilitydata;
    [SerializeField] TMP_Text _goldText;
    [SerializeField] TMP_Text _resourceText;
    [SerializeField] TMP_Text _enemyresourceText;
    [SerializeField] TMP_Text _factoryworkerText;
    private int _gold;
    private int _resource;
    private int _enemyresource;
    // 工員の数が変動するなら保存生成が必要だけど一人固定なのでシーン上においておくことにする
    [SerializeField] private FactoryWorkerController _factoryWorker;
    private int[] _facilitycount = new int[Enum.GetValues(typeof(FacilityEnum)).Length];
    private int[] _facilitystock = new int[Enum.GetValues(typeof(FacilityEnum)).Length];
    private List<ConstructionState> _existFacilities = new List<ConstructionState>();
    private DateTime _lastWarTime;
    public event Action<int> Onchangegold;
    public event Action<int> Onchangeresource;
    public event Action<int> Onchangeenemyresource;
    public event Action<int> Onchangefactoryworker;
    public event Action<int[]> OnChangedFacilityCount;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
        }

        Onchangegold += n => _goldText.text = n.ToString();
        //Onchangeresource += n => _resourceText.text = n.ToString();
        Onchangeenemyresource += n => _enemyresourceText.text = n.ToString();
        //Onchangefactoryworker += n => _factoryworkerText.text = n.ToString();

        InisializeFacilityStock();
        // ゲームデータをロードし、初期化する
        InitializeGameData(_saveLoadManager.GetInitialData());
        
        Invoke(nameof(SaveGameData), 60);
    }

    private void InisializeFacilityStock()
    {
        for (int i = 0;  i < _facilitydata.FacilityData.Count; i++)
        {
            _facilitystock[i] = _facilitydata.FacilityData[i].FacilityStock;
        }
    }

    public int Gold
    {
        get => _gold;

        private set
        {
            _gold = value;
            if (_gold < 0)
            {
                _gold = 0;
            }
            Onchangegold?.Invoke(_gold);
        }
    }

    public int Resource
    {
        get => _resource;

        private set
        {
            _resource = value;
            Onchangeresource?.Invoke(_resource);
        }
    }

    public int EnemyResource
    {
        get => _enemyresource;

        private set
        {
            _enemyresource = value;
            Onchangeenemyresource?.Invoke(_enemyresource);
        }
    }
    
    public FactoryWorkerController FactoryWorkerController
    {
        get => _factoryWorker;
        set => _factoryWorker = value;
    }

    public int[] FacilityCount => _facilitycount;
    public int[] Facilitystock => _facilitystock;

    public DateTime LastWarTime
    {
        get => _lastWarTime;
        set => _lastWarTime = value;
    }

    /// <summary> セーブデータからゲーム状態を初期化する </summary>
    void InitializeGameData(SaveGameData saveGameData)
    {
        Gold = saveGameData._gold;
        Resource = saveGameData._resource;
        EnemyResource = saveGameData._enemyResource;
        _facilitycount = saveGameData._facilityCount;
        _facilitystock = saveGameData._facilityStock;
        LastWarTime = DateTime.Parse(saveGameData._lastWarTime);

        foreach (FacilitySaveData facilitySaveData in saveGameData._facilitySaveDatas)
        {
            ConstructionState constructionState = Instantiate(_facilitydata.FacilityData[(int)facilitySaveData.FacilityEnum].Prefab)
                .GetComponent<ConstructionState>();
            constructionState.InitializeFacilityData(facilitySaveData);
        }
    }

    /// <summary> ゲームの状態を取得し、ゲームデータとして保存する </summary>
    public void SaveGameData()
    {
        FacilitySaveData[] facilitySaveDatas = new FacilitySaveData[_existFacilities.Count];

        for (int i = 0; i < facilitySaveDatas.Length; i++)
        {
            facilitySaveDatas[i] = _existFacilities[i].GetFacilitySaveData();
        }
        
        SaveGameData saveGameData = new SaveGameData(_gold, _resource, _enemyresource
            , _facilitycount, _facilitystock, LastWarTime.ToString(), facilitySaveDatas);
        _saveLoadManager.SaveData(saveGameData);
        
        Invoke(nameof(SaveGameData), 60);
    }

    public void ChangeGold(int value)
    {
        Gold += value;
    }

    public void ChangeResource(int value)
    {
        Resource += value;
    }

    public void ChangeEnemyResource(int value)
    {
        EnemyResource += value;
    }

    public void AddFacilityCount(FacilityEnum facilityEnum)
    {
        FacilityCount[(int)facilityEnum]++;
        OnChangedFacilityCount.Invoke(_facilitycount);
    }

    /// <summary>
    /// ストックを減らす処理
    /// </summary>
    /// <param name="facilityEnum"></param>
    public void DecreaseFacilityStock(FacilityEnum facilityEnum)
    {
        _facilitystock[(int)facilityEnum]--;
    }

    /// <summary> 施設生成時にスクリプトを登録 </summary>
    public void AddFacilityData(ConstructionState constructionState)
    {
        _existFacilities.Add(constructionState);
    }

    /// <summary>
    /// FacilityDataBase内のリストから引数をインデックスとしてデータを返す
    /// </summary>
    public Facility GetFacilitydata(int index)
    {
        return _facilitydata.FacilityData[index];
    }
}
