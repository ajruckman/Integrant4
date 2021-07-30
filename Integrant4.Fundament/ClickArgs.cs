using Microsoft.AspNetCore.Components.Web;

namespace Integrant4.Fundament
{
    public readonly struct ClickArgs
    {
        public ClickArgs(ushort button, ushort x, ushort y, bool shift, bool control)
        {
            Button  = button;
            X       = x;
            Y       = y;
            Shift   = shift;
            Control = control;
        }

        public ClickArgs(MouseEventArgs args)
        {
            Button  = (ushort) args.Button;
            X       = (ushort) args.ClientX;
            Y       = (ushort) args.ClientY;
            Shift   = args.ShiftKey;
            Control = args.CtrlKey;
        }

        public readonly ushort Button;
        public readonly ushort X;
        public readonly ushort Y;
        public readonly bool   Shift;
        public readonly bool   Control;
    }
}