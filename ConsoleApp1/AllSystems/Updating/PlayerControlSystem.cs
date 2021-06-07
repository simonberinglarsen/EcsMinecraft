namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using ConsoleApp1.AllSystems;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public class PlayerControlSystem
    {
        private const float MouseSpeed = 1f;
        private readonly Vector3 up = new Vector3(0, 1, 0);
        private float speed;
        private bool forwardKey;
        private bool backwardKey;
        private bool leftKey;
        private bool rightKey;
        private bool upKey;
        private bool downKey;
        private float deltaYaw;
        private float deltaPitch;
        private Vector3 forward;
        private Vector3 left;

        public void Run(SettingsComponent settings, InputComponent input, PlayerComponent player, CollidableComponent collidable, PhysicsComponent physics)
        {
            if (settings.GameMode != GameMode.InGame)
            {
                forwardKey = false;
                backwardKey = false;
                leftKey = false;
                rightKey = false;
                upKey = false;
                downKey = false;
                deltaYaw = 0;
                deltaPitch = 0;
                return;
            }

            HandleInputs(input);
            UpdateDirections(player);
            UpdateForces(player, collidable, physics);
        }

        private void UpdateForces(PlayerComponent player, CollidableComponent collidable, PhysicsComponent physics)
        {
            Vector3 force = Vector3.Zero;
            speed = 0.2f;
            player.IsRunningPrev = player.IsRunning;
            if (downKey)
            {
                speed *= 2f;
                player.IsRunning = true;
            }
            else
            {
                player.IsRunning = false;
            }

            if (forwardKey)
            {
                force += speed * forward;
            }

            if (backwardKey)
            {
                force -= speed * forward;
            }

            if (leftKey)
            {
                force += speed * left;
            }

            if (rightKey)
            {
                force -= speed * left;
            }

            if (upKey)
            {
                if (collidable.OnGround)
                {
                    physics.Velocity += 0.75f * up;
                    collidable.OnGround = false;
                }
            }

            physics.Force = force;
        }

        private void UpdateDirections(PlayerComponent player)
        {
            forward = new Vector3(player.Direction.X, 0, player.Direction.Z);
            if (forward == Vector3.Zero)
            {
                forward = new Vector3(1, 0, 0);
            }

            forward.Normalize();
            player.Direction = Vector3.Transform(forward, Quaternion.FromAxisAngle(up, deltaYaw * MouseSpeed));
            left = new Vector3(player.Direction.Z, 0, -player.Direction.X);
            left.Normalize();
            const float upperLimit = (float)((Math.PI / 2f) - 0.01f);
            const float lowerLimit = (float)((-Math.PI / 2f) + 0.01f);
            player.Pitch += deltaPitch * MouseSpeed;
            if (player.Pitch > upperLimit)
            {
                player.Pitch = upperLimit;
            }
            else if (player.Pitch < lowerLimit)
            {
                player.Pitch = lowerLimit;
            }

            player.Direction = Vector3.Transform(player.Direction, Quaternion.FromAxisAngle(left, player.Pitch));
        }

        private void HandleInputs(InputComponent input)
        {
            Vector2 mouse = input.MouseState.Position - input.MouseStatePrev.Position;
            var keyboardState = input.KeyboardState;
            forwardKey = keyboardState.IsKeyDown(Keys.W);
            backwardKey = keyboardState.IsKeyDown(Keys.S);
            leftKey = keyboardState.IsKeyDown(Keys.A);
            rightKey = keyboardState.IsKeyDown(Keys.D);
            upKey = keyboardState.IsKeyDown(Keys.Space);
            downKey = keyboardState.IsKeyDown(Keys.LeftShift);
            deltaYaw = -mouse.X / Game.Instance.Size.X;
            deltaPitch = mouse.Y / Game.Instance.Size.X;
        }
    }
}
