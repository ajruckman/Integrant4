using System;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.Element.Constructs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element
{
    public static class Interop
    {
        public static async Task CreateTooltips
            (IJSRuntime jsRuntime, CancellationToken token, string id)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("I4.Element.InitTooltip", token, id);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        public static async Task HighlightPageLink
            (IJSRuntime jsRuntime, CancellationToken token, string id, bool highlighted)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("I4.Element.HighlightPageLink", token, id, highlighted);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        internal static async Task InitDropdown
            (IJSRuntime jsRuntime, CancellationToken token, ElementReference toggleRef, ElementReference contentsRef)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("I4.Element.InitDropdown", token, toggleRef!, contentsRef!);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        internal static async Task InitSelector<TValue>
        (IJSRuntime                                 jsRuntime,
            CancellationToken                       token,
            ElementReference                        elemRef,
            DotNetObjectReference<Selector<TValue>> selector,
            bool                                    filterable
        ) where TValue : IEquatable<TValue>
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("I4.Element.InitSelector", token, elemRef, selector, filterable);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        internal static async Task ShowSelector
        (
            IJSRuntime jsRuntime, CancellationToken token, ElementReference headRef
        )
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("I4.Element.ShowSelector", token, headRef);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        internal static async Task HideSelector
        (
            IJSRuntime jsRuntime, CancellationToken token, ElementReference headRef
        )
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("I4.Element.HideSelector", token, headRef);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }
    }
}