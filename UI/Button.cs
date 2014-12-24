using UnityEngine;
using System.Collections;

public class Button : tk2dButton {
	
	public tk2dTextMesh textMeshBtn;
	
	private System.Action pressedAction;
	
	protected override void Start ()
	{
		base.Start ();
	}
	
	public void Show(string text, System.Action pressed){

		gameObject.SetActive(true);
		
		if(text.Length > 0) {
			textMeshBtn.text = text;
			textMeshBtn.Commit();
		}
		pressedAction = pressed;
		
	}
	
	public void Hide()
	{
		gameObject.SetActive(false);
	}
	
	void HandleButtonPressedEvent (tk2dButton source)
	{
		if(pressedAction != null) {
			pressedAction();
		}
	}
	
	void OnEnable()
	{
		ButtonPressedEvent += HandleButtonPressedEvent;
	}
	
	void OnDisable()
	{
		ButtonPressedEvent -= HandleButtonPressedEvent;
	}
}
