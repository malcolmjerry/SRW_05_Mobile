using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMove : MonoBehaviour {
  NavMeshAgent m_Agent;
  RaycastHit m_HitInfo = new RaycastHit();

  void Start() {
    m_Agent = GetComponent<NavMeshAgent>();
  }

  void Update() {

    if (GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().velocity.sqrMagnitude == 0f) {
      //GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().isStopped = true;
      GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().enabled = false;
      GetComponent<UnitInfo>().GetComponent<NavMeshObstacle>().enabled = true;
    }

    if (Input.GetMouseButtonDown( 0 ) && !Input.GetKey( KeyCode.LeftShift )) {
      GetComponent<UnitInfo>().GetComponent<NavMeshAgent>().enabled = true;
      GetComponent<UnitInfo>().GetComponent<NavMeshObstacle>().enabled = false;

      //Debug.Log( "NavMesh.GetSettingsByIndex( 0 ).agentTypeID: " + NavMesh.GetSettingsByIndex( 0 ).agentTypeID );
      //Debug.Log( "NavMesh.GetSettingsByIndex( 1 ).agentTypeID: " + NavMesh.GetSettingsByIndex( 1 ).agentTypeID );
      //Debug.Log( "GetComponent<NavMeshAgent>().agentTypeID: " + GetComponent<NavMeshAgent>().agentTypeID );

      var surface = GameObject.Find( "Terrain" ).GetComponent<NavMeshSurface>();
      surface.agentTypeID = GetComponent<UnitInfo>().GetMeshAgentTypeId();
      surface.BuildNavMesh();


      var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
      if (Physics.Raycast( ray.origin, ray.direction, out m_HitInfo ))
        m_Agent.destination = m_HitInfo.point;
    }
  }
}
