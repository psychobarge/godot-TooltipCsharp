using Godot;
using System;
using Addons.Nodes;

public class main : Node2D
{
	TooltipCsharp tooltip;
	Button button;
	TextEdit textEdit;
	
	public override void _Ready()
	{	
		tooltip = (TooltipCsharp) GetNode($"%TooltipCsharp");
		button = (Button) GetNode($"%Button");
		textEdit = (TextEdit) GetNode($"%TextEdit");
		
		button.Connect("pressed", this, "_on_Button_pressed");
	}
	
	private void _on_Button_pressed()
	{
		tooltip.Content(textEdit.Text);
	}
}
