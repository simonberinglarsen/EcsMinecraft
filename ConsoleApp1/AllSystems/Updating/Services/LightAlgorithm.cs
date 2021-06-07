namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class LightAlgorithm
    {
        private IWorldAccess worldAccess;
        private Queue<LightNode> graphDark = new Queue<LightNode>();
        private Queue<LightNode> graphLight = new Queue<LightNode>();

        public LightAlgorithm(IWorldAccess worldAccess)
        {
            this.worldAccess = worldAccess;
        }

        public void Propagate(Vector3i start, Action callback)
        {
            var lightlevelBefore = worldAccess.GetLight(start);
            var skyLightlevelBefore = worldAccess.GetSkyLight(start);
            callback();
            var lightlevelAfter = worldAccess.GetLight(start);
            var skyLightlevelAfter = worldAccess.GetSkyLight(start);

            HandleLight(start, lightlevelBefore, lightlevelAfter);
            HandleSkyLight(start, skyLightlevelBefore, skyLightlevelAfter);
        }

        private void HandleLight(Vector3i start, int lightlevelBefore, int lightlevelAfter)
        {
            graphDark.Clear();
            graphLight.Clear();
            if (lightlevelAfter <= lightlevelBefore)
            {
                graphDark.Enqueue(new LightNode { Location = start, LightLevel = lightlevelBefore });
                worldAccess.SetLight(start, 0);
                PropagateDark();
            }
            else
            {
                graphLight.Enqueue(new LightNode { Location = start, LightLevel = lightlevelAfter });
            }

            PropagateLight();
        }

        private void HandleSkyLight(Vector3i start, int lightlevelBefore, int lightlevelAfter)
        {
            graphDark.Clear();
            graphLight.Clear();
            if (lightlevelAfter <= lightlevelBefore)
            {
                graphDark.Enqueue(new LightNode { Location = start, LightLevel = lightlevelBefore });
                worldAccess.SetSkyLight(start, 0);
                PropagateSkyDark();
            }
            else
            {
                graphLight.Enqueue(new LightNode { Location = start, LightLevel = lightlevelAfter });
            }

            PropagateSkyLight();
        }

        private void PropagateLight()
        {
            Action<Vector3i, int> enqueueNode = (p, l) =>
            {
                if (worldAccess.NoLightPasses(p))
                {
                    return;
                }

                if (worldAccess.GetLight(p) >= l)
                {
                    return;
                }

                worldAccess.SetLight(p, l);
                graphLight.Enqueue(new LightNode { Location = p, LightLevel = l });
            };
            while (graphLight.Count > 0)
            {
                var node = graphLight.Dequeue();
                if (worldAccess.OutOfBounds(node.Location))
                {
                    continue;
                }

                var decreasedLight = node.LightLevel - 1;
                enqueueNode(node.Location + new Vector3i(1, 0, 0), decreasedLight);
                enqueueNode(node.Location + new Vector3i(-1, 0, 0), decreasedLight);
                enqueueNode(node.Location + new Vector3i(0, 1, 0), decreasedLight);
                enqueueNode(node.Location + new Vector3i(0, -1, 0), decreasedLight);
                enqueueNode(node.Location + new Vector3i(0, 0, 1), decreasedLight);
                enqueueNode(node.Location + new Vector3i(0, 0, -1), decreasedLight);
            }
        }

        private void PropagateSkyLight()
        {
            Action<Vector3i, int> enqueueNode = (p, l) =>
            {
                if (worldAccess.NoLightPasses(p))
                {
                    return;
                }

                if (worldAccess.GetSkyLight(p) >= l)
                {
                    return;
                }

                worldAccess.SetSkyLight(p, l);
                graphLight.Enqueue(new LightNode { Location = p, LightLevel = l });
            };
            while (graphLight.Count > 0)
            {
                var node = graphLight.Dequeue();
                if (worldAccess.OutOfBounds(node.Location))
                {
                    continue;
                }

                var decreasedLight = node.LightLevel - 1;
                enqueueNode(node.Location + new Vector3i(1, 0, 0), decreasedLight);
                enqueueNode(node.Location + new Vector3i(-1, 0, 0), decreasedLight);
                enqueueNode(node.Location + new Vector3i(0, 1, 0), decreasedLight);
                if (node.LightLevel == LandscapeComponent.MaxLight)
                {
                    enqueueNode(node.Location + new Vector3i(0, -1, 0), LandscapeComponent.MaxLight);
                }
                else
                {
                    enqueueNode(node.Location + new Vector3i(0, -1, 0), decreasedLight);
                }

                enqueueNode(node.Location + new Vector3i(0, 0, 1), decreasedLight);
                enqueueNode(node.Location + new Vector3i(0, 0, -1), decreasedLight);
            }
        }

        private void PropagateDark()
        {
            Action<Vector3i, int> enqueueNode = (p, l) =>
            {
                if (worldAccess.NoLightPasses(p))
                {
                    return;
                }

                var currentLight = worldAccess.GetLight(p);
                if (currentLight == 0)
                {
                    return;
                }

                if (currentLight < l)
                {
                    graphDark.Enqueue(new LightNode { Location = p, LightLevel = currentLight });
                    worldAccess.SetLight(p, 0);
                    return;
                }

                graphLight.Enqueue(new LightNode { Location = p, LightLevel = currentLight });
            };
            while (graphDark.Count > 0)
            {
                var node = graphDark.Dequeue();
                if (worldAccess.OutOfBounds(node.Location))
                {
                    continue;
                }

                var oldLevel = node.LightLevel;
                enqueueNode(node.Location + new Vector3i(1, 0, 0), oldLevel);
                enqueueNode(node.Location + new Vector3i(-1, 0, 0), oldLevel);
                enqueueNode(node.Location + new Vector3i(0, 1, 0), oldLevel);
                enqueueNode(node.Location + new Vector3i(0, -1, 0), oldLevel);
                enqueueNode(node.Location + new Vector3i(0, 0, 1), oldLevel);
                enqueueNode(node.Location + new Vector3i(0, 0, -1), oldLevel);
            }
        }

        private void PropagateSkyDark()
        {
            Action<Vector3i, int> enqueueNode = (p, l) =>
            {
                if (worldAccess.NoLightPasses(p))
                {
                    return;
                }

                var currentLight = worldAccess.GetSkyLight(p);
                if (currentLight == 0)
                {
                    return;
                }

                if (currentLight < l)
                {
                    graphDark.Enqueue(new LightNode { Location = p, LightLevel = currentLight });
                    worldAccess.SetSkyLight(p, 0);
                    return;
                }

                graphLight.Enqueue(new LightNode { Location = p, LightLevel = currentLight });
            };
            while (graphDark.Count > 0)
            {
                var node = graphDark.Dequeue();
                if (worldAccess.OutOfBounds(node.Location))
                {
                    continue;
                }

                var oldLevel = node.LightLevel;
                enqueueNode(node.Location + new Vector3i(1, 0, 0), oldLevel);
                enqueueNode(node.Location + new Vector3i(-1, 0, 0), oldLevel);
                enqueueNode(node.Location + new Vector3i(0, 1, 0), oldLevel);
                if (oldLevel == LandscapeComponent.MaxLight)
                {
                    enqueueNode(node.Location + new Vector3i(0, -1, 0), LandscapeComponent.MaxLight + 1);
                }
                else
                {
                    enqueueNode(node.Location + new Vector3i(0, -1, 0), oldLevel + 1);
                }

                enqueueNode(node.Location + new Vector3i(0, 0, 1), oldLevel);
                enqueueNode(node.Location + new Vector3i(0, 0, -1), oldLevel);
            }
        }

        private struct LightNode
        {
            public Vector3i Location;
            public int LightLevel;
        }
    }
}