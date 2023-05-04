using UnityEngine;
using System.Collections;
//using UnityEditor;
using System;
using UnityEngine.AI;
using System.Collections.Generic;
using static Menu;

public class UnitController : MonoBehaviour {

  [HideInInspector] public UnitStatusEnum status; //0: Ready 1: action end 2: cursor trigger 3. Move mode
  public enum UnitStatusEnum { READY = 0, ACTION_END, MOVE_MODE, AFTER_MOVE }

  public int RemainActionCount;

  [HideInInspector] public MapManager myMapManager;

  public GameObject m_MapMenuPrefab;
  private GameObject myUnitMenu;
  private Menu menu = null;

  //private int unitMoveSpeed = 6;

  private Vector3 originPosition;
  private Quaternion originRotation;
  private UnitMovement unitMovement;

  [HideInInspector]public string modelName;

  private Camera cam;

  private GameObject m_ExplosionPrefab;
  private AudioSource m_ExplosionAudio;
  private ParticleSystem m_ExplosionParticles;
  private GameObject myCanvasGo;

  void Awake() {
    //m_ExplosionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>( "Assets/Prefabs/TankExplosion.prefab" );
    m_ExplosionPrefab = Resources.Load( "TankExplosion" ) as GameObject;
    /*
    m_ExplosionParticles = Instantiate( m_ExplosionPrefab ).GetComponent<ParticleSystem>();
    m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
    m_ExplosionParticles.gameObject.SetActive( false );
    */
    cam = Camera.main;
    unitMovement = GetComponent<UnitMovement>();
    myCanvasGo = GameObject.Find( "MyCanvas" );
  }

  // Use this for initialization
  void Start () {
    //updateActionEndCanvas();
  }

  public void Setup( string modelName ) {
    this.modelName = modelName;
    //GameObject prefab = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath( "Assets/Prefabs/Units/" + modelName + ".prefab", typeof( GameObject ) );
    GameObject prefab = Resources.Load( "Battle/Units/" + modelName ) as GameObject;
    //GameObject clone = Instantiate( prefab, position, prefab.transform.rotation, transform.Find( "Renderer" ) ) as GameObject;
    //clone.transform.parent = transform.Find( "Renderer" );
    GameObject clone = Instantiate( prefab, transform.Find( "Renderer" ) );
    //clone.transform.Rotate( prefab.transform.eulerAngles );
    clone.GetComponent<BoxCollider>().enabled = false;
  }

  public void RechargeAction() {
    RemainActionCount = GetComponent<UnitInfo>().MapFightingUnit.ActionCount;
    updateActionEndCanvas();
  }

  public void SetActionCount( int count ) {
    RemainActionCount = count;
    updateActionEndCanvas();
  }

  void updateActionEndCanvas() {
    if (RemainActionCount == 0) {
      status = UnitStatusEnum.ACTION_END;
      enabled = false;
    }
    else
      status = UnitStatusEnum.READY;

    transform.Find( "ActionEndCanvas" ).gameObject.SetActive( status == UnitStatusEnum.ACTION_END );
    var aec = transform.Find( "ActionEndCanvas" );
    aec.rotation = cam.transform.rotation;
  }

	// Update is called once per frame
	void Update () {
    //myLookAtCam();

	  if (status == UnitStatusEnum.AFTER_MOVE) {
      if (Input.GetButtonDown( "Confirm" )) {
        EffectSoundController.PLAY_MENU_CONFIRM();
        processMenuCmd();
      }
    }
	}

  public void CursorIn() {
    var unitSelect = GetComponent<UnitSelect>();
    if (!unitSelect)
      return;
    unitSelect.enabled = true;
  }

  public void CursorOut() {
    var unitSelect = GetComponent<UnitSelect>();
    if (!unitSelect)
      return;
    unitSelect.enabled = false;
  }

  public void PreMoveMode( Action callback = null ) {
    originPosition = transform.position;
    originRotation = transform.Find( "Renderer" ).rotation;
    unitMovement.Setup( GetComponent<CapsuleCollider>().radius, GetComponent<UnitInfo>().MapFightingUnit.MovePower, originPosition, callback );
    ToMoveMode();
  }

  public void ToMoveMode() {
    if (status == UnitStatusEnum.ACTION_END)
      return;

    status = UnitStatusEnum.MOVE_MODE;
    unitMovement.enabled = true;
    enabled = false;
  }

  public void EndMoveMode() {
    enabled = true;
    //GetComponent<UnitMovement>().enabled = false;
    Destroy( myUnitMenu );
    myUnitMenu = Instantiate( m_MapMenuPrefab ) as GameObject;
    myUnitMenu.name = "EndMoveMenu";
    myUnitMenu.SetActive( false );
    menu = myUnitMenu.GetComponent<Menu>();
    menu.CmEnumList = new List<CmdEnum> { Menu.CmdEnum.ATK, Menu.CmdEnum.ACTION_END };
    if (GetComponent<UnitInfo>().ShipUnit)
      menu.CmEnumList.Insert( menu.CmEnumList.Count - 1, Menu.CmdEnum.ON_SHIP );

    menu.createCommands( Menu.CMD_WIDTH_SHORT, ToMoveMode );
    myUnitMenu.SetActive( true );

    GetComponent<UnitInfo>().RobotInfo.IsMoved = true;

    status = UnitStatusEnum.AFTER_MOVE;

    //myMapManager.AfterMove();
    //CoroutineCommon.CallWaitForOneFrame( () => { status = UnitStatusEnum.AFTER_MOVE_MENU; } );
  }

  public void BackFromMoveMode() {
    status = UnitStatusEnum.READY;
    //GetComponent<UnitMovement>().enabled = false;
    transform.position = originPosition;
    transform.Find( "Renderer" ).rotation = originRotation;
    GetComponent<UnitInfo>().RobotInfo.IsMoved = false;
    CoroutineCommon.CallWaitForOneFrame( () => myMapManager.ShowMenu() );
    //myMapManager.ShowMenu();
    //myMapManager.gameObject.SetActive( true );
  }

  void processMenuCmd() {
    Menu.CmdEnum cmdEnum = menu.GetSelectedAction();
    //myUnitMenu.SetActive( false );
    Destroy( myUnitMenu );
    //myStatus = (int)MapManStatus.DOING_OTHER;

    enabled = false;

    switch (cmdEnum) {
      case Menu.CmdEnum.ACTION_END:
        EndAction();
        myMapManager.unitSelected = GetComponent<UnitController>();
        myMapManager.BackToMap(false);
        break;
      case Menu.CmdEnum.ATK:
        //status = (int)UnitStatusEnum;
        //myMapManager.unitSelected = GetComponent<UnitController>();
        toWeaponMenu();
        break;
      case Menu.CmdEnum.ON_SHIP:
        doOnShip();
        break;
      default:
        break;
    }
    enabled = false;

    //unitSelected.ProcessCmd( cmdIndex );
  }

  void toWeaponMenu() {
    myMapManager.unitWC.Setup( GetComponent<UnitInfo>(), false, EndMoveMode );
    this.enabled = false;
  }

  public void EndAction() {
    RemainActionCount--;
    updateActionEndCanvas();

    //GetComponent<UnitInfo>().Ready = false;
    GetComponent<NavMeshObstacle>().enabled = false;
    GetComponent<NavMeshObstacle>().enabled = true;
    GetComponent<UnitInfo>().RobotInfo.IsMoved = false;  //2021-08-22 Added
  }

  void doOnShip() {
    enabled = false;
    myCanvasGo.transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
      "進入戰艦，將會消耗一次行動力",
      () => {
        int index = GetComponent<UnitInfo>().ShipUnit.StoringUnits.FindIndex( u => u.RobotInfo.HP < GetComponent<UnitInfo>().RobotInfo.HP );
        if (index < 0) index = GetComponent<UnitInfo>().ShipUnit.StoringUnits.Count;

        var unitInfo = GetComponent<UnitInfo>();
        var ship = unitInfo.ShipUnit;
        ship.StoringUnits.Insert( index, unitInfo );
        gameObject.SetActive( false );
        unitInfo.IsOnShip = true;
        EndAction();
        myMapManager.myMapCursor.GetComponent<Cursor>().SetDisable( true );
        myMapManager.myMapCursor.GetComponent<Cursor>().unitSelected = ship.GetComponent<UnitController>();
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/MapUnitLeave" ), 5 );
        myMapManager.BackToMap( false );
      },
      () => {
        EndMoveMode();
      }
    );

    //EffectSoundController.PLAY_MENU_CONFIRM();
  }

  /*
  public void GetReady() {
    status = UnitStatusEnum.READY;
    transform.Find( "ActionEndCanvas" ).gameObject.SetActive( false );
  }
  */

  public void MyDestroyed( Action callback ) {
    var m_Explosion = Instantiate( m_ExplosionPrefab );
    m_ExplosionParticles = m_Explosion.GetComponent<ParticleSystem>();
    m_ExplosionParticles.name = "boom";
    m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

    m_ExplosionParticles.transform.position = transform.position;
    m_ExplosionParticles.gameObject.SetActive( true );
    m_ExplosionParticles.Play();
    EffectSoundController.PLAY( m_ExplosionAudio.clip, 10 );

    gameObject.SetActive( false );
    CoroutineCommon.CallWaitForSeconds( .5f, () => {
      Destroy( m_Explosion );  
      callback();
    } );
  }

  public void MyLeave( Action callback, float wait = 1f ) {
    /*
    var m_Explosion = Instantiate( m_ExplosionPrefab );
    m_ExplosionParticles = m_Explosion.GetComponent<ParticleSystem>();
    m_ExplosionParticles.name = "boom";
    m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

    m_ExplosionParticles.transform.position = transform.position;
    m_ExplosionParticles.gameObject.SetActive( true );
    m_ExplosionParticles.Play();
    */
    //上面把爆炸效果換成撤退效果

    //EffectSoundController.PLAY( m_ExplosionAudio.clip, 10 );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/MapUnitLeave" ), 5 );

    gameObject.SetActive( false );
    CoroutineCommon.CallWaitForSeconds( wait, () => {
      //Destroy( m_Explosion );
      callback();
    } );
  }

}
