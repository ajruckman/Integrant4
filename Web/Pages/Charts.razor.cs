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

            var data = new TimeIntChartDataType(dataPoints);

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
                { "x", new ChartScaleDefinition { Distribution = 2 } },
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await ElementService.ProcessJobs();

            await _chart1.LoadData();
        }
    }
}