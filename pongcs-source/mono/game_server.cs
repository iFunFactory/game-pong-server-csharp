using funapi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Pongcs
{
	public class GameServer
	{
		public static void Install()
		{
			NetworkHandlerRegistry.RegisterSessionHandler (new NetworkHandlerRegistry.SessionOpenedHandler (OnSessionOpened),
			                                               new NetworkHandlerRegistry.SessionClosedHandler (OnSessionClosed));
			NetworkHandlerRegistry.RegisterTcpTransportDetachedHandler (new NetworkHandlerRegistry.TcpTransportDetachedHandler (OnTcpDisconnected));
			NetworkHandlerRegistry.RegisterWebSocketTransportDetachedHandler (new NetworkHandlerRegistry.WebSocketTransportDetachedHandler (OnWebSocketDisconnected));

			NetworkHandlerRegistry.RegisterMessageHandler ("ready", new NetworkHandlerRegistry.JsonMessageHandler (OnReady));
			NetworkHandlerRegistry.RegisterMessageHandler ("relay", new NetworkHandlerRegistry.JsonMessageHandler (OnRelay));
			NetworkHandlerRegistry.RegisterMessageHandler ("result", new NetworkHandlerRegistry.JsonMessageHandler (OnResult));
		}

		// 새 클라이언트가 접속하여 세션이 열릴 때 불리는 함수
		public static void OnSessionOpened(Session session)
		{
			// 세션 접속  Activity Log 를 남깁니다.
			ActivityLog.SessionOpened (session.Id.ToString (), WallClock.Now);
		}

		// 세션이 닫혔을 때 불리는 함수
		public static void OnSessionClosed(Session session, Session.CloseReason reason)
		{
			// 세션 닫힘 Activity Log 를 남깁니다.
			ActivityLog.SessionClosed (session.Id.ToString (), WallClock.Now);
			// 세션을 초기화 합니다.
			FreeUser(session);
		}

		// TCP 연결이 끊기면 불립니다.
		public static void OnTcpDisconnected(Session session)
		{
			string id = null;
			if (session.GetFromContext ("id", out id)) {
				Log.Info ("TCP disconnected: id={0}", id);
			}
			// 세션을 초기화 합니다.
			FreeUser(session);
		}

		// Websocket 연결이 끊기면 불립니다.
		public static void OnWebSocketDisconnected(Session session)
		{
			string id = null;
			if (session.GetFromContext ("id", out id)) {
				Log.Info ("Websocket disconnected: id={0}", id);
			}
			// 세션을 초기화 합니다.
			FreeUser(session);
		}

		// 게임 플레이 준비 메시지를 받으면 불립니다.
		public static void OnReady(Session session, JObject message)
		{
			session.AddToContext ("ready", 1);
			string opponent_id;
			Log.Verify (session.GetFromContext ("opponent", out opponent_id));
			Session opponent_session = AccountManager.FindLocalSession (opponent_id);

			// 상대의 상태를 확인합니다.
			if (opponent_session != null &&
			    (opponent_session.IsTransportAttached (Session.Transport.kTcp) ||
			     opponent_session.IsTransportAttached (Session.Transport.kWebSocket))) {
				long is_opponent_ready = 0;
				opponent_session.GetFromContext ("ready", out is_opponent_ready);
				if (is_opponent_ready == 1) {
					// 둘 다 준비가 되었습니다. 시작 신호를 보냅니다.
					JObject response = Utility.MakeResponse ("ok");
					session.SendMessage ("start", response);
					opponent_session.SendMessage ("start", response);
				}
			}
			else {
				// 아직 상대가 방에 없기 때문에 기다린다.
			}
		}

		public static void OnRelay(Session session, JObject message)
		{
			string opponent_id;
			Log.Verify (session.GetFromContext ("opponent", out opponent_id));
			Session opponent_session = AccountManager.FindLocalSession (opponent_id);
			if (opponent_session != null) {
				Log.Info ("message reply: sid={0}", session.Id);
				opponent_session.SendMessage ("relay", message);
			}
		}

		public static void OnResult(Session session, JObject message)
		{
			// 패배한 쪽만 result를 보내도록 되어있습니다.

			// 내 아이디를 가져옵니다.
			string id;
			session.GetFromContext ("id", out id);

			// 상대방의 아이디와 세션을 가져옵니다.
			string opponent_id;
			Log.Verify (session.GetFromContext ("opponent", out opponent_id));
			Session opponent_session = AccountManager.FindLocalSession (opponent_id);

			User user = User.FetchById (id);
			User opponent_user = User.FetchById (opponent_id);

			Event.AssertNoRollback ();

			// 편의상 아래 함수에서 ORM 데이터 업데이트 처리도 합니다.
			Leaderboard.OnWin (opponent_user);
			Leaderboard.OnLose (user);

			// 상대에게 승리했음을 알립니다.
			if (opponent_session != null) {
				opponent_session.SendMessage ("result", Utility.MakeResponse ("win"));
				opponent_session.DeleteFromContext ("opponent");
				Common.Redirect (opponent_session, "lobby");
				FreeUser (opponent_session);
			}

			// 패배 확인 메세지를 보냅니다.
			session.SendMessage ("result", message);
			session.DeleteFromContext ("opponent");
			Common.Redirect (session, "lobby");
			FreeUser (session);
		}

		// 세션을 정리합니다.
		public static void FreeUser(Session session)
		{
			// 유저를 정리하기 위한 Context 를 읽어옵니다.
			string id;
			string opponent_id;
			session.GetFromContext ("id", out id);
			session.GetFromContext ("opponent", out opponent_id);

			// Session Context 를 초기화 합니다.
			session.Context = new JObject ();

			// 로그아웃하고 세션을 종료합니다.
			if (id != string.Empty) {
				AccountManager.LogoutCallback logout_cb = (string param_account_id,
				                                           Session param_session,
				                                           bool param_result) => {
					Log.InfoIf(param_result, "Logged out(local) by session close: id={0}", param_account_id);
				};
				AccountManager.SetLoggedOutAsync(id, logout_cb);
			}

			// 대전 상대가 있는 경우, 상대가 승리한 것으로 처리하고 로비서버로 보냅니다.
			if (opponent_id == string.Empty) {
				return;
			}
			Session opponent_session = AccountManager.FindLocalSession(opponent_id);
			if (opponent_session == null) {
				return;
			}

			Event.EventFunction update = () => {
				User user = User.FetchById (id);
				User opponent_user = User.FetchById (opponent_id);

				Event.AssertNoRollback ();

				// 편의상 아래 함수에서 ORM 데이터 업데이트 처리도 합니다.
				Leaderboard.OnWin (opponent_user);
				Leaderboard.OnLose (user);

				opponent_session.SendMessage ("result", Utility.MakeResponse ("win"), Session.Encryption.kDefault);

				Common.Redirect (opponent_session, "lobby");

				opponent_session.DeleteFromContext("opponent");
				FreeUser(opponent_session);
			};

			Event.Invoke (update);
		}
	}
}
