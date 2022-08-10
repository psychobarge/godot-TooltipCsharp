using Godot;
using System;
using Addons.Nodes;

// the scene used as tooltip MUST be of type "Control" 
// and implement the ITooltip interface.
// In this example, the content added is a simple text on a label
// Note that i used the "Access as Scene Unique Name" feature of Godot 3.5
public class TooltipScene : Control, ITooltip
{		
	public void SetContent(System.Object content)
	{
		GetNode<Label>($"%Label").Text = (string)content;
	}
}
