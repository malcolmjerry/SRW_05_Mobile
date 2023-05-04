using System;
using System.Collections.Generic;
using UnityEngine;
using static UnitInfo;

[Serializable]
public class SaveMapUnit {

  public bool IsNewToPlayer;

  public bool IsDestroyed;

  public bool IsOnShip;

  public TeamEnum Team;

  public int RemainMoveAttackTurns;

  public string Layer;

  public MyVector3 Position;

  public MyVector3 TheEuler;

  public MyVector3 RendererEuler;

  public MapFightingUnit MapFightingUnit;

  public int RemainActionCount;

  public int? OtherUnitSeqNo = null; //挑發專用

  public int? ShipUnitSeqNo = null; //鄰近的戰艦

  public List<int> StoringUnitSeqNoList = new List<int>();  //戰艦中的機體的編號

  public List<int> DropParts = new List<int>();  //被擊墜時 (HP=0) 掉落的零件
}

