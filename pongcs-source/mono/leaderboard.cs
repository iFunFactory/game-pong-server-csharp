using funapi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Pongcs
{
	public class Leaderboard
	{
		private static readonly string kLeaderboardId = "winning_streak";
		private static readonly string kSingleLeaderboardId = "winning_streak_single";
		private static readonly string kAccountServiceProvider = "none";

		public static void OnWin(User user, bool single = false)
		{
			Event.AssertNoRollback ();

			if (single)
			{
				user.SetWinCountSingle (user.GetWinCountSingle () + 1);
				user.SetWinningStreakSingle (user.GetWinningStreakSingle () + 1);
			}
			else
			{
				user.SetWinCount (user.GetWinCount () + 1);
				user.SetWinningStreak (user.GetWinningStreak () + 1);
			}

			string id = user.GetId ();
			int winning_streak = single ? (int)user.GetWinningStreakSingle () : (int)user.GetWinningStreak ();

			funapi.Leaderboard.ScoreSubmissionRequest request =
				new funapi.Leaderboard.ScoreSubmissionRequest (
					single ? kSingleLeaderboardId : kLeaderboardId, kAccountServiceProvider, id, (double)winning_streak,
					funapi.Leaderboard.ScoreSubmissionType.kHighScore);

			funapi.Leaderboard.ScoreSubmissionResponseHandler cb = (funapi.Leaderboard.ScoreSubmissionRequest req,
			                                                        funapi.Leaderboard.ScoreSubmissionResponse resp,
			                                                        bool error) => {
				if (error) {
					Log.Error("Failed to update leaderboard. Leaderboard system error.");
					return;
				}
			};

			funapi.Leaderboard.SubmitScore(request, cb);
		}

		public static void OnLose(User user, bool single = false)
		{
			Event.AssertNoRollback ();

			if (single)
			{
				user.SetLoseCountSingle (user.GetLoseCountSingle () + 1);
				user.SetWinningStreakSingle (0);
			}
			else
			{
				user.SetLoseCount (user.GetLoseCount () + 1);
				user.SetWinningStreak (0);
			}
		}

		public static int GetRecord(string account_id, bool single = false)
		{
			// TODO: Async API 로 교체

			funapi.Leaderboard.QueryRequest request =
				new funapi.Leaderboard.QueryRequest (
					single ? kSingleLeaderboardId : kLeaderboardId, kAccountServiceProvider,
					account_id, funapi.Leaderboard.Timespan.kAllTime,
					new funapi.Leaderboard.Range (funapi.Leaderboard.RangeType.kNearby, 0, 0));
			funapi.Leaderboard.QueryResponse response;

			if (!funapi.Leaderboard.GetLeaderboardSync (request, out response)) {
				Log.Error ("Failed to query leaderboard. Leaderboard system error.");
				return 0;
			}

			List<funapi.Leaderboard.Record>.Enumerator enumerator = response.Records.GetEnumerator ();
			if (!enumerator.MoveNext ()) {
				return 0;
			}
			return (int)enumerator.Current.Score;
		}

		public static void OnGetRankTop8(
				funapi.Leaderboard.QueryRequest request,	funapi.Leaderboard.QueryResponse response,
				bool error, Session session, bool single) {
			if (error) {
				Log.Error ("Failed to query leaderboard. Leaderboard system error.");
				return;
			}
			string msgtype = single ? "ranklist_single" : "ranklist";

			JObject result = new JObject();
			result ["ranks"] = new JObject();
			int index = 0;
			foreach (funapi.Leaderboard.Record record in response.Records) {
				string index_str = index.ToString ();
				result ["ranks"] [index_str] = new JObject();
				result ["ranks"] [index_str] ["rank"] = record.Rank;
				result ["ranks"] [index_str] ["score"] = record.Score;
				result ["ranks"] [index_str] ["id"] = record.PlayerAccount.Id;
				++index;
			}
			session.SendMessage (msgtype, result, Session.Encryption.kDefault);
		}

		public static void GetAndSendRankTop8(Session session, bool single = false)
		{
			funapi.Leaderboard.QueryRequest request =
				new funapi.Leaderboard.QueryRequest (
					single ? kSingleLeaderboardId : kLeaderboardId, funapi.Leaderboard.Timespan.kAllTime,
					new funapi.Leaderboard.Range (funapi.Leaderboard.RangeType.kFromTop, 0, 7));

			funapi.Leaderboard.GetLeaderboard (request, (_1, _2, _3) => { OnGetRankTop8(_1, _2, _3, session, single); });
		}
	}
}
