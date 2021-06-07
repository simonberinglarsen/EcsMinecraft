namespace ConsoleApp1.AllSystems.Updating
{
    using ConsoleApp1.Components;
    using OpenTK.Mathematics;

    public class CameraSystem
    {
        public CameraSystem()
        {
        }

        public void Run(CameraComponent cameraComponent, PlayerComponent gameObject, PhysicsComponent physics)
        {
            cameraComponent.DirectionPrev = cameraComponent.Direction;
            cameraComponent.PositionPrev = cameraComponent.Position;
            cameraComponent.Direction = gameObject.Direction;
            cameraComponent.Position = physics.Position + gameObject.EyePosition - (gameObject.Direction * 10f);
            cameraComponent.Position = physics.Position + gameObject.EyePosition;
            cameraComponent.Pitch = gameObject.Pitch;
        }
    }
}
