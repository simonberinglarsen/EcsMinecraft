namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class CollidablePhysicsSystem
    {
        private const float PhysicsLimit = 5 * Chunk.ChunkSize;
        private const float PhysicsLimitSquared = PhysicsLimit * PhysicsLimit;

        private LandscapeService landscapeService = new LandscapeService();

        public void Run(PhysicsComponent playerPhysics, CollidableComponent collidable, PhysicsComponent physics, LandscapeComponent landscape)
        {
            physics.PositionPrev = physics.Position;
            var skipUpdate = (playerPhysics.Position - physics.Position).LengthSquared > PhysicsLimitSquared;
            if (skipUpdate)
            {
                return;
            }

            var force = physics.Force;
            physics.Velocity += physics.Gravity;
            force += physics.Velocity;
            Collision(landscape, physics, collidable, force);
            if (physics.Position.Y < 0)
            {
                var entity = physics.Id;
                EntityDatabase.MarkForDeletion(entity);
            }

            if (Math.Abs(physics.Velocity.Length) > 50)
            {
                throw new Exception("Velocity waaay to large!");
            }
        }

        public void Collision(LandscapeComponent landscape, PhysicsComponent physics, CollidableComponent collidable, Vector3 force)
        {
            landscapeService.Bind(landscape);
            const float Epsilon = 0.01f;
            var epsilon = new Vector3(Epsilon, Epsilon, Epsilon);
            Vector3 oldPos = physics.Position;
            var me = physics.AABB;
            physics.Position += force;
            var futureMove = physics.AABB;
            physics.Position = oldPos;
            futureMove.Expand(me);
            var landscapeBoxes = landscapeService.GetSurroundingBoundingBoxes(futureMove.Min, futureMove.Max);
            MoveComponent(new Vector3(1, 0, 0));
            var yCollision = MoveComponent(new Vector3(0, 1, 0));
            MoveComponent(new Vector3(0, 0, 1));
            if (yCollision)
            {
                if (physics.Velocity.Y < 0)
                {
                    collidable.OnGround = true;
                }

                physics.Velocity = physics.Velocity * new Vector3(0.75f, 0, 0.75f);
            }

            me = physics.AABB;
            landscapeBoxes = landscapeService.GetSurroundingBoundingBoxes(me.Min, me.Max);
            foreach (var him in landscapeBoxes)
            {
                if (me.Intersect(him))
                {
                    physics.Position = oldPos;
                    break;
                }
            }

            landscapeService.Unbind();

            bool MoveComponent(Vector3 axis)
            {
                const float MaxStepSize = 0.15f;
                var fullStep = Vector3.Dot(axis, force);
                var sign = Math.Sign(fullStep);
                var length = Math.Abs(fullStep);

                while (true)
                {
                    var step = length > MaxStepSize ? MaxStepSize : length;
                    bool collided = false;
                    physics.Position += axis * step * sign;
                    foreach (var him in landscapeBoxes)
                    {
                        var meQ = physics.AABB;
                        if (!meQ.Intersect(him))
                        {
                            continue;
                        }

                        collided = true;
                        var response = sign < 0 ? him.Max - meQ.Min + epsilon : him.Min - meQ.Max - epsilon;
                        physics.Position += Vector3.Multiply(axis, response);
                    }

                    length -= step;
                    if (length <= (0f + Epsilon) || collided)
                    {
                        return collided;
                    }
                }
            }
        }
    }
}