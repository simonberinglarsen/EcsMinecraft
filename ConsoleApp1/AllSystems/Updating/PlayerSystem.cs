namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public class PlayerSystem
    {
        private static readonly Dictionary<Face, Vector3i> NeighborMap = new Dictionary<Face, Vector3i>
        {
            { Face.Up, new Vector3i(0, 1, 0) },
            { Face.Down, new Vector3i(0, -1, 0) },
            { Face.East, new Vector3i(1, 0, 0) },
            { Face.West, new Vector3i(-1, 0, 0) },
            { Face.North, new Vector3i(0, 0, -1) },
            { Face.South, new Vector3i(0, 0, 1) },
        };

        private int coolDown = 0;
        private LandscapeService ws = new LandscapeService();
        private bool leftClick;
        private bool rightClick;
        private PlayerComponent player;
        private PhysicsComponent physics;
        private InventoryComponent inventory;
        private RayAlgorithm rayAlgorithm = new RayAlgorithm();

        public void Run(
            SettingsComponent settings,
            InputComponent input,
            PlayerComponent player,
            PhysicsComponent physics,
            InventoryComponent inventory,
            LandscapeComponent landscape)
        {
            if (settings.GameMode != GameMode.InGame)
            {
                return;
            }

            if (coolDown-- < 0)
            {
                coolDown = 0;
            }

            var mouseStatePrev = input.MouseStatePrev;
            var mouseState = input.MouseState;
            var keyboardState = input.KeyboardState;
            leftClick = mouseState.IsButtonDown(MouseButton.Left) && coolDown <= 0;
            rightClick = mouseStatePrev.IsButtonDown(MouseButton.Right) && coolDown <= 0;
            var throwItem = keyboardState.IsKeyDown(Keys.Q);
            BindAll(landscape, player, physics, inventory);
            if (throwItem)
            {
                var stack = inventory.SelectedItemStack;
                if (stack.Count > 0)
                {
                    var entity = EntityDatabase.CreateItemAt(physics.Position, stack.BlockId);
                    var itemPhysics = EntityDatabase.PhysicsComponents[entity];
                    itemPhysics.Velocity = player.Direction * 1f;
                    itemPhysics.Position = player.EyePosition + physics.Position;

                    stack.Count--;
                    if (stack.Count == 0)
                    {
                        stack.BlockId = BlockId.Void;
                    }

                    inventory.Dirty = true;
                }
            }

            var pos = physics.Position + player.EyePosition;
            player.SelectedVoxelPrev = player.SelectedVoxel;
            player.SelectedVoxel = null;
            rayAlgorithm.March(OnVisitVoxel, pos, pos + (player.Direction * 100f));
            UnbindAll();
        }

        private void BindAll(LandscapeComponent landscape, PlayerComponent player, PhysicsComponent physics, InventoryComponent inventory)
        {
            ws.Bind(landscape);
            this.player = player;
            this.physics = physics;
            this.inventory = inventory;
        }

        private void UnbindAll()
        {
            ws.Unbind();
            this.player = null;
            this.inventory = null;
        }

        private bool OnVisitVoxel(int x, int y, int z, Face entranceFace)
        {
            var pos = new Vector3i(x, y, z);
            ushort v = ws.GetGlobalVoxelAt(pos);
            bool playerInsideVoxel = entranceFace == Face.None;
            if (v == BlockId.Void || playerInsideVoxel)
            {
                return false;
            }

            player.SelectedVoxel = pos;
            if (leftClick)
            {
                DestroyBlockAt(pos, v);
                return true;
            }

            var itemStack = inventory.SelectedItemStack;
            if (itemStack.BlockId == BlockId.Void)
            {
                return true;
            }

            var blockToPlace = itemStack.BlockId;
            var blockToPlacePos = new Vector3i(x, y, z) + NeighborMap[entranceFace];
            var blockToPlaceAABB = BlockRepository.Get(blockToPlace).BoundingBox.MoveTo(blockToPlacePos);
            if (physics.AABB.Intersect(blockToPlaceAABB))
            {
                return true;
            }

            if (rightClick)
            {
                itemStack.Count--;
                if (itemStack.Count == 0)
                {
                    itemStack.BlockId = BlockId.Void;
                    inventory.Dirty = true;
                }

                ws.SetGlobalVoxelWithLightAt(blockToPlacePos, blockToPlace);
                coolDown = 2;
                return true;
            }

            return true;
        }

        private void DestroyBlockAt(Vector3i pos, ushort blockId)
        {
            ws.SetGlobalVoxelWithLightAt(pos, 0);
            EntityDatabase.CreateItemAt(pos, blockId);
            coolDown = 4;
        }
    }
}