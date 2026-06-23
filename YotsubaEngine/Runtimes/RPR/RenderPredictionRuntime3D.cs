using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Runtime.CPR.Events;
using YotsubaEngine.Runtime.Events;
using YotsubaEngine.Runtime.RPR.Events;

namespace YotsubaEngine.Runtime.RPR
{

    /// <summary>
    /// Runtime de alto rendimiento que calcula y almacena en tiempo real las entidades que pueden ser renderizadas en un entorno 3D.
    /// Ya sea por tener un ModelComponent3D, o un YTBModelComponent3D. (Es requisito que las entidades tengan un TransformComponent)
    /// </summary>
    public class Render_Prediction_Runtime_3D : YTB_Runtime
    {
        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            Entities = new();

            Span<Yotsuba> ents = EntityManager.YotsubaEntities.AsSpan();
            Span<SpriteComponent2D> spriteComponents = GetSpriteComponentsAsSpan();
            if (ents.IsEmpty) return;
            foreach (ref Yotsuba entity in ents)
            {

                if (entity.HasNotComponent(YTBComponent.Transform)) continue;

                if((entity.HasComponent(YTBComponent.Model3D) || entity.HasComponent(YTBComponent.YTBModel3D)))
                {
                    RegisterEntity(entity.Id);
                }

                if (entity.HasComponent(YTBComponent.Sprite))
                {
                    ref SpriteComponent2D sprite = ref spriteComponents[entity.Id];

                    if (sprite.Is2_5D)
                    {
                        RegisterEntity(entity.Id);
                    }
                }
            }


            ListenEvents();
        }


        private void ListenEvents()
        {
            EventManager.Instance.Subscribe<OnEntityTransformIsAdded>(EntityAdd);
            EventManager.Instance.Subscribe<OnEntityModelComponentIsAdded>(EntityAdd);
            EventManager.Instance.Subscribe<OnEntityYTBModelIsAdded>(EntityAdd);
            EventManager.Instance.Subscribe<OnSpriteIsSettedAs2_5D>(EntityAdd);
            EventManager.Instance.Subscribe<OnEntityRemoved>(EntityRemoved);
            EventManager.Instance.Subscribe<OnEntityTransformIsRemoved>(EntityComponentRemoved);
            EventManager.Instance.Subscribe<OnEntityModelComponentIsRemoved>(EntityComponentRemoved);
            EventManager.Instance.Subscribe<OnEntityYTBModelIsRemoved>(EntityComponentRemoved);
            EventManager.Instance.Subscribe<OnSpriteNoLonger2_5D>(EntityComponentRemoved);
            EventManager.Instance.Subscribe<OnEntitySpriteIsRemoved>(EntityComponentRemoved);
        }

        private void EntityAdd(OnSpriteIsSettedAs2_5D d)
        {
            Span<Yotsuba> yotsubas = GetEntitiesAsSpan();
            Span<SpriteComponent2D> spriteComponents = GetSpriteComponentsAsSpan();

            foreach (ref Yotsuba entity in yotsubas)
            {
                if (entity.HasComponent(YTBComponent.Sprite))
                {
                    ref SpriteComponent2D sprite = ref spriteComponents[entity.Id];
                    if (sprite.Is2_5D)
                    {
                        RegisterEntity(entity.Id);
                    }
                }
            }
        }

        private void EntityRemoved(OnEntityRemoved removed)
        {
            UnregisterEntity(removed.EntityId);
        }

        private void UnregisterEntity(int entityId)
        {
            Entities.RemoveFast(entityId);
        }

        private void EntityComponentRemoved(OnEntityTransformIsRemoved removed) => UnregisterEntity(removed.EntityId);
        private void EntityComponentRemoved(OnEntityModelComponentIsRemoved removed) => UnregisterEntity(removed.EntityId);
        private void EntityComponentRemoved(OnEntityYTBModelIsRemoved removed) => UnregisterEntity(removed.EntityId);
        private void EntityComponentRemoved(OnSpriteNoLonger2_5D removed) => UnregisterEntity(removed.EntityId);
        private void EntityComponentRemoved(OnEntitySpriteIsRemoved removed) => UnregisterEntity(removed.EntityId);

        private void EntityAdd(OnEntityYTBModelIsAdded added)
        {
            if (added.Entity.HasComponent(YTBComponent.Transform))
                RegisterEntity(added.Entity.Id);
        }

        private void EntityAdd(OnEntityModelComponentIsAdded added)
        {
            if (added.Entity.HasComponent(YTBComponent.Transform))
                RegisterEntity(added.Entity.Id);
        }

        private void EntityAdd(OnEntityTransformIsAdded added)
        {
            if (added.Entity.HasComponent(YTBComponent.Model3D) || added.Entity.HasComponent(YTBComponent.YTBModel3D))
                RegisterEntity(added.Entity.Id);
        }

        private void RegisterEntity(int entityId)
        {
            ReadOnlySpan<int> entitieIds = Entities.AsReadOnlySpan();
            for (int i = 0; i < entitieIds.Length; i++)
            {
                if (entitieIds[i] == entityId)
                    return;
            }

            Entities.Add(entityId);
        }

        public override void Dispose()
        {
            EventManager.Instance.Unsubscribe<OnEntityModelComponentIsAdded>(EntityAdd);
            EventManager.Instance.Unsubscribe<OnEntityTransformIsAdded>(EntityAdd);
            EventManager.Instance.Unsubscribe<OnEntityYTBModelIsAdded>(EntityAdd);
            EventManager.Instance.Unsubscribe<OnSpriteIsSettedAs2_5D>(EntityAdd);
            EventManager.Instance.Unsubscribe<OnEntityRemoved>(EntityRemoved);
            EventManager.Instance.Unsubscribe<OnEntityTransformIsRemoved>(EntityComponentRemoved);
            EventManager.Instance.Unsubscribe<OnEntityModelComponentIsRemoved>(EntityComponentRemoved);
            EventManager.Instance.Unsubscribe<OnEntityYTBModelIsRemoved>(EntityComponentRemoved);
            EventManager.Instance.Unsubscribe<OnSpriteNoLonger2_5D>(EntityComponentRemoved);
            EventManager.Instance.Unsubscribe<OnEntitySpriteIsRemoved>(EntityComponentRemoved);
            Entities.Clear();
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        /// <summary>
        /// Retorna las entidades que cumplen los requisitos minimos para ser renderizadas en un entorno 3D.
        /// Requisitos minimos:
        /// 
        ///  - Tener TransformComponent (Obligatorio).
        ///  
        ///  - Tener al menos uno de los siguientes componentes:
        ///     || ModelComponent3D
        ///     || YTBModelComponent3D
        ///     || SpriteComponent (Con Is2_5D = true)
        /// 
        /// </summary>
        /// <returns></returns>
        public Span<int> GetEntitieIdsCanRender3D()
        {
            return Entities.AsSpan();
        }
    }
}
