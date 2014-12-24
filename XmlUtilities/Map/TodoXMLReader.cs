using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;



public class TodoXMLReader {

	
	public static XmlDocument getXML(string path){
		XmlDocument xmlDoc;
		try{
			UnityEngine.TextAsset assets = (UnityEngine.TextAsset)UnityEngine.Resources.Load(path, typeof(UnityEngine.TextAsset));
			xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(assets.text);
			return xmlDoc;
		}
		catch(XmlException ex){
			Debug.Log("cached in XMLReader : "+ex);
		}
		return null;
	}
	
	
	
	
	
	
	
	
}
