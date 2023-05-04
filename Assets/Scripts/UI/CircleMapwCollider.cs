using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitInfo;

public class CircleMapwCollider : MonoBehaviour {

  private List<TeamEnum> findTeamList;
  private UnitInfo selfUnitInfo;
  private float maxRange;
  private float minRange;


  [HideInInspector] public List<GameObject> InRangeUnits; //= new List<GameObject>();

  public void Setup( float maxRound, List<TeamEnum> findTeamList, UnitInfo unitInfo, float maxRange, float minRange = 1 ) {
    this.findTeamList = findTeamList;
    this.maxRange = maxRange;
    this.minRange = minRange;
    selfUnitInfo = unitInfo;
    InRangeUnits = new List<GameObject>();
    transform.localScale = new Vector3( maxRound, transform.localScale.y, maxRound );
  }

  void OnTriggerEnter( Collider other ) {
    var unitInfo= other.GetComponent<UnitInfo>();
    if (unitInfo == null)
      return;
    if (selfUnitInfo == unitInfo)
      return;
    if (!PreBattleFormula.IS_IN_RANGE( selfUnitInfo, unitInfo, maxRange, minRange )) {
      return;
    }

    if (findTeamList.Contains( unitInfo.Team )) {
      var unitInRange = unitInfo.GetComponent<UnitInRange>();
      if (unitInRange != null) unitInRange.enabled = true;

      InRangeUnits.Add( other.gameObject );
    }

  }

  public void ClearAll() {
    while (InRangeUnits.Count > 0)
      RemoveFromRange( InRangeUnits[0] );
  }

  public void RemoveFromRange(GameObject go) {
    var unitInRange = go.GetComponent<UnitInRange>();
    if (unitInRange != null) unitInRange.enabled = false;
    InRangeUnits.Remove( go );
  } 

  void OnDestroy() {
    ClearAll();
  }

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
