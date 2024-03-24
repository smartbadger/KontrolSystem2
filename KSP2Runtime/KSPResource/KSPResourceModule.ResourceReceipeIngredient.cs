﻿using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPResource;

public partial class KSPResourceModule {
    [KSClass("ResourceReceipeIngredient")]
    public class ResourceReceipeIngredientAdapter(ResourceUnitsPair pair) {
        [KSField] public double Units => pair.units;

        [KSField]
        public ResourceDefinitionAdapter Resource {
            get {
                var context = KSPContext.CurrentContext;
                var resourceDefinitionData =
                    context.Game.ResourceDefinitionDatabase.GetDefinitionData(pair.resourceID);

                return new ResourceDefinitionAdapter(resourceDefinitionData);
            }
        }
    }
}
