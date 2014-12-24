using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubAnimProperty {

	public int id = 0;
	public string name="";
	public List<SubAnimFrameProperty> frames;
	public List<TriggerProperty> triggers;
	public bool IsFlip = false;
	
	public SubAnimProperty()
	{
		frames = new List<SubAnimFrameProperty>();
		triggers = new List<TriggerProperty>();
	}
}
