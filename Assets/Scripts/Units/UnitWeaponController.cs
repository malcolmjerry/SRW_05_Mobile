using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class UnitWeaponController : MonoBehaviour {

  public GameObject m_WeaponMenuPrefab;
  private GameObject myWeaponMenu;
  private WeaponMenuController wmc;

  private Action callBack;

  private float m_VerInputValue;
  private float m_HorInputValue;

  private UnitInfo unitInfo;

  //private bool isOnMap = true;

  void Awake() {

  }

  // Use this for initialization
  void Start () {

  }
	
	// Update is called once per frame
	void Update () {
    //m_VerInputValue = Input.GetAxis( "Vertical" );
    //m_HorInputValue = Input.GetAxis( "Horizontal" );

    //moveMenu();
    /*
    if (Input.GetButtonDown( "Back" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      this.enabled = false;
      Destroy( myWeaponMenu );
      //myWeaponMenu.SetActive( false );
      //owner.gameObject.SetActive( true );
      callBack();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuConfirm" ) );
      //myTargetCursor = Instantiate( m_targetCursorPrefab ) as GameObject;
      myWeaponMenu.SetActive( false );
      this.enabled = false;

      var chooseTarget = GetComponent<ChooseTarget>();
      chooseTarget.Setup( unitInfo, wmc.GetSelectedWeapon(), () => {
        myWeaponMenu.SetActive( true );
        this.enabled = true;
      } );
    }*/

    //else if (m_VerInputValue != 0) {
      //moveMenu();
    //}
  }

  public void Setup( UnitInfo unitInfo, bool beforeMove, Action callBack ) {
    this.unitInfo = unitInfo; 
    //this.isOnMap = isOnMap;
    this.callBack = callBack;

    Destroy( myWeaponMenu );
    myWeaponMenu = Instantiate( m_WeaponMenuPrefab ) as GameObject;
    //myWeaponMenu.SetActive( false );
    unitInfo.MapFightingUnit.Update();
    var weaponList = unitInfo.MapFightingUnit.WeaponList.AsQueryable().OrderBy( w => w.HitPoint ).ToList();

    wmc = myWeaponMenu.GetComponent<WeaponMenuController>();
    wmc.Setup( weaponList?? new List<WeaponInfo>(), !beforeMove, confirm, backMenu );
    wmc.enabled = true;
    wmc.SelectFirst();
    //myWeaponMenu.SetActive( true );
    this.enabled = true;
  }

  public void backMenu() {
    this.enabled = false;
    Destroy( myWeaponMenu );
    //myWeaponMenu.SetActive( false );
    //owner.gameObject.SetActive( true );
    callBack();
  }

  public void confirm() {
    //myTargetCursor = Instantiate( m_targetCursorPrefab ) as GameObject;
    myWeaponMenu.SetActive( false );
    this.enabled = false;

    var chooseTarget = GetComponent<ChooseTarget>();
    chooseTarget.Setup( unitInfo, /*wmc.GetSelectedWeapon().MaxRange,*/ wmc.GetSelectedWeapon(), () => {
      myWeaponMenu.SetActive( true );
      this.enabled = true;
    } );
  }

  /*
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
      wmc.nextCommand( direction );
      lastTime = 0;
      direction = 0;
      if (!justDown) {
        maxTime = defaultMaxTime;
      }
      justDown = false;
    }
  }
  */

  /*
  void moveMenu() {
    if (Input.GetButtonUp( "Vertical" )) {
      maxTime = defaultMaxTime * 10;
    } else if (Input.GetButtonDown( "Vertical" )) {
      maxTime = lastTime = defaultMaxTime;
    }
    lastTime += Mathf.Abs( m_VerInputValue ) * Time.deltaTime;
    if (lastTime > maxTime) {
      wmc.nextCommand( m_VerInputValue > 0 ? -1 : 1 );
      maxTime = defaultMaxTime;
      lastTime = 0;
    }
  }*/
  /*
  void OnEnable() {
    maxTime = defaultMaxTime * 10;
  }*/
  

}

