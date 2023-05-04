using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public abstract class SelectableMenu : MonoBehaviour {

  private float lastTime;
  private float maxTime;
  private readonly float defaultMaxTime = 0.1f;

  protected Direction directionEnum;
  protected int page = 1;
  protected int row = 1;
  protected int total = 1;
  protected int maxPage { get { return (total - 1)/ maxRow + 1; } }
  protected int currentSelected { get { return (page - 1) * maxRow + row - 1; } }
  protected int currentSelectedHori { get { return (row - 1) * maxPage + page - 1; } }

  private bool autoPage = false;
  private bool keepPlace = false;
  protected int maxRow = 1;
  private int maxRowOfLastPage {
    get {
      var lastMaxRow = total % maxRow;
      return lastMaxRow == 0 ? maxRow : lastMaxRow;
    }
  }
  private int maxRowOfCurrentPage {
    get {
      return page == maxPage ? maxRowOfLastPage : maxRow;
    }
  }
  private int checkedRow {
    get {
      return row > maxRowOfCurrentPage ? maxRowOfCurrentPage : row;
    }
  }

  public enum Direction { Zero = 0, Down = -1, Up = 1, Left = 2, Right = 3 }

  public List<Transform> menuItemTfList = new List<Transform>();
  protected Action callback;
  protected Action<dynamic> next;
  protected bool autoCloseSelf;

  protected void reset() {
    page = 1;
    row = 1;
    baseUpdateSelected();
  }

  protected void setupBase( int total, bool autoPage, bool keepPlace, int? maxRow = null ) {
    this.total = total;
    //this.maxRow = maxRow;
    this.maxRow = maxRow.HasValue? maxRow.Value : menuItemTfList.Count;
    this.autoPage = autoPage;
    this.keepPlace = keepPlace;
    reset();
  }

  protected void processDirection() {
    /*
    if ( maxRow > 1 && (directionEnum == Direction.Up || directionEnum == Direction.Down) ||
         maxPage > 1 && (directionEnum == Direction.Left || directionEnum == Direction.Right))
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    */

    switch (directionEnum) {
      case Direction.Up: moveUp(); break;
      case Direction.Down: moveDown(); break;
      case Direction.Left: prevPage( false ); break;
      case Direction.Right: nextPage( false ); break;
      default: break;
    }
  }

  protected void moveUp() {
    if (maxRow <= 1) {
      directionEnum = Direction.Zero;
      return;
    }
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );

    row--;
    if (row < 1) {
      if (autoPage) prevPage( true );
      else row = maxRowOfCurrentPage;
    }
  }

  protected void prevPage( bool setLast ) {
    if (maxPage <= 1) {
      //directionEnum = Direction.Zero;  //不能清空方向, 會導致某些非換頁功能(如機體改造)出現異常
      return;
    }

    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );

    page--;
    page = page < 1 ? maxPage : page;

    if (setLast) row = maxRowOfCurrentPage;
    else row = keepPlace ? checkedRow : 1;
  }

  protected void moveDown() {
    if (maxRow <= 1) {
      directionEnum = Direction.Zero;
      return;
    }

    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );

    row++;

    if (row > maxRowOfCurrentPage) {
      if (autoPage) nextPage( true );
      else row = 1;
    }
  }

  protected void nextPage( bool setHead ) {
    if (maxPage <= 1) {
      //directionEnum = Direction.Zero;  //不能清空方向, 會導致某些非換頁功能(如機體改造)出現異常
      return;
    }

    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );

    page++;
    page = page > maxPage ? 1 : page;
    if (setHead) row = 1;
    else row = keepPlace ? checkedRow : 1;
  }

  protected void moveCursor() {
    //bool justDown = false;

    directionEnum = Direction.Zero;
    if (Input.GetButton( "Up" )) {
      directionEnum = Direction.Up;
    }
    else if (Input.GetButton( "Down" )) {
      directionEnum = Direction.Down;
    }
    else if (Input.GetButton( "Left" )) {
      directionEnum = Direction.Left;
    }
    else if (Input.GetButton( "Right" )) {
      directionEnum = Direction.Right;
    }

    if (directionEnum == Direction.Zero) return;

    if (Input.GetButtonDown( "Up" ) || Input.GetButtonDown( "Down" ) || Input.GetButtonDown( "Left" ) || Input.GetButtonDown( "Right" )) {
      lastTime = 0;
      maxTime = defaultMaxTime * 5;
      //Debug.Log( $"{GetType().Name} maxTime " + maxTime );
      processDirection();
      return;
    }

    if (Input.GetButtonUp( "Up" ) || Input.GetButtonUp( "Down" ) || Input.GetButtonUp( "Left" ) || Input.GetButtonUp( "Right" )) {
      directionEnum = Direction.Zero;
      return;
    }

    lastTime += Time.deltaTime;
    if (lastTime > maxTime) {
      //Debug.Log( $"{GetType().Name} lastTime > maxTime " + lastTime + " > " + maxTime );
      processDirection();
      lastTime = 0;
      maxTime = defaultMaxTime;
    }
    else { directionEnum = Direction.Zero; }
  }

  protected List<T> GetPageList<T>( List<T> fullList ) {
    return fullList.Skip( (page - 1) * maxRow ).Take( maxRow ).ToList();
  }
  
  public void Setup( Action callback, Action<dynamic> next = null, bool closeSelf = true ) {
    this.callback = callback;
    this.next = next;
    autoCloseSelf = closeSelf;
  }

  protected virtual void closeSelf() {
    if (callback != null) 
      callback.Invoke();
    
    if (autoCloseSelf)
      gameObject.SetActive( false );
    else enabled = false;
  }

  virtual protected void hideSelected() {
    foreach (var menuItem in menuItemTfList) {
      menuItem.Find( "Selected" ).gameObject.SetActive( false );
    }
  }

  virtual protected void UpdateSelectedItem() {
    foreach (var menuItem in menuItemTfList) {
      menuItem.Find( "Selected" ).gameObject.SetActive( false );
    }

    //if (this.enabled) {
      //Debug.Log( $"row {row} currentSelected {currentSelected}" );

      if (total == menuItemTfList.Count)
        menuItemTfList[currentSelected].Find( "Selected" ).gameObject.SetActive( true ); 
      else
        menuItemTfList[row - 1].Find( "Selected" ).gameObject.SetActive( true );
    //}
  }

  protected void SetPageAndRowBySelected( int index ) {
    page = index / maxRow + 1;
    row = index % maxRow + 1;
    baseUpdateSelected();
  }

  protected virtual void Update() {
    moveCursor();
    if (directionEnum != Direction.Zero)
      baseUpdateSelected();

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      closeSelf();
      //Debug.Log( "menuItemTfList.Count: " + menuItemTfList.Count );
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      //Debug.Log( "SelectableMenu Update Confirm " + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) );
      confirm();
    }
    else if (Input.GetButtonDown( "CursorSpeed" )) {
      CursorSpeed();
    }
    else if (Input.GetButtonDown( "Info" )) {
      Info();
    }
  }

  protected virtual void OtherCallback() {
    gameObject.SetActive( true );
    enabled = true;
  }

  void baseUpdateSelected() {
    UpdateDisplayList();
    UpdateSelectedItem();
    UpdateSelectedMore();
  }

  protected abstract void confirm();
  protected virtual void UpdateSelectedMore() { }
  public virtual void UpdateDisplayList() { }
  protected virtual void CursorSpeed() { }
  protected virtual void Info() { }

}


