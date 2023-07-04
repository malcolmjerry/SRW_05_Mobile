using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;


namespace UnityORM {

  public class SqliteException : Exception {
    int errorCode;
    public int ErrorCode { get { return errorCode; } }
    public SqliteException( int errorCode, string message ) : base( message + "(ErrorCode:" + errorCode + ")" ) {
      this.errorCode = errorCode;
    }
  }

  /// <summary>
  /// Sqlite database.
  /// Get from http://gamesforsoul.com/2012/03/sqlite-unity-and-ios-a-rocky-relationship/
  /// and modified.
  /// </summary>
  /// <exception cref='SqliteException'>
  /// Is thrown when the sqlite exception.
  /// </exception>
  public class SqliteDatabase {
    public static bool IsGoodCode( int resultCode ) {
      return resultCode == SQLITE_OK || resultCode == SQLITE_DONE;
    }
    public const int SQLITE_OK = 0;
    const int SQLITE_ROW = 100;
    public const int SQLITE_DONE = 101;
    const int SQLITE_INTEGER = 1;
    const int SQLITE_FLOAT = 2;
    const int SQLITE_TEXT = 3;
    const int SQLITE_BLOB = 4;
    const int SQLITE_NULL = 5;
    public const int SQLITE_ERROR_ALREADY_OPENED = -2;
    public const int SQLITE_ERROR_NOT_OPENED = -1;

    [DllImport( "sqlite3", EntryPoint = "sqlite3_open" )]
    internal static extern int sqlite3_open( string filename, out IntPtr db );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_key" )]
    internal static extern int sqlite3_key( IntPtr db, string key, int keyLength );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_rekey" )]
    internal static extern int sqlite3_rekey( IntPtr db, string key, int keyLength );

    //[DllImport( "sqlite3", EntryPoint = "wxsqlite3_config" )]
    //internal static extern int wxsqlite3_config( IntPtr db, string param, int newValue );

    [DllImport( "sqlite3", EntryPoint = "sqlite3mc_config" )]
    internal static extern int sqlite3mc_config( IntPtr db, string param, int newValue );

    //[DllImport( "sqlite3", EntryPoint = "wxsqlite3_config_cipher" )]
    //internal static extern int wxsqlite3_config_cipher( IntPtr db, string cipherName, string paramName, int newValue );

    [DllImport( "sqlite3", EntryPoint = "sqlite3mc_config_cipher" )]
    internal static extern int sqlite3mc_config_cipher( IntPtr db, string cipherName, string paramName, int newValue );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_close" )]
    internal static extern int sqlite3_close( IntPtr db );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_prepare_v2" )]
    internal static extern int sqlite3_prepare_v2( IntPtr db, string zSql, int nByte, out IntPtr ppStmpt, IntPtr pzTail );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_step" )]
    internal static extern int sqlite3_step( IntPtr stmHandle );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_errcode" )]
    internal static extern int sqlite3_errcode( IntPtr db );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_extended_errcode" )]
    internal static extern int sqlite3_extended_errcode( IntPtr db );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_changes" )]
    internal static extern int sqlite3_changes( IntPtr db );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_finalize" )]
    internal static extern int sqlite3_finalize( IntPtr stmHandle );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_errmsg" )]
    internal static extern IntPtr sqlite3_errmsg( IntPtr db );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_column_count" )]
    internal static extern int sqlite3_column_count( IntPtr stmHandle );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_column_name" )]
    internal static extern IntPtr sqlite3_column_name( IntPtr stmHandle, int iCol );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_column_type" )]
    internal static extern int sqlite3_column_type( IntPtr stmHandle, int iCol );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_column_int" )]
    internal static extern int sqlite3_column_int( IntPtr stmHandle, int iCol );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_column_int64" )]
    internal static extern long sqlite3_column_int64( IntPtr stmHandle, int iCol );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_column_text" )]
    internal static extern IntPtr sqlite3_column_text( IntPtr stmHandle, int iCol );

    [DllImport( "sqlite3", EntryPoint = "sqlite3_column_double" )]
    internal static extern double sqlite3_column_double( IntPtr stmHandle, int iCol );

    private IntPtr _connection;
    private bool IsConnectionOpen { get; set; }


    #region Public Methods

    public void Open( string path, string key) {
      if (IsConnectionOpen) {
        throw new SqliteException( SQLITE_ERROR_ALREADY_OPENED, "There is already an open connection" );
      }
      int openResult = sqlite3_open( path, out _connection );
      if (openResult != SQLITE_OK) {
        throw new SqliteException( openResult, "Could not open database file: " + path );
      }

      sqlite3mc_config( _connection, "cipher", 4 );    //CODEC_TYPE_SQLCIPHER
      //wxsqlite3_config( _connection, "cipher", 4 );  //CODEC_TYPE_SQLCIPHER   //comment this if use a non-encrypted db
      //wxsqlite3_config_cipher( _connection, "sqlcipher", "kdf_iter", 256000 );
      //wxsqlite3_config_cipher( _connection, "sqlcipher", "fast_kdf_iter", 2 ); 
      //wxsqlite3_config_cipher( _connection, "sqlcipher", "hmac_use", 1 );
      //wxsqlite3_config_cipher( _connection, "sqlcipher", "kdf_algorithm", 2 );
      //wxsqlite3_config_cipher( _connection, "sqlcipher", "hmac_algorithm", 2 );

      //sqlite3_key是輸入金鑰，如果資料庫已加密必須先執行此函數並輸入正確金鑰才能進行操作；
      //如果資料庫沒有加密，執行此函數後進行資料庫操作反而會出現“此資料庫已加密或不是一個資料庫檔”的錯誤。
      sqlite3_key( _connection, key, key.Length );  //2023-05-17 commented

      //sqlite3_rekey是變更金鑰或給沒有加密的資料庫添加金鑰或清空金鑰，變更金鑰或清空金鑰前必須先正確執行 sqlite3_key。
      //在正確執行 sqlite3_rekey 之後在 sqlite3_close 關閉資料庫之前可以正常操作資料庫，不需要再執行 sqlite3_key。
      //sqlite3_rekey( _connection, key, key.Length );   //發佈時使用rekey

      //sqlite3_rekey( _connection, "", 0 );   //清空金鑰前必須先正確執行 sqlite3_key

      IsConnectionOpen = true;
    }

    public void Close() {
      if (IsConnectionOpen) {
        sqlite3_close( _connection );
      }

      IsConnectionOpen = false;
    }

    public int ExecuteNonQuery( string query ) {
      if (!IsConnectionOpen) {
        throw new SqliteException( SQLITE_ERROR_NOT_OPENED, "SQLite database is not open." );
      }

      IntPtr stmHandle = Prepare( query );

      int result = sqlite3_step( stmHandle );
      if (result != SQLITE_DONE) {
        int errorCode = sqlite3_errcode( _connection );
        throw new SqliteException( errorCode, "Could not execute SQL statement.ErrorCode:" + errorCode );
      }
      Finalize( stmHandle );
      return sqlite3_changes( _connection );
    }

    public DataTable ExecuteQuery( string query ) {
      if (!IsConnectionOpen) {
        throw new SqliteException( SQLITE_ERROR_NOT_OPENED, "SQLite database is not open." );
      }

      IntPtr stmHandle = Prepare( query );

      int columnCount = sqlite3_column_count( stmHandle );

      var dataTable = new DataTable();
      for (int i = 0; i < columnCount; i++) {
        string columnName = Marshal.PtrToStringAnsi( sqlite3_column_name( stmHandle, i ) );
        dataTable.Columns.Add( columnName );
      }

      //populate datatable
      while (sqlite3_step( stmHandle ) == SQLITE_ROW) {
        object[] row = new object[columnCount];
        for (int i = 0; i < columnCount; i++) {
          switch (sqlite3_column_type( stmHandle, i )) {
            case SQLITE_INTEGER:
              row[i] = sqlite3_column_int64( stmHandle, i );
              break;

            case SQLITE_TEXT:
              IntPtr text = sqlite3_column_text( stmHandle, i );
              row[i] = Marshal.PtrToStringAnsi( text );
              break;

            case SQLITE_FLOAT:
              row[i] = sqlite3_column_double( stmHandle, i );
              break;

            case SQLITE_NULL:
              row[i] = null;
              break;
          }
        }

        dataTable.AddRow( row );
      }

      Finalize( stmHandle );

      return dataTable;
    }

    public void ExecuteScript( string script ) {
      string[] statements = script.Split( ';' );

      foreach (string statement in statements) {
        if (!string.IsNullOrEmpty( statement.Trim() )) {
          ExecuteNonQuery( statement );
        }
      }
    }

    #endregion

    #region Private Methods

    private IntPtr Prepare( string query ) {
      IntPtr stmHandle;
      byte[] queryBytes = System.Text.Encoding.UTF8.GetBytes( query );
      int trueSize = queryBytes.Length;
      int resultCode = sqlite3_prepare_v2( _connection, query, trueSize, out stmHandle, IntPtr.Zero );
      if (resultCode != SQLITE_OK) {
        IntPtr errorMsg = sqlite3_errmsg( _connection );
        throw new SqliteException( resultCode, Marshal.PtrToStringAnsi( errorMsg ) + " FullQuery=|" + query + "|" );
      }

      return stmHandle;
    }

    private void Finalize( IntPtr stmHandle ) {
      int resultCode = sqlite3_finalize( stmHandle );
      if (resultCode != SQLITE_OK) {
        throw new SqliteException( resultCode, "Could not finalize SQL statement." );
      }
    }

    #endregion
  }

}
