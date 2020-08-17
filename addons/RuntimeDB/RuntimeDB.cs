#if TOOLS
using Godot;

namespace Plugin 
{
    [Tool]
    public class RuntimeDB : EditorPlugin
    {
        public Tree dock;
        public static RuntimeDB Instance {get; private set;}

        public override void _EnterTree()
        {
            Instance = this;
            dock = (Tree)GD.Load<PackedScene>("addons/RuntimeDB/RuntimeDB.tscn").Instance();
            AddControlToDock(DockSlot.LeftBl, dock);
        }

        public override void _ExitTree()
        {
            RemoveControlFromDocks(dock);
            dock.Free();
        }
    }
}

#endif