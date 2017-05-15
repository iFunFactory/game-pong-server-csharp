using System;
using System.Collections.Generic;
using funapi;


namespace Pongcs
{
    public enum ExtendedMessageFieldNumber
    {
        FunMessage_pbuf_another = 17,
        FunMessage_pbuf_echo = 16,
        FunMulticastMessage_pbuf_hello = 9,
        FunRpcMessage_echo_rpc = 32,
    }


    public static class ProtobufHelper
    {
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
        FunMessage_multicast = 8,
        FunMulticastMessage_chat = 8,
        FunRpcMessage_error = 8,
        FunRpcMessage_info = 9,
    }


    public static class ProtobufHelper
    {
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