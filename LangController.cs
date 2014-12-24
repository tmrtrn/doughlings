using UnityEngine;
using System.Collections;

public class LangController : MonoBehaviour {

	private static LangController _instance;
	private LangXml langXml;
	private string _dataLangData;
	private LangName currentLangName;
	private LangName defaultLangName = LangName.en_US;
	public static event System.Action OnLangChanged;


	public static LangController Instance {
		get {
			if(_instance == null)
				_instance = FindObjectOfType(typeof(LangController)) as LangController;
			if(_instance == null)
				_instance = new GameObject("LangController").AddComponent<LangController>();
			return _instance;
		}
	}
	
	void Awake(){
		if(_instance == null ) {
			_instance = this;
		}
		GameLangName = defaultLangName;
	}

	public LangName GameLangName
	{
		get {
			if(currentLangName == null)
				GameLangName = defaultLangName;
			return currentLangName;
		}
		set{

			currentLangName = value;
			LoadLangData(currentLangName);
		}
	}

	private void LoadLangData(LangName name)
	{
		_dataLangData = GameStateXML.LoadXML("XML/Lang/"+name.ToString());
		if(_dataLangData.ToString() != "") {
			langXml = (LangXml)GameStateXML.DeserializeObject(_dataLangData,"LangXml");
		}
		Debug.Log("lang "+_dataLangData);
	}

	public LangData GetLangDataByKey(string key)
	{
		LangData foundData = langXml.langsData.Find( (obj) => obj.key == key);
		if(foundData == null) {
			Debug.Log("foundData is null");
			foundData = new LangData();
			foundData.isUppercase = false;
			foundData.key = key;
			foundData.value = key;
		}
		Debug.Log("foundData "+foundData.value);
		return foundData;
	}



}
