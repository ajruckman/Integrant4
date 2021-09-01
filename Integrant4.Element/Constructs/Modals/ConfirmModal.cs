using System;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.Element.Bits;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs.Modals
{
    public class ConfirmModal : IConstruct
    {
        private readonly Modal      _modal;
        private readonly ContentRef _content;
        private readonly Button     _buttonNo;
        private readonly Button     _buttonYes;

        private WriteOnlyHook? _refresher;

        public async Task<bool> Ask(CancellationToken? token = null)
        {
            if (_refresher == null)
            {
                Console.WriteLine(
                    "Call to ConfirmModal.Ask() with uninitialized refresher; has .Renderer() been called?");
            }

            _modal.Show();

            bool? result;

            if (token != null)
            {
                await using CancellationTokenRegistration register = token.Value.Register(() =>
                {
                    _askSource.TrySetCanceled();
                });
                result = await _askSource.Task;
            }
            else
            {
                result = await _askSource.Task;
            }

            _askSource = new TaskCompletionSource<bool>();

            return result == true;
        }

        private TaskCompletionSource<bool> _askSource = new();

        public ConfirmModal
        (
            ContentRef  content,
            ContentRef? contentNo  = null,
            ContentRef? contentYes = null
        )
        {
            _modal   = new Modal();
            _content = content;

            _buttonNo = new Button(contentNo ?? "Cancel".AsStatic(), new Button.Spec
            {
                Style  = () => Button.Style.TransparentBordered,
                Margin = () => new Size(0, 11, 0, 0),
                OnClick = (_, _) =>
                {
                    _modal.Hide();
                    _askSource.SetResult(false);
                },
            });

            _buttonYes = new Button(contentYes ?? "Confirm".AsStatic(), new Button.Spec
            {
                Style = () => Button.Style.Red,
                OnClick = (_, _) =>
                {
                    _modal.Hide();
                    _askSource.SetResult(true);
                },
            });
        }

        public RenderFragment Renderer() => Latch.Create
        (
            builder =>
            {
                int seq = -1;

                builder.OpenComponent<ModalContent>(++seq);
                builder.AddAttribute(++seq, "Modal", _modal);
                builder.AddAttribute(++seq, "ChildContent", (RenderFragment)(builder2 =>
                {
                    builder2.AddContent(++seq, _content.Renderer());
                    builder2.OpenElement(++seq, "div");
                    builder2.AddAttribute(++seq, "class", "I4E-Construct-ModalContent-Row");
                    builder2.AddContent(++seq, _buttonNo.Renderer());
                    builder2.AddContent(++seq, _buttonYes.Renderer());
                    builder2.CloseElement();
                }));
                builder.CloseComponent();
            },
            r => _refresher = r
        );
    }
}