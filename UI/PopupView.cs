using UnityEngine;
using System.Collections;
using System;

public class PopupView : MonoBehaviour {

	public tk2dTextMesh titleTextMesh;
	public tk2dTextMesh contextTextMesh;
	public Button btn1;


	public class PopupData
	{
		public string keyTitle = "";
		public string keyContext = "";
		public string keyBtn1 = "";
		public string keyBtn2 = "";
		public Action btn1Pressed = null;
		public Action btn2Pressed = null;
	}

	PopupData currentData;

	void OnEnable()
	{
		LangController.OnLangChanged += HandleOnLangChanged;
		GUIController.OnPausePressed += HandleOnPauseButtonPressed;
		GUIController.OnResumePressed += HandleOnResumeButtonPressed;
	}

	void HandleOnLangChanged ()
	{
	
	}

	void HandleOnPauseButtonPressed()
	{
		if(btn1 != null) {
			btn1.enabled = false;
		}
	}

	void HandleOnResumeButtonPressed()
	{
		if(btn1 != null) {
			btn1.enabled = true;
		}
	}

	void OnDisable()
	{
		LangController.OnLangChanged -= HandleOnLangChanged;
		GUIController.OnPausePressed -= HandleOnPauseButtonPressed;
		GUIController.OnResumePressed -= HandleOnResumeButtonPressed;
	}

	public void Render(PopupData data) {

		currentData = data;
		SetView();

	}

	void SetView()
	{

		if(currentData.keyTitle.Length > 0) {
			LangData titleLangData = LangController.Instance.GetLangDataByKey(currentData.keyTitle);
			titleTextMesh.text = titleLangData.isUppercase ? titleLangData.value.ToUpper() : titleLangData.value;
		}
		else 
			titleTextMesh.text = "";
		titleTextMesh.Commit();

		if(currentData.keyContext.Length > 0) {
			LangData contextLangData = LangController.Instance.GetLangDataByKey(currentData.keyContext);
			contextTextMesh.text = contextLangData.isUppercase ? contextLangData.value.ToUpper() : contextLangData.value;
		}
		else 
			contextTextMesh.text = "";
		contextTextMesh.Commit();

		if(currentData.keyBtn1.Length > 0) {
			LangData btn1LangData = LangController.Instance.GetLangDataByKey(currentData.keyBtn1);
			btn1.Show( btn1LangData.isUppercase ? btn1LangData.value.ToUpper() : btn1LangData.value, currentData.btn1Pressed);
		}
		else {
			btn1.Hide();
		}


	}

	public void Hide(){
		currentData = null;
		gameObject.SetActive(false);
	}

}
