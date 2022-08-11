using Godot;
using System;
namespace psychobarge.godot
{
    public static class Utils
    {
        public static TooltipCsharp CreateTooltipCsharp(PackedScene visualScene, Node parent)
        {
            TooltipCsharp tooltip = new TooltipCsharp();
            tooltip.visuals_res = visualScene;
            tooltip.owner_path = parent.GetPath();
            return tooltip;
        }
    }
}
