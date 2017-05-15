using funapi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Pongcs
{
	public enum MatchmakingType
	{
		kMatch1vs1
	}

	public class MatchmakingServer
	{
		public static void Install()
		{
			funapi.Matchmaking.Server.Start (new funapi.Matchmaking.Server.MatchChecker (CheckJoinable),
			                                 new funapi.Matchmaking.Server.CompletionChecker (CheckCompletion),
			                                 new funapi.Matchmaking.Server.JoinCallback (OnJoined),
			                                 new funapi.Matchmaking.Server.LeaveCallback (OnLeft));
		}

		private static bool CheckJoinable(funapi.Matchmaking.Player player, funapi.Matchmaking.Match match)
		{
			Log.Assert (match.MatchType == (int)MatchmakingType.kMatch1vs1);
			// 조건없이 바로 매칭합니다.
			return true;
		}

		private static funapi.Matchmaking.Server.MatchState CheckCompletion(funapi.Matchmaking.Match match)
		{
			Log.Assert (match.MatchType == (int)MatchmakingType.kMatch1vs1);

			if (match.Players.Count < 2) {
				return funapi.Matchmaking.Server.MatchState.kMatchNeedMorePlayer;
			}

			Log.Info ("Match completed: team_a={0}, team_b={1}",
			          match.Context ["A"].ToString (Newtonsoft.Json.Formatting.None),
			          match.Context ["B"].ToString (Newtonsoft.Json.Formatting.None));

			return funapi.Matchmaking.Server.MatchState.kMatchComplete;
		}

		private static void OnJoined(funapi.Matchmaking.Player player, funapi.Matchmaking.Match match)
		{
			Log.Assert (match.MatchType == (int)MatchmakingType.kMatch1vs1);

			// 팀을 구성합니다. 1vs1 만 있기 때문에 각 플레이어를 A, B 팀으로 나눕니다.

			JToken value;
			if (!match.Context.TryGetValue ("A", out value)) {
				match.Context ["A"] = player.Id;
			} else {
				match.Context ["B"] = player.Id;
			}
		}

		private static void OnLeft(funapi.Matchmaking.Player player, funapi.Matchmaking.Match match)
		{
			Log.Assert (match.MatchType == (int)MatchmakingType.kMatch1vs1);

			string team_a = Utility.ReadStringFromJsonObject (match.Context, "A");
			if (team_a == player.Id) {
				match.Context.Remove ("A");
				return;
			}
			string team_b = Utility.ReadStringFromJsonObject (match.Context, "B");
			if (team_b == player.Id) {
				match.Context.Remove ("B");
				return;
			}
		}
	}
}
