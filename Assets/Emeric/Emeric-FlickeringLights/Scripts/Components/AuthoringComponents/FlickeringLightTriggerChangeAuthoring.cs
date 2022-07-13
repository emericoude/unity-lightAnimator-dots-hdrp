using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Emeric
{
    [DisallowMultipleComponent]
    [SerializableAttribute]
    [RequireComponent(typeof(FlickeringLightTriggerChangeTagAuthoring))]
    public class FlickeringLightTriggerChangeAuthoring : UnityEngine.MonoBehaviour, Unity.Entities.IConvertGameObjectToEntity, Unity.Entities.IDeclareReferencedPrefabs
    {
        [Tooltip("If enabled, this trigger will be destroyed on a successful collision.")]
        [RegisterBinding(typeof(FlickeringLightTriggerChange), "DisableSelfOnTrigger")]
        public bool DisableSelfOnTrigger;

        [Tooltip("The lights to affect. Make sure your targets have a FlickeringLightAuthoring component.")]
        public GameObject[] LightsToChange;

        [Title("Choose a preset or custom setting.")]
        [EnumToggleButtons]
        [RegisterBinding(typeof(FlickeringLightTriggerChange), "FlickerToSet")]
        public FlickeringLightSettings FlickerToSet;

        [ShowIfGroup("FlickerToSet", FlickeringLightSettings.Custom)]
        [RegisterBinding(typeof(FlickeringLightTriggerChange), "CustomSettings")]
        public FlickeringLight CustomSettings;
        
        public void Convert(Entity __entity, EntityManager __dstManager, GameObjectConversionSystem __conversionSystem)
        {
            Emeric.FlickeringLightTriggerChange component = new Emeric.FlickeringLightTriggerChange();
            component.DisableSelfOnTrigger = DisableSelfOnTrigger;
            Unity.Entities.GameObjectConversionUtility.ConvertGameObjectsToEntitiesField(__conversionSystem, LightsToChange, out component.LightsToChange);
            component.FlickerToSet = FlickerToSet;
            component.CustomSettings = CustomSettings;
            Unity.Entities.EntityManagerManagedComponentExtensions.AddComponentData(__dstManager, __entity, component);
        }

        public void DeclareReferencedPrefabs(global::System.Collections.Generic.List<UnityEngine.GameObject> __referencedPrefabs)
        {
            Unity.Entities.Hybrid.Internal.GeneratedAuthoringComponentImplementation.AddReferencedPrefabs(__referencedPrefabs, LightsToChange);
        }
    }
}
