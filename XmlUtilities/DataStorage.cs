using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataStorage {

	const string morphAnimXmlPATH = "XML/Morph";

	AnimationsXml morphAnimationsXml;
	SubAnimationsXml morphSubAnimationsXml;
	tk2dSpriteAnimation writed2dToolkitAnimation;

	static DataStorage instance;
	public static DataStorage Instance {
		get {
			if(instance == null) {
				instance = new DataStorage();
			}
			return instance;
		}
	}

	public DataStorage()
	{

	}

	public void ReadXmlDB()
	{
		morphAnimationsXml = new AnimationsXml();
		morphSubAnimationsXml = new SubAnimationsXml();
		ReadMorphData();
		ReadMapXML();
	}

	#region Morph XML Data Read and Write To GameObjects
	void ReadMorphData()
	{
		string _morphSubAnimationsXml = GameStateXML.LoadXML(morphAnimXmlPATH+"/SubanimsData");
		string _morphAnimationsXml = GameStateXML.LoadXML(morphAnimXmlPATH+"/AnimationsData");
		morphSubAnimationsXml = (SubAnimationsXml)GameStateXML.DeserializeObject(_morphSubAnimationsXml,"SubAnimationsXml");
		morphAnimationsXml = (AnimationsXml)GameStateXML.DeserializeObject(_morphAnimationsXml, "AnimationsXml");
	}

	public List<SubAnimProperty> GetMorphSubanims
	{
		get{
			return morphSubAnimationsXml.subAnims;
		}
	}

	public List<AnimationProperty> GetMorphAnimationsProperty
	{
		get{
			return morphAnimationsXml.anims;
		}
	}

	public tk2dSpriteAnimation WriteMorphDataTo2DToolkitAnimation(tk2dSpriteCollectionData dependedCollectionData)
	{
		GameObject goEmptySpriteAnimation = new GameObject("emptySpriteAnimation");
		writed2dToolkitAnimation = goEmptySpriteAnimation.AddComponent<tk2dSpriteAnimation>();
		writed2dToolkitAnimation.clips = new tk2dSpriteAnimationClip[GetMorphSubanims.Count];
		int clipIndex = 0;

		GetMorphSubanims.ForEach((obj) => {
			
			tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
			clip.name = obj.name;
			List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
			
			for(int i=0; i<obj.frames.Count; i++)
			{
				int frameLenght = obj.frames[i].frameCount;
				for(int framePoint = 0; framePoint < frameLenght; framePoint ++){
					tk2dSpriteAnimationFrame tk2dFrame = new tk2dSpriteAnimationFrame();
					tk2dFrame.spriteCollection = dependedCollectionData;
					tk2dFrame.spriteId = dependedCollectionData.GetSpriteIdByName(obj.frames[i].imageName);
					frames.Add(tk2dFrame);
				}				
			}
			
			clip.frames = new tk2dSpriteAnimationFrame[frames.Count];
			int frameIndex = 0;
			frames.ForEach( (selFrame) => {
				clip.fps = 15;
				clip.frames[frameIndex] = selFrame;
				
				TriggerProperty triggerProperty = obj.triggers.Find(nextTrigger => nextTrigger.triggeredFrameID == (frameIndex +1));
				if(triggerProperty != null){
					if(triggerProperty.valX != 0 || triggerProperty.valY != 0){
						clip.frames[frameIndex].triggerEvent = true;
						clip.frames[frameIndex].eventInfo = triggerProperty.valX+"\r\n"+triggerProperty.valY;
					}
				}
				
				frameIndex += 1;
			});
			
			writed2dToolkitAnimation.clips[clipIndex] = clip;
			clipIndex += 1;
		});

		return writed2dToolkitAnimation;
	}

	public tk2dSpriteAnimation GetMorph2dToolkitAnimation
	{
		get{
			return writed2dToolkitAnimation;
		}
	}

	#endregion

	#region Map Item XML Reading

	void ReadMapXML()
	{
		TodoGameCollection.ReadAndStoreXmls();
	}

	public List<TodoMap> GetAllMaps() {
		return TodoGameCollection.Get_Stored_Maps();
	}

	public TodoMap GetMapById(int id) {
		return TodoGameCollection.getMap(id);
	}


	#endregion

}
