using Godot;

namespace Debug
{
    public static class DbgUtilites 
    {
        public static void Log(params object[] what){
            GD.Print(what);
        }

        public static void LogErr(params object[] what){
            GD.PrintErr(what);
        }

        //TODO
        public static void LogSucc(params object[] what){
            GD.Print(what);
        }
    }
}
