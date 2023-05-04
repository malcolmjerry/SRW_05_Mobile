using UnityEngine;
using System.Collections;

public class MyShake : MonoBehaviour {

  //public float shakeDuration = 0f;

  private float shakeAmount = 0.1f;
  //public float decreaseFactor = 1.0f;

  private Vector3 originalPos;

  void Awake() {

  }

  public MyShake Setup( float shakeAmount ) {
    this.shakeAmount = shakeAmount;
    return this;
  }

  void OnEnable() {
    originalPos = transform.localPosition;
  }

  void Update() {
    transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
  }

  void OnDisable() {
    transform.localPosition = originalPos;
  }

}