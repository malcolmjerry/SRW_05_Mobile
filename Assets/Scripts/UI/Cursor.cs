using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnitInfo;
using System.Linq;
using UnityEngine.Windows;
using Cinemachine;

public class Cursor : MonoBehaviour {

  private string m_MovementAxisName;
  private string m_TurnAxisName;

  private float m_VerInputValue;
  private float m_HorInputValue;

  private Camera myCamera;
  private Vector3 moveDirection;
  //private Rigidbody m_Rigidbody;
  //public float defaultY = 1;

  public float m_defaultSpeed;
  private float m_Speed;
  //public float m_ExplosionRadius = 5f;
  //public LayerMask m_TankMask;

  [HideInInspector] public UnitController unitSelected;
  public float defaultY = 0f;
  //[HideInInspector] public int MODE = 0;   //0
  [HideInInspector] public enum ModeEnum { MAP = 0, RANGE = 1, Talk }
  [HideInInspector] public ModeEnum MODE_ENUM;

  private UnitInfo fromUnit = null;
  //private int minRange, maxRange;
  private List<GameObject> inRangeUnits;
  private WeaponInfo wpInfo = null;

  public GameObject m_TargetInfoPrefab;
  private Transform renderModel;

  MySRWInput mySRWInput;
  Vector2 moveVector2;
  public Transform CamLootAtMe;

  private void Awake() {
    //m_Rigidbody = GetComponent<Rigidbody>();
    //renderModel = transform.Find( "renderer" );
    mySRWInput = new MySRWInput();
    mySRWInput.Player.Move.performed += ctx => moveVector2 = ctx.ReadValue<Vector2>();
    mySRWInput.Player.Move.canceled += ctx => moveVector2 = Vector2.zero;

    mySRWInput.Player.Accel.performed += ctx => m_Speed = m_defaultSpeed * 3;
    mySRWInput.Player.Accel.canceled += ctx => m_Speed = m_defaultSpeed;

    m_Speed = m_defaultSpeed;
  }

  // Use this for initialization
  void Start() {
    m_MovementAxisName = "Vertical";
    m_TurnAxisName = "Horizontal";
    myCamera = Camera.main;

    //targetInfoGo = Instantiate( m_TargetInfoPrefab );
    //targetInfoGo.SetActive( false );
  }

  // Update is called once per frame
  void Update() {
    //m_VerInputValue = Input.GetAxis( m_MovementAxisName );  //2023-05-04
    //m_HorInputValue = Input.GetAxis( m_TurnAxisName );      //2023-05-04

    //speed();
  }

  //private void FixedUpdate() {
  private void LateUpdate() {
    //if (m_VerInputValue != 0 || m_HorInputValue != 0) {
    if (moveVector2 != Vector2.zero) {   //2023-05-04
      //Debug.Log( "moveVector2 != Vector2.zero" );
      Move();
      MoveCamera();
    }
    else {

    }
  }

  //public void ChangeModeWithDistance( ModeEnum modeEnum, UnitInfo fromUnit, int minRange, int maxRange ) {
  public void ChangeModeWithDistance( ModeEnum modeEnum, UnitInfo fromUnit, List<GameObject> inRangeUnits, WeaponInfo wpInfo ) {
    this.MODE_ENUM = modeEnum;
    this.fromUnit = fromUnit;
    //this.minRange = minRange;
    //this.maxRange = maxRange;
    this.inRangeUnits = inRangeUnits;
    this.wpInfo = wpInfo;
  }

  private void speed() {
    //m_Speed = Input.GetButton( "CursorSpeed" ) ? m_defaultSpeed * 3 : m_defaultSpeed;  //2023-05-04
  }

  private void Move() {
    Vector3 forward = myCamera.transform.TransformDirection( Vector3.forward );
    forward.y = 0;
    //forward = forward.normalized;
    //Vector3 right = new Vector3( forward.x, 0, -forward.z );
    Vector3 right = Quaternion.Euler( 0, 90, 0 ) * forward * (float)1;

    //moveDirection = (m_HorInputValue * right + m_VerInputValue * forward);  //2023-05-04
    //if (m_HorInputValue != 0 && m_VerInputValue != 0) {   //2023-05-04
      ////moveDirection = moveDirection * (float)0.5;
    //}
    moveDirection = moveVector2.x * right + moveVector2.y * forward;
    //Debug.Log( "moveDirection: " + moveDirection );
    moveDirection *= m_Speed; //speed
    moveDirection *= Time.deltaTime; //speed
    transform.Translate( moveDirection );
    boundary();

    /*
    Vector3 forward = myCamera.transform.TransformDirection( Vector3.forward );
    forward.y = 0;
    Vector3 right = Quaternion.Euler( 0, 90, 0 ) * forward * (float)1;

    moveDirection = (m_HorInputValue * right + m_VerInputValue * forward);
    //moveDirection = boundary( moveDirection );
    Vector3 movement = moveDirection * m_Speed * Time.deltaTime;

    m_Rigidbody.MovePosition( m_Rigidbody.position + movement );
    */
  }

  private void MoveCamera() {
    //Debug.Log( "myCamera: " + myCamera.name );
    Vector3 viewPos = myCamera.WorldToViewportPoint( transform.position );

    //Debug.Log( "viewPos: " + viewPos );

    /*
    if (viewPos.x > 0.9F && m_HorInputValue > 0 ||
        viewPos.x < 0.1F && m_HorInputValue < 0 ||
        viewPos.y > 0.9F && m_VerInputValue > 0 ||
        viewPos.y < 0.1F && m_VerInputValue < 0
        ) {
    */
    if (viewPos.x > 1 || viewPos.x < 0 || viewPos.y > 1 || viewPos.y < 0) {
      CamLootAtMe.position = transform.position;
      CamLootAtMe.parent = null;
    }
    else if (viewPos.x > 0.9F && moveVector2.x > 0 || 
        viewPos.x < 0.1F && moveVector2.x < 0 || 
        viewPos.y > 0.9F && moveVector2.y > 0 || 
        viewPos.y < 0.1F && moveVector2.y < 0) {
      //myCamera.transform.parent = transform;   //2023-05-05
      CamLootAtMe.parent = transform;
    }
    else {
      //myCamera.transform.parent = null;   //2023-05-05
      CamLootAtMe.parent = null;   
    }
  }

  void OnTriggerEnter( Collider other ) {
    //if (MODE_ENUM == ModeEnum.Talk)
      //return;

    var newUnitSelected = other.GetComponent<UnitController>();
    if (newUnitSelected == null)
      return;

    if (unitSelected != null) {
      unitSelected.GetComponent<UnitController>().CursorOut();
      unitSelected = null;
    }

    unitSelected = newUnitSelected;
    unitSelected.CursorIn();

    // 0 敵方(紅)  1 味方(藍)  2 味方 NPC(紫)  3 中立 (黃)
    if (MODE_ENUM == ModeEnum.MAP) {
      if (inRangeUnits == null || inRangeUnits.Count == 0 || inRangeUnits.Any( u => u == other.gameObject )) 
        GetComponent<UnitStatusController>().Setup( unitSelected.GetComponent<UnitInfo>() );
    }
    else if (MODE_ENUM == ModeEnum.RANGE) {
      if (inRangeUnits == null || inRangeUnits.Count == 0 || inRangeUnits.Any( u => u == other.gameObject )) 
        showInfoPreview();
    }
  }

  public GameObject targetInfoGo;
  private void showInfoPreview(  ) {
    //targetInfoGo = Instantiate( m_TargetInfoPrefab );
    targetInfoGo.SetActive( true );
    var canvasRect = targetInfoGo.GetComponent<RectTransform>();

    RectTransform uiRect = targetInfoGo.transform.Find( "Panel" ).GetComponent<RectTransform>();

    //then you calculate the position of the UI element
    //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. 
    //Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
    Vector2 viewportPositionUnit = Camera.main.WorldToViewportPoint( unitSelected.transform.position );
    Vector2 previewPos = viewportPositionUnit.x >= 0.5f ? new Vector2( 0.25f, 0.5f ) : new Vector2( 0.75f, 0.5f );

    Vector2 previewPosInCanvas = new Vector2( (previewPos.x - 0.5f) * canvasRect.sizeDelta.x, (previewPos.y - 0.5f) * canvasRect.sizeDelta.y );
    uiRect.anchoredPosition = previewPosInCanvas;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/computer-beep-1" ) );

    targetInfoGo.GetComponent<BattleInfoPreview>().Setup( fromUnit, unitSelected.GetComponent<UnitInfo>(), wpInfo, null );
  }

  /*
  private void destroyInfoPreview() {
    //Destroy( targetInfoGo );
    targetInfoGo?.SetActive( false );
  }
  */

  void OnTriggerExit( Collider other ) {
    //print( "No longer in contact with " + other.transform.name );
    /*
    if (other.gameObject.layer == LayerMask.NameToLayer( "Player" ) || other.gameObject.layer == LayerMask.NameToLayer( "Enermy" )) {
      if (unitSelected != null) {
        unitSelected.GetComponent<UnitController>().CursorOut();
        unitSelected = null;
      }
      //resetBoxCollider();
    }*/
    //if (new List<int> { LayerMask.NameToLayer( "Player" ), LayerMask.NameToLayer( "Enermy" ),
                        //LayerMask.NameToLayer( "Friend_NPC" ), LayerMask.NameToLayer( "Yellow_NPC" ) }.Contains( other.gameObject.layer )) {
    if (unitSelected != null && unitSelected == unitSelected.GetComponent<UnitController>()) {
      unitSelected.GetComponent<UnitController>().CursorOut();
      unitSelected = null;

      if (MODE_ENUM == ModeEnum.RANGE && targetInfoGo) {
        targetInfoGo.SetActive( false );
      }
      else if (MODE_ENUM == ModeEnum.MAP)
        GetComponent<UnitStatusController>().SetInactive();
    }
    //}
    /*
    else 
    if (other.gameObject.layer == LayerMask.NameToLayer( "MapThing" )) {
      //var box = other.GetComponent<BoxCollider>();
      //float boxY = box.center.y + box.size.y;
      //transform.position = new Vector3( transform.position.x, defaultY, transform.position.z );
      renderModel.transform.position = new Vector3( renderModel.transform.position.x, defaultY, renderModel.transform.position.z );
    }
    */
    // 上面清除所有狀態和高度後, 重新觸發Trigger Enter
    resetBoxCollider();
  }


  //private Vector3 boundary( Vector3 moveDirection ) {
  private void boundary() {
    //print( transform.position.x + ", " + transform.position.z );

    //if (transform.position.x >= 49 && m_HorInputValue > 0) {
    if (transform.position.x >= 49.5f) {
      transform.position = new Vector3( 49.5f, transform.position.y, transform.position.z );
      //moveDirection = new Vector3( 0, moveDirection.y, moveDirection.z );
    }
    //if (transform.position.x <= -49 && m_HorInputValue < 0) {
    if (transform.position.x <= -49.5f) {
      transform.position = new Vector3( -49.5f, transform.position.y, transform.position.z );
      //moveDirection = new Vector3( 0, moveDirection.y, moveDirection.z );
    }
    //if (transform.position.z >= 49 && m_VerInputValue > 0) {
    if (transform.position.z >= 49.5f) {
      transform.position = new Vector3( transform.position.x, transform.position.y, 49.5f );
      //moveDirection = new Vector3( moveDirection.x, moveDirection.y, 0 );
    }
    //if (transform.position.z <= -49 && m_VerInputValue < 0) {
    if (transform.position.z <= -49.5f) {
      transform.position = new Vector3( transform.position.x, transform.position.y, -49.5f );
      //moveDirection = new Vector3( moveDirection.x, moveDirection.y, 0 );
    }
    //return moveDirection;
  }


  public void resetBoxCollider() {
    //Debug.Log( "resetBoxCollider GetComponent<BoxCollider>().enabled = false " );
    GetComponent<CursorCollision>().ResetBoxCollider();

    /*
    GetComponent<BoxCollider>().enabled = false;
    CoroutineCommon.CallWaitForOneFrame( () => {
      if (this)
        GetComponent<BoxCollider>().enabled = true;
    } );
    */
  }

  public void SetPosition( Vector3 position, bool moveCam = true ) {
    transform.position = new Vector3( position.x, defaultY, position.z );
    resetBoxCollider();

    if (moveCam) {
      ResetCamera();
    }
  }

  public void ResetCamera() {
    Vector3 viewPos = Camera.main.WorldToViewportPoint( transform.position );

    if (viewPos.x > 0.9F || viewPos.x < 0.1F || viewPos.y > 0.9F || viewPos.y < 0.1F) {
      SetMainCamToSelf();
    }
  }

  public void SetMainCamToSelf() {
    //var a = Camera.main.GetComponent<MainCamera>();
    Camera.main.GetComponent<MainCamera>().MoveCamToCenterObject( transform );
  }

  /*
  private static readonly Plane plane = new Plane( Vector3.up, Vector3.zero );
  private static readonly Vector3 v3Center = new Vector3( 0.5f, 0.5f, 0.0f );
  public void SetMainCamToSelf() {
    Vector3 v3Hit = GetPositionByRay( v3Center );
    Vector3 goPos = new Vector3( transform.position.x, 0, transform.position.z );
    Vector3 v3Delta = goPos - v3Hit;
    var camPos = Camera.main.transform.position;
    Camera.main.transform.position = new Vector3( camPos.x + v3Delta.x, camPos.y, camPos.z + v3Delta.z );
  }

  private Vector3 GetPositionByRay( Vector3 viewPort ) {
    Ray ray = Camera.main.ViewportPointToRay( viewPort );
    float fDist;
    plane.Raycast( ray, out fDist );
    Vector3 v3Hit = ray.GetPoint( fDist );
    return v3Hit;
  }
  */
  public void SetDisable( bool clearUnitSelected = false ) {
    if (myCamera)
      myCamera.transform.parent = null;
    //enabled = false;  //以此觸發 OnDisable
    if (unitSelected != null) {
      unitSelected.GetComponent<UnitController>().CursorOut();
      //targetInfoGo?.SetActive( false );
      //GetComponent<UnitStatusController>().SetInactive();
      if (MODE_ENUM == ModeEnum.RANGE && targetInfoGo)
        targetInfoGo.SetActive( false );
      else if (MODE_ENUM == ModeEnum.MAP)
        GetComponent<UnitStatusController>().SetInactive();
    }

    if (clearUnitSelected)
      unitSelected = null;

    gameObject.SetActive( false );
  }

  void OnDisable() {
    /*
    if (unitSelected != null) {
      unitSelected.GetComponent<UnitController>().CursorOut();
      //unitSelected = null;   //雖然游標不顯示, 但原本選中的機體可能還有用, 例如 Menu 倒退時
      destroyInfoPreview();
      GetComponent<UnitStatusController>().SetInactive();
    }
    */
    mySRWInput.Disable();
  }
  
  void OnEnable() {
    //resetBoxCollider();
    mySRWInput.Enable();
    //CamLootAtMe.position = transform.position;
  }

}
