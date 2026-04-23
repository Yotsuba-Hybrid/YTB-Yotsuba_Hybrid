using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.HighestPerformanceTypes;
using YotsubaEngine.Runtime.CPR.Events;

namespace YotsubaEngine.Runtime.CPR
{
    public class Collision_Prediction_Runtime_2D : YTB_Runtime
    {
        private static bool DistanceIsSetted = false;
        /// <summary>
        /// Distancia que se utiliza pn muchas entidades.
        /// </summary>ara determinar si dos entidades pueden colisionar físicamente a corto plazo. Si la distancia entre dos entidades es mayor que esta distancia, se considera que
        /// es imposible que colisionen en el proximo frame y se omite la comprobación de colisión para esas entidades. Esto puede mejorar el rendimiento al reducir el número de comprobaciones de colisión necesarias,
        /// especialmente en escenarios co
        public static int UnPhysicalCollisionDistance
        {
            get => unPhysicalCollisionDistance;
            set
            {
                if (DistanceIsSetted) return;
                DistanceIsSetted = true;
                unPhysicalCollisionDistance = value;
            }
        }
        private GenericObjectPool<YTB<int>> SpatialGridStorage;

        public Dictionary<Point, YTB<int>> SpatialHashGrid;
        private Dictionary<int, Point> EntityPoint;
        private static int unPhysicalCollisionDistance;
        public override void InitializeSystem(EntityManager entityManager)
        {
            SpatialGridStorage = new(150);
            SpatialHashGrid = new Dictionary<Point, YTB<int>>();
            EntityPoint = new Dictionary<int, Point>();
            Entities = new();
            EntityManager = entityManager;

            Span<TransformComponent> transformComponents = GetTransformComponentsAsSpan();
            foreach(ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (entity.HasComponent(YTBComponent.Rigibody2D) && entity.HasComponent(YTBComponent.Transform) && entity.HasNotComponent(YTBComponent.TileMap))
                {
                    Entities.Add(entity.Id);
                    ref TransformComponent transform = ref transformComponents[entity.Id];
                    ref RigidBodyComponent2D rigidBody = ref GetRigidBodyComponentsAsSpan()[entity.Id];

                    Point point = GetSpatialHash(ref transform);
                    if (!SpatialHashGrid.TryGetValue(point, out YTB<int> list))
                    {
                        list = SpatialGridStorage.Rent();
                        SpatialHashGrid.Add(point, list);
                    }
                    list.Add(entity.Id);

                    EntityPoint[entity.Id] = point;
                }
            }

            EventManager.Instance.Subscribe<OnEntityRigidBody2DIsAdded>(EntityAdd);
            EventManager.Instance.Subscribe<OnEntityTransformIsAdded>(EntityAdd);
        }

        public YTB<int> IsPhysicalPossibleCollide(ref TransformComponent transformComponent, int entityId, YTB<int> entitiesCanCollide)
        {
            entitiesCanCollide.Clear();
            Point point = GetSpatialHash(ref transformComponent);
            Point lastPoint = EntityPoint[entityId];

            if(point != lastPoint)
            { 
                if (SpatialHashGrid.TryGetValue(lastPoint, out YTB<int> list))
                {
                    list.RemoveFast(entityId);

                    if(list.Count == 0)
                    {
                        SpatialHashGrid.Remove(lastPoint);
                        list.Clear();
                        SpatialGridStorage.Return(list);
                    }
                }
                
                if(SpatialHashGrid.TryGetValue(point, out var newList))
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

                    // Calculamos la llave del vecino sumando el offset a nuestra celda actual
                    Point neighborCell = new Point(point.X + x, point.Y + y);

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

            return entitiesCanCollide;
        }


        private Point GetSpatialHash(ref TransformComponent transform)
        {
            return new Point(
                  (int)(transform.Position.X / UnPhysicalCollisionDistance),
                  (int)(transform.Position.Y / UnPhysicalCollisionDistance)    
            );
        }

        public override void Dispose()
        {
            DistanceIsSetted = false;
            EventManager.Instance.Unsubscribe<OnEntityRigidBody2DIsAdded>(EntityAdd);
            EventManager.Instance.Unsubscribe<OnEntityTransformIsAdded>(EntityAdd);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        private void RegisterEntity(int entityId)
        {
            Entities.Add(entityId);
            ref TransformComponent transform = ref GetTransform2DComponent(entityId);
            ref RigidBodyComponent2D rigidBody = ref GetRigidBodyComponent(entityId);

            Point point = GetSpatialHash(ref transform);
            if (!SpatialHashGrid.TryGetValue(point, out YTB<int> list))
            {
                list = SpatialGridStorage.Rent();
                SpatialHashGrid.Add(point, list);
            }
            list.Add(entityId);

            EntityPoint[entityId] = point;
        }

        private void EntityAdd(OnEntityRigidBody2DIsAdded added)
        {
            if (added.Entity.HasComponent(YTBComponent.Transform) && added.Entity.HasNotComponent(YTBComponent.TileMap))
            {
                RegisterEntity(added.Entity.Id);
            }
        }

        private void EntityAdd(OnEntityTransformIsAdded added)
        {
            if (added.Entity.HasComponent(YTBComponent.Rigibody2D) && added.Entity.HasNotComponent(YTBComponent.TileMap))
            {
                RegisterEntity(added.Entity.Id);
            }
        }
    }
}
