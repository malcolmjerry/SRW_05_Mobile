using UnityEngine;
using System.Collections;

public class CursorOld : MonoBehaviour {

  private string m_MovementAxisName;
  private string m_TurnAxisName;

  private float m_VerInputValue;
  private float m_HorInputValue;

  private Camera myCamera;
  private Vector3 moveDirection;

  // Use this for initialization
  void Start() {
    m_MovementAxisName = "Vertical";
    m_TurnAxisName = "Horizontal";
    myCamera = Camera.main;
  }

  // Update is called once per frame
  void Update() {
    m_VerInputValue = Input.GetAxis( m_MovementAxisName );
    m_HorInputValue = Input.GetAxis( m_TurnAxisName );
    Move();
    MoveCamera();
  }


  private void Move() {
    Vector3 forward = myCamera.transform.TransformDirection( Vector3.forward );
    forward.y = 0;
    //forward = forward.normalized;
    //Vector3 right = new Vector3( forward.x, 0, -forward.z );
    Vector3 right = Quaternion.Euler( 0, 90, 0 ) * forward * (float)1;

    moveDirection = (m_HorInputValue * right + m_VerInputValue * forward);
    if (m_HorInputValue != 0 && m_VerInputValue != 0) {
      moveDirection = moveDirection * (float)0.5;
    }
    moveDirection *= (float)1; //speed

    Boundary();

    transform.Translate( moveDirection );
  }

  private void MoveCamera() {
    Vector3 viewPos = myCamera.WorldToViewportPoint( transform.position );


    if (viewPos.x > 0.8F && m_HorInputValue > 0 ||
        viewPos.x < 0.2F && m_HorInputValue < 0 ||
        viewPos.y > 0.8F && m_VerInputValue > 0 ||
        viewPos.y < 0.2F && m_VerInputValue < 0
        ) {
      myCamera.transform.parent = transform;
    }
    else {
      myCamera.transform.parent = null;
    }

  }

  
  private void Boundary() {
    //print( transform.position.x + ", " + transform.position.z );


    if (transform.position.x >= 49) {
      transform.position = new Vector3( 49, transform.position.y, transform.position.z );
      //moveDirection = new Vector3( 0, moveDirection.y, moveDirection.z );
    }
    if (transform.position.x <= -49) {
      transform.position = new Vector3( -49, transform.position.y, transform.position.z );
      //moveDirection = new Vector3( 0, moveDirection.y, moveDirection.z );
    }
    if (transform.position.z >= 49) {
      transform.position = new Vector3( transform.position.x, transform.position.y, 49 );
      //moveDirection = new Vector3( moveDirection.x, moveDirection.y, 0 );
    }
    if (transform.position.z <= -49) {
      transform.position = new Vector3( transform.position.x, transform.position.y, -49 );
      //moveDirection = new Vector3( moveDirection.x, moveDirection.y, 0 );
    }
  }

 }
