namespace ConsoleApp1.AllSystems
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public static class EntityDatabase
    {
        private static List<IDictionary> entityDictionaries = new List<IDictionary>();

        static EntityDatabase()
        {
            foreach (var p in typeof(EntityDatabase).GetProperties())
            {
                var t = p.PropertyType;
                bool isEntityDictionary = t.IsGenericType
                    && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                    && t.GetGenericArguments()[0] == typeof(Entity);
                if (isEntityDictionary)
                {
                    entityDictionaries.Add(p.GetValue(null) as IDictionary);
                }
            }
        }

        public static List<Entity> MarkedForDeletion { get; set; } = new List<Entity>();

        public static Dictionary<Entity, CrosshairComponent> CrosshairComponents { get; set; } = new Dictionary<Entity, CrosshairComponent>();

        public static Dictionary<Entity, ScreenComponent> ScreenComponents { get; set; } = new Dictionary<Entity, ScreenComponent>();

        public static Dictionary<Entity, ScreenComponent> ScreenOverlayComponents { get; set; } = new Dictionary<Entity, ScreenComponent>();

        public static Dictionary<Entity, ScreenRenderComponent> ScreenRenderComponents { get; set; } = new Dictionary<Entity, ScreenRenderComponent>();

        public static Dictionary<Entity, ScreenRenderComponent> ScreenOverlayRenderComponents { get; set; } = new Dictionary<Entity, ScreenRenderComponent>();

        public static Dictionary<Entity, InputTagComponent> InputTagComponents { get; set; } = new Dictionary<Entity, InputTagComponent>();

        public static Dictionary<Entity, DebugTagComponent> DebugTagComponents { get; set; } = new Dictionary<Entity, DebugTagComponent>();

        public static Dictionary<Entity, InfoComponent> InfoComponents { get; set; } = new Dictionary<Entity, InfoComponent>();

        public static Dictionary<Entity, HealthComponent> HealthComponents { get; set; } = new Dictionary<Entity, HealthComponent>();

        public static Dictionary<Entity, InventoryComponent> InventoryComponents { get; set; } = new Dictionary<Entity, InventoryComponent>();

        public static Dictionary<Entity, CameraTagComponent> CameraTagComponents { get; set; } = new Dictionary<Entity, CameraTagComponent>();

        public static Dictionary<Entity, PlayerComponent> PlayerComponents { get; set; } = new Dictionary<Entity, PlayerComponent>();

        public static Dictionary<Entity, CollidableComponent> CollidableComponents { get; set; } = new Dictionary<Entity, CollidableComponent>();

        public static Dictionary<Entity, PhysicsComponent> PhysicsComponents { get; set; } = new Dictionary<Entity, PhysicsComponent>();

        public static Dictionary<Entity, ItemComponent> ItemComponents { get; set; } = new Dictionary<Entity, ItemComponent>();

        public static Dictionary<Entity, ItemRenderComponent> ItemRenderComponents { get; set; } = new Dictionary<Entity, ItemRenderComponent>();

        public static InputComponent InputComponent { get; set; } = new InputComponent();

        public static CameraComponent CameraComponent { get; set; } = new CameraComponent();

        public static SettingsComponent SettingsComponent { get; } = new SettingsComponent();

        public static LandscapeComponent LandscapeComponent { get; } = new LandscapeComponent();

        public static LandscapeRendererComponent LandscapeRendererComponent { get; } = new LandscapeRendererComponent();

        public static WindowComponent MainWindow { get; } = new WindowComponent();

        public static PerformanceComponent PerformanceComponent { get; } = new PerformanceComponent();

        public static void MarkForDeletion(Entity entity)
        {
            MarkedForDeletion.Add(entity);
        }

        public static void CleanUp()
        {
            MarkedForDeletion.ForEach(e =>
            {
                foreach (var dictionary in entityDictionaries)
                {
                    if (dictionary.Contains(e))
                    {
                        var val = dictionary[e];
                        (val as IDisposable)?.Dispose();
                        dictionary.Remove(e);
                    }
                }
            });
            MarkedForDeletion.Clear();
        }

        public static void CreatePlayer()
        {
            var entity = new Entity("player");
            var health = new HealthComponent() { Id = entity };
            var inventory = new InventoryComponent()
            {
                Id = entity,
                SelectedSlot = 0,
            };
            inventory.Slots[0] = new ItemStack { BlockId = BlockId.Void, Count = 0, };
            inventory.Slots[1] = new ItemStack { BlockId = BlockId.Torch, Count = 4, };
            inventory.Slots[2] = new ItemStack { BlockId = BlockId.Planks, Count = 999, };
            inventory.Slots[3] = new ItemStack { BlockId = BlockId.PC, Count = 6, };
            inventory.Slots[4] = new ItemStack { BlockId = BlockId.Stone, Count = 7, };
            inventory.Slots[5] = new ItemStack { BlockId = BlockId.Sand, Count = 8, };
            inventory.Slots[6] = new ItemStack { BlockId = BlockId.Glass, Count = 9, };
            inventory.Slots[7] = new ItemStack { BlockId = BlockId.Leaves, Count = 10, };

            var hud = new ScreenComponent(24, 5)
            {
                Id = entity,
                BackgroundColor = Color.DarkGrey | Color.Inverted,
                Anchoring = Anchor.Bottom,
            };
            var countScreen = new ScreenComponent(24, 5)
            {
                Id = entity,
                Anchoring = Anchor.Bottom,
                IsOverlay = true,
            };
            var playerComponent = new PlayerComponent
            {
                Id = entity,
                Pitch = .5f,
                Direction = new Vector3(0, 0, -1),
                EyePosition = new Vector3(0, 1.6f, 0),
            };
            var input = new InputTagComponent { Id = entity };
            var camera = new CameraTagComponent { Id = entity };
            var collidableComponent = new CollidableComponent { Id = entity, CollideWithLandscape = true };
            PhysicsComponents[entity] = new PhysicsComponent
            {
                Id = entity,
                Position = new Vector3(70, 90, 95),
                Width = 0.8f,
                Height = 1.8f,
            };
            ScreenComponents[entity] = hud;
            ScreenOverlayComponents[entity] = countScreen;
            ScreenRenderComponents[entity] = new ScreenRenderComponent { Id = entity };
            ScreenOverlayRenderComponents[entity] = new ScreenRenderComponent { Id = entity };
            HealthComponents[entity] = health;
            InventoryComponents[entity] = inventory;
            InputTagComponents[entity] = input;
            CameraTagComponents[entity] = camera;
            PlayerComponents[entity] = playerComponent;
            CollidableComponents[entity] = collidableComponent;
        }

        public static Entity QueryPlayerEntity()
        {
            return PlayerComponents.First().Key;
        }

        public static Entity CreateItemAt(Vector3 pos, ushort blockId)
        {
            var entity = new Entity("item");
            var collidableComponent = new CollidableComponent { Id = entity, CollideWithLandscape = true };
            var rnd = new Random();
            var v = 0.001f * new Vector3(
                rnd.Next(-100, 100),
                rnd.Next(100, 400),
                rnd.Next(-100, 100));
            ItemComponents[entity] = new ItemComponent
            {
                Id = entity,
                BlockId = blockId,
                Born = EntityDatabase.SettingsComponent.GameTick,
                Random = rnd.Next(1000),
            };
            PhysicsComponents[entity] = new PhysicsComponent
            {
                Id = entity,
                Position = pos + new Vector3(0.5f, 0.5f, 0.5f),
                Width = 0.4f,
                Height = 0.4f,
                Velocity = v,
                Gravity = new Vector3(0, -0.075f, 0),
            };
            ItemRenderComponents[entity] = new ItemRenderComponent
            {
                Id = entity,
            };
            CollidableComponents[entity] = collidableComponent;
            return entity;
        }

        public static void CreateUI()
        {
            CreateCrossHair();
            CreateInfoUI();
            CreateDebug();
            CreateMainMenuWindow();
        }

        private static void CreateMainMenuWindow()
        {
            var entity = new Entity("mainwindow");
            var screen = new ScreenComponent(30, 15)
            {
                Id = entity,
                Anchoring = Anchor.Center,
                BackgroundColor = Color.LightPeach | Color.Inverted,
            };
            MainWindow.Id = entity;
            ScreenComponents[entity] = screen;
            ScreenRenderComponents[entity] = new ScreenRenderComponent { Id = entity };
        }

        private static void CreateInfoUI()
        {
            var entity = new Entity("info");
            var screen = new ScreenComponent(15, 4)
            {
                Id = entity,
                Anchoring = Anchor.TopRight,
            };
            var info = new InfoComponent()
            {
                Id = entity,
                Text = DateTime.Now.ToString(),
            };
            InfoComponents[entity] = info;
            ScreenComponents[entity] = screen;
            ScreenRenderComponents[entity] = new ScreenRenderComponent { Id = entity };
        }

        private static void CreateCrossHair()
        {
            var entity = new Entity("crosshair");
            var screen = new ScreenComponent(1, 1)
            {
                Id = entity,
                Anchoring = Anchor.Center,
            };
            var crosshair = new CrosshairComponent()
            {
                Id = entity,
            };
            CrosshairComponents[entity] = crosshair;
            ScreenComponents[entity] = screen;
            ScreenRenderComponents[entity] = new ScreenRenderComponent { Id = entity };
        }

        private static void CreateDebug()
        {
            var entity = new Entity("debug");
            var screen = new ScreenComponent(30, 15)
            {
                Id = entity,
                Anchoring = Anchor.TopLeft,
            };
            var debug = new DebugTagComponent() { Id = entity };
            ScreenComponents[entity] = screen;
            ScreenRenderComponents[entity] = new ScreenRenderComponent { Id = entity };
            DebugTagComponents[entity] = debug;
        }
    }
}