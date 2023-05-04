using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMovement : MonoBehaviour {

  public int m_PlayerNumber = 1;
  public float m_TurnSpeed = 180f;

  public AudioSource m_MoveSartAudio;
  public AudioSource m_MovementAudio;
  //public AudioClip m_MoveStart;
  //public AudioClip m_MoveDriving;
  public bool isStanding = true;

  //public float m_PitchRange = 0.2f;
  public float m_defaultSpeed = 18f;
  float m_Speed;

  private string m_MovementAxisName;
  private string m_TurnAxisName;
  private Rigidbody m_Rigidbody;
  private float m_MovementInputValue;
  private float m_TurnInputValue;
  private float m_OriginalPitch;

  private Camera myCamera;
  private Vector3 moveDirection;

  private Light directLight;
  private float intensityNormal;
  private float intensityDark;

  public GameObject MyUnitWallPrefab;
  private GameObject myUnitWall;

  //public GameObject MyMoveAreaSplotLightPrefab;
  //private GameObject myMoveAreaSplot;

  public GameObject MapRangeCanvasPrefab;
  private GameObject mapRangeCanvas;

  //private float movePower;
  //private float unitRadius;
  private float moveRange;
  private Vector3 originPos;
  //private Quaternion originRotation;
  Vector3 forward;
  Vector3 right;

  private Action callback;

  private void Awake() {
    m_Rigidbody = GetComponent<Rigidbody>();
    directLight = GameObject.Find( "Directional light" ).GetComponent<Light>();
    intensityNormal = directLight.intensity;
    intensityDark = intensityNormal * 0;   //(float)0.75;
    myCamera = Camera.main;
  }

  private void OnEnable() {
    m_Rigidbody.isKinematic = false;
    m_MovementInputValue = 0f;
    m_TurnInputValue = 0f;


    myUnitWall = Instantiate( MyUnitWallPrefab, new Vector3( originPos.x, MyUnitWallPrefab.transform.position.y, originPos.z ),
                          MyUnitWallPrefab.transform.rotation ) as GameObject;
    //myUnitWall.transform.Find( "CapsuleUnitWallRenderer" ).GetComponent<AddInvertedMeshCollider>().Setup( moveRange );
    myUnitWall.GetComponent<RangeColider>().Setup( moveRange );
    /*
    myMoveAreaSplot = Instantiate( MyMoveAreaSplotLightPrefab, new Vector3( originPos.x, MyMoveAreaSplotLightPrefab.transform.position.y, originPos.z ),
                                   MyMoveAreaSplotLightPrefab.transform.rotation ) as GameObject;
    float spotAngle = (movePower - 1) / 19 * 100 + 50;
    myMoveAreaSplot.GetComponent<Light>().spotAngle = spotAngle;
    */

    /*
    mapRangeCanvas = Instantiate( MapRangeCanvasPrefab, new Vector3( -8, MapRangeCanvasPrefab.transform.position.y, -6 ),
                                   MapRangeCanvasPrefab.transform.rotation ) as GameObject;*/
    mapRangeCanvas = Instantiate( MapRangeCanvasPrefab ) as GameObject;
    mapRangeCanvas.transform.position = new Vector3( originPos.x, MapRangeCanvasPrefab.transform.position.y, originPos.z );
    mapRangeCanvas.transform.localScale = new Vector3(  moveRange, moveRange, transform.localScale.z );

    directLight.intensity = intensityDark;

    //myCamera = Camera.main;
    forward = Vector3.Scale( myCamera.transform.forward, new Vector3( 1, 0, 1 ) ).normalized;
    right = Vector3.Scale( myCamera.transform.right, new Vector3( 1, 0, 1 ) ).normalized;

  }

  public void Setup( float unitRadius, float movePower, Vector3 originPos, Action callback = null ) {
    //this.unitRadius = unitRadius;
    //this.movePower = movePower;
    //moveRange = unitRadius + movePower * 2;
    this.callback = callback;
    moveRange = unitRadius + PreBattleFormula.BasicRadiusMove * movePower;

    this.originPos = originPos;
  }


  private void OnDisable() {
    m_Rigidbody.isKinematic = true;

    Destroy( myUnitWall );
    //Destroy( myMoveAreaSplot );
    Destroy( mapRangeCanvas );

    if (directLight != null) directLight.intensity = intensityNormal;
  }


  private void Start() {
    m_MovementAxisName = "Vertical";// + m_PlayerNumber;
    m_TurnAxisName = "Horizontal";// + m_PlayerNumber;

    //m_OriginalPitch = m_MovementAudio.pitch;
  }


  private void Update() {
    m_MovementInputValue = Input.GetAxis( m_MovementAxisName );
    m_TurnInputValue = Input.GetAxis( m_TurnAxisName );
    speed();
    EngineAudio();

    if (Input.GetButtonDown( "Confirm" )) {
      if (!checkOverlap()) {
        EffectSoundController.PLAY_ACTION_FAIL();
        Debug.Log( "不能站在此處" );
        return;
      }
      EffectSoundController.PLAY_MENU_CONFIRM();
      GetComponent<UnitController>().EndMoveMode();
      m_MovementAudio.Stop();
      isStanding = true;
      enabled = false;
    }
    else if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      m_MovementAudio.Stop();
      isStanding = true;
      enabled = false;

      if (callback != null)
        callback();
      else GetComponent<UnitController>().BackFromMoveMode();
    }
  }

  private void speed() {
    m_Speed = Input.GetButton( "CursorSpeed" ) ? m_defaultSpeed * 2 : m_defaultSpeed;
  }

  private void EngineAudio() {
    // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
    if (Mathf.Abs( m_MovementInputValue ) < 0.1f && Mathf.Abs( m_TurnInputValue ) < 0.1f) {
      if (m_MovementAudio.isPlaying) {
        //m_MovementAudio.clip = m_EngineIdling;
        //m_MovementAudio.pitch = Random.Range( m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange );
        m_MovementAudio.Stop();
        isStanding = true;
      }
    } else {
      if (!m_MovementAudio.isPlaying) {
        //m_MovementAudio.clip = m_EngineDriving;
        //m_MovementAudio.pitch = Random.Range( m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange );
        m_MovementAudio.loop = true;
        m_MovementAudio.Play();
      }
      if (isStanding) {
        m_MoveSartAudio.Play();
        m_MoveSartAudio.loop = false;
        isStanding = false;
      }
    }
  }


  private void FixedUpdate() {
    //m_Rigidbody.velocity = new Vector3( 0, 0, 0 );
    // Move and turn the tank.
    Move();
    Turn();
    MoveCamera();
  }

  private void Move() {
    // Adjust the position of the tank based on the player's input.
    /*
    Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
    m_Rigidbody.MovePosition( m_Rigidbody.position + movement );
    */


    ///Vector3 forward = myCamera.transform.TransformDirection( Vector3.forward );
    ///forward.y = 0;
    ///Vector3 right = Quaternion.Euler( 0, 90, 0 ) * forward;// * (float)1;

    //Vector3 forward = Vector3.Scale( myCamera.transform.forward, new Vector3( 1, 0, 1 ) ).normalized;
    //Vector3 right = Vector3.Scale( myCamera.transform.right, new Vector3( 1, 0, 1 ) ).normalized;

    moveDirection = (m_TurnInputValue * right + m_MovementInputValue * forward).normalized;
    //moveDirection = (m_TurnInputValue * right + m_MovementInputValue * forward);
    ////Vector3 movement = moveDirection * m_Speed * Time.deltaTime;
    Vector3 movement = moveDirection * m_Speed;

    /*
    if (m_TurnInputValue != 0 && m_MovementInputValue != 0) {
      movement = movement * (float)1;
    }*/

    //m_Rigidbody.MovePosition( m_Rigidbody.position + movement );
    //Debug.Log( $"Move() movement {movement}" );
    m_Rigidbody.velocity = movement;

    //boundary();

    //moveDirection *= (float)1; //speed
    //Boundary();
    //transform.Translate( moveDirection );

  }

  public void MoveOneStep( Action callback ) {
    forward = Vector3.Scale( myCamera.transform.forward, new Vector3( .1f, 0, .1f ) ).normalized;
    right = Vector3.Scale( myCamera.transform.right, new Vector3( .1f, 0, .1f ) ).normalized;
    float hori = 1; //UnityEngine.Random.Range( -1f, 1 );
    float vertical = 1; //UnityEngine.Random.Range( -1f, 1 );

    //Debug.Log( $"hori {hori}, vertical {vertical}" );
    //Debug.Log( $"right {right} forward {forward}" );

    moveDirection = (hori * right + vertical * forward).normalized * m_defaultSpeed;

    //Debug.Log( $"MoveOneStep() moveDirection {moveDirection}" );

    m_Rigidbody.velocity = moveDirection;

    CoroutineCommon.CallWaitForSeconds( .1f, 
      () => {
        m_Rigidbody.velocity = Vector3.zero;
        callback();
      }
    );

    /*
    CoroutineCommon.CallWaitForOneFrame( () => {
      m_Rigidbody.velocity = Vector3.zero;
      callback();
    } );
    */
  }

  private void Turn() {
    // Adjust the rotation of the tank based on the player's input.
    /*
    float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
    Quaternion turnRotation = Quaternion.Euler( 0f, turn, 0f );
    m_Rigidbody.MoveRotation( m_Rigidbody.rotation * turnRotation );
    */
    if (!Input.GetButton( "Vertical" ) && !Input.GetButton( "Horizontal" )) {
      m_TurnInputValue = m_MovementInputValue = 0;
    }

    if (m_MovementInputValue != 0 || m_TurnInputValue != 0) {
      transform.Find( "Renderer" ).rotation = Quaternion.LookRotation( moveDirection );
    }
  }

  
  private void MoveCamera() {
    Vector3 viewPos = myCamera.WorldToViewportPoint( transform.position );


    if (viewPos.x > 0.8F && m_TurnInputValue > 0 ||
        viewPos.x < 0.2F && m_TurnInputValue < 0 ||
        viewPos.y > 0.8F && m_MovementInputValue > 0 ||
        viewPos.y < 0.2F && m_MovementInputValue < 0
        ) {
      //Debug.Log( "AAA" );
      myCamera.transform.parent = transform;
    } else {
      //Debug.Log( "BBB" );
      myCamera.transform.parent = null;
    }

  }

  public void ShowMoveRange() {
    var unitRadius = GetComponent<CapsuleCollider>().radius;
    var movePower = GetComponent<UnitInfo>().MapFightingUnit.MovePower;
    //moveRange = unitRadius + 2 * unitRadius * movePower;
    moveRange = unitRadius + PreBattleFormula.BasicRadiusMove * movePower;

    mapRangeCanvas = Instantiate( MapRangeCanvasPrefab ) as GameObject;
    mapRangeCanvas.transform.position = new Vector3( transform.position.x, MapRangeCanvasPrefab.transform.position.y, transform.position.z );
    mapRangeCanvas.transform.localScale = new Vector3( moveRange, moveRange, transform.localScale.z );

    directLight.intensity = intensityDark;
  }

  public void CancelMoveRange() {
    Destroy( mapRangeCanvas );
    if (directLight != null) 
      directLight.intensity = intensityNormal;
  }

  private bool checkOverlap() {

    if (collidedUnits.Count == 0)
      return true;

    foreach (var colUnit in collidedUnits) {
      if (colUnit.gameObject.layer == LayerMask.NameToLayer( "Player" ) || colUnit.gameObject.layer == LayerMask.NameToLayer( "Enemy" )) {
        var twoRadius = colUnit.GetComponent<CapsuleCollider>().radius + transform.GetComponent<CapsuleCollider>().radius;
        if (Vector3.Distance( transform.position, colUnit.position ) < (twoRadius - .1f)) {
          return false;
        }
      }
    }

    return true;
  }

  private List<Transform> collidedUnits = new List<Transform>();
  private void OnCollisionEnter( Collision collision ) {
    if (collision.gameObject.layer == LayerMask.NameToLayer( "Player" ) || collision.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
      collidedUnits.Add( collision.transform );
  }

  private void OnCollisionExit( Collision collision ) {
    if (collision.gameObject.layer == LayerMask.NameToLayer( "Player" ) || collision.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
      collidedUnits.Remove( collision.transform );
  }

  /*
  private void boundary() {
    //print( transform.position.x + ", " + transform.position.z );

    //if (transform.position.x >= 49 && m_HorInputValue > 0) {
    if (transform.position.x - originPos.x >= moveRange) {
      transform.position = new Vector3( originPos.x + moveRange, transform.position.y, transform.position.z );
      //moveDirection = new Vector3( 0, moveDirection.y, moveDirection.z );
    }
    //if (transform.position.x <= -49 && m_HorInputValue < 0) {
    if (originPos.x - transform.position.x >= moveRange) {
      transform.position = new Vector3( originPos.x - moveRange, transform.position.y, transform.position.z );
      //moveDirection = new Vector3( 0, moveDirection.y, moveDirection.z );
    }
    //if (transform.position.z >= 49 && m_VerInputValue > 0) {
    if (transform.position.z - originPos.z >= moveRange) {
      transform.position = new Vector3( transform.position.x, transform.position.y, originPos.z + moveRange );
      //moveDirection = new Vector3( moveDirection.x, moveDirection.y, 0 );
    }
    //if (transform.position.z <= -49 && m_VerInputValue < 0) {
    if (originPos.z - transform.position.z >= moveRange) {
      transform.position = new Vector3( transform.position.x, transform.position.y, originPos.z - moveRange );
      //moveDirection = new Vector3( moveDirection.x, moveDirection.y, 0 );
    }
    //return moveDirection;
  }*/

}