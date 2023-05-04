using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCanvas : MonoBehaviour {

  private static Transform helpDesc;
  private static Text helpDescText1;
  private static Text helpDescText2;
  private static Text helpDescText3;

  private static Transform phaseCtl;
  private static Transform gameOverCtl;

  private static Vector2 buttomMin = new Vector2( 0.15f, .1f );
  private static Vector2 buttomMax = new Vector2( 0.85f, .34f );
  
  private static Vector2 topMin = new Vector2( 0.15f, .66f );
  private static Vector2 topMax = new Vector2( 0.85f, .9f );

  private static FadeInOut fadeInOut;

  public static Transform topMsg;

  void Awake() {
    helpDesc = transform.Find( "HelpDesc" );
    helpDescText1 = helpDesc.Find( "Text1" ).GetComponent<Text>();
    helpDescText2 = helpDesc.Find( "Text2" ).GetComponent<Text>();
    helpDescText3 = helpDesc.Find( "Text3" ).GetComponent<Text>();
    phaseCtl = transform.Find( "PhaseCtl" );
    gameOverCtl = transform.Find( "GameOverCtl" );
    fadeInOut = GetComponent<FadeInOut>();
    topMsg = transform.Find( "TopMsg" );
  }

  // Update is called once per frame
  void Update() {

  }

  public static void SHOW_HELP_DESC( string desc, bool isTop ) {
    if (string.IsNullOrWhiteSpace( desc ))
      return;

    helpDesc.gameObject.SetActive( true );

    helpDescText1.text = "";
    helpDescText2.text = "";
    helpDescText3.text = "";

    var lines = desc.Split( '\n' );
    helpDescText1.text = lines[0];
    if (lines.Length > 1) helpDescText2.text = lines[1];
    if (lines.Length > 2) helpDescText3.text = lines[2];

    if (isTop) {
      helpDesc.GetComponent<RectTransform>().anchorMin = topMin;
      helpDesc.GetComponent<RectTransform>().anchorMax = topMax;
    }
    else {
      helpDesc.GetComponent<RectTransform>().anchorMin = buttomMin;
      helpDesc.GetComponent<RectTransform>().anchorMax = buttomMax;
    }
  }

  public static void HIDE_HELP_DESC() {
    helpDesc.gameObject.SetActive( false );
  }

  public static void SHOW_PHASE( string phaseName, Action workload, Action callback ) {
    phaseCtl.gameObject.SetActive( true );
    phaseCtl.GetComponent<PhaseCtl>().Do( phaseName, workload, callback );
  }

  public static void SHOW_GAMEOVER( Action workload, Action callback ) {
    gameOverCtl.gameObject.SetActive( true );
    gameOverCtl.GetComponent<GameOverCtl>().Do( workload, callback );
  }

  public static void FadeOut( float sec, Action callback, Action doProcess = null, float blackTime = 1f, bool hold = false ) {
    fadeInOut.FadeOut( sec, callback, doProcess, blackTime, hold );
  }

  public static void FullIn() {
    fadeInOut.FullIn();
  }

  public static void SHOW_MSG( string msg ) {
    if (string.IsNullOrWhiteSpace( msg ))
      return;

    topMsg.transform.Find( "Background/Message" ).GetComponent<Text>().text = msg;
    topMsg.gameObject.SetActive( true );
  }

  public static void Hide_MSG() {
    topMsg.gameObject.SetActive( false );
  }
}
