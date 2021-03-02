using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Fundament
{
    public class ResourceService
    {
        private readonly ResourceSet _resourceSet;

        public ResourceService(ResourceSet resourceSet)
        {
            _resourceSet = resourceSet;
        }

        public RenderFragment RenderInvariableResources() =>
            _resourceSet.RenderInvariableResources();

        public RenderFragment RenderVariableResources(Dictionary<string, string> variables) =>
            _resourceSet.RenderVariableResources(variables);
    }
}