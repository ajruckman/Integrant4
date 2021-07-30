using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Fundament
{
    public class ServiceInjector<T> : ComponentBase where T : class
    {
        [Inject] public T Service { get; set; } = null!;

        [Parameter] public Action<T>[] OnInject { get; set; } = null!;

        protected override void OnParametersSet()
        {
            foreach (Action<T> action in OnInject)
            {
                action.Invoke(Service);
            }
        }

        public static void Inject
        (
            RenderTreeBuilder  builder,
            ref    int         seq,
            params Action<T>[] onInject
        )
        {
            builder.OpenComponent<ServiceInjector<T>>(++seq);
            builder.AddAttribute(++seq, "OnInject", onInject);
            builder.CloseComponent();
        }
    }
}