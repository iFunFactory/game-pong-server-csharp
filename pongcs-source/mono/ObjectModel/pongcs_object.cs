// Copyright (C) 2013-2016 iFunFactory Inc. All Rights Reserved.
//
// This work is confidential and proprietary to iFunFactory Inc. and
// must not be used, disclosed, copied, or distributed without the prior
// consent of iFunFactory Inc.

// THIS FILE WAS AUTOMATICALLY GENERATED. DO NOT EDIT.

using System.Collections.Generic;


namespace Pongcs {

/// <summary>
/// iFunEngine ORM Class which represent "User" of Object Model.
/// </summary>
public partial class User
{
  /// <summary>
  /// Creates User object.
  /// </summary>
  public static User Create(string id)
  {
    if (FetchById(id) != null)
    {
      return null;
    }

    Dictionary<string, object> key_params = null;
    key_params = new Dictionary<string, object>();
    key_params["Id"] = id;
    funapi.Object obj = funapi.Object.Create("User", key_params);
    if (obj == null) {
      // key string length error, etc.
      return null;
    }
    return (User)obj.GetWrapperObject(typeof(User));
  }

  /// <summary>
  /// Fetches User object from in-memory cache or remote server or database.
  /// </summary>
  public static User Fetch(System.Guid object_id, funapi.LockType lock_type = funapi.LockType.kWriteLock)
  {
    funapi.Object obj = funapi.Object.FetchById("User", object_id, lock_type);
    if (obj == null)
    {
      return null;
    }
    return (User)obj.GetWrapperObject(typeof(User));
  }

  /// <summary>
  /// Fetches User objects from in-memory cache or remote server or database.
  /// </summary>
  public static Dictionary<System.Guid, User> Fetch(SortedSet<System.Guid> object_ids, funapi.LockType lock_type = funapi.LockType.kWriteLock)
  {
    Dictionary<System.Guid, funapi.Object> objects = new Dictionary<System.Guid, funapi.Object> ();
    funapi.Object.FetchById ("User", object_ids, lock_type, objects);

    Dictionary<System.Guid, User> objects2 = new Dictionary<System.Guid, User>();
    foreach(KeyValuePair<System.Guid, funapi.Object> v in objects)
    {
      User obj = null;
      if (v.Value != null)
      {
        obj = (User)v.Value.GetWrapperObject(typeof(User));
      }
      objects2.Add(v.Key, obj);
    }

    return objects2;
  }

  /// <summary>
  /// Fetches User object from in-memory cache or remote server or database.
  /// </summary>
  public static User FetchById(string value, funapi.LockType lock_type = funapi.LockType.kWriteLock)
  {
    funapi.Object obj = funapi.Object.FetchByKey("User", "Id", value, lock_type);
    if (obj == null)
    {
      return null;
    }
    return (User)obj.GetWrapperObject(typeof(User));
  }

  /// <summary>
  /// Fetches User objects from in-memory cache or remote server or database.
  /// </summary>
  public static Dictionary<string, User> FetchById(SortedSet<string> values, funapi.LockType lock_type = funapi.LockType.kWriteLock)
  {
    // TODO(seunghyun): remove unnecessary casting, boxing and unboxing for SortedSet values.
    SortedSet<object> values2 = new SortedSet<object>();
    foreach (object v in values) {
      values2.Add(v);
    }
    Dictionary<object, funapi.Object> objects = funapi.Object.FetchByKey("User", "Id", values2, lock_type);

    Dictionary<string, User> objects2 = new Dictionary<string, User>();
    foreach(KeyValuePair<object, funapi.Object> v in objects)
    {
      User obj = null;
      if (v.Value != null) {
        obj = (User)v.Value.GetWrapperObject(typeof(User));
      }
      objects2.Add((string)v.Key, obj);
    }

    return objects2;
  }

  /// <summary>
  /// Register condition and action trigger that is fired when object is created/changed
  /// </summary>
  public static void RegisterIdTrigger (funapi.Object.TriggerCondition condition, funapi.Object.TriggerAction action)
  {
    funapi.Object.RegisterAttributeTrigger ("User", "Id", condition, action);
  }

  /// <summary>
  /// Search multiple objects with specific attribute
  /// </summary>
  public static void SelectById (funapi.ConditionType cond_type, string cond_value, funapi.Object.SelectCallback cb)
  {
    funapi.Object.Select ("User", "Id", "", cond_type, cond_value, cb);
  }

  /// <summary>
  /// Register condition and action trigger that is fired when object is created/changed
  /// </summary>
  public static void RegisterWinCountTrigger (funapi.Object.TriggerCondition condition, funapi.Object.TriggerAction action)
  {
    funapi.Object.RegisterAttributeTrigger ("User", "WinCount", condition, action);
  }

  /// <summary>
  /// Search multiple objects with specific attribute
  /// </summary>
  public static void SelectByWinCount (funapi.ConditionType cond_type, string cond_value, funapi.Object.SelectCallback cb)
  {
    funapi.Object.Select ("User", "WinCount", "", cond_type, cond_value, cb);
  }

  /// <summary>
  /// Register condition and action trigger that is fired when object is created/changed
  /// </summary>
  public static void RegisterLoseCountTrigger (funapi.Object.TriggerCondition condition, funapi.Object.TriggerAction action)
  {
    funapi.Object.RegisterAttributeTrigger ("User", "LoseCount", condition, action);
  }

  /// <summary>
  /// Search multiple objects with specific attribute
  /// </summary>
  public static void SelectByLoseCount (funapi.ConditionType cond_type, string cond_value, funapi.Object.SelectCallback cb)
  {
    funapi.Object.Select ("User", "LoseCount", "", cond_type, cond_value, cb);
  }

  /// <summary>
  /// Register condition and action trigger that is fired when object is created/changed
  /// </summary>
  public static void RegisterWinningStreakTrigger (funapi.Object.TriggerCondition condition, funapi.Object.TriggerAction action)
  {
    funapi.Object.RegisterAttributeTrigger ("User", "WinningStreak", condition, action);
  }

  /// <summary>
  /// Search multiple objects with specific attribute
  /// </summary>
  public static void SelectByWinningStreak (funapi.ConditionType cond_type, string cond_value, funapi.Object.SelectCallback cb)
  {
    funapi.Object.Select ("User", "WinningStreak", "", cond_type, cond_value, cb);
  }

  /// <summary>
  /// Register condition and action trigger that is fired when object is created/changed
  /// </summary>
  public static void RegisterWinningStreakDayOfYearTrigger (funapi.Object.TriggerCondition condition, funapi.Object.TriggerAction action)
  {
    funapi.Object.RegisterAttributeTrigger ("User", "WinningStreakDayOfYear", condition, action);
  }

  /// <summary>
  /// Search multiple objects with specific attribute
  /// </summary>
  public static void SelectByWinningStreakDayOfYear (funapi.ConditionType cond_type, string cond_value, funapi.Object.SelectCallback cb)
  {
    funapi.Object.Select ("User", "WinningStreakDayOfYear", "", cond_type, cond_value, cb);
  }

  /// <summary>
  /// Fetches User object randomly from in-memory cache or remote server or database.
  /// </summary>
  public static List<User> FetchRandomly(ulong count, funapi.LockType lock_type = funapi.LockType.kWriteLock)
  {
    List<funapi.Object> objects = funapi.Object.FetchRandomly("User", count, lock_type);
    List<User> objects2 = new List<User>();
    foreach(funapi.Object v in objects)
    {
      User obj = (User)v.GetWrapperObject(typeof(User));
      objects2.Add(obj);
    }
    return objects2;
  }

  /// <summary>
  /// Updates a unwritten object data immediately
  /// </summary>
  public void WriteImmediately()
  {
    object_.WriteImmediately();
  }

  /// <summary>
  /// Gets object id.
  /// </summary>
  public System.Guid Id
  {
    get { return object_.Id; }
  }

  /// <summary>
  /// Checks if the object is deleted.
  /// </summary>
  public bool IsNull()
  {
    return object_.IsNull();
  }

  /// <summary>
  /// Checks if the object is accessible in the current event.
  /// </summary>
  public bool IsFresh()
  {
    return object_.IsFresh();
  }

  /// <summary>
  /// Makes the object accessible in the current event.
  /// </summary>
  public bool Refresh(funapi.LockType lock_type)
  {
    if (IsFresh() && object_.LockType >= lock_type)
    {
      return true;
    }

    funapi.Object obj = funapi.Object.FetchById("User", Id, lock_type);
    if (obj == null)
    {
      return false;
    }

    object_ = obj;
    return true;
  }

  /// <summary>
  /// Sets attributes of the object by JSON.
  /// </summary>
  // Populate from JSON
  public bool PopulateFrom(Newtonsoft.Json.Linq.JObject json)
  {
    if (!ObjectImplHelper.ValidateUserJson(json)) {
      return false;
    }
    ObjectImplHelper.PopulateUserFromJson(this, json);
    return true;
  }

  /// <summary>
  /// Get lock type of fetched object
  /// </summary>
  public funapi.LockType GetLockType()
  {
    if (IsNull()) {
      return funapi.LockType.kNoneLock;
    }

    return object_.LockType;
  }

  /// <summary>
  /// Deletes the object permanently.
  /// </summary>
  public void Delete()
  {
    if (IsNull()) {
      return;
    }
    object_.Delete();
  }

  /// <summary>
  /// Gets the value of Id.
  /// </summary>
  public string GetId()
  {
    return object_.GetString("Id");
  }

  /// <summary>
  /// Sets the value of Id.
  /// </summary>
  public void SetId(string value)
  {
    object_.SetString("Id", value);
  }

  /// <summary>
  /// Gets the value of WinCount.
  /// </summary>
  public long GetWinCount()
  {
    return object_.GetInteger("WinCount");
  }

  /// <summary>
  /// Sets the value of WinCount.
  /// </summary>
  public void SetWinCount(long value)
  {
    object_.SetInteger("WinCount", value);
  }

  /// <summary>
  /// Gets the value of LoseCount.
  /// </summary>
  public long GetLoseCount()
  {
    return object_.GetInteger("LoseCount");
  }

  /// <summary>
  /// Sets the value of LoseCount.
  /// </summary>
  public void SetLoseCount(long value)
  {
    object_.SetInteger("LoseCount", value);
  }

  /// <summary>
  /// Gets the value of WinningStreak.
  /// </summary>
  public long GetWinningStreak()
  {
    return object_.GetInteger("WinningStreak");
  }

  /// <summary>
  /// Sets the value of WinningStreak.
  /// </summary>
  public void SetWinningStreak(long value)
  {
    object_.SetInteger("WinningStreak", value);
  }

  /// <summary>
  /// Gets the value of WinningStreakDayOfYear.
  /// </summary>
  public long GetWinningStreakDayOfYear()
  {
    return object_.GetInteger("WinningStreakDayOfYear");
  }

  /// <summary>
  /// Sets the value of WinningStreakDayOfYear.
  /// </summary>
  public void SetWinningStreakDayOfYear(long value)
  {
    object_.SetInteger("WinningStreakDayOfYear", value);
  }

  /// <summary>
  /// Gets the value of the object as JSON
  /// </summary>
  public Newtonsoft.Json.Linq.JObject ToJson ()
  {
    Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject ();
    if (object_ != null && !object_.IsNull ()) {
      json["Id"] = object_.GetString("Id");
      json["WinCount"] = object_.GetInteger("WinCount");
      json["LoseCount"] = object_.GetInteger("LoseCount");
      json["WinningStreak"] = object_.GetInteger("WinningStreak");
      json["WinningStreakDayOfYear"] = object_.GetInteger("WinningStreakDayOfYear");
    }

    return json;
  }

}


///////////////////////////////////////////////////////////////////////////////
// THE FOLLOWING CODES ARE JUST FOR IMPLEMENTATION. DO NOT USE THIS ARBITRARILY.
///////////////////////////////////////////////////////////////////////////////

public partial class User
{
  // Constructor
  private User(funapi.Object obj)
  {
    object_ = obj;
  }

  // Equals() and == Operator
  public override bool Equals(System.Object other)
  {
    if (other == null)
    {
      return false;
    }

    User other2 = other as User;
    if (other2 == null)
    {
      return false;
    }

    return this.Equals(other2);
  }

  public bool Equals(User obj)
  {
    if (obj == null)
    {
      return false;
    }
    return System.Object.ReferenceEquals(object_, obj.object_);
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }

  public static bool operator ==(User lhs, User rhs)
  {
    if (System.Object.ReferenceEquals(lhs, rhs))
    {
      return true;
    }

    if (((object)lhs == null) || ((object)rhs == null)) {
      return false;
    }

    return lhs.Equals(rhs);
  }

  public static bool operator !=(User lhs, User rhs)
  {
    return !(lhs == rhs);
  }

  // Member Variables
  private funapi.Object object_;
}


public static class ObjectImplHelper
{
  public static bool ValidateUserJson(Newtonsoft.Json.Linq.JObject json)
  {
    if (json["Id"] != null) {
      funapi.Log.Error("Wrong JSON: User/Id is not modifiable(key attribute)");
      return false;
    }
    
    if (json["WinCount"] != null) {
      if (json["WinCount"].Type != Newtonsoft.Json.Linq.JTokenType.Integer) {
        funapi.Log.Error("Wrong JSON: User/WinCount type error");
        return false;
      }
    }
    
    if (json["LoseCount"] != null) {
      if (json["LoseCount"].Type != Newtonsoft.Json.Linq.JTokenType.Integer) {
        funapi.Log.Error("Wrong JSON: User/LoseCount type error");
        return false;
      }
    }
    
    if (json["WinningStreak"] != null) {
      if (json["WinningStreak"].Type != Newtonsoft.Json.Linq.JTokenType.Integer) {
        funapi.Log.Error("Wrong JSON: User/WinningStreak type error");
        return false;
      }
    }
    
    if (json["WinningStreakDayOfYear"] != null) {
      if (json["WinningStreakDayOfYear"].Type != Newtonsoft.Json.Linq.JTokenType.Integer) {
        funapi.Log.Error("Wrong JSON: User/WinningStreakDayOfYear type error");
        return false;
      }
    }
    
    return true;
  }

  public static void PopulateUserFromJson(User obj, Newtonsoft.Json.Linq.JObject json)
  {
    funapi.Log.Assert(json["Id"] == null);
    
    if (json["WinCount"] != null) {
      funapi.Log.Assert(json["WinCount"].Type == Newtonsoft.Json.Linq.JTokenType.Integer);
      Newtonsoft.Json.Linq.JValue win_count = json["WinCount"] as Newtonsoft.Json.Linq.JValue;
      obj.SetWinCount((long)win_count);
    }
    
    if (json["LoseCount"] != null) {
      funapi.Log.Assert(json["LoseCount"].Type == Newtonsoft.Json.Linq.JTokenType.Integer);
      Newtonsoft.Json.Linq.JValue lose_count = json["LoseCount"] as Newtonsoft.Json.Linq.JValue;
      obj.SetLoseCount((long)lose_count);
    }
    
    if (json["WinningStreak"] != null) {
      funapi.Log.Assert(json["WinningStreak"].Type == Newtonsoft.Json.Linq.JTokenType.Integer);
      Newtonsoft.Json.Linq.JValue winning_streak = json["WinningStreak"] as Newtonsoft.Json.Linq.JValue;
      obj.SetWinningStreak((long)winning_streak);
    }
    
    if (json["WinningStreakDayOfYear"] != null) {
      funapi.Log.Assert(json["WinningStreakDayOfYear"].Type == Newtonsoft.Json.Linq.JTokenType.Integer);
      Newtonsoft.Json.Linq.JValue winning_streakday_ofyear = json["WinningStreakDayOfYear"] as Newtonsoft.Json.Linq.JValue;
      obj.SetWinningStreakDayOfYear((long)winning_streakday_ofyear);
    }
    
  }
}

}  // namespace Pongcs
