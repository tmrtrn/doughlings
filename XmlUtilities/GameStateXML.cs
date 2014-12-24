using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 

public class GameStateXML { 


   /* The following metods came from the referenced URL */ 
   public static string UTF8ByteArrayToString(byte[] characters) 
   {      
      UTF8Encoding encoding = new UTF8Encoding(); 
      string constructedString = encoding.GetString(characters); 
      return (constructedString); 
   } 
    
   public static byte[] StringToUTF8ByteArray(string pXmlString) 
   { 
      UTF8Encoding encoding = new UTF8Encoding(); 
      byte[] byteArray = encoding.GetBytes(pXmlString); 
      return byteArray; 
   } 
    
   // Here we serialize our UserData object of myData 
   public static string SerializeObject(object pObject,string typeName) 
   { 
      System.Type type = System.Type.GetType(typeName);
	  string XmlizedString = null; 
      MemoryStream memoryStream = new MemoryStream(); 
      XmlSerializer xs = new XmlSerializer(type); 
      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
      xs.Serialize(xmlTextWriter, pObject); 
      memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
      XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
      return XmlizedString; 
   } 
    
   // Here we deserialize it back into its original form 
   public static object DeserializeObject(string pXmlizedString,string myType) 
   { 
      System.Type type = System.Type.GetType(myType);
	  XmlSerializer xs = new XmlSerializer(type); 
      MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
//      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
      return xs.Deserialize(memoryStream); 
   } 
    
   
	
   public static void CreateXML(string _FileName,string _data) 
   { 
      string _FileLocation = Application.dataPath;
	  StreamWriter writer;
	  FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
      if(!t.Exists) 
      { 
         writer = t.CreateText(); 
      } 
      else 
      { 
         t.Delete(); 
         writer = t.CreateText(); 
      } 
      writer.Write(_data); 
      writer.Close(); 
   } 
	
   public static string LoadXML(string _FileName ) 
   { 
		/*
      string _FileLocation = Application.dataPath;
	  StreamReader r;
	  r = File.OpenText(_FileLocation+"\\"+ _FileName); 
      string _info = r.ReadToEnd();
      r.Close(); 
	  return _info;
	  */
		string _FileLocation = Application.dataPath;
	  StreamReader r;
	  UnityEngine.TextAsset assets = (UnityEngine.TextAsset)UnityEngine.Resources.Load(_FileName, typeof(UnityEngine.TextAsset));//File.OpenText(_FileLocation+"\\"+ _FileName); 
      string _info = assets.text;//r.ReadToEnd();
    //  r.Close(); 
	  return _info;
   } 
} 