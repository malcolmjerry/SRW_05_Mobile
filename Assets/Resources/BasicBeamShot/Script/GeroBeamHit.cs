using UnityEngine;
using System.Collections;
using static UnityEngine.ParticleSystem;

public class GeroBeamHit : MonoBehaviour {
  private GameObject ParticleA;
  private GameObject ParticleB;
  private GameObject HitFlash;

  private float PatA_rate;
  private float PatB_rate;

  private ParticleSystem PatA;
  private ParticleSystem PatB;
  public Color col;

  EmissionModule emA, emB;

  public void SetViewPat( bool b ) {
    if (b) {
      //PatA.emissionRate = PatA_rate;
      emA.rateOverTime = PatA_rate;

      //PatB.emissionRate = PatB_rate;
      emB.rateOverTime = PatB_rate;

      HitFlash.GetComponent<Renderer>().enabled = true;
    }
    else {
      //PatA.emissionRate = 0;
      //PatB.emissionRate = 0;
      emA.rateOverTime = 0;
      emB.rateOverTime = 0;

      HitFlash.GetComponent<Renderer>().enabled = false;
    }
  }

  // Use this for initialization
  void Start() {


    col = new Color( 1, 1, 1 );
    ParticleA = transform.Find( "GeroParticleA" ).gameObject;
    ParticleB = transform.Find( "GeroParticleB" ).gameObject;
    HitFlash = transform.Find( "BeamFlash" ).gameObject;
    PatA = ParticleA.gameObject.GetComponent<ParticleSystem>();
    //PatA_rate = PatA.emissionRate;
    //PatA.emissionRate = 0;
    PatB = ParticleB.gameObject.GetComponent<ParticleSystem>();
    //PatB_rate = PatB.emissionRate;
    //PatB.emissionRate = 0;

    emA = PatA.emission;
    emB = PatB.emission;

    PatA_rate = emA.rateOverTime.constant;
    emA.rateOverTime = 0;

    PatB_rate = emB.rateOverTime.constant;
    emB.rateOverTime = 0;

    HitFlash.GetComponent<Renderer>().enabled = false;
  }

  // Update is called once per frame
  void Update() {
    //PatA.startColor = col;
    var mainA = PatA.main;
    mainA.startColor = col;

    //PatB.startColor = col;
    var mainB = PatB.main;
    mainB.startColor = col;

    HitFlash.GetComponent<Renderer>().material.SetColor( "_Color", col*1.5f );
  }
}
