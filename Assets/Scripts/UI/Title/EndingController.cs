using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingController : MonoBehaviour {

  private Transform textTf;

	// Use this for initialization
	void Start () {
    var gameDataService = DIContainer.Instance.GameDataService;

    textTf = transform.Find( "TimeTxt" );
    textTf.GetComponent<Text>().text = $"爆機時間: {DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" )}";

    BGMController.SET_BGM( "2G_Ending" );

    enabled = false;

    GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeIn( 5f, () => {

    } );

    CoroutineCommon.CallWaitForSeconds( 10f, () => enabled = true );
  }
	
	// Update is called once per frame
	void Update () {
    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {

    }
    else if (Input.GetButtonDown( "Start" )) {
      enabled = false;
      loadTitle();
    }
    else if (Input.GetButton( "Confirm" )) {
      enabled = false;
      loadTitle();
    }
  }

  void loadTitle() {
    var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();
    GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( 3f, () => {   //測試 .5, 正式 2f
      mySceneManager.UnloadAndLoadScene( "Title", SceneManager.GetActiveScene() );
    }, blackTime: 0f, hold: true );
  }

}
