using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DataModel.Service.GameDataService;

public class StoryController : MonoBehaviour {

	// Use this for initialization
	void Start () {
    Scene storyScene = SceneManager.GetSceneByName( $"Story" );
    SceneManager.SetActiveScene( storyScene );

    string stageName;

    if (DIContainer.Instance.GameDataService.StoryPhase > StoryPhaseEnum.Start)
      stageName = $"Story_{DIContainer.Instance.GameDataService.GameData.Stage.ToString().PadLeft( 3, '0' )}_End";
    else {
      int stageNo = DIContainer.Instance.GameDataService.GenerateNewStage();
      stageName = $"Story_{stageNo.ToString().PadLeft( 3, '0' )}";
    }

    //int stageNo = DIContainer.Instance.GameDataService.GenerateNewStage();
    //string end = DIContainer.Instance.GameDataService.StoryPhase > StoryPhaseEnum.Start ? "_End" : "";
    //Debug.Log( stageNo + "" + end + stageName );
    gameObject.AddComponent( Type.GetType( stageName ) ); 
    
    //gameObject.AddComponent<Story_001>();
  }
	
	// Update is called once per frame
	void Update () {
		
	}



}
