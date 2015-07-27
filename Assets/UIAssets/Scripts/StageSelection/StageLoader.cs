using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 

public class StageLoader : MonoBehaviour {

	public GameObject stageSlotButton = null;
	public GameObject sectionToFill = null;
	public Image stageLogo = null;
	public Text stageNameText = null;

	// Use this for initialization
	void Start () {
		DataManager dataManager = DataManager.getDataManagerInstance();

		if( stageSlotButton == null || sectionToFill == null || stageLogo == null || stageNameText == null )
		{
			Debug.LogError( "Fill all game objects needed by the Stage Loader.");
			return;
		}


		if( dataManager == null )
		{
			Debug.LogError( "The Stage Loader can't find the data manager object in the scene.");
			return;
		}

		List<string> stageNames = dataManager.getAvailableStages();
		int i = 0;
		for( i = 0; i < stageNames.Count; i++ )
		{
			GameObject go = GameObject.Instantiate( stageSlotButton ) as GameObject;
			StageSlotUI stu = go.GetComponent<StageSlotUI>();
			if( stu == null )
			{
				Debug.LogError( "The Stage Loader can't find the data manager object in the scene.");
				return;
			}
			stu.setStageName( stageNames[i] );
			stu.setStageLogoElement( stageLogo );
			stu.setStageTextElement( stageNameText );
			go.transform.SetParent( sectionToFill.transform, false );
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
