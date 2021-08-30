using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.Element;
using Integrant4.Element.Charts;
using Microsoft.AspNetCore.Components;

namespace Web.Pages
{
    public partial class Charts
    {
        private Chart _chart1;

        [Inject] public ElementService ElementService { get; set; } = null!;

        protected override void OnInitialized()
        {
            var      rng        = new Random(1);
            var      dataPoints = new List<TimeIntChartDataPoint>();
            DateTime start      = DateTime.Now.Subtract(TimeSpan.FromDays(90));

            for (DateTime day = start; day.Date < DateTime.Now; day = day.AddDays(1))
                if (day.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday))
                    dataPoints.Add(new TimeIntChartDataPoint(day, rng.Next(4, 40 / 5)));

            var data = new TimeIntChartDataSet(dataPoints);

            _chart1 = new Chart(new ChartDefinition
            {
                // Width  = 500,
                Height = 300,
            }, async () => data, new ChartSeriesDefinition[]
            {
                new()
                {
                    Label    = "Hours",
                    Stroke   = "red",
                    SpanGaps = false,
                },
            }, scales: new()
            {
                {"x", new ChartScaleDefinition {Distribution = 2}},
            }, axes: new[]
            {
                new ChartAxisDefinition
                {
                    Increments = new[]
                    {
                        1,
                        5,
                        10,
                        15,
                        30,
                        // hour divisors
                        60,
                        60 * 5,
                        60 * 10,
                        60 * 15,
                        60 * 30,
                        // day divisors
                        3600,
                    },
                    Values = new[]
                    {
                        // new ChartAxisValueFormatRule(3600 * 24 * 365, "{YYYY}")
                        // {
                        //     Year = null, Month = null, Day = null, Hour = null, Minute = null, Second = null, Mode = 1
                        // },
                        // new ChartAxisValueFormatRule(3600 * 24 * 28, "{MMM}")
                        // {
                        //     Year = "\n{YYYY}", Month = null, Day = null, Hour = null, Minute = null, Second = null,
                        //     Mode = 1
                        // },
                        // new ChartAxisValueFormatRule(3600 * 24, "{M}/{D}")
                        // {
                        //     Year = "\n{YYYY}", Month = null, Day = null, Hour = null, Minute = null, Second = null,
                        //     Mode = 1
                        // },
                        new ChartAxisValueFormatRule(3600, "{YYYY}/{MM}")
                        {
                            Year   = null, Month = "\n{MM}", Day = null, Hour = null, Minute = null,
                            Second = null, Mode           = 1
                        },
                        // new ChartAxisValueFormatRule(60, "{h}:{mm}{aa}")
                        // {
                        //     Year   = "\n{M}/{D}/{YY}", Month = null, Day = "\n{M}/{D}", Hour = null, Minute = null,
                        //     Second = null, Mode              = 1
                        // },
                        // new ChartAxisValueFormatRule(1, ":{ss}")
                        // {
                        //     Year = "\n{M}/{D}/{YY} {h}:{mm}{aa}", Month = null, Day = "\n{M}/{D} {h}:{mm}{aa}",
                        //     Hour = null, Minute                         = "\n{h}:{mm}{aa}", Second = null, Mode = 1
                        // },
                        // new ChartAxisValueFormatRule(0.001, ":{ss}.{fff}")
                        // {
                        //     Year = "\n{M}/{D}/{YY} {h}:{mm}{aa}", Month = null, Day = "\n{M}/{D} {h}:{mm}{aa}",
                        //     Hour = null, Minute                         = "\n{h}:{mm}{aa}", Second = null, Mode = 1
                        // },
                    }
                },
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await ElementService.ProcessJobs();

            await _chart1.LoadData();
        }
    }
}