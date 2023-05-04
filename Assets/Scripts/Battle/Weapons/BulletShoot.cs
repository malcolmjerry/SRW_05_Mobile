using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShoot : MonoBehaviour {

  private float lastTime;
  public float DeadTime = 40f;
  public float speed = 20f;
  public float PlaySound = 1f;
  public string SfxName;
  public Vector3 direction;
  public bool Trail;
  public string TrailName;
  public bool Show = true;
  public float TrailTime, Width;

  // Use this for initialization
  void Start() {
    lastTime = 0;

    if (!string.IsNullOrWhiteSpace(SfxName)) {
      if (!Trail)
        CoroutineCommon.CallWaitForSeconds( .2f, () => { EffectSoundController.PLAY((AudioClip)Resources.Load($"SFX/Weapon/{SfxName}"), PlaySound); } );  //M4A1_Single
      else
        EffectSoundController.PLAY((AudioClip)Resources.Load($"SFX/Weapon/{SfxName}"), PlaySound);
    }

    try {
      GetComponent<TrailRenderer>().enabled = Trail;
      GetComponent<TrailRenderer>().time = TrailTime;
      GetComponent<TrailRenderer>().widthMultiplier = Width;
      GetComponent<TrailRenderer>().material = Resources.Load<Material>( $"Materials/{TrailName}" );

      transform.Find( "Renderer" ).gameObject.SetActive( Show );
    }
    catch { }
  }

  // Update is called once per frame
  void Update() {
    lastTime += Time.deltaTime;

    if (lastTime > DeadTime) {
      Destroy( gameObject );
    }
    else {
      //transform.position = new Vector3( transform.position.x, transform.position.y + speed, transform.position.z );
      //transform.Translate( Vector3.down * speed * Time.deltaTime );
      transform.Translate( direction * speed * Time.deltaTime );
    }

  }

  void OnTriggerEnter( Collider other ) {
    //Debug.Log( "BulletShoot OnTriggerEnter.." );
    if (other.gameObject.layer == LayerMask.NameToLayer( "Hitable" )) {
      selfDestroy();
    }
  }

  public GameObject m_ExplosionPrefab;

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
