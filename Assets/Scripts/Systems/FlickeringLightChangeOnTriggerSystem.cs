using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))] //Makes this system run after physics has done calculating
public partial class FlickeringLightChangeOnTriggerSystem : SystemBase
{
    private StepPhysicsWorld _stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem _commandBufferSystem;

    protected override void OnCreate()
    {
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        _commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        Dependency = new ChangeLightFlickerOnTriggerJob
        {
            triggers = GetComponentDataFromEntity<FlickeringLightTriggerChangeTag>(true),
            players = GetComponentDataFromEntity<PlayerTag>(true),
            entityCommandBuffer = _commandBufferSystem.CreateCommandBuffer(),
            entityManager = EntityManager
        }.Schedule(_stepPhysicsWorld.Simulation, Dependency);

        _commandBufferSystem.AddJobHandleForProducer(Dependency);
        Dependency.Complete();
    }

    struct ChangeLightFlickerOnTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<FlickeringLightTriggerChangeTag> triggers;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> players;

        public EntityCommandBuffer entityCommandBuffer;
        public EntityManager entityManager;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            //if both entities hold the same component, return
            if (triggers.HasComponent(entityA) && triggers.HasComponent(entityB))
                return;
            if (players.HasComponent(entityA) && players.HasComponent(entityB))
                return;

            //Store necessary values based on which entity is which
            FlickeringLightTriggerChange lightChangeTrigger;
            Entity lightChangeTriggerEntity;
            if (triggers.HasComponent(entityA) && players.HasComponent(entityB))
            {
                lightChangeTrigger = entityManager.GetComponentData<FlickeringLightTriggerChange>(entityA);
                lightChangeTriggerEntity = entityA;
            }
            else if (triggers.HasComponent(entityB) && players.HasComponent(entityA))
            {
                lightChangeTrigger = entityManager.GetComponentData<FlickeringLightTriggerChange>(entityB);
                lightChangeTriggerEntity = entityB;
            }
            else
            {
                return;
            }



            //Change the flicker data
            if (lightChangeTrigger.LightsToChange.Length > 0)
            {
                FlickeringLight flickerToSet = lightChangeTrigger.GetFlickerToApply();

                for (int i = 0; i < lightChangeTrigger.LightsToChange.Length; i++)
                {
                    entityManager.SetComponentData(lightChangeTrigger.LightsToChange[i], flickerToSet);
                }

                if (lightChangeTrigger.DisableSelfOnTrigger)
                {
                    entityCommandBuffer.DestroyEntity(lightChangeTriggerEntity);
                    //entityManager.SetEnabled(lightChangeTriggerEntity, false);
                }
            }
        }
    }
}
