using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityORM;

public static class MyHelper {

  public static T DeepClone<T>( T obj ) {
    using (var ms = new MemoryStream()) {
      var formatter = new BinaryFormatter();
      formatter.Serialize( ms, obj );
      ms.Position = 0;

      return (T)formatter.Deserialize( ms );
    }
  }

  public static T Clone<T>( this T obj ) where T : new() {
    using (var ms = new MemoryStream()) {
      var formatter = new BinaryFormatter();
      formatter.Serialize( ms, obj );
      ms.Position = 0;

      return (T)formatter.Deserialize( ms );
    }
  }

  public static T CloneForeach<T>( this T o ) where T : new() {
    return (T)CloneObject( o ); 
  }

  static object CloneObject( object obj ) {
    if (ReferenceEquals( obj, null )) return null;

    var type = obj.GetType();
    if (type.IsValueType || type == typeof( string ))
      return obj;
    else if (type.IsArray) {
      var array = obj as Array;
      var arrayType = Type.GetType( type.FullName.Replace( "[]", string.Empty ) );
      var arrayInstance = Array.CreateInstance( arrayType, array.Length );

      for (int i = 0; i < array.Length; i++)
        arrayInstance.SetValue( CloneObject( array.GetValue( i ) ), i );
      return Convert.ChangeType( arrayInstance, type );
    }
    else if (type.IsClass) {
      var instance = Activator.CreateInstance( type );
      var fields = type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );

      foreach (var field in fields) {
        //if (field.Name.StartsWith( "_" ))
        //continue;

        //SQLite 需要 ignore Object, 會有沖突
        //if (Attribute.IsDefined( field, typeof( IgnoreAttribute ) )) {
          //continue;
        //}

        var fieldValue = field.GetValue( obj );
        if (ReferenceEquals( fieldValue, null )) continue;
        try {
          field.SetValue( instance, CloneObject( fieldValue ) );
        }
        catch (Exception e) {
          Debug.Log( e.Message );
        }
      }
      return instance;
    }
    else
      return null;
  }

}
