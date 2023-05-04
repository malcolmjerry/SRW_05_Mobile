using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEditor;
using UnityEngine.SceneManagement;
using static MapFightingUnit;
using static TerrainHelper;
using System.Linq;

public class AbilityController : MonoBehaviour {

  public Color32 unSelectedColor;
  public Color32 SelectedColor;

  public GameObject m_AbilityPrefab;
  private GameObject myAbilityCanvas;

  private MapFightingUnit unitInfo;
  private Action callback;
  private Action<int, int> nextPrev;

  private WeaponMenuController wmc;

  private int previousHead = 0; //0=common 1=robot 2=pilot 3=weapon
  private int currentHead = 0; //0=common 1=robot 2=pilot 3=weapon
  public enum PAGE { Common, Robot, Pilot, Weapon };

  private List<Transform> headList = new List<Transform>();  //0=common 1=robot 2=pilot 3=weapon
  private List<Transform> bodyList = new List<Transform>();  //0=common 1=robot 2=pilot 3=weapon

  private float m_VerInputValue;
  private float m_HorInputValue;

  private GameObject model;

  private Scene originScene;
  private Camera originCam;

  public void MyAwake() {
    myAbilityCanvas = Instantiate( m_AbilityPrefab ) as GameObject;
    //myAbilityCanvas.name = "myAbilityCanvas";
    myAbilityCanvas.SetActive( false );

    headList.Add( myAbilityCanvas.transform.Find( "CommonHead" ) );
    headList.Add( myAbilityCanvas.transform.Find( "RobotHead" ) );
    headList.Add( myAbilityCanvas.transform.Find( "PilotHead" ) );
    headList.Add( myAbilityCanvas.transform.Find( "WeaponHead" ) );

    bodyList.Add( myAbilityCanvas.transform.Find( "CommonBody" ) );
    bodyList.Add( myAbilityCanvas.transform.Find( "RobotBody" ) );
    bodyList.Add( myAbilityCanvas.transform.Find( "PilotBody" ) );
    bodyList.Add( myAbilityCanvas.transform.Find( "WeaponBody" ) );
  }

  // Use this for initialization
  void Start() {
    /*
    //Debug.Log( "Ab start." );
    myAbilityCanvas = Instantiate( m_AbilityPrefab ) as GameObject;
    //myAbilityCanvas.SetActive( false );
    
    headList.Add( myAbilityCanvas.transform.FindChild( "CommonHead" ) );
    headList.Add( myAbilityCanvas.transform.FindChild( "RobotHead" ) );
    headList.Add( myAbilityCanvas.transform.FindChild( "PilotHead" ) );
    headList.Add( myAbilityCanvas.transform.FindChild( "WeaponHead" ) );

    bodyList.Add( myAbilityCanvas.transform.FindChild( "CommonBody" ) );
    */
  }

  // Update is called once per frame
  void Update() {
    //m_VerInputValue = Input.GetAxis( "Vertical" );
    //m_HorInputValue = Input.GetAxis( "Horizontal" );

    //if (m_HorInputValue != 0) {
    //preMoveHead();
    //}

    //model.transform.Rotate( 0, 0, 20 * Time.deltaTime );
    preMoveHead();

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      myAbilityCanvas.SetActive( false );
      this.enabled = false;

      //SceneManager.UnloadScene( "RobotView" );
      SceneManager.UnloadSceneAsync( "RobotView" );
      SceneManager.SetActiveScene( originScene );
      //CoroutineCommon.CallWaitForOneFrame( () => { Camera.main.enabled = true; } );
      originCam.enabled = true;

      callback();
    }
    else if (Input.GetButtonDown( "Confirm" ) || Input.GetButtonDown( "Up" ) || Input.GetButtonDown( "Down" )) {
      EffectSoundController.PLAY_MENU_CONFIRM();
      if (currentHead == 3 && unitInfo.RobotInfo != null) {   //武器
        wmc.enabled = true;
        this.enabled = false;
        wmc.SelectFirst();
      }
    }
    else if (Input.GetButtonDown( "NextUnit" )) {
      nextPrev?.Invoke( 1, currentHead );
    }
    else if (Input.GetButtonDown( "PrevUnit" )) {

      nextPrev?.Invoke( -1, currentHead );
    }
  }

  public void Setup( MapFightingUnit unitInfo, Action callback, PAGE defaultPage = PAGE.Common, Action<int, int> nextPrev = null ) {
    this.unitInfo = unitInfo;
    this.callback = callback;
    this.nextPrev = nextPrev;

    //MyDestroy();

    //myAbilityCanvas = Instantiate( m_AbilityPrefab ) as GameObject;
    //myAbilityCanvas.SetActive( false );
    /*
    headList.Add( myAbilityCanvas.transform.Find( "CommonHead" ) );
    headList.Add( myAbilityCanvas.transform.Find( "RobotHead" ) );
    headList.Add( myAbilityCanvas.transform.Find( "PilotHead" ) );
    headList.Add( myAbilityCanvas.transform.Find( "WeaponHead" ) );

    bodyList.Add( myAbilityCanvas.transform.Find( "CommonBody" ) );
    bodyList.Add( myAbilityCanvas.transform.Find( "RobotBody" ) );
    bodyList.Add( myAbilityCanvas.transform.Find( "PilotBody" ) );
    bodyList.Add( myAbilityCanvas.transform.Find( "WeaponBody" ) );
    */
    ////Destroy( myAbilityCanvas );
    ////myAbilityCanvas = Instantiate( m_AbilityPrefab ) as GameObject;

    //Camera.main.enabled = false;
    StartCoroutine( changeScene() );

    unitInfo.Update();
    prepareBody();

    currentHead = (int)defaultPage;   //0=common 1=robot 2=pilot 3=weapon
    setHead();
    setBody();
  }

  private void setHead() {
    headList[previousHead].GetComponent<Image>().color = unSelectedColor;
    headList[currentHead].GetComponent<Image>().color = SelectedColor;
  }

  private void setBody() {
    bodyList[previousHead].gameObject.SetActive( false );
    bodyList[currentHead].gameObject.SetActive( true );

    /*
    if (currentHead == 3) {
      this.enabled = false;
      wmc.enabled = true;
    }*/
  }

  private void prepareBody() {
    setupCommonBody();
    setupRobotBody();
    setupPilotBody();
    setupWeaponBody();
  }

  private void setupCommonBody() {
    var commonBody = bodyList[0];

    var pilotPart = commonBody.Find( "PilotPart" );
    pilotPart.Find( "PilotName" ).GetComponent<Text>().text = unitInfo.PilotInfo?.ShortName?? "------";
    //pilotPart.Find( "PilotPic" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + unitInfo.PilotInfo.PicNo + "_1" );
    pilotPart.Find( "PilotPic" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + unitInfo.PilotInfo.PicNo + "_1" );
    pilotPart.Find( "LvText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.Level.ToString()?? "---";
    pilotPart.Find( "WillPowerText" ).GetComponent<Text>().text = unitInfo.PilotInfo != null? $"{unitInfo.PilotInfo.Willpower}" : "---";
    pilotPart.Find( "ExpText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.NextLevel.ToString()?? "---";
    pilotPart.Find( "SpText" ).GetComponent<Text>().text = unitInfo.PilotInfo != null ? $"{unitInfo.PilotInfo.RemainSp}" : "---";

    setPilotAbilities( pilotPart );
    /*
    pilotPart.Find( "MeleeText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.Melee.ToString()?? "---";
    pilotPart.Find( "ShootText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.Shoot.ToString()?? "---";
    pilotPart.Find( "HitText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.Hit.ToString()?? "---";
    pilotPart.Find( "DodgeText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.Dodge.ToString()?? "---";
    pilotPart.Find( "DefenseText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.Defense.ToString()?? "---";
    pilotPart.Find( "DexText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.Dex.ToString()?? "---";
    */

    var robotPart = commonBody.Find( "RobotPart" );
    robotPart.Find( "RobotName" ).GetComponent<Text>().text = unitInfo.RobotInfo?.RobotInstance.Robot.FullName?? "(無搭乘)";
    robotPart.Find( "HpTxt" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "------" : (unitInfo.RobotInfo.HP + " / " + unitInfo.RobotInfo.MaxHP);
    robotPart.Find( "HpSlider" ).GetComponent<Slider>().value = unitInfo.RobotInfo == null ? 0 : ((float)unitInfo.RobotInfo.HP / unitInfo.RobotInfo.MaxHP);
    robotPart.Find( "EnTxt" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "----" : (unitInfo.RobotInfo.EN + " / " + unitInfo.RobotInfo.MaxEN);
    robotPart.Find( "EnSlider" ).GetComponent<Slider>().value = unitInfo.RobotInfo == null ? 0 : ((float)unitInfo.RobotInfo.EN / unitInfo.RobotInfo.MaxEN);
    robotPart.Find( "MovePowerText" ).GetComponent<Text>().text = unitInfo.RobotInfo == null? "--" : unitInfo.MovePower.ToString();
    robotPart.Find( "SizeText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.GetSizeStr()?? "—";
    robotPart.Find( "MotilityText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.Motility.ToString()?? "---";
    robotPart.Find( "ArmorText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.Armor.ToString()?? "----";
    robotPart.Find( "HitRateText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.HitRate.ToString()?? "---";
    robotPart.Find( "MoveTypeText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.GetMoveTypeStr()?? "— — — —";
    robotPart.Find( "SkyText" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "—" : TerrainHelper.AvergeTerrainStr( unitInfo, TerrainEnum.Sky );
    robotPart.Find( "LandText" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "—" : TerrainHelper.AvergeTerrainStr( unitInfo, TerrainEnum.Land );
    robotPart.Find( "SeaText" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "—" : TerrainHelper.AvergeTerrainStr( unitInfo, TerrainEnum.Sea );
    robotPart.Find( "SpaceText" ).GetComponent<Text>().text =  unitInfo.RobotInfo == null ? "—" : TerrainHelper.AvergeTerrainStr( unitInfo, TerrainEnum.Space );
  }

  private void setupRobotBody() {
    var robotBody = bodyList[1];
    robotBody.Find( "RobotName" ).GetComponent<Text>().text = unitInfo.RobotInfo?.RobotInstance.Robot.FullName?? "(無搭乘)";
    robotBody.Find( "HpTxt" ).GetComponent<Text>().text = unitInfo.RobotInfo == null? "----" : ( unitInfo.RobotInfo.HP + " / " + unitInfo.RobotInfo.MaxHP);
    robotBody.Find( "HpSlider" ).GetComponent<Slider>().value = unitInfo.RobotInfo == null ? 0 : ((float)unitInfo.RobotInfo.HP / unitInfo.RobotInfo.MaxHP);
    robotBody.Find( "EnTxt" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "----" : (unitInfo.RobotInfo.EN + " / " + unitInfo.RobotInfo.MaxEN);
    robotBody.Find( "EnSlider" ).GetComponent<Slider>().value = unitInfo.RobotInfo == null ? 0 : ((float)unitInfo.RobotInfo.EN / unitInfo.RobotInfo.MaxEN);
    robotBody.Find( "MovePowerText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.MovePower.ToString()?? "--";
    robotBody.Find( "SizeText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.GetSizeStr()?? "—";
    robotBody.Find( "MotilityText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.Motility.ToString()?? "---";
    robotBody.Find( "ArmorText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.Armor.ToString()?? "----";
    robotBody.Find( "HitRateText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.HitRate.ToString()?? "---";
    robotBody.Find( "MoveTypeText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.GetMoveTypeStr()?? "— — — —";
    robotBody.Find( "SkyText" ).GetComponent<Text>().text = unitInfo.RobotInfo == null? "—" : TerrainHelper.GET_TerrainRank( unitInfo.RobotInfo.TerrainSky );
    robotBody.Find( "LandText" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( unitInfo.RobotInfo.TerrainLand );
    robotBody.Find( "SeaText" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( unitInfo.RobotInfo.TerrainSea );
    robotBody.Find( "SpaceText" ).GetComponent<Text>().text = unitInfo.RobotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( unitInfo.RobotInfo.TerrainSpace );
    robotBody.Find( "PriceText" ).GetComponent<Text>().text = unitInfo.RobotInfo?.RobotInstance.Robot.RepairPrice.ToString()?? "----";

    try {
      setRobotSkillText( robotBody.Find( "Skill1" ).GetComponent<Text>(), unitInfo.RobotInfo?.RobotInstance.RobotSkill1, unitInfo );
      setRobotSkillText( robotBody.Find( "Skill2" ).GetComponent<Text>(), unitInfo.RobotInfo?.RobotInstance.RobotSkill2, unitInfo );
      setRobotSkillText( robotBody.Find( "Skill3" ).GetComponent<Text>(), unitInfo.RobotInfo?.RobotInstance.RobotSkill3, unitInfo );
      setRobotSkillText( robotBody.Find( "Skill4" ).GetComponent<Text>(), unitInfo.RobotInfo?.RobotInstance.RobotSkill4, unitInfo );
      setRobotSkillText( robotBody.Find( "Skill5" ).GetComponent<Text>(), unitInfo.RobotInfo?.RobotInstance.RobotSkill5, unitInfo );
      setRobotSkillText( robotBody.Find( "Skill6" ).GetComponent<Text>(), unitInfo.RobotInfo?.RobotInstance.RobotSkill6, unitInfo );

      setRobotPartsText( robotBody, 1 );
      setRobotPartsText( robotBody, 2 );
      setRobotPartsText( robotBody, 3 );
      setRobotPartsText( robotBody, 4 );

    }
    catch (ArgumentOutOfRangeException) { }

    robotBody.Find( "ShieldIcon" ).gameObject.SetActive( unitInfo.RobotInfo?.RobotInstance.Robot.Shield?? false );
    robotBody.Find( "CutIcon" ).gameObject.SetActive( unitInfo.RobotInfo?.RobotInstance.Robot.Cutter?? false );
  }

  private void setupPilotBody() {
    if (unitInfo.PilotInfo == null)
      return;

    var pilotBody = bodyList[2];
    var pilotPart = pilotBody.Find( "PilotPart" );

    pilotPart.Find( "PilotPic" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + unitInfo.PilotInfo.PicNo + "_1" );
    pilotPart.Find( "FullName" ).GetComponent<Text>().text = unitInfo.PilotInfo == null ? "" : unitInfo.PilotInfo.FullName;
    pilotPart.Find( "ShortName" ).GetComponent<Text>().text = unitInfo.PilotInfo == null ? "" : unitInfo.PilotInfo.ShortName;
    pilotPart.Find( "LvText" ).GetComponent<Text>().text = unitInfo?.PilotInfo.Level.ToString()?? "--";
    pilotPart.Find( "PPText" ).GetComponent<Text>().text = unitInfo?.PilotInfo.PilotInstance.PP.ToString()?? "--";
    pilotPart.Find( "SpText" ).GetComponent<Text>().text = unitInfo.PilotInfo == null ? "----" : (unitInfo.PilotInfo.RemainSp.ToString() + " / " + unitInfo.PilotInfo.MaxSp.ToString());
    pilotPart.Find( "ExpText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.NextLevel.ToString()?? "---";
    pilotPart.Find( "SumText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.PilotInstance.Exp.ToString()?? "-----";
    pilotPart.Find( "KillsText" ).GetComponent<Text>().text = unitInfo.PilotInfo?.PilotInstance.Kills.ToString()?? "----";
    pilotPart.Find( "WillPowerText" ).GetComponent<Text>().text = unitInfo.PilotInfo != null ? $"{unitInfo.PilotInfo.Willpower} / {unitInfo.PilotInfo.MaxWillpower}" : "---";

    pilotPart.Find( "AceStar1" ).gameObject.SetActive( unitInfo.PilotInfo == null ? false : (unitInfo.PilotInfo.PilotInstance.Kills >= 50) );
    pilotPart.Find( "AceStar2" ).gameObject.SetActive( unitInfo.PilotInfo == null ? false : (unitInfo.PilotInfo.PilotInstance.Kills >= 100) );
    pilotPart.Find( "AceStar3" ).gameObject.SetActive( unitInfo.PilotInfo == null ? false : (unitInfo.PilotInfo.PilotInstance.Kills >= 150) );
    pilotPart.Find( "AceStar4" ).gameObject.SetActive( unitInfo.PilotInfo == null ? false : (unitInfo.PilotInfo.PilotInstance.Kills >= 200) );

    pilotPart.Find( "AceBonusText1" ).GetComponent<Text>().text = unitInfo.PilotInfo?.PilotInstance.Kills >= 50 ? AceBonusHelper.AceBonus( unitInfo.PilotInfo.PilotInstance.Pilot.Ace1 ) : "??????";
    pilotPart.Find( "AceBonusText2" ).GetComponent<Text>().text = unitInfo.PilotInfo?.PilotInstance.Kills >= 100 ? AceBonusHelper.AceBonus( unitInfo.PilotInfo.PilotInstance.Pilot.Ace2 ) : "??????";
    pilotPart.Find( "AceBonusText3" ).GetComponent<Text>().text = unitInfo.PilotInfo?.PilotInstance.Kills >= 150 ? AceBonusHelper.AceBonus( unitInfo.PilotInfo.PilotInstance.Pilot.Ace3 ) : "??????";
    pilotPart.Find( "AceBonusText4" ).GetComponent<Text>().text = unitInfo.PilotInfo?.PilotInstance.Kills >= 200 ? AceBonusHelper.AceBonus( unitInfo.PilotInfo.PilotInstance.Pilot.Ace4 ) : "??????";

    var spList = unitInfo.PilotInfo.PilotInstance.Pilot.SPComPilots;
    //for (int i = 0; i<spList.Count; i++) {
    for (int i = 1; i<=7; i++) {
      var spItem = pilotPart.Find( $"SPComlItem{i}" );
      if (unitInfo.PilotInfo == null || spList.Count < i || spList[i - 1] == null) {
        spItem.GetComponent<Text>().text = "------";
      }
      else if (spList[i - 1].Level > unitInfo.PilotInfo.Level)
        spItem.GetComponent<Text>().text = "??????";
      else {
        spItem.GetComponent<Text>().text = $"{spList[i - 1].SPCommand.Name}　({spList[i - 1].SP})";
        spItem.GetComponent<HelpDesc>().Desc = spList[i - 1].SPCommand.Desc;
      }
    }

    setPilotAbilities( pilotPart );

    pilotPart.Find( "SkyText" ).GetComponent<Text>().text = unitInfo.PilotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( unitInfo.PilotInfo.TerrainSky );
    pilotPart.Find( "LandText" ).GetComponent<Text>().text = unitInfo.PilotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( unitInfo.PilotInfo.TerrainLand );
    pilotPart.Find( "SeaText" ).GetComponent<Text>().text = unitInfo.PilotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( unitInfo.PilotInfo.TerrainSea );
    pilotPart.Find( "SpaceText" ).GetComponent<Text>().text = unitInfo.PilotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( unitInfo.PilotInfo.TerrainSpace );

    setPilotSkillText( pilotPart.Find( "PilotSkillItem1" ).GetComponent<Text>(), unitInfo, 1 );
    setPilotSkillText( pilotPart.Find( "PilotSkillItem2" ).GetComponent<Text>(), unitInfo, 2 );
    setPilotSkillText( pilotPart.Find( "PilotSkillItem3" ).GetComponent<Text>(), unitInfo, 3 );
    setPilotSkillText( pilotPart.Find( "PilotSkillItem4" ).GetComponent<Text>(), unitInfo, 4 );
    setPilotSkillText( pilotPart.Find( "PilotSkillItem5" ).GetComponent<Text>(), unitInfo, 5 );
    setPilotSkillText( pilotPart.Find( "PilotSkillItem6" ).GetComponent<Text>(), unitInfo, 6 );
    setPilotSkillText( pilotPart.Find( "PilotSkillItem7" ).GetComponent<Text>(), unitInfo, 7 );
  }

  private void setupWeaponBody() {
    var weaponList = unitInfo.WeaponList?.AsQueryable().OrderBy( w => w.HitPoint ).ToList();
    wmc = bodyList[3].GetComponent<WeaponMenuController>();   //第[3]頁是武器表
    wmc.Setup( weaponList?? new List<WeaponInfo>(), null, null, myCallback );
  }

  private float lastTime;
  private float maxTime;
  private float defaultMaxTime = 0.2f;
  private int direction = 0;

  private void preMoveHead() {
    previousHead = currentHead;

    bool justDown = false;

    if (Input.GetButtonDown( "Left" ) || Input.GetButtonDown( "Right" )) {
      lastTime = defaultMaxTime;
      maxTime = defaultMaxTime * 3;
      justDown = true;
    }

    if (Input.GetButton( "Left" )) {
      direction = -1;
    }
    else if (Input.GetButton( "Right" )) {
      direction = 1;
    }

    if (Input.GetButtonUp( "Left" ) || Input.GetButtonUp( "Right" )) {
      direction = 0;
    }


    lastTime += Time.deltaTime;
    if (direction != 0 && (lastTime > maxTime || justDown)) {
      moveHead( direction );
      lastTime = 0;
      direction = 0;
      if (!justDown) {
        maxTime = defaultMaxTime;
      }
      justDown = false;
    }
  }

  private void moveHead( int move ) {
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), EffectSoundController.EffectSoundSale );

    currentHead = currentHead + move;
    if (currentHead >= headList.Count)
      currentHead = 0;
    else if (currentHead < 0) {
      currentHead = headList.Count - 1;
    }

    //Debug.Log( currentHead );
    setHead();
    setBody();
  }

  private void MyDestroy() {
    //Destroy( myAbilityCanvas );
    //headList?.Clear();
    //bodyList?.Clear();
  }

  IEnumerator changeScene() {
    if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName( "RobotView" )) {
      originScene = SceneManager.GetActiveScene();
      //Camera.main.enabled = false;
      originCam = Camera.main;

      yield return SceneManager.LoadSceneAsync( "RobotView", LoadSceneMode.Additive );
      Scene robotViewScene = SceneManager.GetSceneByName( "RobotView" );
      SceneManager.SetActiveScene( robotViewScene );

      originCam.enabled = false;
      GameObject.Find( "Preview Camera" ).GetComponent<Camera>().enabled = true;

      enabled = true;
      myAbilityCanvas.SetActive( true );
    }

    Destroy( model );
    if (unitInfo.RobotInfo != null)
      createModel();
  }

  public void myCallback() {
    this.enabled = true;
  }

  private void createModel( /*MapFightingUnit fightingUnit/*, string modelName, Vector3 position*/ ) {
    //this.modelName = modelName;
    //GameObject prefab = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath( "Assets/Prefabs/Units/" + unitInfo.RobotInfo.Name + ".prefab", typeof( GameObject ) );
    GameObject prefab = Resources.Load( "Battle/Units/" + unitInfo.RobotInfo.RobotInstance.Robot.Name ) as GameObject;
    model = Instantiate( prefab, new Vector3( 0, 128, 0 ), prefab.transform.rotation );
    model.AddComponent<Rotating>();
    //clone.transform.parent = transform.Find( "Renderer" );

  }

  private void setRobotSkillText( Text textItem, RobotSkill robotSkill, MapFightingUnit mapFightingUnit ) {
    if (robotSkill == null || mapFightingUnit.RobotInfo == null) {
      textItem.text = "";
      return;
    }

    textItem.text = robotSkill.Name;
    textItem.GetComponent<HelpDesc>().Desc = robotSkill.Desc;
    Type rbSkillType = Type.GetType( "RobotSkill_" + robotSkill.ID );
    ICheckOnOff rbSkillOnOff = Activator.CreateInstance( rbSkillType ) as ICheckOnOff;
    if (rbSkillOnOff != null && rbSkillOnOff.IsHighlight( mapFightingUnit )) {
      textItem.color = new Color32( 200, 200, 255, 255 );
    }
    else {
      textItem.color = Color.white;
    }
  }

  private void setRobotPartsText( Transform robotBody, int place ) {
    Text textItem = robotBody.Find( $"Parts{place}" ).GetComponent<Text>();
    if (unitInfo.RobotInfo == null) {
      textItem.text = "------";
      return;
    }

    if (unitInfo.RobotInfo.PartsSlot >= place) {
      ItemOnOff parts = unitInfo.GetPartsItem( place );

      if (parts == null) {
        textItem.text = "------";
        return;
      }

      textItem.text = parts.Name;
      textItem.GetComponent<HelpDesc>().Desc = parts.Desc;

      /*
      Type rbSkillType = Type.GetType( "Parts_" + parts.ID );
      ICheckOnOff rbSkillOnOff = Activator.CreateInstance( rbSkillType ) as ICheckOnOff;
      if (rbSkillOnOff != null && rbSkillOnOff.IsHighlight( unitInfo.MapFightingUnit )) {
        textItem.color = new Color32( 200, 200, 255, 255 );
      }
      else {
        textItem.color = Color.white;
      }*/
    }
    else { robotBody.Find( $"Parts{place}" ).GetComponent<Text>().text = ""; }

    
  }

  private void setPilotSkillText( Text textItem, MapFightingUnit mapFightingUnit, int order ) {
    if (mapFightingUnit.PilotInfo == null || order > mapFightingUnit.PilotInfo.PilotInstance.PilotSkillInstanceList.Count) {
      textItem.text = "------";
      return;
    }

    var pilotSkillID = mapFightingUnit.PilotInfo.PilotInstance.PilotSkillInstanceList[order-1].PilotSkillID;

    Type pilotSkillType = Type.GetType( "PilotSkill_" + pilotSkillID );
    //ISkillOnOff skillOnOff = (ISkillOnOff)Activator.CreateInstance( pilotSkillType );
    PilotSkillBase psiBase = Activator.CreateInstance( pilotSkillType ) as PilotSkillBase;
    ItemOnOff itemOnOff = psiBase.CheckSkill( mapFightingUnit, order );

    textItem.text = itemOnOff.Name;
    textItem.GetComponent<HelpDesc>().Desc = DIContainer.Instance.PilotService.GetPilotSkillByID( pilotSkillID ).Desc;

    if (itemOnOff.Highlight) textItem.color = new Color32( 200, 200, 255, 255 );
    else textItem.color = Color.white;
  }

  private void setPilotAbilities( Transform pilotPart ) {
    var text = pilotPart.Find( "MeleeText" ).GetComponent<Text>();
    if (unitInfo.PilotInfo == null) text.text = "---";
    else setPilotAbility( text, unitInfo.PilotInfo.Melee, unitInfo.PilotInfo.BuffMelee );

    text = pilotPart.Find( "ShootText" ).GetComponent<Text>();
    if (unitInfo.PilotInfo == null) text.text = "---";
    else setPilotAbility( text, unitInfo.PilotInfo.Shoot, unitInfo.PilotInfo.BuffShoot );

    text = pilotPart.Find( "HitText" ).GetComponent<Text>();
    if (unitInfo.PilotInfo == null) text.text = "---";
    else setPilotAbility( text, unitInfo.PilotInfo.Hit, unitInfo.PilotInfo.BuffHit );

    text = pilotPart.Find( "DodgeText" ).GetComponent<Text>();
    if (unitInfo.PilotInfo == null) text.text = "---";
    else setPilotAbility( text, unitInfo.PilotInfo.Dodge, unitInfo.PilotInfo.BuffDodge );

    text = pilotPart.Find( "DefenseText" ).GetComponent<Text>();
    if (unitInfo.PilotInfo == null) text.text = "---";
    else setPilotAbility( text, unitInfo.PilotInfo.Defense, unitInfo.PilotInfo.BuffDefense );

    text = pilotPart.Find( "DexText" ).GetComponent<Text>();
    if (unitInfo.PilotInfo == null) text.text = "---";
    else setPilotAbility( text, unitInfo.PilotInfo.Dex, unitInfo.PilotInfo.BuffDex );
  }

  private void setPilotAbility( Text text, int value, int buff ) {
    text.text = value.ToString();
    if (buff > 0)
      text.color = new Color32( 200, 200, 255, 255 );
    else if (buff < 0)
      text.color = Color.red;
  }

}
