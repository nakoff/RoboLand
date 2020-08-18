#if TOOLS
using Godot;

namespace Plugin 
{
    [Tool]
    public class LvlEditor : EditorPlugin
    {
        private Control dock;

        public override void _EnterTree()
        {
            dock = (Control)GD.Load<PackedScene>("addons/LvlEditor/LvlEditor.tscn").Instance();
            AddControlToBottomPanel(dock, "Level Editor");
        }

        public override void _ExitTree()
        {
            RemoveControlFromDocks(dock);
            dock.Free();
        }
    }
}

#endif