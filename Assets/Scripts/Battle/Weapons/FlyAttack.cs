using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAttack : MonoBehaviour {

  public float Speed = 10f;
  public bool PlaySound;
  public string SfxName;
  public Vector3 Direction;

  //private Transform target;
  private bool isCut;
  private Action callback;

  // Use this for initialization
  void Start() {
    if (PlaySound)
      EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/{SfxName}" ), 3 );  //Sickle ?
  }

  // Update is called once per frame
  void Update() {
    transform.Translate( Direction * Speed * Time.deltaTime );   
  }

  public void Setup( Transform target, bool isCut, Action callback ) {
    //this.target = target;
    this.isCut = isCut;
    this.callback = callback;
  }

  void OnTriggerEnter( Collider other ) {
    Debug.Log( "BulletShoot OnTriggerEnter.." );
    if (other.gameObject.layer == LayerMask.NameToLayer( "Hitable" )) {
      if (!isCut) {
        hitStop();
        GetComponent<Collider>().enabled = false;
      }
      else {
        selfDestroy();
      }
    }
  }

  public GameObject m_ExplosionPrefab;

  private void hitStop() {
    var animation = transform.Find( "Renderer" ).GetComponent<Animation>();
    animation["Attacking"].time = 1;
    animation.Stop();
    Speed = 0;
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
