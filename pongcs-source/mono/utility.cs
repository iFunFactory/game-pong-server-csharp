using funapi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Pongcs
{
	public class Utility
	{
		public static void RefreshWinningStreak(User user)
		{
			int now_day_of_year = WallClock.Now.DayOfYear;
			int day_of_year = (int)user.GetWinningStreakDayOfYear ();
			if (now_day_of_year != day_of_year) {
				user.SetWinningStreak (0);
			}
		}

		public static System.Guid PickServerRandomly(string server_tag)
		{
			Dictionary<System.Guid, System.Net.IPEndPoint> servers;
			Rpc.GetPeersWithTag (out servers, server_tag, true);
			Dictionary<System.Guid, System.Net.IPEndPoint>.KeyCollection peerids = servers.Keys;

			int rnd = (new System.Random()).Next(peerids.Count - 1);
			int idx = 0;
			foreach (Guid peerid in peerids) {
				if (idx == rnd) {
					return peerid;
				}
				++idx;
			}

			return Guid.Empty;
		}

		public static string ReadStringFromJsonObject(JObject json, string property_name)
		{
			JToken value = null;
			if (json.TryGetValue (property_name, out value)) {
				if (value.Type == JTokenType.String) {
					return value.Value<string> ();
				}
			}
			return null;
		}

		public static JObject MakeResponse(string result, string message)
		{
			JObject response = new JObject ();
			response["result"] = result;
			response["message"] = message;
			return response;
		}

		public static JObject MakeResponse(string result)
		{
			JObject response = new JObject ();
			response["result"] = result;
			return response;
		}
	}
}