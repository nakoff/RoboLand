using System.Collections.Generic;

namespace Core
{
    public static class HandlerManager
    {
        private static Dictionary<NET_CMD, BaseHandler> handlers = new Dictionary<NET_CMD, BaseHandler>();
        
        public static string AddHandler(NET_CMD cmd, BaseHandler handler){
            if (handlers.ContainsKey(cmd)){
                return "handler already exists";
            }

            handlers.Add(cmd, handler);
            return null;
        }

        public static BaseHandler GetHandler(NET_CMD cmd){
            if (handlers.ContainsKey(cmd)){
                return handlers[cmd];
            }
            return null;
        }
    }
}

