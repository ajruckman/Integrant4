using System;
using System.Collections.Generic;
using System.Diagnostics;
using Integrant4.Colorant.Generators.Schema;
using Newtonsoft.Json;

namespace Integrant4.Colorant.Generators.ColorGeneratorSupport
{
    internal sealed class Caller
    {
        private readonly Process _p;

        public Caller()
        {
            _p = new Process
            {
                StartInfo =
                {
                    UseShellExecute        = false,
                    RedirectStandardInput  = true,
                    RedirectStandardOutput = true,
                    FileName               = "node",
                    Arguments              = "./ColorGenerator/index.js",
                },
            };
            _p.Start();
        }

        public void Kill()
        {
            _p.Kill();
        }

        public List<Color> Call(Block block, ColorRange s)
        {
            _p.StandardInput.WriteLine(
                $"{block.IDs.Count} "                                +
                $"{s.HueStart} {s.HueEnd} {s.HueCurve} "             +
                $"{s.SatStart} {s.SatEnd} {s.SatCurve} {s.SatRate} " +
                $"{s.LumStart} {s.LumEnd} {s.LumCurve} "             +
                $"{s.Modifier}");

            string json = _p.StandardOutput.ReadLine() ?? throw new Exception();

            var range = JsonConvert.DeserializeObject<List<Color>>
            (
                json,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}
            );

            return range ?? throw new Exception();
        }
    }
}