using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class TodoGameCollection {
	
	public const string xmlPATH = "XML/Map";
	//public enum ObstacleType{Blank, Blue, Green, Yellow, Red, Gray, Shapeshift, Star, Life }
	
	private static XmlDocument maps_xmlDoc;
	private static XmlDocument map_pieces_xmlDoc;
	private static XmlDocument obstacles_xmlDoc;
	
	static List<TodoMap> maps;


	public static void ReadAndStoreXmls(){
		
		maps_xmlDoc = TodoXMLReader.getXML(xmlPATH+"/Maps");
		map_pieces_xmlDoc = TodoXMLReader.getXML(xmlPATH+"/MapPieces");
		obstacles_xmlDoc = TodoXMLReader.getXML(xmlPATH+"/Blocks");
		
		ReadMapItems();
		
		maps = new List<TodoMap>();
		foreach(XmlNode node_map in maps_xmlDoc.GetElementsByTagName("Map")){
			
			TodoMap map = new TodoMap();
			map.set_id(int.Parse(node_map.Attributes["iID"].Value.Trim()))
				.set_name(node_map.Attributes["sName"].Value);
			map.DefaultLife = int.Parse(node_map.Attributes["iDefaultLife"].Value.Trim());


			int rowCounter = 0;
			List<TodoMapCell> mapCells = new List<TodoMapCell>();
			XmlNodeList nodes_mapPieces = node_map.SelectNodes("MapPiece");
			for(int i = nodes_mapPieces.Count - 1; i >= 0; i--){
	//		for(int i = 0; i < nodes_mapPieces.Count; i++){
                int mapPeaceId = int.Parse(nodes_mapPieces[i].Attributes["iMapID"].Value.Trim());

                XmlNode node_sel_map_piece = FindMapPiece(mapPeaceId);
                XmlNodeList node_rows = node_sel_map_piece.SelectNodes("Row");
               
                for (int j = node_rows.Count - 1; j >= 0; j-- )
                {
					
                    int rowId = int.Parse(node_rows[j].Attributes["iID"].Value.Trim());
                    XmlNode node_row = node_rows[j];
                    XmlNodeList node_columns = node_row.SelectNodes("Column");
                    for(int k = 0; k < node_columns.Count; k++)
                    {
						
                        XmlNode node_column = node_columns[k];
                //        Debug.Log("node_column k :" + k + " value: " + node_column.Attributes["iValue"].Value);
						
                        TodoMapCell cell = new TodoMapCell();
                        
                        cell.row = rowCounter;//rowId;
                        cell.column = int.Parse(node_column.Attributes["iID"].Value.Trim());

                        cell.row_xml = rowId;
                        cell.column_xml = int.Parse(node_column.Attributes["iID"].Value.Trim());
                        cell.mapPieceId = int.Parse(nodes_mapPieces[i].Attributes["iMapID"].Value.Trim());

                        int itemId = int.Parse(node_column.Attributes["iValue"].Value.Trim());
						cell.itemType = (MapItemType)itemId;//TodoMapItemType.GetByItemId(itemId);
                        mapCells.Add(cell);

                    }
                    rowCounter += 1;
                }
				
			}
			map.mapCells = mapCells;
//			Debug.Log("mapCells Count "+mapCells.Count+ " : rowCounter "+rowCounter);
			maps.Add(map);
		}
		
	}
	
	private static void ReadMapItems()
	{
		/*
		foreach(XmlNode node in obstacles_xmlDoc.SelectNodes("Blocks/Block")){
			int Id = int.Parse(node.Attributes["iID"].Value.Trim());
			string Name = node.Attributes["sName"].Value.Trim();
			TodoMapItemType item = new TodoMapItemType(Id, Name);
			item.Description = node.Attributes["sDescription"].Value.Trim();
			item.Score = int.Parse(node.Attributes["iScore"].Value.Trim());
			item.LootName = node.Attributes["sLootName"].Value.Trim();
			item.LootChance = int.Parse(node.Attributes["iLootChance"].Value.Trim());
			item.IconName = node.Attributes["sIconName"].Value.Trim();
			
		} */
	}
	
	private static XmlNode FindBlock(int id){
		XmlNode sel_node = null;
		foreach(XmlNode node in obstacles_xmlDoc.SelectNodes("Blocks/Block")){
			if(int.Parse(node.Attributes["iID"].Value.Trim()) == id){
				sel_node = node;
				break;
			}
			
		}
		return sel_node;
	}
	
	private static XmlNode FindMapPiece(int id){
		XmlNode sel_Node = null;
		foreach(XmlNode nextNode in map_pieces_xmlDoc.SelectNodes("MapPieces/MapPiece")){
			if(id == int.Parse(nextNode.Attributes["iID"].Value.Trim()))
			{
				sel_Node = nextNode;
				break;
			}
		}
		
		return sel_Node;
	}
	
	
	public static List<TodoMap> Get_Stored_Maps(){
		return maps;
	}
	
	public static TodoMap getMap(int map_id){
		return maps.Find(c => c.get_id() == map_id);
	}
	
	
	public static TodoMapCell getMapCell(TodoMap map, int row, int column){
		
		return map.mapCells.Find( c => (c.row == row && c.column == column) );
	}
	
	public static int getMapCellsCount(TodoMap map){
		return map.mapCells.Count;
	}

	
	
	
	
}
