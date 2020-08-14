using SimpleJSON;

namespace Core
{
    public abstract class BaseHandler
    {
        public abstract string Run(JSONNode responseData);
    }
}

