using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataManager
{
	private static readonly string _DATA_PATH = "MasterData/";

    public static List<List<Entity_CardData.Param>> cardData = null;
	public static List<List<Entity_CardTextData.Param>> cardTextData = null;

    public static void LoadAllData()
	{
		cardData = Load<Entity_CardData, Entity_CardData.Sheet, Entity_CardData.Param>("CardData");
		cardTextData = Load<Entity_CardTextData, Entity_CardTextData.Sheet, Entity_CardTextData.Param>("CardTextData");
		CardMasterUtility.MakeCardDataList();
    }

	private static List<List<T3>> Load<T1, T2, T3>(string dataName) where T1 : ScriptableObject
	{
		// データを読み込む
		T1 sourceData = Resources.Load<T1>(_DATA_PATH + dataName);
		// 名称指定でシートを取得
		var sheetField = typeof(T1).GetField("sheets");
		List<T2> listData = sheetField.GetValue(sourceData) as List<T2>;

		// 名称指定でフィールドを取得
		var listField = typeof(T2).GetField("list");
		List<List<T3>> paramList = new List<List<T3>>();
		foreach (var elem in listData) {
			List<T3> param = listField.GetValue(elem) as List<T3>;
			paramList.Add(param);
		}
		return paramList;
	}

}
