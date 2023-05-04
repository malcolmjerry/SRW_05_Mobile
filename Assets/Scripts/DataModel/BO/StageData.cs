using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageData {

  public static StageData Instance { get; } = new StageData();

  private StageData() {

  }

  //public List<MapFightingUnit> MapFightingUnits { get; set; }
  public List<UnitInfo> MapFightingUnits { get; set; }

}
