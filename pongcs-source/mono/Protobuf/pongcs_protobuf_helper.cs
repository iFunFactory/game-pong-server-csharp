using System;
using System.Collections.Generic;
using funapi;


namespace Pongcs
{
    public enum ExtendedMessageFieldNumber
    {
        FunMessage_game_relay = 32,
        FunMessage_game_result = 31,
        FunMessage_game_start = 30,
        FunMessage_lobby_cancel_match_req = 23,
        FunMessage_lobby_login_repl = 21,
        FunMessage_lobby_login_req = 20,
        FunMessage_lobby_match_repl = 24,
        FunMessage_lobby_match_req = 22,
        FunMessage_lobby_rank_list_repl = 26,
        FunMessage_lobby_rank_list_req = 25,
        FunMessage_lobby_single_result = 27,
        FunMessage_pbuf_another = 17,
        FunMessage_pbuf_echo = 16,
        FunMessage_pong_error = 63,
        FunMulticastMessage_pbuf_hello = 16,
        FunRpcMessage_echo_rpc = 32,
    }


    public static class ProtobufHelper
    {
        public static bool TryGetExtension_game_relay (this FunMessage message, out GameRelayMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_game_relay, out value);
        }

        public static void AppendExtension_game_relay (this FunMessage message, GameRelayMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_game_relay, value);
        }

        public static bool TryGetExtension_game_result (this FunMessage message, out GameResultMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_game_result, out value);
        }

        public static void AppendExtension_game_result (this FunMessage message, GameResultMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_game_result, value);
        }

        public static bool TryGetExtension_game_start (this FunMessage message, out GameStartMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_game_start, out value);
        }

        public static void AppendExtension_game_start (this FunMessage message, GameStartMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_game_start, value);
        }

        public static bool TryGetExtension_lobby_cancel_match_req (this FunMessage message, out LobbyCancelMatchRequest value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_cancel_match_req, out value);
        }

        public static void AppendExtension_lobby_cancel_match_req (this FunMessage message, LobbyCancelMatchRequest value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_cancel_match_req, value);
        }

        public static bool TryGetExtension_lobby_login_repl (this FunMessage message, out LobbyLoginReply value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_login_repl, out value);
        }

        public static void AppendExtension_lobby_login_repl (this FunMessage message, LobbyLoginReply value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_login_repl, value);
        }

        public static bool TryGetExtension_lobby_login_req (this FunMessage message, out LobbyLoginRequest value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_login_req, out value);
        }

        public static void AppendExtension_lobby_login_req (this FunMessage message, LobbyLoginRequest value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_login_req, value);
        }

        public static bool TryGetExtension_lobby_match_repl (this FunMessage message, out LobbyMatchReply value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_match_repl, out value);
        }

        public static void AppendExtension_lobby_match_repl (this FunMessage message, LobbyMatchReply value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_match_repl, value);
        }

        public static bool TryGetExtension_lobby_match_req (this FunMessage message, out LobbyMatchRequest value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_match_req, out value);
        }

        public static void AppendExtension_lobby_match_req (this FunMessage message, LobbyMatchRequest value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_match_req, value);
        }

        public static bool TryGetExtension_lobby_rank_list_repl (this FunMessage message, out LobbyRankListReply value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_rank_list_repl, out value);
        }

        public static void AppendExtension_lobby_rank_list_repl (this FunMessage message, LobbyRankListReply value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_rank_list_repl, value);
        }

        public static bool TryGetExtension_lobby_rank_list_req (this FunMessage message, out LobbyRankListRequest value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_rank_list_req, out value);
        }

        public static void AppendExtension_lobby_rank_list_req (this FunMessage message, LobbyRankListRequest value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_rank_list_req, value);
        }

        public static bool TryGetExtension_lobby_single_result (this FunMessage message, out LobbySingleModeResultMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_single_result, out value);
        }

        public static void AppendExtension_lobby_single_result (this FunMessage message, LobbySingleModeResultMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_lobby_single_result, value);
        }

        public static bool TryGetExtension_pbuf_another (this FunMessage message, out PbufAnotherMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_pbuf_another, out value);
        }

        public static void AppendExtension_pbuf_another (this FunMessage message, PbufAnotherMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_pbuf_another, value);
        }

        public static bool TryGetExtension_pbuf_echo (this FunMessage message, out PbufEchoMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_pbuf_echo, out value);
        }

        public static void AppendExtension_pbuf_echo (this FunMessage message, PbufEchoMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_pbuf_echo, value);
        }

        public static bool TryGetExtension_pong_error (this FunMessage message, out PongErrorMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_pong_error, out value);
        }

        public static void AppendExtension_pong_error (this FunMessage message, PongErrorMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_pong_error, value);
        }

        public static bool TryGetExtension_pbuf_hello (this FunMulticastMessage message, out PbufHelloMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMulticastMessage_pbuf_hello, out value);
        }

        public static void AppendExtension_pbuf_hello (this FunMulticastMessage message, PbufHelloMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMulticastMessage_pbuf_hello, value);
        }

        public static bool TryGetExtension_echo_rpc (this FunRpcMessage message, out EchoRpcMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunRpcMessage_echo_rpc, out value);
        }

        public static void AppendExtension_echo_rpc (this FunRpcMessage message, EchoRpcMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunRpcMessage_echo_rpc, value);
        }

    }
}


namespace funapi
{
    public enum ExtendedMessageFieldNumber
    {
        FunDedicatedServerRpcMessage_ds_rpc_sys = 8,
        FunMessage_multicast = 8,
        FunMulticastMessage_chat = 10,
        FunRpcMessage_error = 8,
        FunRpcMessage_info = 9,
    }


    public static class ProtobufHelper
    {
        public static bool TryGetExtension_ds_rpc_sys (this FunDedicatedServerRpcMessage message, out FunDedicatedServerRpcSystemMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunDedicatedServerRpcMessage_ds_rpc_sys, out value);
        }

        public static void AppendExtension_ds_rpc_sys (this FunDedicatedServerRpcMessage message, FunDedicatedServerRpcSystemMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunDedicatedServerRpcMessage_ds_rpc_sys, value);
        }

        public static bool TryGetExtension_multicast (this FunMessage message, out FunMulticastMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMessage_multicast, out value);
        }

        public static void AppendExtension_multicast (this FunMessage message, FunMulticastMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMessage_multicast, value);
        }

        public static bool TryGetExtension_chat (this FunMulticastMessage message, out FunChatMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunMulticastMessage_chat, out value);
        }

        public static void AppendExtension_chat (this FunMulticastMessage message, FunChatMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunMulticastMessage_chat, value);
        }

        public static bool TryGetExtension_error (this FunRpcMessage message, out FunRpcErrorMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunRpcMessage_error, out value);
        }

        public static void AppendExtension_error (this FunRpcMessage message, FunRpcErrorMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunRpcMessage_error, value);
        }

        public static bool TryGetExtension_info (this FunRpcMessage message, out FunRpcPeerInfoMessage value)
        {
            return ProtoBuf.Extensible.TryGetValue (message, (int)ExtendedMessageFieldNumber.FunRpcMessage_info, out value);
        }

        public static void AppendExtension_info (this FunRpcMessage message, FunRpcPeerInfoMessage value)
        {
            ProtoBuf.Extensible.AppendValue (message, (int)ExtendedMessageFieldNumber.FunRpcMessage_info, value);
        }

    }
}