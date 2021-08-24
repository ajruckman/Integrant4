using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Integrant4.Element.Constructs;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Charts
{
    public class Chart : IConstruct
    {
        public delegate Task<IChartDataType> ChartDataGetter();

        private readonly ChartDefinition _chartDefinition;
        private readonly ChartDataGetter _dataGetter;

        private ElementReference? _chartRef;
        private ElementService?   _elementService;

        private object? _data;

        public Chart
        (
            ChartDefinition                           chartDefinition, ChartDataGetter dataGetter,
            ChartSeriesDefinition[]                   series,
            ChartAxisDefinition[]?                    axes   = null,
            Dictionary<string, ChartScaleDefinition>? scales = null
        )
        {
            _chartDefinition = chartDefinition;
            _dataGetter      = dataGetter;

            _chartDefinition.Series.AddRange(series);
            _chartDefinition.Series.Insert(0, new ChartSeriesDefinition());

            if (axes != null) _chartDefinition.Axes.AddRange(axes);
            _chartDefinition.Axes.Insert(0, new ChartAxisDefinition());

            if (scales != null)
                foreach (var (k, v) in scales)
                    _chartDefinition.Scales[k] = v;
        }

        public RenderFragment Renderer() => Latch.Create(builder =>
            {
                int seq = -1;

                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Chart");

                builder.OpenElement(++seq, "div");
                builder.AddElementReferenceCapture(++seq, r => _chartRef = r);
                builder.CloseElement();

                builder.CloseElement();
            },
            _ => { },
            async firstRender =>
            {
                if (!firstRender) return;

                if (_chartRef == null || _elementService == null)
                {
                    Console.WriteLine(
                        "Not initializing Chart because either a required service failed to inject " +
                        "or the chart's element reference was not captured.");
                    return;
                }

                await _elementService.JSInvokeVoidAsync
                (
                    "I4.Element.InitChart", _chartRef, _chartDefinition, _data!
                );
            });

        private async Task Init()
        {
            if (_elementService != null && _chartRef != null)
                await _elementService.JSInvokeVoidAsync
                (
                    "I4.Element.InitChart", _chartRef, _chartDefinition, _data!
                );
        }

        public async Task InvalidateData()
        {
            _data = null;
            await Init();
        }

        public async Task LoadData()
        {
            _data = (await _dataGetter.Invoke()).Serialize();
            await Init();
        }
    }

    public static class ChartDataSerializers
    {
        public static object SerializeTimeIntDataPoints(TimeIntChartDataPoint[] data)
        {
            int     numValues = data[0].Values.Length;
            int[][] result    = new int[numValues + 1][];

            for (var i = 0; i < numValues + 1; i++)
                result[i] = new int[data.Length];

            for (var i = 0; i < data.Length; i++)
            {
                TimeIntChartDataPoint v = data[i];

                result[0][i] = v.DateTime;

                for (var j = 0; j < numValues; j++)
                {
                    result[j + 1][i] = v.Values[j];
                }
            }

            return result;
        }
    }

    public interface IChartDataType
    {
        public object Serialize();
    }

    public class TimeIntChartDataType : IChartDataType
    {
        public TimeIntChartDataType(IEnumerable<TimeIntChartDataPoint> values)
        {
            Values = values.ToArray();
        }

        private TimeIntChartDataPoint[] Values { get; }

        public object Serialize() => ChartDataSerializers.SerializeTimeIntDataPoints(Values);
    }

    public class TimeIntChartDataPoint
    {
        public TimeIntChartDataPoint(DateTime dateTime, params int[] values)
        {
            DateTime = dateTime.AsUnixTime();
            Values   = values;
        }

        public int   DateTime { get; }
        public int[] Values   { get; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
    public class ChartDefinition
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ID { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Class { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Width { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Height { get; init; }

        [JsonPropertyName("series")] public List<ChartSeriesDefinition>              Series { get; } = new();
        [JsonPropertyName("axes")]   public List<ChartAxisDefinition>                Axes   { get; } = new();
        [JsonPropertyName("scales")] public Dictionary<string, ChartScaleDefinition> Scales { get; } = new();
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ChartSeriesDefinition
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Label { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Scale { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Stroke { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Fill { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Width { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? Dash { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("show")]
        public bool? ShownByDefault { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? SpanGaps { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("band")]
        public bool? IsBand { get; init; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ChartAxisDefinition
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("show")]
        public bool? Shown { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Label { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? LabelSize { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LabelFont { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Font { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Gap { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Size { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Stroke { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Scale { get; init; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ChartScaleDefinition
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Auto { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Time { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? Range { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("distr")]
        public int? Distribution { get; set; }
    }

    public static class ChartExtensions
    {
        private static readonly DateTime UnixStart = new(1970, 1, 1);

        public static int AsUnixTime(this DateTime v)
        {
            return (int)v.Subtract(UnixStart).TotalSeconds;
        }
    }
}