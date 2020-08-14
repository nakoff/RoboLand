using Godot;
using System;
using System.Collections.Generic;

namespace Core
{
    public static class ResponseMap
    {
        private static Dictionary<NET_CMD, BaseHandler> handlers = new Dictionary<NET_CMD, BaseHandler>();
        
        public static (bool, string) SetHandler(NET_CMD cmd, BaseHandler handler){
            if (handlers.ContainsKey(cmd)){
                return (false, "handler already exists");
            }

            handlers.Add(cmd, handler);
            return (true, "");
        }

        public static BaseHandler GetHandler(NET_CMD cmd){
            if (handlers.ContainsKey(cmd)){
                return handlers[cmd];
            }
            return null;
        }
    }
}

