namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class ItemPhysicsSystem
    {
        private const float PhysicsLimit = 5 * Chunk.ChunkSize;
        private const float PhysicsLimitSquared = PhysicsLimit * PhysicsLimit;
        private Dictionary<Vector3i, List<PhysicsComponent>> itemGrid = new Dictionary<Vector3i, List<PhysicsComponent>>();
        private List<PhysicsComponent> itemsNearPlayer = new List<PhysicsComponent>();
        private Vector3 playerPos;
        private Entity playerId;
        private LandscapeService ls = new LandscapeService();
        private Vector3i[] cubeOffsets;

        public ItemPhysicsSystem()
        {
            List<Vector3i> offsets = new List<Vector3i>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        offsets.Add(new Vector3i(x, y, z));
                    }
                }
            }

            cubeOffsets = offsets.ToArray();
        }

        public void Run1(PhysicsComponent playerPhysics, PhysicsComponent physics, ItemComponent item)
        {
            playerId = playerPhysics.Id;
            playerPos = playerPhysics.Position;
            var playerDistSquared = (playerPhysics.Position - physics.Position).LengthSquared;
            var skipUpdate = playerDistSquared > PhysicsLimitSquared;
            if (skipUpdate)
            {
                return;
            }

            if (playerDistSquared < 2.25f)
            {
                itemsNearPlayer.Add(physics);
            }

            var pos = ls.ToVector3i(physics.Position);
            if (!itemGrid.ContainsKey(pos))
            {
                itemGrid[pos] = new List<PhysicsComponent>(2);
            }

            physics.Force = Vector3.Zero;
            itemGrid[pos].Add(physics);
        }

        public void Run2()
        {
            foreach (var pair in itemGrid)
            {
                ApplyItem2ItemForce(pair.Key);
            }

            ApplyItem2PlayerForce();
            itemsNearPlayer.Clear();
            itemGrid.Clear();
        }

        private void ApplyItem2PlayerForce()
        {
            if (playerId == null)
            {
                return;
            }

            var inventory = EntityDatabase.InventoryComponents[playerId];

            foreach (var p1 in itemsNearPlayer)
            {
                var i1 = EntityDatabase.ItemComponents[p1.Id];
                var availableSlot = inventory.Slots.FirstOrDefault(s =>
                    (s.BlockId == i1.BlockId && i1.Count < 64)
                    || s.BlockId == BlockId.Void);
                var canBePickedUp = availableSlot != null;
                if (!canBePickedUp)
                {
                    continue;
                }

                Vector3 delta = playerPos - p1.Position;
                float length = delta.Length;
                if (length <= 0.50f)
                {
                    // pick up
                    while (availableSlot != null)
                    {
                        while (availableSlot.Count < 64 && i1.Count > 0)
                        {
                            availableSlot.BlockId = i1.BlockId;
                            availableSlot.Count++;
                            i1.Count--;
                        }

                        if (i1.Count == 0)
                        {
                            break;
                        }

                        availableSlot = inventory.Slots.FirstOrDefault(s =>
                            (s.BlockId == i1.BlockId && s.Count < 64)
                            || s.BlockId == BlockId.Void);
                    }

                    inventory.Dirty = true;
                    EntityDatabase.MarkForDeletion(p1.Id);
                    continue;
                }

                if (length <= 1f)
                {
                    p1.Position = playerPos;
                    p1.Force = Vector3.Zero;
                    continue;
                }

                delta.Normalize();
                var force = 0.1f * delta;
                p1.Force += force;
            }
        }

        private void ApplyItem2ItemForce(Vector3i basePos)
        {
            var items = new List<PhysicsComponent>();
            foreach (var pos in cubeOffsets.Select(offset => offset + basePos))
            {
                if (itemGrid.ContainsKey(pos))
                {
                    items.AddRange(itemGrid[pos]);
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                for (int j = i + 1; j < items.Count; j++)
                {
                    var p1 = items[i];
                    var p2 = items[j];
                    var i1 = EntityDatabase.ItemComponents[p1.Id];
                    var i2 = EntityDatabase.ItemComponents[p2.Id];
                    Vector3 delta = p2.Position - p1.Position;
                    float length = delta.Length;
                    bool sameItem = i1.BlockId == i2.BlockId;
                    if (!sameItem && length > 1f)
                    {
                        continue;
                    }

                    if (length < 0.001f)
                    {
                        length = 0.1f;
                        delta = new Vector3(1, 1, 1);
                    }

                    if (sameItem && length < 0.15f)
                    {
                        EntityDatabase.MarkForDeletion(p2.Id);
                        i1.Count += i2.Count;
                        i2.Count = 0;
                        continue;
                    }

                    delta.Normalize();
                    var force = 0.01f * delta;
                    force = sameItem ? force : -force;
                    p1.Force += force;
                    p2.Force -= force;
                }
            }
        }
    }
}