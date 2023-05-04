using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAutoMove : MonoBehaviour {

  public float m_defaultSpeed = 20f;
  public float m_lowSpeed = 5f;

  public AudioSource m_MoveSartAudio;
  public AudioSource m_MovementAudio;
  public bool isStanding = true;

  private Rigidbody m_Rigidbody;

  private Camera myCamera;
  private Vector3 moveDirection;

  private Light directLight;
  private float intensityNormal;
  private float intensityDark;

  public GameObject MyUnitWallPrefab;
  private GameObject myUnitWall;

  public GameObject MapRangeCanvasPrefab;
  private GameObject mapRangeCanvas;

  private float moveRange;
  private Vector3 originPos;
  private Vector3 targetPos;

  private NavMeshAgent m_Agent;
  private Action callback;
  private Vector3 lastPos;

  private void Awake() {
    m_Rigidbody = GetComponent<Rigidbody>();
    directLight = GameObject.Find( "Directional light" ).GetComponent<Light>();
    intensityNormal = directLight.intensity;
    intensityDark = intensityNormal * 0;  //(float)0.75;
    m_Agent = GetComponent<NavMeshAgent>();
  }

  private void OnEnable() {
    wallCount = 0;
    GetComponent<NavMeshAgent>().speed = m_defaultSpeed;

    m_Rigidbody.isKinematic = false;
    
    CoroutineCommon.CallWaitForFrames( 1, () => {
      myUnitWall = Instantiate( MyUnitWallPrefab, new Vector3( originPos.x, MyUnitWallPrefab.transform.position.y, originPos.z ),
                      MyUnitWallPrefab.transform.rotation ) as GameObject;
      myUnitWall.GetComponent<RangeColider>().Setup( moveRange );
    } );

    mapRangeCanvas = Instantiate( MapRangeCanvasPrefab ) as GameObject;
    mapRangeCanvas.transform.position = new Vector3( originPos.x, MapRangeCanvasPrefab.transform.position.y, originPos.z );
    mapRangeCanvas.transform.localScale = new Vector3( moveRange, moveRange, transform.localScale.z );

    directLight.intensity = intensityDark;


    lastTime = 0f;
    lastPos = transform.position;
    m_Rigidbody.velocity = new Vector3( 0, 0, 0 );

    CoroutineCommon.CallWaitForOneFrame( () => { 
      var surface = GameObject.Find( "Terrain" ).GetComponent<NavMeshSurface>();
      surface.agentTypeID = GetComponent<UnitInfo>().GetMeshAgentTypeId();

      Debug.Log( "surface.BuildNavMesh()" );
      surface.BuildNavMesh();
      GetComponent<UnitInfo>().GetComponent<NavMeshObstacle>().enabled = false;
      GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().enabled = true;
      m_MovementAudio.Play();
      m_Agent.destination = targetPos;
    } );
   
  }

  public void Setup( float unitRadius, float movePower, Vector3 originPos, Vector3 targetPos, Action callback ) {
    //moveRange = unitRadius + 2 * unitRadius * movePower;
    moveRange = unitRadius + PreBattleFormula.BasicRadiusMove * movePower;

    this.originPos = originPos;
    this.targetPos = targetPos;
    this.callback = callback;

    //this.enabled = true;
    myCamera = Camera.main;
  }


  private void OnDisable() {
    m_Rigidbody.isKinematic = true;

    Destroy( myUnitWall );
    //Destroy( myMoveAreaSplot );
    Destroy( mapRangeCanvas );

    if (directLight != null) directLight.intensity = intensityNormal;

    callback();
  }

  int wallCount = 0;

  private void OnCollisionEnter( Collision collision ) {
    if (collision.gameObject.CompareTag( MyTags.WALL ))
      wallCount++;

    //Debug.Log( $"GetComponent<NavMeshAgent>().speed = m_lowSpeed {m_lowSpeed}" );
    GetComponent<NavMeshAgent>().speed = m_lowSpeed;
  }

  private void OnCollisionExit( Collision collision ) {
    if (collision.gameObject.CompareTag( MyTags.WALL ))
      wallCount = Math.Max( wallCount - 1, 0 );

    if (wallCount == 0) {
      //Debug.Log( $"GetComponent<NavMeshAgent>().speed = m_defaultSpeed {m_defaultSpeed}" );
      GetComponent<NavMeshAgent>().speed = m_defaultSpeed;
    }
  }

  private void Start() {

  }

  private float lastTime;
  void Update() {
    lastTime += Time.deltaTime;
    if (lastTime > .5f) {
      lastTime = 0;

      float distance = (transform.position - lastPos).magnitude;
      Debug.Log( "distance: " + distance );

      lastPos = transform.position;

      if (distance < 1) {
        GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<UnitInfo>().GetComponent<NavMeshObstacle>().enabled = true;

        if (m_MovementAudio.isPlaying) {
          m_MovementAudio.Stop();
          //isStanding = true;
        }
        this.enabled = false;
      }

    }

    /*
    Debug.Log( "<NavMeshAgent>().velocity.sqrMagnitude: " + GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().velocity.sqrMagnitude );
    if (GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().velocity.sqrMagnitude < 2f) {
      //GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().isStopped = true;
      lastTime += Time.deltaTime;

      if (lastTime > .5f) {
        GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<UnitInfo>().GetComponent<NavMeshObstacle>().enabled = true;

        if (m_MovementAudio.isPlaying) {
          m_MovementAudio.Stop();
          //isStanding = true;
        }

        this.enabled = false;
      }
    }*/
  }

  private void LateUpdate() {
    myCamera.GetComponent<MainCamera>().MoveCamToCenterObject( transform );
    /*
    Vector3 viewPos = myCamera.WorldToViewportPoint( transform.position );

    if (viewPos.x > 0.8F ||
        viewPos.x < 0.2F ||
        viewPos.y > 0.8F ||
        viewPos.y < 0.2F
    ) {
      myCamera.GetComponent<MainCamera>().MoveCamToCenterObject( transform );
    }*/
  }

  /*
  private void Update() {
    // Store the player's input and make sure the audio for the engine is playing.
    m_MovementInputValue = Input.GetAxis( m_MovementAxisName );
    m_TurnInputValue = Input.GetAxis( m_TurnAxisName );
    speed();
    EngineAudio();

    if (Input.GetButtonDown( "Confirm" )) {
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuConfirm" ) );
      GetComponent<UnitController>().EndMoveMode();
      m_MovementAudio.Stop();
      isStanding = true;
      this.enabled = false;
    }
    else if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      GetComponent<UnitController>().BackFromMoveMode();
      m_MovementAudio.Stop();
      isStanding = true;
      this.enabled = false;
    }
  }*/

  /*
  private void speed() {
    m_Speed = Input.GetButton( "CursorSpeed" ) ? m_defaultSpeed * 2 : m_defaultSpeed;
  }*/
  /*
  private void EngineAudio() {
    // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
    if (Mathf.Abs( m_MovementInputValue ) < 0.1f && Mathf.Abs( m_TurnInputValue ) < 0.1f) {
      if (m_MovementAudio.isPlaying) {
        m_MovementAudio.Stop();
        isStanding = true;
      }
    }
    else {
      if (!m_MovementAudio.isPlaying) {
        m_MovementAudio.loop = true;
        m_MovementAudio.Play();
      }
      if (isStanding) {
        m_MoveSartAudio.Play();
        m_MoveSartAudio.loop = false;
        isStanding = false;
      }
    }
  }*/

}
