using UnityEngine;
using UnityEngine.UI;

public class UnitHealth : MonoBehaviour {
  public float m_StartingHealth = 100f;
  public Slider m_Slider;
  public Image m_FillImage;
  public Color m_FullHealthColor; //= Color.green;
  public Color m_ZeroHealthColor = Color.gray;
  public GameObject m_ExplosionPrefab;


  private AudioSource m_ExplosionAudio;
  private ParticleSystem m_ExplosionParticles;
  private float m_CurrentHealth;
  private bool m_Dead;

  public Transform myCanvasTrans;

  private void Awake() {
    //m_ExplosionParticles = Instantiate( m_ExplosionPrefab ).GetComponent<ParticleSystem>();
    //m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
    //m_ExplosionParticles.gameObject.SetActive( false );
  }

  private void Update() {
    //HealthLookAt();
  }

  private void OnEnable() {
    //m_CurrentHealth = m_StartingHealth / 2;
    m_Dead = false;

    SetHealthUI();
  }


  public void TakeDamage( float amount ) {
    // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
    m_CurrentHealth -= amount;
    SetHealthUI();

    if (m_CurrentHealth <= 0f && !m_Dead) {
      OnDeath();
    }
  }


  public void SetHealthUI() {
    int maxHp = GetComponent<UnitInfo>().RobotInfo.MaxHP;
    int nowHp = GetComponent<UnitInfo>().RobotInfo.HP;
    float percent = (float)nowHp * 100 / maxHp;
    m_Slider.value = percent;
    // Adjust the value and colour of the slider.
    //m_Slider.value = m_CurrentHealth;
    //m_FillImage.color = Color.Lerp( m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth );
  }


  private void OnDeath() {
    // Play the effects for the death of the tank and deactivate it.

    m_ExplosionParticles = Instantiate( m_ExplosionPrefab ).GetComponent<ParticleSystem>();
    m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
    //m_ExplosionParticles.gameObject.SetActive( false );

    m_Dead = true;
    m_ExplosionParticles.transform.position = transform.position;
    m_ExplosionParticles.gameObject.SetActive( true );
    m_ExplosionParticles.Play();
    m_ExplosionAudio.Play();
    gameObject.SetActive( false );

    Destroy( m_ExplosionParticles, 2 );
  }


  private void HealthLookAt() {
    //var canvas = GetComponent<Canvas>();
    //var cam = Camera.main;
    //var canvasTrans = myCanvasTrans;
    //Vector3 viewport = cam.WorldToViewportPoint( myCanvasTrans.position );

    //Vector3 v = cam.transform.position - myCanvasTrans.position ;
    //myCanvasTrans.transform.LookAt( cam.transform.position );
    //v.x = v.z = 0.0f;
    //myCanvasTrans.transform.LookAt( cam.transform.position );
    //var xRotate = (((float)0.5 - viewport.y) / (float)0.5 ) / ;

    //myCanvasTrans.transform.Rotate( , 0, 0 );
  }

}