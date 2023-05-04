using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public class BaseSelection : MonoBehaviour {

  private float lastTime;
  private float maxTime;
  private readonly float defaultMaxTime = 0.1f;

  protected Direction directionEnum;
  protected int page = 1;
  protected int row = 1;
  protected int total = 1;
  protected int maxPage { get{ return (total - 1)/ maxRow + 1; } }
  protected int currentSelected { get { return (page - 1) * maxRow + row - 1; } }
  protected int currentSelectedHori { get { return (row - 1) * maxPage + page - 1; } }

  private bool autoPage = false;
  private bool keepPlace = false;
  protected int maxRow = 1;
  private int maxRowOfLastPage {
    get {
      var lastMaxRow = total % maxRow;
      return lastMaxRow == 0? maxRow : lastMaxRow;
    }
  }
  private int maxRowOfCurrentPage {
    get {
      return page == maxPage? maxRowOfLastPage : maxRow;
    }
  }
  private int checkedRow {
    get {
      return row > maxRowOfCurrentPage ? maxRowOfCurrentPage : row;
    }
  }

  public enum Direction { Zero = 0, Down = -1, Up = 1, Left = 2, Right = 3 }

  protected void reset() {
    page = 1;
    row = 1;
  }

  protected void setupBase( int total, int maxRow, bool autoPage, bool keepPlace ) {
    this.total = total;
    this.maxRow = maxRow;
    this.autoPage = autoPage;
    this.keepPlace = keepPlace;
  }

  protected void processDirection() {
    if (maxPage != 1 || directionEnum == Direction.Up || directionEnum == Direction.Down )
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );

    switch (directionEnum) {
      case Direction.Up:    moveUp();  break;
      case Direction.Down:  moveDown();  break;
      case Direction.Left:  prevPage( false );  break;
      case Direction.Right: nextPage( false ); break;
      default: break;
    }
  }

  protected void moveUp() {
    row--;
    if (row < 1) {
      if (autoPage) prevPage( true );
      else row = maxRowOfCurrentPage;
    }
  }

  protected void prevPage( bool setLast ) {
    page--;
    page = page < 1 ? maxPage : page;

    if (setLast) row = maxRowOfCurrentPage;
    else row = keepPlace ? checkedRow : 1;
  }

  protected void moveDown() {
    row++;

    if (row > maxRowOfCurrentPage) {
      if (autoPage) nextPage( true );
      else row = 1;
    }
  }

  protected void nextPage( bool setHead ) {
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
      processDirection();
      return;
    }

    if (Input.GetButtonUp( "Up" ) || Input.GetButtonUp( "Down" ) || Input.GetButtonUp( "Left" ) || Input.GetButtonUp( "Right" )) {
      directionEnum = Direction.Zero;
      return;
    }

    lastTime += Time.deltaTime;
    if (lastTime > maxTime) {
      processDirection();
      lastTime = 0;
      maxTime = defaultMaxTime;     
    }
  }

  protected List<T> GetPageList<T>( List<T> fullList ) {
    return fullList.Skip( (page - 1) * maxRow ).Take( maxRow ).ToList();
  }

  protected Action callback;

  protected void closeSelf() {
    gameObject.SetActive( false );
    callback();
  }

}

