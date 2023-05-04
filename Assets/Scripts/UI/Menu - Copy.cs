using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;

public class Menu_bk : MonoBehaviour {

  public GameObject commandPrefab;

  //[HideInInspector] public GameObject[] allCmdList;
  
  [HideInInspector] public GameObject[] cmdList;

  public List<int> CmdIndexList;

  [HideInInspector] public int status = 0;  //0: Nothing entered  1: doing something

  private float selRectHeight;
  private float cmdTextRectHeight;
  //[HideInInspector] public GameObject unitSelected;

  //public string[] cmdTextList = { "移動", "攻擊", "精神", "能力" };
  public UnitCommand[] cmdTextList = {
    new UnitCommand () { index = (int)CmdEnum.MOVE,  text = "移動" },
    new UnitCommand () { index = (int)CmdEnum.ATK  , text = "攻擊" },
    new UnitCommand () { index = (int)CmdEnum.SPCMD, text = "精神" },
    new UnitCommand () { index = (int)CmdEnum.INFO,  text = "能力" },
    new UnitCommand () { index = (int)CmdEnum.ACTION_END,  text = "待機" },
    new UnitCommand () { index = (int)CmdEnum.PHASE_END,  text = "回合終了" },
    new UnitCommand () { index = (int)CmdEnum.STAGEINFO,  text = "作戰目的" },
    new UnitCommand () { index = (int)CmdEnum.TEAMINFO,  text = "部隊表" },
    new UnitCommand () { index = (int)CmdEnum.BACK_TO_MAIN,  text = "回到主選單" },
    new UnitCommand () { index = (int)CmdEnum.MAPSAVE,  text = "SAVE" }
  };
  public enum CmdEnum { MOVE = 0, ATK, SPCMD, INFO, ACTION_END, PHASE_END, STAGEINFO, TEAMINFO, BACK_TO_MAIN, MAPSAVE }

  public Color32 unSelectedColor;
  public Color32 SelectedColor;

  private int cmdSelected;

  private float m_VerInputValue;
  private float m_HorInputValue;

  private Action backAction;

  [HideInInspector]public const int CMD_WIDTH_SHORT = 75;
  [HideInInspector]public const int CMD_WIDTH_LONG = 150;
  [HideInInspector]private int cmdWidth = 75;

  // Use this for initialization
  void Start () {

	}

	// Update is called once per frame
	void Update () {
    //m_VerInputValue = Input.GetAxis( "Vertical" );
    //m_HorInputValue = Input.GetAxis( "Horizontal" );

    moveMenu();
    /*
    if (m_VerInputValue != 0) {
      moveMenu();
    }*/

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      backToCaller();
    }
  }

  public void createCommands( int commandWidth, Action backAction ) {
    this.backAction = backAction;
    //Debug.Log( this.gameObject.transform.childCount );
    //CmdIndexList = cmds;

    cmdWidth = commandWidth;
    commandPrefab.transform.Find( "SelectRect" ).GetComponent<RectTransform>().sizeDelta = new Vector2( cmdWidth, selRectHeight );
    commandPrefab.transform.Find( "SelectRect" ).Find( "Text" ).GetComponent<RectTransform>().sizeDelta = new Vector2( cmdWidth, cmdTextRectHeight );

    var bkg = transform.Find( "Background" );
    var bkgRect = bkg.GetComponent<RectTransform>();
    bkgRect.sizeDelta = new Vector2( cmdWidth + 10, CmdIndexList.Count * selRectHeight + 10 );

    clearPreviousCmd();

    cmdList = new GameObject[CmdIndexList.Count];
    float topY = ((cmdList.Length - 1)/2f) * selRectHeight;
    for (int i = 0; i < CmdIndexList.Count; i++) {
      
      var cmd = Instantiate( commandPrefab ) as GameObject;
      var selRect = cmd.transform.Find( "SelectRect" );
      var textTransform = selRect.transform.Find( "Text" );
      UnitCommand unitComm = cmdTextList.FirstOrDefault( c => c.index == CmdIndexList[i] );
      textTransform.GetComponent<Text>().text = (unitComm == null? "---" : unitComm.text);


      cmdList[i] = cmd;//allCmdList[cmds[i]];
      cmdList[i].transform.parent = bkg;
      cmdList[i].transform.localPosition = new Vector3( 0, topY , 0 );
      cmdList[i].transform.Find( "SelectRect" ).GetComponent<Image>().color = unSelectedColor;
      topY -= selRectHeight;
    }

    nextCommand( 0 );
  }
  
  private void Awake() {
    selRectHeight = commandPrefab.transform.Find( "SelectRect" ).GetComponent<RectTransform>().rect.height;
    cmdTextRectHeight = commandPrefab.transform.Find( "SelectRect" ).Find( "Text" ).GetComponent<RectTransform>().rect.height;
    /*
    allCmdList = new GameObject[cmdTextList.Length];
    for (int i = 0; i < allCmdList.Length; i++) {
      var cmd = allCmdList[i] = Instantiate( commandPrefab ) as GameObject;
      var selRect = cmd.transform.FindChild( "SelectRect" );
      var textTransform = selRect.transform.FindChild( "Text" );
      textTransform.GetComponent<Text>().text = cmdTextList[i].text;
    }*/
  }

  public void nextCommand( int move ) {
    if (move != 0) {
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    }
    cmdList[cmdSelected].transform.Find( "SelectRect" ).GetComponent<Image>().color = unSelectedColor;
    cmdSelected += move;
    if (cmdSelected > cmdList.Length - 1)
      cmdSelected = 0;
    else if (cmdSelected < 0) {
      cmdSelected = cmdList.Length - 1;
    }
    cmdList[cmdSelected].transform.Find( "SelectRect" ).GetComponent<Image>().color = SelectedColor;
  }

  public void selectCommandByCmdIndex( int index ) {
    //EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    for (int i = 0; i < cmdList.Length; i++) {
      if (CmdIndexList[i] == index) {
        cmdList[i].transform.Find( "SelectRect" ).GetComponent<Image>().color = SelectedColor;
        cmdSelected = i;
      } else {
        cmdList[i].transform.Find( "SelectRect" ).GetComponent<Image>().color = unSelectedColor;
      }
    }
  }

  private float lastTime;
  private float maxTime;
  private float defaultMaxTime = 0.1f;
  private int direction = 0;

  private void moveMenu() {
    bool justDown = false; 

    if (Input.GetButtonDown( "Up" ) || Input.GetButtonDown( "Down" )) {
      lastTime = defaultMaxTime;
      maxTime = defaultMaxTime * 5;
      justDown = true;
    }

    if (Input.GetButton( "Up" )) {
      direction = -1;
    }
    else if (Input.GetButton( "Down" )) {
      direction = 1;
    }

    if (Input.GetButtonUp( "Up" ) || Input.GetButtonUp( "Down" )) {
      direction = 0;
    }

    lastTime += Time.deltaTime;
    if (direction != 0 && (lastTime > maxTime || justDown)) {
      nextCommand( direction );
      lastTime = 0;
      direction = 0;
      if (!justDown) {
        maxTime = defaultMaxTime;
      }
      justDown = false;
    }

  }
  /*
  void moveMenu() {
    if (Input.GetButtonUp( "Vertical" )) {
      maxTime = defaultMaxTime * 10;
    } else if (Input.GetButtonDown( "Vertical" )) {
      maxTime = lastTime = defaultMaxTime;
    }
    lastTime += Mathf.Abs( m_VerInputValue ) * Time.deltaTime;
    if (lastTime > maxTime) {
      nextCommand( m_VerInputValue > 0 ? -1 : 1 );
      maxTime = defaultMaxTime;
      lastTime = 0;
    }
  }*/

  public CmdEnum GetSelectedAction() {
    return (CmdEnum)CmdIndexList[cmdSelected];
  }

  void OnEnable() {
    //maxTime = defaultMaxTime * 10;
    direction = 0;
  }

  void OnDisable() {
    /*
    cmdSelected = 0;
    for (int i = 0; i < cmdList.Length; i++) {
      Destroy( cmdList[i] );
    }*/
  }

  void clearPreviousCmd() {
    cmdSelected = 0;
    for (int i = 0; i < cmdList.Length; i++) {
      Destroy( cmdList[i] );
    }
  }

  void backToCaller() {
    gameObject.SetActive( false );
    backAction();
  }

  public class UnitCommand {
    public int index;
    public string text;
  }

}
