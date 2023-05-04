using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using static Cursor;
using static Weapon;
using static UnitInfo;
using System.Collections.Generic;

public class ChooseTarget : MonoBehaviour {

  public GameObject m_targetCursorPrefab;
  public GameObject myTargetCursor;

  //public GameObject MyUnitWallPrefab;
  //private GameObject myUnitWall;

  //public GameObject MyMoveAreaSplotLightPrefab;
  //private GameObject myMoveAreaSplot;

  public GameObject MapRangeCanvasPrefab;
  private GameObject maxRangeCanvas;
  private GameObject minRangeCanvas;

  public GameObject CircleMapwColliderPrefab;
  private GameObject circleMapwCollider;

  private WeaponInfo weaponInfo;

  private Light directionLight;
  //private float originLightIntensity;
  private float intensityNormal;
  private float intensityDark;

  private Action backToCaller;
  private Vector3 originPos;
  //private float range;
  //private float radius;
  private UnitInfo fromUnit;

  private List<TeamEnum> teamList;
  private List<GameObject> inRangeUnits;
 
  private void Awake() {
    directionLight = GameObject.Find( "Directional light" ).GetComponent<Light>();
    intensityNormal = directionLight.intensity;
    intensityDark = intensityNormal * 0; //* (float)0.75;
  }

  void Start () {

  }
	
	// Update is called once per frame
	void Update () {
    Weapon wp = weaponInfo.WeaponInstance.Weapon;

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      myTargetCursor.GetComponent<Cursor>().SetPosition( originPos );
      myTargetCursor.GetComponent<Cursor>().SetMainCamToSelf();
      endSelf();
      backToCaller();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      //一般單體武器
      if (!wp.IsMap) {
        var unitSelected = myTargetCursor.GetComponent<Cursor>().unitSelected;
        if (unitSelected != null) {
          if (unitSelected.gameObject.layer != LayerMask.NameToLayer( "Enermy" )) {
            EffectSoundController.PLAY_ACTION_FAIL();
            Debug.Log( "只能攻擊敵人" );
            return;
          }
          
          if (!PreBattleFormula.IS_IN_RANGE( fromUnit, unitSelected.GetComponent<UnitInfo>(), weaponInfo.MaxRange, weaponInfo.MinRange )) {
            EffectSoundController.PLAY_ACTION_FAIL();
            Debug.Log( "射程不足" );
            return;
          }

          EffectSoundController.PLAY_MENU_CONFIRM();
          GetComponent<MakeBattleManager>().Setup( fromUnit, unitSelected.GetComponent<UnitInfo>(), weaponInfo, null, true, Resume, endSelf );

          pending();
        }
        else {
          EffectSoundController.PLAY_ACTION_FAIL();
        }
      }
      else {
        EffectSoundController.PLAY_MENU_CONFIRM();
        endSelf();
        //GetComponent<MakeMapWManager>().Setup( fromUnit, weaponInfo, inRangeUnits, () => { /*endSelf();*/ } );
        GameObject makeMapWManagerGo = new GameObject( "MakeMapWManager" );
        makeMapWManagerGo.AddComponent<MakeMapWManager>().Setup( fromUnit, weaponInfo, inRangeUnits, () => {} );
      }
    }
  }

  public void Setup( UnitInfo unitInfo, /*float range,*/ WeaponInfo weaponInfo, Action backToCaller ) {
    this.weaponInfo = weaponInfo;
    this.backToCaller = backToCaller;
    this.fromUnit = unitInfo;
    this.originPos = unitInfo.transform.position;
    //this.range = range;

    endSelf();
    //myTargetCursor = Instantiate( m_targetCursorPrefab ) as GameObject;
    //myTargetCursor.name = "ChooseTargetCursor";
    myTargetCursor.SetActive( true );
    myTargetCursor.GetComponent<Cursor>().SetPosition( originPos );
    //myTargetCursor.GetComponent<Cursor>().MODE = (int)Cursor.ModeEnum.ENERMY;
    //myTargetCursor.GetComponent<Cursor>().MODE_ENUM = Cursor.ModeEnum.ENERMY_RANGE;
    //if (weaponInfo.WeaponInstance.Weapon.RangeType == (int)Weapon.RangeTypeEnum.NORMAL)
    //myTargetCursor.GetComponent<Cursor>().ChangeModeWithDistance( Cursor.ModeEnum.ENERMY_RANGE, fromUnit, weaponInfo.MinRange, weaponInfo.MaxRange );
    circleMapwCollider = Instantiate( CircleMapwColliderPrefab, new Vector3( originPos.x, CircleMapwColliderPrefab.transform.position.y, originPos.z ), CircleMapwColliderPrefab.transform.rotation );
    
    Weapon wp = weaponInfo.WeaponInstance.Weapon;

    /*
    myUnitWall = Instantiate( MyUnitWallPrefab, new Vector3( originPos.x, MyUnitWallPrefab.transform.position.y, originPos.z ),
                      MyUnitWallPrefab.transform.rotation ) as GameObject;
    myUnitWall.GetComponent<RangeColider>().Setup( lightRange );*/
    //Debug.Log( "radius: " + lightRange );

    /*
    myMoveAreaSplot = Instantiate( MyMoveAreaSplotLightPrefab, new Vector3( originPos.x, MyMoveAreaSplotLightPrefab.transform.position.y, originPos.z ),
                                   MyMoveAreaSplotLightPrefab.transform.rotation ) as GameObject;
    myMoveAreaSplot.GetComponent<Light>().spotAngle = spotAngle;

    directionLight.intensity /= 2;
    */

    //float lightRange = 1.6f * 2f + range * 6f;
    //float spotAngle = (range - 1) / 19 * 100 + 50;
    //radius = lightRange / 2;
    //float lightRange = unitInfo.RobotInfo.Radius + range * 2;

    //originLightIntensity = directionLight.intensity;
    if (wp.RangeType == (int)RangeTypeEnum.NORMAL || wp.RangeType == (int)RangeTypeEnum.MAP_SELF_CIRCLE) {
      float maxRound;
      maxRound = unitInfo.RobotInfo.Radius + 1f * PreBattleFormula.BasicRadius * weaponInfo.MaxRange; 

      maxRangeCanvas = Instantiate( MapRangeCanvasPrefab, new Vector3( originPos.x, MapRangeCanvasPrefab.transform.position.y, originPos.z ),
                                    MapRangeCanvasPrefab.transform.rotation ) as GameObject;
      maxRangeCanvas.transform.position = new Vector3( originPos.x, MapRangeCanvasPrefab.transform.position.y, originPos.z );
      maxRangeCanvas.transform.localScale = new Vector3( maxRound, maxRound, transform.localScale.z );

      if (weaponInfo.MinRange > 1) {
        float minRound = unitInfo.RobotInfo.Radius + 1f * PreBattleFormula.BasicRadius * (weaponInfo.MinRange - 1);// unitInfo.RobotInfo.Radius * (weaponInfo.MinRange - 1);
        minRangeCanvas = Instantiate( MapRangeCanvasPrefab, new Vector3( originPos.x, MapRangeCanvasPrefab.transform.position.y, originPos.z ),
                                      MapRangeCanvasPrefab.transform.rotation ) as GameObject;
        minRangeCanvas.transform.position = new Vector3( originPos.x, MapRangeCanvasPrefab.transform.position.y, originPos.z );
        minRangeCanvas.transform.localScale = new Vector3( minRound, minRound, transform.localScale.z );
        Image img = minRangeCanvas.transform.Find( "Background" ).GetComponent<Image>();
        img.color = new Color( Color.black.r, Color.black.g, Color.black.b, img.color.a );

      }

      //創建攻擊範圍判斷 
      circleMapwCollider.transform.position = new Vector3( originPos.x, CircleMapwColliderPrefab.transform.position.y, originPos.z );
      //circleMapwCollider.transform.localScale = new Vector3( maxRound, transform.localScale.y, maxRound );
      teamList = getTeamListByWeapon( wp );
      circleMapwCollider.GetComponent<CircleMapwCollider>().Setup( maxRound, teamList, unitInfo, weaponInfo.MaxRange, weaponInfo.MinRange );

      inRangeUnits = circleMapwCollider.GetComponent<CircleMapwCollider>().InRangeUnits;
    }


    myTargetCursor.GetComponent<Cursor>().ChangeModeWithDistance( Cursor.ModeEnum.RANGE, fromUnit, inRangeUnits, weaponInfo );

    /*
    switch (wp.RangeType) {
      case (int)Weapon.RangeTypeEnum.NORMAL:
        myTargetCursor.GetComponent<Cursor>().ChangeModeWithDistance( Cursor.ModeEnum.RANGE, fromUnit,  );
        break;
      case (int)Weapon.RangeTypeEnum.MAP_SELF_CIRCLE:
        myTargetCursor.GetComponent<Cursor>().ChangeModeWithDistance( Cursor.ModeEnum.RANGE, fromUnit, weaponInfo.MinRange, weaponInfo.MaxRange );
        break;
    }*/

    //directionLight.intensity = intensityDark;

    this.enabled = true;
  }

  private void pending() {
    this.enabled = false;
    myTargetCursor.GetComponent<Cursor>().SetDisable();
    //myUnitWall.SetActive( false );
    //myMoveAreaSplot.SetActive( false );
    maxRangeCanvas.SetActive( false );
    if (minRangeCanvas != null) minRangeCanvas.SetActive( false );
    directionLight.intensity = intensityNormal;
    circleMapwCollider.GetComponent<CircleMapwCollider>().ClearAll();
    circleMapwCollider.GetComponent<CapsuleCollider>().enabled = false;
  }

  public void Resume() {
    this.enabled = true;
    myTargetCursor.SetActive( true );
    //myUnitWall.SetActive( true );
    //myMoveAreaSplot.SetActive( true );
    maxRangeCanvas.SetActive( true );
    if (minRangeCanvas != null) minRangeCanvas.SetActive( true );
    directionLight.intensity = intensityDark;
    circleMapwCollider.GetComponent<CapsuleCollider>().enabled = true;
  }

  private void endSelf() {
    this.enabled = false;
    if (myTargetCursor != null) {
      myTargetCursor.GetComponent<Cursor>().SetDisable();
    }
    //Destroy( myTargetCursor );  //2021-12-12
    //Destroy( myUnitWall );
    //Destroy( myMoveAreaSplot );
    Destroy( maxRangeCanvas );
    Destroy( circleMapwCollider );
    if (minRangeCanvas != null) Destroy( minRangeCanvas );
    directionLight.intensity = intensityNormal;
  }

  private List<TeamEnum> getTeamListByWeapon( Weapon wp ) {
    var teamList = new List<TeamEnum> { TeamEnum.Enermy, TeamEnum.NPC_Yellow };
    if (!wp.IsFriendly && wp.IsMap) {
      teamList.Add( TeamEnum.Player );
      teamList.Add( TeamEnum.NPC_Friend );
    }
    return teamList;
  }

}
