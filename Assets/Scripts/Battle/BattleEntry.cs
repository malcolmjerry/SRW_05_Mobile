using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleEntry : MonoBehaviour {

  public string RightModelName;
  public string LeftModelName;

  public int FullHpR;
  public int HpR;
  public int FullEnR;
  public int EnR;
  public int FullHpL;
  public int HpL;
  public int FullEnL;
  public int EnL;

  public UnitInfo UnitInfoR;
  public UnitInfo UnitInfoL;

  //public int RightWeaponNum;
  //public int LeftWeaponNum;

  public List<AttackData> AttackDataList { get; set; }

  public List<CmdForAnim> cmdForAnims { get; set; }

  public string BGM_Name;

  public GameObject[] AllGameObjects;
  public GameObject TempGO;

  // Use this for initialization
  void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class CmdForAnim {

  public string Side { get; set; }

  public int Weapon { get; set; }

  public int TotalDamage { get; set; }

  public int SpendEn { get; set; }

  public bool HitMiss { get; set; }

}
