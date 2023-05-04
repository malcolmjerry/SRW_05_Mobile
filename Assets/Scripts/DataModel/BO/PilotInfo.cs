using UnityEngine;
using System.Collections;
using System;
using UnityORM;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class PilotInfo {

  private PilotInstance pilotInstance = null;
  public PilotInstance PilotInstance { 
    set { pilotInstance = value; } 
    get {
      if (pilotInstance == null) {
        pilotInstance = DIContainer.Instance.PilotService.LoadPilotInstance( PilotInstanceSeqNo );
      }
      return pilotInstance;
    } 
  }

  public int PilotInstanceSeqNo;

  public PilotInfo() { }

  public PilotInfo( PilotInstance pilotInstance ) {
    PilotInstance = pilotInstance;
    PilotInstanceSeqNo = pilotInstance.SeqNo;

    Update();
  }

  public int Shoot;

  public int Melee;

  public int Dodge;

  public int Hit;

  public int Dex;

  public int Defense;

  public int TerrainSky;

  public int TerrainLand;

  public int TerrainSea;

  public int TerrainSpace;

  public int MaxSp;

  [SerializeField]
  private int _RemainSp;
  public int RemainSp {
    get { return _RemainSp; }
    set {
      _RemainSp = Math.Min( value, MaxSp );
    }
  }

  public int Level { get { return PilotInstance.Exp / 500 + 1; } }

  public int NextLevel { get { return 500 - PilotInstance.Exp % 500; } }

  public int MaxWillpower;

  [SerializeField]
  private int willPower;
  public int Willpower { get { return willPower; } set { willPower = Mathf.Min( value, MaxWillpower ); } }

  public List<SPCommand> ActiveSPCommandList = new List<SPCommand>();

  public string FirstName;
  public string LastName;
  public string FullName {
    get {
      if (PilotInstance.Pilot.NameType == 0)
        return FirstName + " ＝ " + LastName;
      return LastName + FirstName;
    }
  }
  public string ShortName;
  public int PicNo;

  //public List<PilotSkillInfo> PilotSkillInfoList = new List<PilotSkillInfo>();

  /*
  public void Normalize() {
    RemainSp = Mathf.Min( MaxSp, Math.Max( 0, RemainSp ) );
    Willpower = Mathf.Min( MaxWillpower, Math.Max( 50, Willpower ) );

    Shoot = Mathf.Min( 400, Math.Max( 0, Shoot ) );
    Melee = Mathf.Min( 400, Math.Max( 0, Melee ) );
    Dodge = Mathf.Min( 400, Math.Max( 0, Dodge ) );
    Hit = Mathf.Min( 400, Math.Max( 0, Hit ) );
    Dex = Mathf.Min( 400, Math.Max( 0, Dex ) );
    Defense = Mathf.Min( 400, Math.Max( 0, Defense ) );
  }*/

  public void Update() {
    int statusLvAdd = (Level - 1) * 2;
    this.Shoot = PilotInstance.Pilot.Shoot + statusLvAdd + PilotInstance.ShootAdded;
    this.Melee = PilotInstance.Pilot.Melee + statusLvAdd + PilotInstance.MeleeAdded;
    this.Dodge = PilotInstance.Pilot.Dodge + statusLvAdd + PilotInstance.DodgeAdded;
    this.Hit = PilotInstance.Pilot.Hit + statusLvAdd + PilotInstance.HitAdded;
    this.Dex = PilotInstance.Pilot.Dex + statusLvAdd + PilotInstance.DexAdded;
    this.Defense = PilotInstance.Pilot.Defense + statusLvAdd + PilotInstance.DefenseAdded;
    this.TerrainSky = PilotInstance.Pilot.TerrainSky + PilotInstance.TerrainSkyAdded;
    this.TerrainLand = PilotInstance.Pilot.TerrainLand + PilotInstance.TerrainLandAdded;
    this.TerrainSea = PilotInstance.Pilot.TerrainSea + PilotInstance.TerrainSeaAdded;
    this.TerrainSpace = PilotInstance.Pilot.TerrainSpace + PilotInstance.TerrainSpaceAdded;
    this.MaxSp = PilotInstance.Pilot.MaxSp + (int)(Level * 1.5f);

    this.FirstName = PilotInstance.HeroSeqNo.HasValue ? PilotInstance.Hero.FirstName : PilotInstance.Pilot.FirstName;
    this.LastName = PilotInstance.HeroSeqNo.HasValue ? PilotInstance.Hero.LastName : PilotInstance.Pilot.LastName;
    this.ShortName = PilotInstance.HeroSeqNo.HasValue ? PilotInstance.Hero.ShortName : PilotInstance.Pilot.ShortName;
    PicNo = PilotInstance.HeroSeqNo.HasValue? PilotInstance.Hero.PicNo : PilotInstance.Pilot.ID;

    //this.Level =  PilotInstance.Exp / 500 + 1;
    //this.NextLevel = 500 - PilotInstance.Exp % 500;
    this.MaxWillpower = 150;    //在 MapFightingUnit 中的 UpdateInfoByPilotSkills, 檢查有沒有氣力限界突破

    //BuffShoot = 0;
    //BuffMelee = 0;
    //BuffDodge = 0;
    //BuffHit = 0;
    //BuffDex = 0;
    //BuffDefense = 0;
  }

  public void InitValue() {
    this.RemainSp = (MaxSp + 1) / 2;
    this.Willpower = 100 + BuffWillPower;
  }

  public bool AddExp( int exp ) {
    int oldLv = Level;
    PilotInstance.Exp += exp;
    //Level =  PilotInstance.Exp / 500 + 1;
    //NextLevel = 500 - PilotInstance.Exp % 500;
    return Level > oldLv;
  }

  public int BuffShoot;
  public int BuffMelee;
  public int BuffDodge;
  public int BuffHit;
  public int BuffDex;
  public int BuffDefense;

  public int BuffWillPower;

}
