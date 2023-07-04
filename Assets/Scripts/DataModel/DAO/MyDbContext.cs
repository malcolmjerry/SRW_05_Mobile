using System.Collections.Generic;
using UnityEngine;

public abstract class MyDbContext {
  /*
  public MyDbContext( IRobotDao RobotDao, IPilotDao PilotDao, IPilotDialogDao PilotDialogDao, IPartsDao PartsDao ) {
    this.RobotDao = RobotDao;
    this.PilotDao = PilotDao;
    this.PilotDialogDao = PilotDialogDao;
    this.PartsDao = PartsDao;
  }*/

  public virtual string FROM_DB_FILE { protected set; get; }

  public abstract void Init( string distFile );

  [field: SerializeField]
  public IRobotDao RobotDao { get; set; }

  [field: SerializeField]
  public IPilotDao PilotDao { get; set; }

  //public IHeroDao HeroDao { get; set; }

  public IPilotDialogDao PilotDialogDao { get; set; }

  public IPartsDao PartsDao { get; set; }

  public IGameDataDao GameDataDao { get; set; }

}

