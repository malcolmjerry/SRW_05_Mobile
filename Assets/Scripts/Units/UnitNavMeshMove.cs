using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//public class UnitNavMeshMove : MonoBehaviour {

//  public bool isStanding = true;

//  public AudioSource m_MoveSartAudio;
//  public AudioSource m_MovementAudio;

//  NavMeshAgent m_Agent;
//  RaycastHit m_HitInfo = new RaycastHit();

//  private Camera myCamera;

//  private Light directLight;
//  private float intensityNormal;
//  private float intensityDark;

//  public GameObject MyUnitWallPrefab;
//  private GameObject myUnitWall;

//  public GameObject MyMoveAreaSplotLightPrefab;
//  //private GameObject myMoveAreaSplot;

//  public GameObject MapRangeCanvasPrefab;
//  private GameObject mapRangeCanvas;

//  //private float movePower;
//  //private float unitRadius;
//  private float moveRange;
//  private Vector3 originPos;
//  //private Quaternion originRotation;

//  private void Awake() {
//    m_Rigidbody = GetComponent<Rigidbody>();
//    directLight = GameObject.Find( "Directional light" ).GetComponent<Light>();
//    intensityNormal = directLight.intensity;
//    intensityDark = intensityNormal * (float)0.75;
//  }

//  private void OnEnable() {
//    m_Rigidbody.isKinematic = false;
//    m_MovementInputValue = 0f;
//    m_TurnInputValue = 0f;


//    myUnitWall = Instantiate( MyUnitWallPrefab, new Vector3( originPos.x, MyUnitWallPrefab.transform.position.y, originPos.z ),
//                          MyUnitWallPrefab.transform.rotation ) as GameObject;
//    //myUnitWall.transform.Find( "CapsuleUnitWallRenderer" ).GetComponent<AddInvertedMeshCollider>().Setup( moveRange );
//    myUnitWall.GetComponent<RangeColider>().Setup( moveRange );
//    /*
//    myMoveAreaSplot = Instantiate( MyMoveAreaSplotLightPrefab, new Vector3( originPos.x, MyMoveAreaSplotLightPrefab.transform.position.y, originPos.z ),
//                                   MyMoveAreaSplotLightPrefab.transform.rotation ) as GameObject;
//    float spotAngle = (movePower - 1) / 19 * 100 + 50;
//    myMoveAreaSplot.GetComponent<Light>().spotAngle = spotAngle;
//    */

//    /*
//    mapRangeCanvas = Instantiate( MapRangeCanvasPrefab, new Vector3( -8, MapRangeCanvasPrefab.transform.position.y, -6 ),
//                                   MapRangeCanvasPrefab.transform.rotation ) as GameObject;*/
//    mapRangeCanvas = Instantiate( MapRangeCanvasPrefab ) as GameObject;
//    mapRangeCanvas.transform.position = new Vector3( originPos.x, MapRangeCanvasPrefab.transform.position.y, originPos.z );
//    mapRangeCanvas.transform.localScale = new Vector3( moveRange, moveRange, transform.localScale.z );

//    directLight.intensity = intensityDark;
//  }

//  public void Setup( float unitRadius, float movePower, Vector3 originPos ) {
//    //this.unitRadius = unitRadius;
//    //this.movePower = movePower;
//    //moveRange = unitRadius + movePower * 2;
//    moveRange = unitRadius + 2 * unitRadius * movePower;

//    this.originPos = originPos;
//  }


//  private void OnDisable() {
//    m_Rigidbody.isKinematic = true;

//    Destroy( myUnitWall );
//    //Destroy( myMoveAreaSplot );
//    Destroy( mapRangeCanvas );

//    if (directLight != null) directLight.intensity = intensityNormal;
//  }


//  private void Start() {
//    myCamera = Camera.main;
//  }


//  private void Update() {
//    speed();
//    EngineAudio();

//    if (Input.GetButtonDown( "Confirm" )) {   //判斷是否已行到目標地點或遇到障礙已停止
//      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuConfirm" ) );
//      GetComponent<UnitController>().EndMoveMode();
//      m_MovementAudio.Stop();
//      isStanding = true;
//      this.enabled = false;
//    }

//  }

//  private void speed() {

//  }

//  private void EngineAudio() {
//    // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
//    if (Mathf.Abs( m_MovementInputValue ) < 0.1f && Mathf.Abs( m_TurnInputValue ) < 0.1f) {
//      if (m_MovementAudio.isPlaying) {
//        //m_MovementAudio.clip = m_EngineIdling;
//        //m_MovementAudio.pitch = Random.Range( m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange );
//        m_MovementAudio.Stop();
//        isStanding = true;
//      }
//    }
//    else {
//      if (!m_MovementAudio.isPlaying) {
//        //m_MovementAudio.clip = m_EngineDriving;
//        //m_MovementAudio.pitch = Random.Range( m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange );
//        m_MovementAudio.loop = true;
//        m_MovementAudio.Play();
//      }
//      if (isStanding) {
//        m_MoveSartAudio.Play();
//        m_MoveSartAudio.loop = false;
//        isStanding = false;
//      }
//    }
//  }


//  private void FixedUpdate() {
//    AutoMove();

//    MoveCamera();
//  }

//  private void AutoMove() {

//  }

//  private void MoveCamera() {
//    Vector3 viewPos = myCamera.WorldToViewportPoint( transform.position );

//    if (viewPos.x > 0.8F && m_TurnInputValue > 0 ||
//        viewPos.x < 0.2F && m_TurnInputValue < 0 ||
//        viewPos.y > 0.8F && m_MovementInputValue > 0 ||
//        viewPos.y < 0.2F && m_MovementInputValue < 0
//        ) {
//      myCamera.transform.parent = transform;
//    }
//    else {
//      myCamera.transform.parent = null;
//    }

//  }

//}
