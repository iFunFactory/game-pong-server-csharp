// Copyright (C) 2013-2017 iFunFactory Inc. All Rights Reserved.
//
// This work is confidential and proprietary to iFunFactory Inc. and
// must not be used, disclosed, copied, or distributed without the prior
// consent of iFunFactory Inc.

// THIS FILE WAS AUTOMATICALLY GENERATED. DO NOT EDIT.


using System;
using System.Collections.Generic;

namespace Pongcs {
  public static class ActivityLog {

    public static void SessionOpened(string arg_session_id, System.DateTime arg_when)
    {
      List<string> values = new List<string>();
      values.Add(arg_session_id);
      values.Add(funapi.WallClock.GetTimestring(arg_when));
      funapi.ActivityLog.LogActivity("SessionOpened", values);
    }

    public static void SessionClosed(string arg_session_id, System.DateTime arg_when)
    {
      List<string> values = new List<string>();
      values.Add(arg_session_id);
      values.Add(funapi.WallClock.GetTimestring(arg_when));
      funapi.ActivityLog.LogActivity("SessionClosed", values);
    }

    public static void PlayerLoggedIn(string arg_session_id, string arg_account_id, System.DateTime arg_when)
    {
      List<string> values = new List<string>();
      values.Add(arg_session_id);
      values.Add(arg_account_id);
      values.Add(funapi.WallClock.GetTimestring(arg_when));
      funapi.ActivityLog.LogActivity("PlayerLoggedIn", values);
    }

    static ActivityLog()
    {

      funapi.ActivityLog.RegisterActivity(
          "SessionOpened",
          "{\"session_id\": \"string\", \"when\": \"datetime2\"}",
          new string[2] {"session_id", "when"},
          new funapi.ActivityLog.LogColumnCppType[2] {funapi.ActivityLog.LogColumnCppType.LCT_STRING, funapi.ActivityLog.LogColumnCppType.LCT_WALLCLOCK_VALUE},
          new bool[2] {true, true});
      funapi.ActivityLog.RegisterActivity(
          "SessionClosed",
          "{\"session_id\": \"string\", \"when\": \"datetime2\"}",
          new string[2] {"session_id", "when"},
          new funapi.ActivityLog.LogColumnCppType[2] {funapi.ActivityLog.LogColumnCppType.LCT_STRING, funapi.ActivityLog.LogColumnCppType.LCT_WALLCLOCK_VALUE},
          new bool[2] {true, true});
      funapi.ActivityLog.RegisterActivity(
          "PlayerLoggedIn",
          "{\"session_id\": \"string\", \"account_id\": \"string\", \"when\": \"datetime2\"}",
          new string[3] {"session_id", "account_id", "when"},
          new funapi.ActivityLog.LogColumnCppType[3] {funapi.ActivityLog.LogColumnCppType.LCT_STRING, funapi.ActivityLog.LogColumnCppType.LCT_STRING, funapi.ActivityLog.LogColumnCppType.LCT_WALLCLOCK_VALUE},
          new bool[3] {true, true, true});
    }
  }
}
