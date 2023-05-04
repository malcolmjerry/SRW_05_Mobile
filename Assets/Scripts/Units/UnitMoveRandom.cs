using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveRandom : MonoBehaviour {

  public float m_defaultSpeed = 18f;

  private Rigidbody m_Rigidbody;
  private Camera myCamera;

  private void Awake() {
    m_Rigidbody = GetComponent<Rigidbody>();
    myCamera = Camera.main;
  }

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  private void OnEnable() {
    m_Rigidbody.isKinematic = false;
    moveOneStep();
  }

  private void OnDisable() {
    m_Rigidbody.isKinematic = true;
  }

  private void moveOneStep() {
    Vector3 forward = Vector3.Scale( myCamera.transform.forward, new Vector3( 1, 0, 1 ) ).normalized;
    Vector3 right = Vector3.Scale( myCamera.transform.right, new Vector3( 1, 0, 1 ) ).normalized;
    float hori = UnityEngine.Random.Range( -1f, 1 );
    float vertical = UnityEngine.Random.Range( -1f, 1 );
    Vector3 moveDirection = (hori * right + vertical * forward).normalized;
    m_Rigidbody.velocity = moveDirection;
    transform.Find( "Renderer" ).rotation = Quaternion.LookRotation( moveDirection );
  }

}
