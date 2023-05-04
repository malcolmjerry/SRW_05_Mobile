using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour {

  public float Speed = 10f;
  public bool isStop = true;
  public string SfxName;
  public Vector3 Direction;

  //private Transform target;
  private bool isCut;
  private Action callback;

  Animator animator;

  private void Awake() {
    animator = transform.Find( "Renderer" ).GetComponent<Animator>();
  }

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (!isStop)
      transform.Translate( Direction * Speed * Time.deltaTime );
  }

  public void Charge( Action callback ) {
    this.callback = callback;
    isStop = true;

    EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/EnergyCharge" ), 3 );

    animator.Play( "Charge" );
    CoroutineCommon.CallWaitForAnimator( animator, "Charge", () => {
      EffectSoundController.STOP();
      callback();
    } );
  }

  public void Attack( Transform target, bool isCut, Action callback ) {
    transform.LookAt( target );
    this.isCut = isCut;
    this.callback = callback;
    isStop = false;
    animator.enabled = false;

    Debug.Log( "transform.Find( \"Renderer\" ).localScale" + transform.Find( "Renderer" ).localScale );
    transform.Find( "Renderer" ).localScale = new Vector3( 1, 1, 1 );
    Debug.Log( "transform.Find( \"Renderer\" ).localScale" + transform.Find( "Renderer" ).localScale );

    EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/{SfxName}" ), 3 );  //eg. Big Gun
  }

  void OnTriggerEnter( Collider other ) {
    //Debug.Log( "BulletShoot OnTriggerEnter.." );
    if (other.gameObject.layer == LayerMask.NameToLayer( "Hitable" )) {
      if (!isCut) {
        hitStop();
        transform.Find( "Renderer" ).GetComponent<Collider>().enabled = false;
      }
      else {
        selfDestroy();
      }
    }
  }

  public GameObject m_ExplosionPrefab;

  private void hitStop() {
    isStop = true;
    callback();
  }

  private void selfDestroy() {
    var m_Explosion = Instantiate( m_ExplosionPrefab );
    var m_ExplosionParticles = m_Explosion.GetComponent<ParticleSystem>();
    var m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

    m_ExplosionParticles.transform.position = transform.position;
    m_ExplosionParticles.gameObject.SetActive( true );
    m_ExplosionParticles.Play();
    EffectSoundController.PLAY( m_ExplosionAudio.clip, 10 );

    gameObject.SetActive( false );
    CoroutineCommon.CallWaitForSeconds( 2, () => {
      Destroy( m_Explosion );
    } );
  }

}
