using funapi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Pongcs
{
	public class Server
	{
		public static bool Install(ArgumentMap arguments)
		{
			string flavor = Flags.GetString ("app_flavor");
			if (flavor == "lobby") {
				// Lobby 서버 역할로 초기화 합니다.
				Log.Info ("Install lobby server");
				Common.Install ();
				LobbyServer.Install ();
			} else if (flavor == "game") {
				// Game 서버 역할로 초기화 합니다.
				Log.Info ("Install game server");
				Common.Install ();
				GameServer.Install ();
			} else if (flavor == "matchmaker") {
				// Matchmaker 서버 역할로 초기화 합니다.
				Log.Info ("Install matchmaker server");
				MatchmakingServer.Install ();
			} else {
				Log.Assert (false);
			}

			return true;
		}

		public static bool Start()
		{
			string flavor = Flags.GetString ("app_flavor");
			Log.Info ("Starting " + flavor + " server");
			return true;
		}

		public static bool Uninstall()
		{
			return true;
		}
	}
}
