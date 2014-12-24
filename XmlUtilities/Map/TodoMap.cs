using UnityEngine;
using System.Collections.Generic;

public class TodoMap {

	private int id = -1;
	private string name;
	private int defaultLife = 12;
	public List<TodoMapCell> mapCells = new List<TodoMapCell>();
	
	public TodoMap set_id(int id){
		this.id = id;
		return this;
	}
	public int get_id(){
		return id;
	}
	public TodoMap set_name(string name){
		this.name = name;
		return this;
	}
	public string get_name(){
		return name;
	}

	public int DefaultLife{
		get{return defaultLife;}
		set{this.defaultLife = value;}
	}
	
	
}
