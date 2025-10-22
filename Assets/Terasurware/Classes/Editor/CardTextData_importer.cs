using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class CardTextData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/MasterData/CardTextData.xlsx";
	private static readonly string exportPath = "Assets/Resources/MasterData/CardTextData.asset";
	private static readonly string[] sheetNames = { "BasicBeyond","LegendsRise","InfinityEvolved","HeirsOfTheOmen","SkyboundDragons","Basic","CLC","DRK","ROB","TOG","WLD","SFL","CGS","DBN","BOS","OOT","ALT","STR","ROG","VEC","UCL","WUP","FOH","SOR","ETA","DOV","RSC","DOC","OOS","EOP","RGW","CDB","EAA","AOA","HOR","ORS","RSL","HOS", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_CardTextData data = (Entity_CardTextData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_CardTextData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_CardTextData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_CardTextData.Sheet s = new Entity_CardTextData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_CardTextData.Param p = new Entity_CardTextData.Param ();
						
					cell = row.GetCell(0); p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.CardText = (cell == null ? "" : cell.StringCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
