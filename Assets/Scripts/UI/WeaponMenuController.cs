using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public class WeaponMenuController : MonoBehaviour {

  public GameObject weaponItemPrefab;

  public Sprite Fist;
  public Sprite Shoot;

  public Color32 unSelectedColor;
  public Color32 SelectedColor;

  public int itemPerPage;     // default: 6;
  public float itemRowHeight; //default: 0.1f;

  [HideInInspector]public List<WeaponInfo> WeaponInfoList;
  //[HideInInspector] public List<WeaponInfo> WeaponInfoListInPage;
  private List<GameObject> weaponItemList = null;
  private List<GameObject> weaponItemListPage;

  private bool? afterMove;
  private Action next;
  private Action callback;
  private UnitInfo fromUnitInfo;
  private UnitInfo toUnitInfo;

  void Awake() {

  }

  // Use this for initialization
  void Start () {

	}
	
	// Update is called once per frame
	void Update () {
    moveMenu();
    
    if (Input.GetButtonDown( "Back" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      clearSelection();
      /*if (ActualIndex < WeaponInfoList.Count)*/ weaponItemList[ActualIndex].GetComponent<Image>().enabled = false;
      setPage( 1 );
      this.enabled = false;
      callback();
    }
    else if (Input.GetButtonDown( "Confirm" ) && next != null) {
      if (WeaponInfoList[ActualIndex].IsUsable  && (afterMove != true || WeaponInfoList[ActualIndex].IsMove) &&
          (
          !fromUnitInfo || !toUnitInfo ||
          PreBattleFormula.IS_IN_RANGE( fromUnitInfo, toUnitInfo, WeaponInfoList[ActualIndex].MaxRange, WeaponInfoList[ActualIndex].MinRange )
          )
      ) {
        EffectSoundController.PLAY_MENU_CONFIRM();
        next?.Invoke();
      }
      else EffectSoundController.PLAY_ACTION_FAIL();
    }
  }

  void OnEnable() {
    /*
    maxTime = defaultMaxTime * 5;

    this.enabled = true;
    transform.Find( "PageArrow" ).gameObject.SetActive( maxPage > 1 );
    setPage( 1 );
    weaponItemList[ActualIndex].GetComponent<Image>().enabled = true;
    updateSelection();
    */
  }

  public void SelectFirst() {
    maxTime = defaultMaxTime * 5;

    this.enabled = true;
    transform.Find( "PageArrow" ).gameObject.SetActive( maxPage > 1 );
    setPage( 1 );
    if (ActualIndex < weaponItemList.Count) 
      weaponItemList[ActualIndex].GetComponent<Image>().enabled = true;

    updateSelection();
  }


  public void Setup( List<WeaponInfo> WeaponInfoList, bool? afterMove, Action next, Action callback, UnitInfo fromUnitInfo = null, UnitInfo toUnitInfo = null ) {
    this.WeaponInfoList = WeaponInfoList;
    this.next = next;
    this.callback = callback;
    this.afterMove = afterMove;
    this.fromUnitInfo = fromUnitInfo;
    this.toUnitInfo = toUnitInfo;

    background = transform.Find( "Background" );
    pageText = background.Find( "Header" ).Find( "Page" ).GetComponent<Text>();

    clearItemList();
    weaponItemList = new List<GameObject>();

    maxPage = Math.Max(0, (WeaponInfoList.Count-1)) / itemPerPage + 1;

    float anchorY = 0.87f;

    for (int i = 0; i < WeaponInfoList.Count; i++) {
      int place = i % itemPerPage;
      float startHeight = anchorY - place * itemRowHeight;

      var weaponItem = Instantiate( weaponItemPrefab ) as GameObject;
      weaponItemList.Add( weaponItem );
      weaponItem.transform.SetParent( background, false );
      weaponItem.GetComponent<RectTransform>().anchorMax = new Vector2( 1, startHeight );
      weaponItem.GetComponent<RectTransform>().anchorMin = new Vector2( 0, startHeight - itemRowHeight );
      setItemContent( weaponItem, i );
      weaponItem.SetActive( false );
      //anchorY -= itemRowHeight;
    }
    
    setPage( 1 );
    clearSelection();

    /*
    //weaponItemList[ActualIndex].GetComponent<Image>().enabled = true;
    //updateSelection();
    */
  }

  private void nextPage() {
    int targetPage = currentPage + 1;
    int resultPage = targetPage > maxPage ? 1 : targetPage;
    setPage( resultPage );
  }

  private void prevPage() {
    int targetPage = currentPage - 1;
    int resultPage = targetPage < 1 ? maxPage : targetPage;
    setPage( resultPage );
  }

  private void setPage( int page ) {
    currentPage = page;
    pageText.text = currentPage + "/" + maxPage;

    foreach (var i in weaponItemList) { i.SetActive( false ); }

    weaponItemListPage = weaponItemList.AsQueryable().Skip( (page - 1) * itemPerPage ).Take( itemPerPage ).ToList();
    foreach (var i in weaponItemListPage) { i.SetActive( true ); }

    /*
    if (currentSelectedInPage >= weaponItemListPage.Count)
      currentSelectedInPage = weaponItemListPage.Count - 1;*/

    currentSelectedInPage = 0;   //預設換頁後選第一個, 但根據調用者的最後決定才亮燈
    //weaponItemList[ActualIndex].GetComponent<Image>().enabled = true;
  }

  private void setItemContent( GameObject weaponItem, int index ) {
    WeaponInfo weapon = WeaponInfoList[index];
    weaponItem.transform.Find( "WeaponName" ).GetComponent<Text>().text = weapon.WeaponInstance.Weapon.Name;
    weaponItem.transform.Find( "TypeImg" ).GetComponent<Image>().sprite = weapon.WeaponInstance.Weapon.IsMelee ? Fist : Shoot;
    weaponItem.transform.Find( "Type" ).GetComponent<Text>().text = weapon.TypeStr();
    weaponItem.transform.Find( "HitPoint" ).GetComponent<Text>().text = weapon.HitPoint.ToString();
    weaponItem.transform.Find( "Range" ).GetComponent<Text>().text = weapon.MinRange + "~" + weapon.MaxRange;
    weaponItem.transform.Find( "HitRate" ).GetComponent<Text>().text = weapon.HitRate.ToString( "+#;-#;+0");
    weaponItem.transform.Find( "CRI" ).GetComponent<Text>().text = weapon.CRI.ToString( "+#;-#;+0" );
    weaponItem.transform.Find( "Remain" ).GetComponent<Text>().text = weapon.MaxBullets > 0? weapon.RemainBullets.ToString() : "----" ;
    weaponItem.transform.Find( "EN" ).GetComponent<Text>().text = weapon.EN.ToString();

    if (next != null) {
      if (!weapon.IsUsable || afterMove == true && !weapon.IsMove || 
          fromUnitInfo && toUnitInfo && !PreBattleFormula.IS_IN_RANGE( fromUnitInfo, toUnitInfo, weapon.MaxRange, weapon.MinRange )
        ) {
        weaponItem.transform.Find( "WeaponName" ).GetComponent<Text>().color = Color.red;
        weaponItem.transform.Find( "TypeImg" ).GetComponent<Image>().color = Color.red;
        weaponItem.transform.Find( "Type" ).GetComponent<Text>().color = Color.red;
        weaponItem.transform.Find( "HitPoint" ).GetComponent<Text>().color = Color.red;
        weaponItem.transform.Find( "Range" ).GetComponent<Text>().color = Color.red;
        weaponItem.transform.Find( "HitRate" ).GetComponent<Text>().color = Color.red;
        weaponItem.transform.Find( "CRI" ).GetComponent<Text>().color = Color.red;
        weaponItem.transform.Find( "Remain" ).GetComponent<Text>().color = Color.red;
        weaponItem.transform.Find( "EN" ).GetComponent<Text>().color = Color.red;
      }
    }
  }

  public void moveUpDown( int move ) {
    currentSelectedInPage = currentSelectedInPage + move;
    if (currentSelectedInPage >= weaponItemListPage.Count) {
      //nextPage();       //最尾一行是否自動換頁
      currentSelectedInPage = 0;
    }
    else if (currentSelectedInPage < 0) {
      //prevPage();          //最前一行是否自動換頁
      currentSelectedInPage = weaponItemListPage.Count - 1;
    }
  }

  private void processCommand( Direction directionEnum ) {
    if (WeaponInfoList.Count == 0) return;

    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    weaponItemList[ActualIndex].GetComponent<Image>().enabled = false;
    switch (directionEnum) {
      case Direction.Down:
        moveUpDown( 1 ); break;
      case Direction.Up:
        moveUpDown( -1 ); break;
      case Direction.Left:
        prevPage(); break;
      case Direction.Right:
        nextPage(); break;
      default: break;
    }

    weaponItemList[ActualIndex].GetComponent<Image>().enabled = true;
    updateSelection();
  }

  public WeaponInfo GetSelectedWeapon() {
    if (ActualIndex > WeaponInfoList.Count - 1)
      return null;
    return WeaponInfoList[ActualIndex];
  }

  private int ActualIndex { get { return (currentPage - 1)*itemPerPage + currentSelectedInPage; } }

  private Text pageText;
  private int maxPage;
  private int currentPage;

  private Transform background;
  private int currentSelectedInPage;

  private float lastTime;
  private float maxTime;
  private readonly float defaultMaxTime = 0.2f;
  private Direction directionEnum;

  private enum Direction { Zero = 0, Down = -1, Up = 1, Left = 2, Right = 3 }

  private void moveMenu() {
    bool justDown = false;

    if (Input.GetButtonDown( "Up" ) || Input.GetButtonDown( "Down" ) || Input.GetButtonDown( "Left" ) || Input.GetButtonDown( "Right" )) {
      //lastTime = defaultMaxTime;
      //lastTime = 0;
      maxTime = defaultMaxTime * 5;
      justDown = true;
      //Debug.Log( $"lastTime: {lastTime}; maxTime: {maxTime}" );
    }

    if (Input.GetButton( "Up" )) {
      directionEnum = Direction.Up;
    }
    else if (Input.GetButton( "Down" )) {
      directionEnum = Direction.Down;
    }
    else if (Input.GetButton( "Left" )) {
      directionEnum = Direction.Left;
    }
    else if (Input.GetButton( "Right" )) {
      directionEnum = Direction.Right;
    }

    if (Input.GetButtonUp( "Up" ) || Input.GetButtonUp( "Down" ) || Input.GetButtonUp( "Left" ) || Input.GetButtonUp( "Right" )) {
      directionEnum = Direction.Zero;
      return;
    }

    lastTime += Time.deltaTime;
    if (directionEnum != Direction.Zero && (lastTime > maxTime || justDown)) {
      processCommand( directionEnum );

      lastTime = 0;
      directionEnum = Direction.Zero;
      if (!justDown) {
        maxTime = defaultMaxTime;
      }
      justDown = false;
    }
  }

  private void updateSelection() {
    var weaponInfo = GetSelectedWeapon();
    
    if (weaponInfo?.MaxBullets > 0) {
      background.Find( "UseBullet/RemainText" ).GetComponent<Text>().text = weaponInfo.RemainBullets.ToString();
      background.Find( "UseBullet/MaxText" ).GetComponent<Text>().text = weaponInfo.MaxBullets.ToString();
    }
    else {
      background.Find( "UseBullet/RemainText" ).GetComponent<Text>().text = " - - - ";
      background.Find( "UseBullet/MaxText" ).GetComponent<Text>().text = " - - - ";
    }
    
    background.Find( "UseEN/NeedText" ).GetComponent<Text>().text = weaponInfo.EN > 0 ? weaponInfo.EN.ToString() : " - - - ";
    background.Find( "UseEN/MaxText" ).GetComponent<Text>().text = $" ( {weaponInfo.RobotInfo.EN} / {weaponInfo.RobotInfo.MaxEN} )";

    background.Find( "NeedWillPower/NeedText" ).GetComponent<Text>().text = weaponInfo.WillPower > 0 ? weaponInfo.WillPower.ToString() : " - - - ";
    background.Find( "NeedWillPower/CurrentText" ).GetComponent<Text>().text = $" ( {weaponInfo.PilotInfo.Willpower} / {weaponInfo.PilotInfo.MaxWillpower} )";
    
    background.Find( "TerrainLv/SkyText" ).GetComponent<Text>().text = TerrainHelper.GET_TerrainRank( weaponInfo.TerrainSky );
    background.Find( "TerrainLv/LandText" ).GetComponent<Text>().text = TerrainHelper.GET_TerrainRank( weaponInfo.TerrainLand );
    background.Find( "TerrainLv/SeaText" ).GetComponent<Text>().text = TerrainHelper.GET_TerrainRank( weaponInfo.TerrainSea );
    background.Find( "TerrainLv/SpaceText" ).GetComponent<Text>().text =  TerrainHelper.GET_TerrainRank( weaponInfo.TerrainSpace );

    background.Find( "ImproveBar" ).GetComponent<ImproveBarScript>().SetLevel( weaponInfo.WeaponInstance.Level );
  }

  public void clearSelection() {
    var weaponInfo = GetSelectedWeapon();
    background.Find( "UseBullet/RemainText" ).GetComponent<Text>().text = " - - - ";
    background.Find( "UseBullet/MaxText" ).GetComponent<Text>().text = " - - - ";  
    background.Find( "UseEN/NeedText" ).GetComponent<Text>().text = " - - - ";
    background.Find( "UseEN/MaxText" ).GetComponent<Text>().text = weaponInfo?.RobotInfo == null? " ( --- / --- )" : $" ( {weaponInfo.RobotInfo.EN} / {weaponInfo.RobotInfo.MaxEN} )";

    background.Find( "NeedWillPower/NeedText" ).GetComponent<Text>().text = " - - - ";
    background.Find( "NeedWillPower/CurrentText" ).GetComponent<Text>().text = weaponInfo?.RobotInfo == null ? " ( --- / --- )" : $" ( {weaponInfo.PilotInfo?.Willpower.ToString()?? "---"} / {weaponInfo.PilotInfo?.MaxWillpower.ToString()?? "---"} )";

    background.Find( "TerrainLv/SkyText" ).GetComponent<Text>().text =    " — ";
    background.Find( "TerrainLv/LandText" ).GetComponent<Text>().text =   " — ";
    background.Find( "TerrainLv/SeaText" ).GetComponent<Text>().text =    " — ";
    background.Find( "TerrainLv/SpaceText" ).GetComponent<Text>().text =  " — ";

    background.Find( "ImproveBar" ).GetComponent<ImproveBarScript>().SetLevel( 0 );

    transform.Find( "PageArrow" ).gameObject.SetActive( false );
  }

  public void clearItemList() {
    if (weaponItemList == null) return;
    for (int i = 0; i<weaponItemList.Count; i++) {
      Destroy( weaponItemList[i] );
    }
  } 

}
