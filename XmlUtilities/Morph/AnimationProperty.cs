using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationProperty {

	public string name;
	public string type;
	public List<RootProperty> limbElements;
	public bool loop = false;
	
	public AnimationProperty()
	{
		limbElements = new List<RootProperty>();
	}
}
