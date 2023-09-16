using Godot;
using System;

namespace psychobarge.godot
{
    [Tool]
    public partial class TooltipCsharp : Node
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
        public float offset_x = 5;

        [Export(PropertyHint.Range, "0, 100, 1")]
        public float offset_y = 5;

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
            if (owner_path != null)
            {
                delay = 0.5f;
                owner_node = (Node)GetNode(owner_path);

                // create the visuals
                _visuals = (Control)visuals_res.Instantiate();
                AddChild(_visuals);

                // calculate the extents
                extents = _visuals.Size;

                // connect signals
                owner_node.Connect("mouse_entered", new Callable(this, "_mouse_entered"));
                owner_node.Connect("mouse_exited", new Callable(this, "_mouse_exited"));

                // initialize the timer
                _timer = new Timer();
                AddChild(_timer);

                _timer.Connect("timeout", new Callable(this, "_custom_show"));
                // default to hide
                _visuals.Hide();
            }
        }

        public override string[] _GetConfigurationWarnings()
        {
            var warning = "";
            if (visuals_res == null)
            {
                warning +=
                    "Requires 'Visual Res' to be set. The root of this scene MUST be of type Control.";
            }
            if (owner_path == null)
            {
                warning += "Requires 'Owner Path3D' to be set.";
            }
            return new string[] { warning };
        }

        public override void _Process(double delta)
        {
            if (_visuals != null && _visuals.Visible)
            {
                Rect2 visibleRect = GetViewport().GetVisibleRect();

                var border = visibleRect.Size - new Vector2(padding_x, padding_y);
                extents = _visuals.Size;
                var base_pos = GetScreenPos();
                // test if need to display to the left
                var final_x = base_pos.X + offset_x;
                if (final_x + extents.X > border.X)
                {
                    final_x = base_pos.X - offset_x - extents.X;
                }
                // test if need to display below
                var final_y = base_pos.Y - extents.Y - offset_y;
                if (final_y < padding_y)
                {
                    final_y = base_pos.Y + offset_y;
                }
                _visuals.Position = new Vector2(final_x, final_y);
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
                position = ((Node2D)owner_node).GetGlobalTransformWithCanvas().Origin;
            }
            else if (owner_node is Node3D)
            {
                position = GetViewport()
                    .GetCamera3D()
                    .UnprojectPosition(
                        new Vector3(
                            ((Node3D)owner_node).GlobalTransform.Origin.X,
                            ((Node3D)owner_node).GlobalTransform.Origin.Y,
                            0
                        )
                    );
            }
            else if (owner_node is Control)
            {
                position = ((Control)owner_node).Position;
            }

            return position;
        }

        public void Content(System.Object content)
        {
            ((ITooltip)_visuals).SetContent(content);
        }
    }
}
