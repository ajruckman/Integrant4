using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Colorant;
using Integrant4.Colorant.Themes.Main;
using Integrant4.Element;
using Integrant4.Element.Bits;
using Integrant4.Element.Bits.BitPresets;
using Integrant4.Element.Constructs.Headers;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Web.Shared
{
    public partial class MainLayout
    {
        private readonly Stopwatch     _stopwatch            = new();
        private          VariantLoader _defaultVariantLoader = null!;
        private          VariantLoader _solidsVariantLoader  = null!;

        private PrimaryHeader _header  = null!;
        private PrimaryHeader _header2 = null!;

        [Inject] public IJSRuntime     JSRuntime      { get; set; } = null!;
        [Inject] public ElementService ElementService { get; set; } = null!;

        protected override void OnInitialized()
        {
            ElementService.UseInteractionLogger(interaction =>
            {
                Console.WriteLine($"INTERACTION: {JsonConvert.SerializeObject(interaction)}");
            });
            
            _stopwatch.Start();

            _defaultVariantLoader = new VariantLoader
            (
                StorageService, JSRuntime,
                new Theme(),
                Variants.Dark.ToString()
            );

            _defaultVariantLoader.OnComplete += _ =>
            {
                InvokeAsync(StateHasChanged);

                _stopwatch.Stop();
            };
            _defaultVariantLoader.OnVariantChange += _ => InvokeAsync(StateHasChanged);

            //

            _solidsVariantLoader = new VariantLoader
            (
                StorageService, JSRuntime,
                new Integrant4.Colorant.Themes.Solids.Theme(),
                Integrant4.Colorant.Themes.Solids.Variants.Normal.ToString()
            );

            //

            _header = new PrimaryHeader(new IRenderable[]
            {
                new HeaderLink("Integrant4".AsStatic(), () => "/"),
                new Filler(),
                new Dropdown(new IRenderable[]
                {
                    new HeaderLink(new IRenderable[]
                    {
                        "Elements".AsStatic(),
                        new BootstrapIcon("chevron-down"),
                    }.AsStatic(), () => "/elements"),
                }.AsStatic(), new IRenderable[]
                {
                    new DropdownLinkButton("Benchmark".AsStatic(),        () => "/benchmark"),
                    new DropdownLinkButton("Bit Components".AsStatic(),   () => "/bitcomponents"),
                    new DropdownLinkButton("Constructs".AsStatic(),       () => "/constructs"),
                    new DropdownLinkButton("DropZones".AsStatic(),        () => "/dropzones"),
                    new DropdownLinkButton("Markdown Editors".AsStatic(), () => "/markdowneditors"),
                    new DropdownLinkButton("Radios".AsStatic(),           () => "/radios"),
                    new DropdownLinkButton("Selectors".AsStatic(),        () => "/selectors"),
                    new DropdownLinkButton("Tables".AsStatic(),           () => "/tables"),
                }.AsStatic()),
                new VerticalLine(),
                new HeaderLink(new IRenderable[]
                {
                    "Benchmarks".AsContent(),
                }.AsStatic(), () => "/benchmarks"),
                new VerticalLine(),
                new Dropdown
                (
                    new IRenderable[]
                    {
                        new HeaderLink(new IRenderable[]
                        {
                            "Dropdown 1".AsContent(),
                            new BootstrapIcon("chevron-down"),
                        }.AsStatic(), () => "/elements"),
                    }.AsStatic(),
                    new IRenderable[]
                    {
                        new TextBlock(new IRenderable[] {"Content".AsContent()}.AsStatic()),
                        new HorizontalLine(),
                        new TextBlock(new IRenderable[] {"Content 2".AsContent()}.AsStatic()),
                    }.AsStatic()
                ),
            }.AsStatic(), null);

            _header2 = new PrimaryHeader(new IRenderable[]
                {
                    new HeaderLink("Integrant4".AsStatic(), () => "/"),
                }.AsStatic(),
                new IRenderable[]
                {
                    new Dropdown(new IRenderable[]
                    {
                        new TextBlock(new IRenderable[]
                        {
                            "Elements".AsStatic(),
                            new BootstrapIcon("chevron-down"),
                        }.AsStatic()),
                    }.AsStatic(), new IRenderable[]
                    {
                        new DropdownLinkButton("Benchmark".AsStatic(),        () => "/benchmark"),
                        new DropdownLinkButton("Bit Components".AsStatic(),   () => "/bitcomponents"),
                        new DropdownLinkButton("Constructs".AsStatic(),       () => "/constructs"),
                        new DropdownLinkButton("DropZones".AsStatic(),        () => "/dropzones"),
                        new DropdownLinkButton("Markdown Editors".AsStatic(), () => "/markdowneditors"),
                        new DropdownLinkButton("Radios".AsStatic(),           () => "/radios"),
                        new DropdownLinkButton("Selectors".AsStatic(),        () => "/selectors"),
                        new DropdownLinkButton("Tables".AsStatic(),           () => "/tables"),
                    }.AsStatic()),
                    new VerticalLine(),
                    new HeaderLink("Elements".AsStatic(), () => "/elements"),
                    new VerticalLine(),
                    new HeaderLink("Benchmark".AsStatic(), () => "/benchmark"),
                    new VerticalLine(),
                    new HeaderLink("Bit Components".AsStatic(), () => "/bitcomponents"),
                    new VerticalLine(),
                    new HeaderLink("Constructs".AsStatic(), () => "/constructs"),
                    new VerticalLine(),
                    new HeaderLink("DropZones".AsStatic(), () => "/dropzones"),
                    new VerticalLine(),
                    new HeaderLink("Markdown Editors".AsStatic(), () => "/markdowneditors"),
                    new VerticalLine(),
                    new HeaderLink("Radios".AsStatic(), () => "/radios"),
                    new VerticalLine(),
                    new HeaderLink("Selectors".AsStatic(), () => "/selectors"),
                    new VerticalLine(),
                    new HeaderLink("Tables".AsStatic(), () => "/tables"),
                }.AsStatic());
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _defaultVariantLoader.Load();

                // Thread.Sleep(100);
                // throw new Exception();
            }

            await ElementService.ProcessJobs();
        }
    }
}