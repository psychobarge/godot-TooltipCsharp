using Godot;
using System;
using psychobarge.godot;

public class main : Node2D
{
	TooltipCsharp tooltip;
	Button button;
	TextEdit textEdit;

	public override void _Ready()
	{
		tooltip = (TooltipCsharp)GetNode($"%TooltipCsharp");
		button = (Button)GetNode($"%Button");
		textEdit = (TextEdit)GetNode($"%TextEdit");

		button.Connect("pressed", this, "_on_Button_pressed");

		GetNode<Label>($"%Label2").AddChild(Utils.CreateTooltipCsharp((PackedScene)ResourceLoader.Load("res://example/TooltipScene.tscn"), GetNode<Label>($"%Label2").GetParent()));
	}

	private void _on_Button_pressed()
	{
		tooltip.Content(textEdit.Text);
	}
}
