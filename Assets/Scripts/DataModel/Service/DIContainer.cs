using DataModel.Service;
using DataModel.DAO.Impl.SQLite;
using System.Collections.Generic;

public class DIContainer {

  public static DIContainer Instance { get; } = new DIContainer();

  public MyDbContext db;

  private DIContainer() {
     db = MyDbSQLite.Instance;       //不同的數據庫ORM實現改這裡
    _robotService = new RobotService( db );
    _pilotService = new PilotService( db );
    _partsService = new PartsService( db );
    _gameDataService = new GameDataService( db );
    _autoPlayService = new AutoPlayService();
  }

  private RobotService _robotService;
  public RobotService RobotService { get { return _robotService; } }

  private PilotService _pilotService;
  public PilotService PilotService { get { return _pilotService; } }

  private PartsService _partsService;
  public PartsService PartsService { get { return _partsService; } }

  private GameDataService _gameDataService;
  public GameDataService GameDataService { get { return _gameDataService; } }

  private AutoPlayService _autoPlayService;
  public AutoPlayService AutoPlayService { get { return _autoPlayService; } }
}

