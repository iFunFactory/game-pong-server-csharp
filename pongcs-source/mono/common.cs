using funapi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Pongcs
{
	public class Common
	{
		public static void Install()
		{
			AccountManager.RegisterRedirectionHandler (new AccountManager.RedirectionCallback (OnClientRedirected));
			AccountManager.RegisterRemoteLogoutHandler (new AccountManager.RemoteLogoutCallback (OnLoggedOutRemotely));
		}

		// 클라이언트를 다른 서버로 이동시킵니다.
		public static void Redirect(Session session, string server_tag_to_redirect)
		{
			// 아이디를 가져옵니다.
			string account_id;
			session.GetFromContext ("id", out account_id);

			System.Guid target = Utility.PickServerRandomly (server_tag_to_redirect);
			if (target == System.Guid.Empty) {
				Log.Error ("Failed to redirect. No target server: account_id={0}, server_tag={1}", account_id, server_tag_to_redirect);
				return;
			}

			string session_context;
			lock (session) {
				session_context = session.Context.ToString (Newtonsoft.Json.Formatting.None);
			}

			if (AccountManager.RedirectClient (session, target, session_context)) {
				Log.Info ("Client redirecting: account_id={0}, server_tag={1}", account_id, server_tag_to_redirect);
			} else {
				Log.Error ("Client redirecting failure: account_id={0}, server_tag={1}", account_id, server_tag_to_redirect);
			}
		}

		// 클라이언트가 다른 서버에서 이동해 왔을 때 불립니다.
		private static void OnClientRedirected(string account_id, Session session, bool success, string extra_data)
		{
			if (!success) {
				Log.Error ("Failed to redirect client: account_id={0}", account_id);
				return;
			}

			JObject session_context = JObject.Parse (extra_data);
			Log.Assert (session_context != null);

			lock (session) {
				session.Context = session_context;
			}

			Log.Info ("Client redirected: account_id={0}", account_id);
		}

		// 이 서버에 로그인된 유저를 다른 서버에서 로그아웃 시킬 때 불립니다.
		private static void OnLoggedOutRemotely(string account_id, Session session)
		{
			Log.Info ("Client logged out by remote server: account_id={0}", account_id);
			session.Close ();
		}
	}
}
