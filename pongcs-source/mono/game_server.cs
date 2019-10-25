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
			Session.EncodingScheme encoding = Session.EncodingScheme.kUnknownEncoding;
			UInt64 tcp_json_port = Flags.GetUInt64 ("tcp_json_port");
			UInt64 udp_json_port = Flags.GetUInt64 ("udp_json_port");
			UInt64 http_json_port = Flags.GetUInt64 ("http_json_port");
			UInt64 websocket_json_port = Flags.GetUInt64 ("websocket_json_port");
			UInt64 tcp_protobuf_port = Flags.GetUInt64 ("tcp_protobuf_port");
			UInt64 udp_protobuf_port = Flags.GetUInt64 ("udp_protobuf_port");
			UInt64 http_protobuf_port = Flags.GetUInt64 ("http_protobuf_port");
			UInt64 websocket_protobuf_port = Flags.GetUInt64 ("websocket_protobuf_port");
			if (tcp_json_port != 0 || udp_json_port != 0 || http_json_port != 0 || websocket_json_port != 0) {
				encoding = Session.EncodingScheme.kJsonEncoding;
			}
			if (tcp_protobuf_port != 0 || udp_protobuf_port != 0 || http_protobuf_port != 0|| websocket_protobuf_port != 0) {
				if (encoding != Session.EncodingScheme.kUnknownEncoding) {
					Log.Fatal ("Cannot set both JSON and Protobuf. Enable only one in MANIFEST.game.json");
				}
				encoding = Session.EncodingScheme.kProtobufEncoding;
			}
			if (encoding == Session.EncodingScheme.kUnknownEncoding) {
				Log.Fatal ("Either JSON or Protobuf must be enabled.");
			}
			NetworkHandlerRegistry.RegisterSessionHandler (
					new NetworkHandlerRegistry.SessionOpenedHandler (OnSessionOpened),
			    (_1, _2) => { OnSessionClosed(_1, _2, encoding); });
			NetworkHandlerRegistry.RegisterTcpTransportDetachedHandler ((_1) => { OnTcpDisconnected(_1, encoding); });
			NetworkHandlerRegistry.RegisterWebSocketTransportDetachedHandler ((_1) => { OnWebSocketDisconnected(_1, encoding); });

			// for json
			NetworkHandlerRegistry.RegisterMessageHandler ("ready", new NetworkHandlerRegistry.JsonMessageHandler (OnReady));
			NetworkHandlerRegistry.RegisterMessageHandler ("relay", new NetworkHandlerRegistry.JsonMessageHandler (OnRelay));
			NetworkHandlerRegistry.RegisterMessageHandler ("result", new NetworkHandlerRegistry.JsonMessageHandler (OnResult));

			// for pbuf
			NetworkHandlerRegistry.RegisterMessageHandler ("ready", new NetworkHandlerRegistry.ProtobufMessageHandler (OnReady2));
			NetworkHandlerRegistry.RegisterMessageHandler ("relay", new NetworkHandlerRegistry.ProtobufMessageHandler (OnRelay2));
			NetworkHandlerRegistry.RegisterMessageHandler ("result", new NetworkHandlerRegistry.ProtobufMessageHandler (OnResult2));
		}

		// 새 클라이언트가 접속하여 세션이 열릴 때 불리는 함수
		public static void OnSessionOpened(Session session)
		{
			// 세션 접속  Activity Log 를 남깁니다.
			ActivityLog.SessionOpened (session.Id.ToString (), WallClock.Now);
		}

		// 세션이 닫혔을 때 불리는 함수
		public static void OnSessionClosed(Session session, Session.CloseReason reason, Session.EncodingScheme encoding)
		{
			// 세션 닫힘 Activity Log 를 남깁니다.
			ActivityLog.SessionClosed (session.Id.ToString (), WallClock.Now);
			// 세션을 초기화 합니다.
			FreeUser(session, encoding);
		}

		// TCP 연결이 끊기면 불립니다.
		public static void OnTcpDisconnected(Session session, Session.EncodingScheme encoding)
		{
			string id = null;
			if (session.GetFromContext ("id", out id)) {
				Log.Info ("TCP disconnected: id={0}", id);
			}
			// 세션을 초기화 합니다.
			FreeUser(session, encoding);
		}

		// Websocket 연결이 끊기면 불립니다.
		public static void OnWebSocketDisconnected(Session session, Session.EncodingScheme encoding)
		{
			string id = null;
			if (session.GetFromContext ("id", out id)) {
				Log.Info ("Websocket disconnected: id={0}", id);
			}
			// 세션을 초기화 합니다.
			FreeUser(session, encoding);
		}

		// 게임 플레이 준비 메시지를 받으면 불립니다.
		public static void OnReady(Session session, JObject message)
		{
			HandleReadySignal(session, Session.EncodingScheme.kJsonEncoding);
		}

		public static void OnReady2(Session session, FunMessage message)
		{
			HandleReadySignal(session, Session.EncodingScheme.kProtobufEncoding);
		}

		public static void HandleReadySignal(Session session, Session.EncodingScheme encoding)
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
					if (encoding == Session.EncodingScheme.kJsonEncoding) {
						JObject response = Utility.MakeResponse ("ok");
						session.SendMessage ("start", response);
						opponent_session.SendMessage ("start", response);
					} else {
						Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
						FunMessage funmsg = new FunMessage ();
						GameStartMessage msg = new GameStartMessage();
						msg.result = "ok";
						session.SendMessage ("start", funmsg);
						funmsg.AppendExtension_game_start (msg);
						opponent_session.SendMessage ("start", funmsg);
					}
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

		public static void OnRelay2(Session session, FunMessage message)
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
			OnHandleResult(session, Session.EncodingScheme.kJsonEncoding);
		}

		public static void OnResult2(Session session, FunMessage message)
		{
			OnHandleResult(session, Session.EncodingScheme.kProtobufEncoding);
		}

		public static void OnHandleResult(Session session, Session.EncodingScheme encoding)
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
				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					opponent_session.SendMessage ("result", Utility.MakeResponse ("win"));
				} else {
					Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					GameResultMessage msg = new GameResultMessage();
					msg.result = "win";
					funmsg.AppendExtension_game_result (msg);
					opponent_session.SendMessage ("result", funmsg);
				}

				opponent_session.DeleteFromContext ("opponent");
				Common.Redirect (opponent_session, "lobby");
				FreeUser (opponent_session, encoding);
			}

			// 패배 확인 메세지를 보냅니다.
			if (encoding == Session.EncodingScheme.kJsonEncoding) {
				session.SendMessage ("result", Utility.MakeResponse ("lose"));
			} else {
				Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
				FunMessage funmsg = new FunMessage ();
				GameResultMessage msg = new GameResultMessage();
				msg.result = "lose";
				funmsg.AppendExtension_game_result (msg);
				session.SendMessage ("result", funmsg);
			}
			session.DeleteFromContext ("opponent");
			Common.Redirect (session, "lobby");
			FreeUser (session, encoding);
		}

		// 세션을 정리합니다.
		public static void FreeUser(Session session, Session.EncodingScheme encoding)
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

				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					opponent_session.SendMessage ("result", Utility.MakeResponse ("win"));
				} else {
					Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					GameResultMessage msg = new GameResultMessage();
					msg.result = "win";
					funmsg.AppendExtension_game_result (msg);
					opponent_session.SendMessage ("result", funmsg);
				}

				Common.Redirect (opponent_session, "lobby");

				opponent_session.DeleteFromContext("opponent");
				FreeUser(opponent_session, encoding);
			};

			Event.Invoke (update);
		}
	}
}
