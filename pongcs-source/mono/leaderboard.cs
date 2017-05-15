using funapi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Pongcs
{
	public class Leaderboard
	{
		private static readonly string kLeaderboardId = "winning_streak";
		private static readonly string kAccountServiceProvider = "none";

		public static void OnWin(User user)
		{
			Event.AssertNoRollback ();

			Utility.RefreshWinningStreak (user);

			user.SetWinCount (user.GetWinCount () + 1);
			user.SetWinningStreak (user.GetWinningStreak () + 1);
			user.SetWinningStreakDayOfYear (WallClock.Now.DayOfYear);

			string id = user.GetId ();
			int winning_streak = (int)user.GetWinningStreak ();

			// TODO: 업데이트 요청과 동시에 리더보드가 리셋되어 전 날 값으로 업데이트되는 상황 고려

			funapi.Leaderboard.ScoreSubmissionRequest request =
				new funapi.Leaderboard.ScoreSubmissionRequest (
					kLeaderboardId, kAccountServiceProvider, id, (double)winning_streak,
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

		public static void OnLose(User user)
		{
			Event.AssertNoRollback ();

			Utility.RefreshWinningStreak (user);

			user.SetLoseCount (user.GetLoseCount () + 1);
			user.SetWinningStreak (0);
			user.SetWinningStreakDayOfYear (WallClock.Now.DayOfYear);
		}

		public static int GetRank(string account_id)
		{
			// TODO: Async API 로 교체

			funapi.Leaderboard.QueryRequest request =
				new funapi.Leaderboard.QueryRequest (
					kLeaderboardId, kAccountServiceProvider, account_id, funapi.Leaderboard.Timespan.kDaily,
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
			return (int)enumerator.Current.Rank;
		}

		public static JObject GetRankTop8()
		{
			// TODO: Async API 로 교체

			JObject result = new JObject();

			funapi.Leaderboard.QueryRequest request =
				new funapi.Leaderboard.QueryRequest (
					kLeaderboardId, funapi.Leaderboard.Timespan.kDaily,
					new funapi.Leaderboard.Range (funapi.Leaderboard.RangeType.kFromTop, 0, 7));
			funapi.Leaderboard.QueryResponse response;

			if (!funapi.Leaderboard.GetLeaderboardSync (request, out response)) {
				Log.Error ("Failed to query leaderboard. Leaderboard system error.");
				return result;
			}

			int index = 0;
			foreach (funapi.Leaderboard.Record record in response.Records) {
				string index_str = index.ToString ();
				result [index_str] ["rank"] = record.Rank;
				result [index_str] ["score"] = record.Score;
				result [index_str] ["id"] = record.PlayerAccount.Id;
				++index;
			}

			return result;
		}
	}
}
