namespace ConsoleApp1.AllSystems.Updating
{
    using ConsoleApp1.AllSystems.Services;
    using ConsoleApp1.Components;

    public class UpdateSystem : SystemBase
    {
        private DebugSystem debugSystem;
        private HudSystem hudSystem = new HudSystem();
        private InfoSystem infoSystem = new InfoSystem();
        private CrossHairSystem crosshairSystem = new CrossHairSystem();
        private ScreenSystem screenSystem = new ScreenSystem();
        private WindowSystem windowSystem = new WindowSystem();
        private InventorySystem inventorySystem = new InventorySystem();
        private LandscapeSystem landscapeSystem;
        private CameraSystem cameraSystem = new CameraSystem();
        private PlayerControlSystem playerControlSystem = new PlayerControlSystem();
        private PlayerSystem playerSystem = new PlayerSystem();
        private ItemSystem itemSystem = new ItemSystem();
        private CollidablePhysicsSystem collidablePhysicsSystem = new CollidablePhysicsSystem();
        private ItemPhysicsSystem itemPhysicsSystem = new ItemPhysicsSystem();
        private GameSettingsSystem gameSettingsSystem = new GameSettingsSystem();
        private GameChangeDetectionSystem gameChangeDetectionSystem = new GameChangeDetectionSystem();
        private PerformanceService performance;

        public UpdateSystem(JobPool jobPool, PerformanceComponent performance)
        {
            debugSystem = new DebugSystem(jobPool, performance);
            landscapeSystem = new LandscapeSystem(jobPool, performance);
            this.performance = new PerformanceService(performance);
        }

        public void Run(Game game)
        {
            performance.Increase(PerformanceService.Section.UPDATE_CNT_FRAME);
            performance.Profile(PerformanceService.Section.UPDATE_TOTAL, () =>
            {
                performance.Profile(PerformanceService.Section.UPDATE_SETTINGS, () =>
                {
                    gameSettingsSystem.Run(EntityDatabase.SettingsComponent, EntityDatabase.InputComponent, game);
                });
                performance.Profile(PerformanceService.Section.UPDATE_PHYSICS, () =>
                {
                    Join(
                        EntityDatabase.InputTagComponents,
                        EntityDatabase.PlayerComponents,
                        EntityDatabase.CollidableComponents,
                        EntityDatabase.PhysicsComponents,
                        (a, b, c, d) =>
                    {
                        playerControlSystem.Run(EntityDatabase.SettingsComponent, EntityDatabase.InputComponent, b, c, d);
                    });
                });
                performance.Profile(PerformanceService.Section.UPDATE_ITEMS, () =>
                {
                    Join(
                        EntityDatabase.InputTagComponents,
                        EntityDatabase.PlayerComponents,
                        EntityDatabase.PhysicsComponents,
                        EntityDatabase.InventoryComponents,
                        (a, b, c, d) =>
                    {
                        playerSystem.Run(EntityDatabase.SettingsComponent, EntityDatabase.InputComponent, b, c, d, EntityDatabase.LandscapeComponent);
                    });
                });
                performance.Profile(PerformanceService.Section.UPDATE_ITEMS, () =>
                {
                    foreach (var item in EntityDatabase.ItemComponents.Values)
                    {
                        itemSystem.Run(item);
                    }
                });
                performance.Profile(PerformanceService.Section.UPDATE_COLLISION, () =>
                {
                    var playerPhysics = EntityDatabase.PhysicsComponents[EntityDatabase.QueryPlayerEntity()];
                    Join(
                        EntityDatabase.CollidableComponents,
                        EntityDatabase.PhysicsComponents,
                        (a, b) =>
                        {
                            collidablePhysicsSystem.Run(playerPhysics, a, b, EntityDatabase.LandscapeComponent);
                        });
                    Join(
                        EntityDatabase.PhysicsComponents,
                        EntityDatabase.ItemComponents,
                        (a, b) =>
                        {
                            itemPhysicsSystem.Run1(playerPhysics, a, b);
                        });
                    itemPhysicsSystem.Run2();
                });
                performance.Profile(PerformanceService.Section.UPDATE_LANDSCAPE, () =>
                {
                    landscapeSystem.Run(EntityDatabase.LandscapeComponent, EntityDatabase.SettingsComponent, EntityDatabase.PhysicsComponents[EntityDatabase.QueryPlayerEntity()]);
                });

                performance.Profile(PerformanceService.Section.UPDATE_UI, () =>
                {
                    UISystems();
                });
                performance.Profile(PerformanceService.Section.UPDATE_CAM, () =>
                {
                    Join(EntityDatabase.CameraTagComponents, EntityDatabase.PlayerComponents, EntityDatabase.PhysicsComponents, (a, b, c) =>
                    {
                        cameraSystem.Run(EntityDatabase.CameraComponent, b, c);
                    });
                });
            });
            gameChangeDetectionSystem.Run(EntityDatabase.SettingsComponent, EntityDatabase.InputComponent, EntityDatabase.PerformanceComponent, game);
        }

        public void UISystems()
        {
            Join(EntityDatabase.DebugTagComponents, EntityDatabase.ScreenComponents, (a, b) =>
            {
                debugSystem.Run(EntityDatabase.LandscapeComponent, EntityDatabase.SettingsComponent, b);
            });

            Join(EntityDatabase.InfoComponents, EntityDatabase.ScreenComponents, (a, b) =>
            {
                infoSystem.Run(a, b);
            });

            Join(EntityDatabase.CrosshairComponents, EntityDatabase.ScreenComponents, (a, b) =>
            {
                crosshairSystem.Run(a, b);
            });

            foreach (var a in EntityDatabase.ScreenComponents.Values)
            {
                screenSystem.Run(EntityDatabase.SettingsComponent, a);
            }

            foreach (var a in EntityDatabase.ScreenOverlayComponents.Values)
            {
                screenSystem.Run(EntityDatabase.SettingsComponent, a);
            }

            windowSystem.Run(
                EntityDatabase.MainWindow,
                EntityDatabase.ScreenComponents[EntityDatabase.MainWindow.Id],
                EntityDatabase.InputComponent,
                EntityDatabase.SettingsComponent);

            Join(
                EntityDatabase.InventoryComponents,
                EntityDatabase.InputTagComponents,
                (a, b) =>
            {
                inventorySystem.Run(a, EntityDatabase.InputComponent);
            });

            Join(
                EntityDatabase.HealthComponents,
                EntityDatabase.ScreenComponents,
                EntityDatabase.ScreenOverlayComponents,
                EntityDatabase.InventoryComponents,
                EntityDatabase.InputTagComponents,
                (a, b, c, d, e) =>
                {
                    hudSystem.Run(EntityDatabase.SettingsComponent, a, b, c, d, EntityDatabase.InputComponent);
                });
        }
    }
}