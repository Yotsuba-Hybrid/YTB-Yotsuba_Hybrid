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
    public class Collision_Prediction_Runtime : YTB_Runtime
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

        public Dictionary<Point, YTB<int>> SpatialHashGrid;
        private Dictionary<int, Point> EntityPoint;
        private static int unPhysicalCollisionDistance;
        public override void InitializeSystem(EntityManager entityManager)
        {

            SpatialHashGrid = new Dictionary<Point, YTB<int>>();
            EntityPoint = new Dictionary<int, Point>();

            EntityManager = entityManager;

            Span<TransformComponent> transformComponents = GetTransformComponentsAsSpan();
            foreach(ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (entity.HasComponent(YTBComponent.Rigibody) && entity.HasComponent(YTBComponent.Transform))
                {
                    Entities.Add(entity.Id);
                    ref TransformComponent transform = ref transformComponents[entity.Id];
                    ref RigidBodyComponent2D rigidBody = ref GetRigidBodyComponentsAsSpan()[entity.Id];

                    Point point = GetSpatialHash(ref transform);
                    if (!SpatialHashGrid.TryGetValue(point, out YTB<int> list))
                    {
                        list = new YTB<int>();
                        SpatialHashGrid.Add(point, list);
                    }
                    list.Add(entity.Id);

                    EntityPoint[entity.Id] = point;
                }
            }

            EventManager.Instance.Subscribe<OnEntityRigidBodyIsAdded>(EntityAdd);
            EventManager.Instance.Subscribe<OnEntityTransformIsAdded>(EntityAdd);
        }

        public bool IsPhysicalPossibleCollide(ref TransformComponent transformComponent, int entityId)
        {
            Point point = GetSpatialHash(ref transformComponent);
            Point lastPoint = EntityPoint[entityId];

            if(point != lastPoint)
            { 
               SpatialHashGrid[lastPoint].Remove(entityId);
                if (SpatialHashGrid.TryGetValue(point, out var list))
                {
                    list.Add(entityId);
                }
                else
                {
                    list = new YTB<int>();
                    list.Add(entityId);
                    SpatialHashGrid.Add(point, list);
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
                    if (SpatialHashGrid.TryGetValue(neighborCell, out YTB<int> neighborList))
                    {
                        // Si la celda existe, comprobamos si tiene al menos 1 entidad dentro
                        if (neighborList.Count > 0)
                        {
                            if(x == 0 && y == 0)
                            {
                                if (neighborList.Count > 1) return true;
                            }
                            // ¡Peligro! Hay alguien en una celda contigua.
                            // Podríamos chocar en los bordes, así que devolvemos true.
                            else return true;
                        }
                    }
                }
            }

            return false;
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
            EventManager.Instance.Unsubscribe<OnEntityRigidBodyIsAdded>(EntityAdd);
            EventManager.Instance.Unsubscribe<OnEntityTransformIsAdded>(EntityAdd);
            base.Dispose();
        }

        private void RegisterEntity(int entityId)
        {
            Entities.Add(entityId);
            ref TransformComponent transform = ref GetTransform2DComponent(entityId);
            ref RigidBodyComponent2D rigidBody = ref GetRigidBodyComponent(entityId);

            Point point = GetSpatialHash(ref transform);
            if (!SpatialHashGrid.TryGetValue(point, out YTB<int> list))
            {
                list = new YTB<int>();
                SpatialHashGrid.Add(point, list);
            }
            list.Add(entityId);

            EntityPoint[entityId] = point;
        }

        private void EntityAdd(OnEntityRigidBodyIsAdded added)
        {
            if (added.Entity.HasComponent(YTBComponent.Transform))
            {
                RegisterEntity(added.Entity.Id);
            }
        }

        private void EntityAdd(OnEntityTransformIsAdded added)
        {
            if (added.Entity.HasComponent(YTBComponent.Rigibody))
            {
                RegisterEntity(added.Entity.Id);
            }
        }
    }
}
