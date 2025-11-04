using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameEnum;

public class ConditionManager : MonoBehaviour
{
    public static ConditionManager Instance;

    [SerializeField]
    private GameObject ConditionPanel;

    [SerializeField]
    private GameObject CostToggles;
    [SerializeField]
    private GameObject TypeToggles;
    [SerializeField]
    private GameObject LeaderToggles;
    [SerializeField]
    private GameObject RarityToggles;
    [SerializeField]
    private GameObject PackToggles;

    private List<Toggle> costToggleList;
    private List<Toggle> typeToggleList;
    private List<Toggle> leaderToggleList;
    private List<Toggle> rarityToggleList;
    private List<Toggle> packToggleList;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);


        ConditionPanel.SetActive(false);

        // 各Toggleをリストに格納
        costToggleList = new List<Toggle>(CostToggles.GetComponentsInChildren<Toggle>());
        typeToggleList = new List<Toggle>(TypeToggles.GetComponentsInChildren<Toggle>());
        leaderToggleList = new List<Toggle>(LeaderToggles.GetComponentsInChildren<Toggle>());
        rarityToggleList = new List<Toggle>(RarityToggles.GetComponentsInChildren<Toggle>());
        packToggleList = new List<Toggle>(PackToggles.GetComponentsInChildren<Toggle>());

        // 各リセットボタンにリスナーを追加
        costToggleList[0].onValueChanged.AddListener((isOn) => { ResetCostCondition(); });
        typeToggleList[0].onValueChanged.AddListener((isOn) => { ResetTypeCondition(); });
        leaderToggleList[0].onValueChanged.AddListener((isOn) => { ResetLeaderCondition(); });
        rarityToggleList[0].onValueChanged.AddListener((isOn) => { ResetRarityCondition(); });
        packToggleList[0].onValueChanged.AddListener((isOn) => { ResetPackCondition(); });

        costToggleList.RemoveAt(0);
        typeToggleList.RemoveAt(0);
        leaderToggleList.RemoveAt(0);
        rarityToggleList.RemoveAt(0);
        packToggleList.RemoveAt(0);

        var gray = ColorBlock.defaultColorBlock;
        gray.selectedColor = Color.gray;
        gray.normalColor = Color.gray;
        gray.pressedColor = Color.gray;
        gray.disabledColor = Color.gray;
        gray.highlightedColor = Color.gray;
        var white = ColorBlock.defaultColorBlock;
        white.selectedColor = Color.white;
        white.normalColor = Color.white;
        white.pressedColor = Color.white;
        white.disabledColor = Color.white;
        white.highlightedColor = Color.white;

        // 各トグルに押したら色を変えるリスナーを追加
        for (int i = 0; i < costToggleList.Count; i++)
        {
            int index = i;
            costToggleList[i].onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    costToggleList[index].colors = gray;
                }
                else
                {
                    costToggleList[index].colors = white;
                }
            });
        }
        for (int i = 0; i < typeToggleList.Count; i++)
        {
            int index = i;
            typeToggleList[i].onValueChanged.AddListener((isOn) => {
                if (isOn){
                    typeToggleList[index].colors = gray;
                }
                else
                {
                    typeToggleList[index].colors = white;
                }
            });
        }
        for (int i = 0; i < leaderToggleList.Count; i++)
        {
            int index = i;
            leaderToggleList[i].onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    leaderToggleList[index].colors = gray;
                }
                else
                {
                    leaderToggleList[index].colors = white;
                }
            });
        }
        for (int i = 0; i < rarityToggleList.Count; i++)
        {
            int index = i;
            rarityToggleList[i].onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    rarityToggleList[index].colors = gray;
                }
                else
                {
                    rarityToggleList[index].colors = white;
                }
            });
        }
        for (int i = 0; i < packToggleList.Count; i++)
        {
            int index = i;
            packToggleList[i].onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    packToggleList[index].colors = gray;
                }
                else
                {
                    packToggleList[index].colors = white;
                }
            });
        }
    }

    public void Open()
    {
        ConditionPanel.SetActive(true);
    }

    public void Close()
    {
        ConditionPanel.SetActive(false);
    }

    public void ResetCondition()
    {
        ResetCostCondition();
        ResetTypeCondition();
        ResetLeaderCondition();
        ResetRarityCondition();
        ResetPackCondition();
    }

    public void ResetCostCondition()
    {
        for (int i = 0; i < costToggleList.Count; i++)
        {
            costToggleList[i].isOn = false;
        }
    }
    public void ResetTypeCondition()
    {
        for (int i = 0; i < typeToggleList.Count; i++)
        {
            typeToggleList[i].isOn = false;
        }
    }
    public void ResetLeaderCondition()
    {
        for (int i = 0; i < leaderToggleList.Count; i++)
        {
            leaderToggleList[i].isOn = false;
        }
    }
    public void ResetRarityCondition()
    {   
        for (int i = 0; i < rarityToggleList.Count; i++)
        {
            rarityToggleList[i].isOn = false;
        }
    }
    public void ResetPackCondition()
    {
        for (int i = 0; i < packToggleList.Count; i++)
        {
            packToggleList[i].isOn = false;
        }
    }

    public List<bool> GetCostCondition()
    {
        List<bool> conditions = new List<bool>();
        for (int i = 0; i < costToggleList.Count; i++)
        {
            conditions.Add(costToggleList[i].isOn);
        }
        conditions = CheckToggleAllOff(conditions);

        return conditions;
    }
    public List<bool> GetTypeCondition()
    {
        List<bool> conditions = new List<bool>();
        for (int i = 0; i < typeToggleList.Count; i++)
        {
            conditions.Add(typeToggleList[i].isOn);
        }
        conditions = CheckToggleAllOff(conditions);
        return conditions;
    }
    public List<bool> GetLeaderCondition()
    {
        List<bool> conditions = new List<bool>();
        for (int i = 0; i < leaderToggleList.Count; i++)
        {
            conditions.Add(leaderToggleList[i].isOn);
        }
        conditions = CheckToggleAllOff(conditions);
        return conditions;
    }
    public List<bool> GetRarityCondition()
    {
        List<bool> conditions = new List<bool>();
        for (int i = 0; i < rarityToggleList.Count; i++)
        {
            conditions.Add(rarityToggleList[i].isOn);
        }
        conditions = CheckToggleAllOff(conditions);
        return conditions;
    }
    public List<bool> GetPackCondition()
    {
        List<bool> conditions = new List<bool>();
        for (int i = 0; i < packToggleList.Count; i++)
        {
            conditions.Add(packToggleList[i].isOn);
        }
        conditions = CheckToggleAllOff(conditions);
        return conditions;
    }

    // 全ての要素がfalseかどうか
    private List<bool> CheckToggleAllOff(List<bool> bools)
    {
        bool allOff = true;
        foreach (var b in bools)
        {
            if (b)
            {
                allOff = false;
                break;
            }
        }
        List<bool> result = new List<bool>(bools);
        if (allOff)
        {
            for (int i = 0; i < bools.Count; i++)
            {
                result[i] = true;
            }
        }
        return result;
    }
}
