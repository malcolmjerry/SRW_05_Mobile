using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

  UnitInfo parentUnitInfo;

  // Start is called before the first frame update
  void Start() {
    parentUnitInfo = transform.parent.GetComponentInParent<UnitInfo>();
  }

  // Update is called once per frame
  void Update() {

  }

  void OnTriggerEnter( Collider other ) {
    var unitInfo = other.GetComponent<UnitInfo>();
    if (unitInfo && unitInfo.RobotInfo.RobotInstance.Robot.IsShip != true && parentUnitInfo.Team == unitInfo.Team) {
      unitInfo.ShipUnit = parentUnitInfo;
    }

  }

  void OnTriggerExit( Collider other ) {
    var unitInfo = other.GetComponent<UnitInfo>();
    if (unitInfo && unitInfo.ShipUnit == parentUnitInfo && parentUnitInfo.Team == unitInfo.Team) {
      unitInfo.ShipUnit = null;
    }
  }
}
