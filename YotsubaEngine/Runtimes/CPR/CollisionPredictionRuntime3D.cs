using System;
using System.Collections.Generic;
using System.Linq;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.HighestPerformanceTypes;
using YotsubaEngine.Runtime.CPR.Events;

namespace YotsubaEngine.Runtime.CPR
{
    public class Collision_Prediction_Runtime_3D : YTB_Runtime
    {
        private static bool DistanceIsSetted = false;

        public static int UnPhysicalCollisionDistance
        {
            get => unPhysicalCollisionDistance;
            set
            {
                if (DistanceIsSetted) return;
                DistanceIsSetted = true;
                if (value <= 0) unPhysicalCollisionDistance = 1;
                else
                unPhysicalCollisionDistance = value;
            }
        }
        private GenericObjectPool<YTB<int>> SpatialGridStorage;

        public Dictionary<Point3, YTB<int>> SpatialHashGrid;
        private Dictionary<int, Point3> EntityPoint;
        private static int unPhysicalCollisionDistance;

        public override void InitializeSystem(EntityManager entityManager)
        {
            SpatialGridStorage = new(500);
            SpatialHashGrid = new Dictionary<Point3, YTB<int>>();
            EntityPoint = new Dictionary<int, Point3>();
            Entities = new();
            EntityManager = entityManager;

            Span<TransformComponent> transformComponents = GetTransformComponentsAsSpan();
            Span<RigidBodyComponent3D> rigidBodyComponents = GetRigidBody3DComponentsAsSpan();
            foreach (ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (entity.HasComponent(YTBComponent.Rigibody3D) && entity.HasComponent(YTBComponent.Transform))
                {
                    Entities.Add(entity.Id);
                    ref TransformComponent transform = ref transformComponents[entity.Id];
                    ref RigidBodyComponent3D rigidBody = ref rigidBodyComponents[entity.Id];

                    Point3 point = GetSpatialHash(ref transform);
                    if (!SpatialHashGrid.TryGetValue(point, out YTB<int> list))
                    {
                        list = SpatialGridStorage.Rent();
                        SpatialHashGrid.Add(point, list);
                    }
                    list.Add(entity.Id);

                    EntityPoint[entity.Id] = point;
                }
            }

            EventManager.Instance.Subscribe<OnEntityRigidBody3DIsAdded>(EntityAdd);
            EventManager.Instance.Subscribe<OnEntityTransformIsAdded>(EntityAdd);
            EventManager.Instance.Subscribe<OnEntityRemoved>(EntityRemoved);
            EventManager.Instance.Subscribe<OnEntityTransformIsRemoved>(EntityComponentRemoved);
            EventManager.Instance.Subscribe<OnEntityRigidBody3DIsRemoved>(EntityComponentRemoved);
        }

        public YTB<int> IsPhysicalPossibleCollide(ref TransformComponent transformComponent, int entityId, YTB<int> entitiesCanCollide)
        {
            entitiesCanCollide.Clear();
            Point3 point = GetSpatialHash(ref transformComponent);
            if (!EntityPoint.TryGetValue(entityId, out Point3 lastPoint))
            {
                RegisterEntity(entityId);

                if (!EntityPoint.TryGetValue(entityId, out lastPoint))
                {
                    // En YTB mode: log claro
                    return entitiesCanCollide;
                }
            }

            if (point != lastPoint)
            {
                if (SpatialHashGrid.TryGetValue(lastPoint, out YTB<int> list))
                {
                    list.RemoveFast(entityId);

                    if (list.Count == 0)
                    {
                        SpatialHashGrid.Remove(lastPoint);
                        list.Clear();
                        SpatialGridStorage.Return(list);
                    }
                }

                if (SpatialHashGrid.TryGetValue(point, out var newList))
                {
                    newList.Add(entityId);
                }
                else
                {
                    newList = SpatialGridStorage.Rent();
                    newList.Add(entityId);
                    SpatialHashGrid.Add(point, newList);
                }
                EntityPoint[entityId] = point;
            }

            // Iteramos por las columnas de la izquierda (-1), centro (0) y derecha (1)
            for (int x = -1; x <= 1; x++)
            {
                // Iteramos por las filas de arriba (-1), centro (0) y abajo (1)
                for (int y = -1; y <= 1; y++)
                {

                    for (int z = -1; z <= 1; z++)
                    {
                        // Calculamos la llave del vecino sumando el offset a nuestra celda actual
                        Point3 neighborCell = new Point3(point.X + x, point.Y + y, point.Z + z);

                        // Le preguntamos al Diccionario: "¿Existe esta celda vecina en la cuadrícula?"
                        // Usamos TryGetValue porque es muy probable que la celda vecina esté vacía (no exista en el diccionario).
                        if (SpatialHashGrid.TryGetValue(neighborCell, out YTB<int> posibleCollision))
                        {
                            Span<int> span = posibleCollision.AsSpan();
                            for (int en = 0; en < span.Length; en++)
                            {
                                if (span[en] != entityId)
                                {
                                    entitiesCanCollide.Add(span[en]);
                                }
                            }
                        }
                    }
                }
            }

            return entitiesCanCollide;
        }

        private void EntityAdd(OnEntityRigidBody3DIsAdded added)
        {
            if(added.Entity.HasComponent(YTBComponent.Transform))
                RegisterEntity(added.Entity.Id);
        }

        private void EntityAdd(OnEntityTransformIsAdded added)
        {
            if (added.Entity.HasComponent(YTBComponent.Rigibody3D))
                RegisterEntity(added.Entity.Id);
        }

        private void EntityRemoved(OnEntityRemoved removed)
        {
            UnregisterEntity(removed.EntityId);
        }

        private void EntityComponentRemoved(OnEntityTransformIsRemoved removed)
        {
            UnregisterEntity(removed.EntityId);
        }

        private void EntityComponentRemoved(OnEntityRigidBody3DIsRemoved removed)
        {
            UnregisterEntity(removed.EntityId);
        }

        private void RegisterEntity(int entityId)
        {
            bool alreadyRegistered = false;
            Span<int> entitiesSpan = Entities.AsSpan();

            for (int i = 0; i < entitiesSpan.Length; i++)
            {
                if (entitiesSpan[i] == entityId)
                {
                    alreadyRegistered = true;
                    break;
                }
            }

            if (!alreadyRegistered)
                Entities.Add(entityId);

            ref TransformComponent transform = ref GetTransformComponent(entityId);

            Point3 point = GetSpatialHash(ref transform);

            if (!SpatialHashGrid.TryGetValue(point, out YTB<int> list))
            {
                list = SpatialGridStorage.Rent();
                SpatialHashGrid.Add(point, list);
            }

            bool alreadyInCell = false;
            Span<int> cellSpan = list.AsSpan();

            for (int i = 0; i < cellSpan.Length; i++)
            {
                if (cellSpan[i] == entityId)
                {
                    alreadyInCell = true;
                    break;
                }
            }

            if (!alreadyInCell)
                list.Add(entityId);

            EntityPoint[entityId] = point;
        }



        private void UnregisterEntity(int entityId)
        {
            Entities.RemoveFast(entityId);

            if (EntityPoint.TryGetValue(entityId, out Point3 point))
            {
                if (SpatialHashGrid.TryGetValue(point, out YTB<int> list))
                {
                    list.RemoveFast(entityId);
                    if (list.Count == 0)
                    {
                        SpatialHashGrid.Remove(point);
                        list.Clear();
                        SpatialGridStorage.Return(list);
                    }
                }

                EntityPoint.Remove(entityId);
            }
        }

        private Point3 GetSpatialHash(ref TransformComponent transform)
        {
            return new(
                  ((int)(transform.Position.X / UnPhysicalCollisionDistance)),
                  ((int)(transform.Position.Y / UnPhysicalCollisionDistance)),
                  ((int)(transform.Position.Z / UnPhysicalCollisionDistance))
            );
        }

        public override void Dispose()
        {
            DistanceIsSetted = false;
            EventManager.Instance.Unsubscribe<OnEntityRigidBody3DIsAdded>(EntityAdd);
            EventManager.Instance.Unsubscribe<OnEntityTransformIsAdded>(EntityAdd);
            EventManager.Instance.Unsubscribe<OnEntityRemoved>(EntityRemoved);
            EventManager.Instance.Unsubscribe<OnEntityTransformIsRemoved>(EntityComponentRemoved);
            EventManager.Instance.Unsubscribe<OnEntityRigidBody3DIsRemoved>(EntityComponentRemoved);

            foreach (var kv in SpatialHashGrid)
            {
                kv.Value.Clear();
                SpatialGridStorage.Return(kv.Value);
            }
            SpatialHashGrid.Clear();
            EntityPoint.Clear();
            Entities.Clear();
            base.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
