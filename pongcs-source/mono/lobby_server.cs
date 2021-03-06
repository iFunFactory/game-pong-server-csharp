using funapi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Pongcs
{
	public class LobbyServer
	{
		// Matchmaking 최대 대기 시간은 10 초입니다.
		private static readonly TimeSpan kMatchmakingTimeout = WallClock.FromSec (10);

		public static void Install()
		{
			NetworkHandlerRegistry.RegisterSessionHandler (new NetworkHandlerRegistry.SessionOpenedHandler (OnSessionOpened),
			                                               new NetworkHandlerRegistry.SessionClosedHandler (OnSessionClosed));
			NetworkHandlerRegistry.RegisterTcpTransportDetachedHandler (new NetworkHandlerRegistry.TcpTransportDetachedHandler (OnTcpDisconnected));

			// for json
			NetworkHandlerRegistry.RegisterMessageHandler ("login", new NetworkHandlerRegistry.JsonMessageHandler (OnLogin));
			NetworkHandlerRegistry.RegisterMessageHandler ("singleresult", new NetworkHandlerRegistry.JsonMessageHandler (OnSingleModeResultReceived));
			NetworkHandlerRegistry.RegisterMessageHandler ("match", new NetworkHandlerRegistry.JsonMessageHandler (OnMatchmaking));
			NetworkHandlerRegistry.RegisterMessageHandler ("cancelmatch", new NetworkHandlerRegistry.JsonMessageHandler (OnCancelMatchmaking));
			NetworkHandlerRegistry.RegisterMessageHandler ("ranklist", new NetworkHandlerRegistry.JsonMessageHandler (OnRanklistRequested));
			NetworkHandlerRegistry.RegisterMessageHandler ("ranklist_single", new NetworkHandlerRegistry.JsonMessageHandler (OnSingleRanklistRequested));

			// for pbuf
			NetworkHandlerRegistry.RegisterMessageHandler ("login", new NetworkHandlerRegistry.ProtobufMessageHandler (OnLogin2));
			NetworkHandlerRegistry.RegisterMessageHandler ("singleresult", new NetworkHandlerRegistry.ProtobufMessageHandler (OnSingleModeResultReceived2));
			NetworkHandlerRegistry.RegisterMessageHandler ("match", new NetworkHandlerRegistry.ProtobufMessageHandler (OnMatchmaking2));
			NetworkHandlerRegistry.RegisterMessageHandler ("cancelmatch", new NetworkHandlerRegistry.ProtobufMessageHandler (OnCancelMatchmaking2));
			NetworkHandlerRegistry.RegisterMessageHandler ("ranklist", new NetworkHandlerRegistry.ProtobufMessageHandler (OnRanklistRequested2));
			NetworkHandlerRegistry.RegisterMessageHandler ("ranklist_single", new NetworkHandlerRegistry.ProtobufMessageHandler (OnSingleRanklistRequested2));
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

			// 세션을 초기과 합니다.
			FreeUser(session);
		}

		public static void OnLogin(Session session, JObject message)
		{
			string account_id = Utility.ReadStringFromJsonObject (message, "id");
			string type = Utility.ReadStringFromJsonObject (message, "type");

			if (account_id == null || type == null) {
				Log.Warning ("Wrong login request: sid={0}", session.Id);
				session.Close ();
				return;
			}

			if (type == "fb") {
				// Facebook 인증을 먼저 합니다.
				string access_token = Utility.ReadStringFromJsonObject (message, "access_token");
				if (access_token == null) {
					Log.Warning ("Wrong login request. No access token: sid={0}, account_id={1}", session.Id, account_id);
					session.Close ();
					return;
				}

				FacebookAuthentication.AuthenticationRequest request =
						new FacebookAuthentication.AuthenticationRequest (access_token);
				FacebookAuthentication.Authenticate (request, (_1, _2, _3) => {
						OnFacebookAuthenticated(_1, _2, _3, session, account_id, Session.EncodingScheme.kJsonEncoding); });
			} else {
				// Guest 는 별도의 인증 없이 로그인 합니다.
				AccountManager.CheckAndSetLoggedInAsync (account_id, session, (_1, _2, _3) => { OnLogin_Completed(_1, _2, _3, Session.EncodingScheme.kJsonEncoding); });
			}
		}

		public static void OnLogin2(Session session, FunMessage message)
		{
			LobbyLoginRequest request = new LobbyLoginRequest();
			if (!message.TryGetExtension_lobby_login_req (
					out request))
			{
				Log.Error ("OnLogin2: Wrong message.");
				return;
			}

			string account_id = request.id;
			string type = request.type;

			if (account_id == null || type == null) {
				Log.Warning ("Wrong login request: sid={0}", session.Id);
				session.Close ();
				return;
			}

			if (type == "fb") {
				// Facebook 인증을 먼저 합니다.
				string access_token = request.access_token;
				if (access_token == null) {
					Log.Warning ("Wrong login request. No access token: sid={0}, account_id={1}", session.Id, account_id);
					session.Close ();
					return;
				}

				FacebookAuthentication.AuthenticationRequest fb_request =
						new FacebookAuthentication.AuthenticationRequest (access_token);
				FacebookAuthentication.Authenticate (fb_request, (_1, _2, _3) => {
						OnFacebookAuthenticated(_1, _2, _3, session, account_id, Session.EncodingScheme.kProtobufEncoding); });
			} else {
				// Guest 는 별도의 인증 없이 로그인 합니다.
				AccountManager.CheckAndSetLoggedInAsync (account_id, session, (_1, _2, _3) => { OnLogin_Completed(_1, _2, _3, Session.EncodingScheme.kProtobufEncoding); });
			}
		}

		private static void OnFacebookAuthenticated(
				FacebookAuthentication.AuthenticationRequest param_request,
				FacebookAuthentication.AuthenticationResponse param_response,
				bool param_error, Session session, string account_id, Session.EncodingScheme encoding)
		{
			if (param_error) {
				// 인증에 오류가 있습니다. 장애 오류입니다.
				Log.Error("Failed to authenticate. Facebook authentication error: id={0}", account_id);
				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					session.SendMessage("login", Utility.MakeResponse("nop", "facebook authentication error"),
															Session.Encryption.kDefault);
				} else {
					Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					LobbyLoginReply reply = new LobbyLoginReply();
					reply.result = "nop";
					reply.msg = "facebook authentication error";
					funmsg.AppendExtension_lobby_login_repl (reply);
					session.SendMessage("login", funmsg);
				}
				return;
			}

			if (!param_response.Success) {
				// 인증에 실패했습니다. 올바르지 않은 access token 입니다.
				Log.Info("Failed to authenticate. Wrong Facebook access token: id={0}", account_id);
				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					session.SendMessage("login",
															Utility.MakeResponse("nop", "facebook authentication fail: " + param_response.Error.Message),
															Session.Encryption.kDefault);
				} else {
					Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					LobbyLoginReply reply = new LobbyLoginReply();
					reply.result = "nop";
					reply.msg = "facebook authentication fail: " + param_response.Error.Message;
					funmsg.AppendExtension_lobby_login_repl (reply);
					session.SendMessage("login", funmsg);
				}
				return;
			}

			// 인증에 성공했습니다.
			Log.Info("Succeed to authenticate facebook account: id={0}", account_id);

			// 이어서 로그인 처리를 진행합니다.
			AccountManager.CheckAndSetLoggedInAsync(account_id, session, (_1, _2, _3) => { OnLogin_Completed(_1, _2, _3, encoding); });
		}

		private static void OnLogin_Completed(string account_id, Session session, bool success, Session.EncodingScheme encoding)
		{
			if (!success) {
				// 로그인에 실패 응답을 보냅니다. 중복 로그인이 원인입니다.
				// (1. 같은 ID 로 이미 다른 Session 이 로그인 했거나,
				//  2. 이 Session 이 이미 로그인 되어 있는 경우)
				Log.Info ("Failed to login: id={0}", account_id);
				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					session.SendMessage("login",
															Utility.MakeResponse("nop", "failed to login"),
															Session.Encryption.kDefault);
				} else {
					Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					LobbyLoginReply reply = new LobbyLoginReply();
					reply.result = "nop";
					reply.msg = "failed to login";
					funmsg.AppendExtension_lobby_login_repl (reply);
					session.SendMessage("login", funmsg);
				}

				// 아래 로그아웃 처리를 한 후 자동으로 로그인 시킬 수 있지만
				// 일단 클라이언트에서 다시 시도하도록 합니다.

				// 1. 이 ID 의 로그인을 풀어버립니다.(로그아웃)
				AccountManager.LogoutCallback logout_cb = (string param_account_id, Session param_session, bool param_success) => {
					if (!param_success) {
						return;
					}
					if (param_session != null) {
						// 같은 서버에 로그인 되어 있었습니다.
						Log.Info("Logged out(local) by duplicated login request: id={0}", param_account_id);
						session.Close();
					} else {
						// 다른 서버에 로그인 되어 있었습니다.
						// 해당 서버의 OnLoggedOutRemotely() 에서 처리합니다.
						Log.Info("Logged out(remote) by duplicated login request: id={0}", param_account_id);
					}
				};

				AccountManager.SetLoggedOutGlobalAsync (account_id, new AccountManager.LogoutCallback (logout_cb));

				// 2. 이 Session 의 로그인을 풀어버립니다.(로그아웃)
				string account_id_logged_in = AccountManager.FindLocalAccount (session);
				if (account_id_logged_in != string.Empty) {
					// OnSessionClosed 에서 처리합니다.
					Log.Info ("Close session. by duplicated login request: id={0}", account_id);
					session.Close ();
				}

				return;
			}

			// User Object 를 가져옵니다.
			User user = User.FetchById (account_id);
			if (user == null) {
				// 새로운 유저를 생성합니다.
				user = User.Create (account_id);
				Log.Info ("Registered new user: id={0}", account_id);
			}

			Event.AssertNoRollback ();

			Log.Info ("Succeed to login: id={0}", account_id);

			// 로그인 Activitiy Log 를 남깁니다.
			ActivityLog.PlayerLoggedIn (session.Id.ToString (), account_id, WallClock.Now);

			// Session 에 Login 한 ID 를 저장합니다.
			session.AddToContext ("id", account_id);

			if (encoding == Session.EncodingScheme.kJsonEncoding) {
				JObject response = Utility.MakeResponse ("ok");
				response ["id"] = account_id;
				response ["winCount"] = user.GetWinCount ();
				response ["loseCount"] = user.GetLoseCount ();
				response ["curRecord"] = Leaderboard.GetRecord (account_id);
				response ["singleWinCount"] = user.GetWinCountSingle ();
				response ["singleLoseCount"] = user.GetLoseCountSingle ();
				response ["singleCurRecord"] = Leaderboard.GetRecord (account_id, true);

				session.SendMessage ("login", response, Session.Encryption.kDefault);
			} else {
				Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
				FunMessage funmsg = new FunMessage ();
				LobbyLoginReply reply = new LobbyLoginReply();
				reply.result = "ok";
				reply.id =  account_id;
				reply.win_count = (int) user.GetWinCount ();
				reply.lose_count = (int) user.GetLoseCount ();
				reply.cur_record = (int) Leaderboard.GetRecord (account_id);
				reply.win_count_single = (int) user.GetWinCountSingle ();
				reply.lose_count_single = (int) user.GetLoseCountSingle ();
				reply.cur_record_single = Leaderboard.GetRecord (account_id, true);
				funmsg.AppendExtension_lobby_login_repl (reply);

				session.SendMessage ("login", funmsg, Session.Encryption.kDefault);
			}
		}

		public static void OnSingleModeResultReceived(Session session, JObject message)
		{
			bool win = Utility.ReadStringFromJsonObject (message, "result") == "win";
		  HandleSingleModeResult(session, win, Session.EncodingScheme.kJsonEncoding);
		}

		public static void OnSingleModeResultReceived2(Session session, FunMessage message)
		{
			LobbySingleModeResultMessage request = new LobbySingleModeResultMessage();
			if (!message.TryGetExtension_lobby_single_result (
					out request))
			{
				Log.Error ("OnSingleModeResultReceived2: Wrong message.");
				return;
			}

			bool win = (request.result == "win");
		  HandleSingleModeResult(session, win, Session.EncodingScheme.kProtobufEncoding);
		}

		public static void HandleSingleModeResult(Session session, bool win, Session.EncodingScheme encoding) {
			string id;
			if (!session.GetFromContext("id", out id))
			{
				Log.Warning ("Failed to update singlemode game result. Not logged in.");
				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					session.SendMessage("error", Utility.MakeResponse("fail", "not logged in"));
				} else {
					Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					PongErrorMessage msg = new PongErrorMessage();
					msg.result = "fail";
					msg.msg = "not logged in";
					funmsg.AppendExtension_pong_error (msg);
					session.SendMessage ("error", funmsg);
				}
				return;
			}

			User user = User.FetchById (id);
			if (user == null)
			{
				Log.Error ("Cannot find uwer's id in db: id={0}", id);
				return;
			}

			if (win)
			{
				Leaderboard.OnWin(user, true);
			}
			else
			{
				Leaderboard.OnLose(user, true);
			}
		}

		public static void OnMatchmaking(Session session, JObject message)
		{
			StartMatchmaking(session, Session.EncodingScheme.kJsonEncoding);
		}

		public static void OnMatchmaking2(Session session, FunMessage message)
		{
			StartMatchmaking(session, Session.EncodingScheme.kProtobufEncoding);
		}

		private static void StartMatchmaking(Session session, Session.EncodingScheme encoding)
		{
			// 로그인 한 Id 를 가져옵니다.
			string id;
			if (!session.GetFromContext ("id", out id)) {
				Log.Warning ("Failed to request matchmaking. Not logged in.");
				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					session.SendMessage ("error", Utility.MakeResponse ("fail", "not logged in"));
				} else {
					Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					PongErrorMessage msg = new PongErrorMessage();
					msg.result = "fail";
					msg.msg = "not logged in";
					funmsg.AppendExtension_pong_error (msg);
					session.SendMessage ("error", funmsg);
				}
				return;
			}

			funapi.Matchmaking.Client.MatchCallback match_cb = (string player_id, funapi.Matchmaking.Match match,
			                                                    funapi.Matchmaking.MatchResult result) => {
				JObject response = new JObject();
				LobbyMatchReply reply = new LobbyMatchReply();

				if (result == funapi.Matchmaking.MatchResult.kSuccess) {
					// Matchmaking 에 성공했습니다.
					Log.Info("Succeed in matchmaking: id={0}", player_id);

					string player_a_id = Utility.ReadStringFromJsonObject(match.Context, "A");
					string player_b_id = Utility.ReadStringFromJsonObject(match.Context, "B");
					Log.Assert(player_a_id != null && player_b_id != null);

					string opponent_id;
					if (player_a_id == player_id) {
						opponent_id = player_b_id;
					} else {
						opponent_id = player_a_id;
					}

					session.AddToContext("opponent", opponent_id);
					session.AddToContext("matching", "done");
					session.AddToContext("ready", 0);

					if (encoding == Session.EncodingScheme.kJsonEncoding) {
						response = Utility.MakeResponse("Success");
						response["A"] = player_a_id;
						response["B"] = player_b_id;
					} else {
						Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
						reply.result = "Success";
						reply.player1 = player_a_id;
						reply.player2 = player_b_id;
					}
					// 유저를 Game 서버로 보냅니다.
					Common.Redirect(session, "game");
				} else if (result == funapi.Matchmaking.MatchResult.kAlreadyRequested) {
					// Matchmaking 요청을 중복으로 보냈습니다.
					Log.Info("Failed in matchmaking. Already requested: id={0}", player_id);
					session.AddToContext("matching", "failed");
					if (encoding == Session.EncodingScheme.kJsonEncoding) {
						response = Utility.MakeResponse("AlreadyRequested");
					} else {
						Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
						reply.result = "AlreadyRequested";
					}
				} else if (result == funapi.Matchmaking.MatchResult.kTimeout) {
					// Matchmaking 처리가 시간 초과되었습니다.
					Log.Info("Failed in matchmaking. Timeout: id={0}", player_id);
					session.AddToContext("matching", "failed");
					if (encoding == Session.EncodingScheme.kJsonEncoding) {
						response = Utility.MakeResponse("Timeout");
					} else {
						Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
						reply.result = "Timeout";
					}
				} else {
					// Matchmaking 에 오류가 발생했습니다.
					Log.Error("Failed in matchmaking. Error: id={0}", player_id);
					session.AddToContext("matching", "failed");
					if (encoding == Session.EncodingScheme.kJsonEncoding) {
						response = Utility.MakeResponse("Error");
					} else {
						Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
						reply.result = "Error";
					}
				}

				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					session.SendMessage("match", response, Session.Encryption.kDefault);
				} else {
					FunMessage funmsg = new FunMessage ();
					funmsg.AppendExtension_lobby_match_repl (reply);
					session.SendMessage("match", funmsg, Session.Encryption.kDefault);
				}
			};

			// 빈 Player Context 를 만듭니다. 지금 구현에서는 Matchmaking 서버가
			// 조건 없이 Matching 합니다. Level 등의 조건으로 Matching 하려면
			// 여기에 Level 등의 Matching 에 필요한 정보를 넣습니다.
			JObject player_ctxt = new JObject ();

			// Matchmaking 을 요청합니다.
			funapi.Matchmaking.Client.Start (
				(int)MatchmakingType.kMatch1vs1, id, player_ctxt, new funapi.Matchmaking.Client.MatchCallback(match_cb),
				funapi.Matchmaking.Client.TargetServerSelection.kMostNumberOfPlayers, null, kMatchmakingTimeout);
		}

		public static void OnCancelMatchmaking(Session session, JObject message)
		{
			CancelMatchmaking(session, Session.EncodingScheme.kJsonEncoding);
		}

		public static void OnCancelMatchmaking2(Session session, FunMessage message)
		{
			CancelMatchmaking(session, Session.EncodingScheme.kProtobufEncoding);
		}

		private static void CancelMatchmaking(Session session, Session.EncodingScheme encoding)
		{
			// 로그인 한 Id 를 가져옵니다.
			string id;
			if (!session.GetFromContext ("id", out id)) {
				Log.Warning ("Failed to cancel matchmaking. Not logged in.");
				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					session.SendMessage ("error", Utility.MakeResponse ("fail", "not logged in"));
				} else {
					Log.Assert(encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					PongErrorMessage msg = new PongErrorMessage();
					msg.result = "fail";
					msg.msg = "not logged in";
					funmsg.AppendExtension_pong_error (msg);
					session.SendMessage ("error", funmsg);
				}
				return;
			}

			// 매치메이킹 취소 상태로 변경합니다.
			session.AddToContext("matching", "cancel");

			funapi.Matchmaking.Client.CancelCallback cancel_cb = (string player_id,
			                                                      funapi.Matchmaking.CancelResult result) => {
				if (encoding == Session.EncodingScheme.kJsonEncoding) {
					JObject response = new JObject();

					if (result == funapi.Matchmaking.CancelResult.kSuccess) {
						Log.Info("Succeed to cancel matchmaking: id={0}", player_id);
						response = Utility.MakeResponse("Cancel");
					} else if (result == funapi.Matchmaking.CancelResult.kNoRequest) {
						response = Utility.MakeResponse("NoRequest");
					} else {
						response = Utility.MakeResponse("Error");
					}

					session.SendMessage("match", response, Session.Encryption.kDefault);
				} else {
					Log.Assert (encoding == Session.EncodingScheme.kProtobufEncoding);
					FunMessage funmsg = new FunMessage ();
					LobbyMatchReply reply = new LobbyMatchReply();

					if (result == funapi.Matchmaking.CancelResult.kSuccess) {
						Log.Info("Succeed to cancel matchmaking: id={0}", player_id);
						reply.result = "Cancel";
					} else if (result == funapi.Matchmaking.CancelResult.kNoRequest) {
						reply.result = "NoRequest";
					} else {
						reply.result = "Error";
					}

					funmsg.AppendExtension_lobby_match_repl (reply);
					session.SendMessage("match", funmsg, Session.Encryption.kDefault);
				}
			};

			funapi.Matchmaking.Client.Cancel ((int)MatchmakingType.kMatch1vs1, id, (_1, _2) => { cancel_cb(_1, _2); });
		}

		public static void OnRanklistRequested(Session session, JObject message)
		{
			Leaderboard.GetAndSendRankTop8 (session, Session.EncodingScheme.kJsonEncoding);
		}

		public static void OnRanklistRequested2(Session session, FunMessage message)
		{
			Leaderboard.GetAndSendRankTop8 (session, Session.EncodingScheme.kProtobufEncoding);
		}

		public static void OnSingleRanklistRequested(Session session, JObject message)
		{
			Leaderboard.GetAndSendRankTop8 (session, Session.EncodingScheme.kJsonEncoding, true);
		}

		public static void OnSingleRanklistRequested2(Session session, FunMessage message)
		{
			Leaderboard.GetAndSendRankTop8 (session, Session.EncodingScheme.kProtobufEncoding, true);
		}

		// 세션을 정리합니다.
		public static void FreeUser(Session session)
		{
			// 유저를 정리하기 위한 Context 를 읽어옵니다.
			string matching_state;
			string id;
			session.GetFromContext ("matching", out matching_state);
			session.GetFromContext ("id", out id);

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

			// 매치메이킹이 진행 중이면 취소합니다.
			if (matching_state == "doing") {
				// Matchmaking cancel 결과를 처리할 람다 함수입니다.
				funapi.Matchmaking.Client.CancelCallback cancel_cb =
					(string player_id, funapi.Matchmaking.CancelResult result) => {
					if (result == funapi.Matchmaking.CancelResult.kSuccess) {
						Log.Info("Succeed to cancel matchmaking by session close: id={0}", id);
					} else {
						Log.Info("Failed to cancel matchmaking by session close: id={0}", id);
					}
				};
				funapi.Matchmaking.Client.Cancel ((long)MatchmakingType.kMatch1vs1, id, cancel_cb);
			}
		}
	}
}
