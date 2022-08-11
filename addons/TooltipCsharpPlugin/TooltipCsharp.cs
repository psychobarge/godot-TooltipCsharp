using Godot;
using System;
namespace psychobarge.godot
{
	[Tool]
	public class TooltipCsharp : Node
	{
		[Export]
		public PackedScene visuals_res = null;

		[Export]
		public NodePath owner_path = null;

		[Export(PropertyHint.Range, "0, 10, 0.05")]
		public float delay;

		[Export]
		public bool follow_mouse = true;

		[Export(PropertyHint.Range, "0, 100, 1")]
		public float offset_x = 0;

		[Export(PropertyHint.Range, "0, 100, 1")]
		public float offset_y = 0;

		[Export(PropertyHint.Range, "0, 100, 1")]
		public float padding_x = 0;

		[Export(PropertyHint.Range, "0, 100, 1")]
		public float padding_y = 0;

		private Timer _timer;
		private Control _visuals;
		private Node owner_node;
		private Vector2 extents;
				
		public override void _Ready()
		{
			if(owner_path != null){
				delay = 0.5f;
				owner_node = (Node)GetNode(owner_path);

				// create the visuals
				_visuals = (Control)visuals_res.Instance();
				AddChild(_visuals);

				// calculate the extents
				extents = _visuals.RectSize;

				// connect signals
				owner_node.Connect("mouse_entered", this, "_mouse_entered");
				owner_node.Connect("mouse_exited", this, "_mouse_exited");

				// initialize the timer
				_timer = new Timer();
				AddChild(_timer);

				_timer.Connect("timeout", this, "_custom_show");
				// default to hide
				_visuals.Hide();
			}
		}
		
		public override string _GetConfigurationWarning()
		{
			var warning = "";
			if(visuals_res == null)
			{
				warning += "Requires 'Visual Res' to be set. The root of this scene MUST be of type Control.";
			}
			if(owner_path == null)
			{
				warning += "Requires 'Owner Path' to be set.";
			}
			return warning;
		}

		public override void _Process(float delta)
		{
			if (_visuals != null && _visuals.Visible)
			{
				var border = GetViewport().Size - new Vector2(padding_x, padding_y);
				extents = _visuals.RectSize;
				var base_pos = GetScreenPos();
				// test if need to display to the left
				var final_x = base_pos.x + offset_x;
				if (final_x + extents.x > border.x)
				{
					final_x = base_pos.x - offset_x - extents.x;
				}
				// test if need to display below
				var final_y = base_pos.y - extents.y - offset_y;
				if (final_y < padding_y)
				{
					final_y = base_pos.y + offset_y;
				}
				_visuals.RectPosition = new Vector2(final_x, final_y);
			}
		}

		private void _mouse_entered()
		{
			_timer.Start(delay);
		}

		private void _mouse_exited()
		{
			_timer.Stop();
			_visuals.Hide();
		}

		private void _custom_show()
		{
			_timer.Stop();
			_visuals.Show();
		}

		private Vector2 GetScreenPos()
		{
			if (follow_mouse)
			{
				return GetViewport().GetMousePosition();
			}
			var position = new Vector2();
			if (owner_node is Node2D)
			{
				position = ((Node2D)owner_node).GetGlobalTransformWithCanvas().origin;
			}
			else if (owner_node is Spatial)
			{
				position = GetViewport().GetCamera().UnprojectPosition(new Vector3(((Spatial)owner_node).GlobalTransform.origin.x, ((Spatial)owner_node).GlobalTransform.origin.y, 0));
			}

			else if (owner_node is Control)
			{
				position = ((Control)owner_node).RectPosition;
			}

			return position;
		}
		
		public void Content(System.Object content)
		{
			((ITooltip)_visuals).SetContent(content);
		}
	}
}
