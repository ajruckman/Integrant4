using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Html;
using Superset.Web.State;

namespace Integrant4.Fundament
{
    public class ResourceSet
    {
        private static readonly Regex MatchVariableKeys =
            new("^.*{{(\\w+)}}.*$", RegexOptions.Multiline | RegexOptions.Compiled);

        private readonly HashSet<string> _scriptsInvariable;
        private readonly HashSet<string> _stylesheetsInvariable;

        private readonly HashSet<string> _scriptsVariable;
        private readonly HashSet<string> _stylesheetsVariable;

        private readonly Dictionary<string, ResourceSet> _dependencies;

        public readonly string Assembly;
        public readonly string ID;
        public readonly string CompositeID;

        private IHtmlContent? _invariableResourceHTMLCache;

        public ResourceSet
        (
            string           assembly,
            string           id,
            HashSet<string>? scriptsInternal     = null,
            HashSet<string>? scriptsExternal     = null,
            HashSet<string>? stylesheetsInternal = null,
            HashSet<string>? stylesheetsExternal = null,
            ResourceSet[]?   dependencies        = null
        )
        {
            Assembly    = assembly;
            ID          = id;
            CompositeID = $"{Assembly}+{ID}";

            string rootPath = $"/_content/{assembly}/";

            _scriptsInvariable     = new HashSet<string>();
            _stylesheetsInvariable = new HashSet<string>();
            _scriptsVariable       = new HashSet<string>();
            _stylesheetsVariable   = new HashSet<string>();

            _dependencies = new Dictionary<string, ResourceSet>();
            // _variables    = new Dictionary<string, string>();

            //

            if (scriptsInternal != null)
                foreach (string script in scriptsInternal)
                    AddScript(rootPath + script);

            if (scriptsExternal != null)
                foreach (string script in scriptsExternal)
                    AddScript(script);

            if (stylesheetsInternal != null)
                foreach (string sylesheet in stylesheetsInternal)
                    AddStylesheet(rootPath + sylesheet);

            if (stylesheetsExternal != null)
                foreach (string stylesheet in stylesheetsExternal)
                    AddStylesheet(stylesheet);

            //

            if (dependencies != null)
                foreach (ResourceSet dependency in dependencies)
                    AddDependency(dependency);
        }

        private void AddScript(string path)
        {
            if (!MatchVariableKeys.IsMatch(path))
            {
                if (_scriptsInvariable.Contains(path))
                    Console.WriteLine(
                        $"Notice: Re-registering script '{path}' in manifest with ID '{CompositeID}'.");
                _scriptsInvariable.Add(path);
            }
            else
            {
                if (_scriptsVariable.Contains(path))
                    Console.WriteLine(
                        $"Notice: Re-registering script '{path}' in manifest with ID '{CompositeID}'.");
                _scriptsVariable.Add(path);
            }
        }

        private void AddStylesheet(string path)
        {
            if (!MatchVariableKeys.IsMatch(path))
            {
                if (_stylesheetsInvariable.Contains(path))
                    Console.WriteLine(
                        $"Notice: Re-registering stylesheet '{path}' in manifest with ID '{CompositeID}'.");
                _stylesheetsInvariable.Add(path);
            }
            else
            {
                if (_stylesheetsVariable.Contains(path))
                    Console.WriteLine(
                        $"Notice: Re-registering stylesheet '{path}' in manifest with ID '{CompositeID}'.");
                _stylesheetsVariable.Add(path);
            }
        }

        private void AddDependency(ResourceSet dependency)
        {
            if (_dependencies.ContainsKey(dependency.ID))
                Console.WriteLine(
                    $"Notice: Overwriting previously registered ResourceSet dependency with ID '{dependency.ID}' in manifest with ID '{CompositeID}'.");
            _dependencies[dependency.ID] = dependency;
        }

        //

        public RenderFragment RenderInvariableResources()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "section");
                builder.AddAttribute(++seq, "hidden", "hidden");
                builder.AddContent(++seq, $"ID: {CompositeID}");
                builder.CloseElement();

                foreach (string script in EnumerateScriptsInvariable())
                    builder.AddContent(++seq, RenderScript(script));

                foreach (string stylesheet in EnumerateStylesheetsInvariable())
                    builder.AddContent(++seq, RenderStylesheet(stylesheet));
            }

            return Fragment;
        }

        public IHtmlContent RenderInvariableResourcesAsHTML()
        {
            if (_invariableResourceHTMLCache != null) return _invariableResourceHTMLCache;

            StringBuilder sb = new();

            sb.Append($"<section hidden='hidden'>ID: {CompositeID}</section>");

            foreach (string script in EnumerateScriptsInvariable())
                sb.Append(RenderScriptString(script));

            foreach (string stylesheet in EnumerateStylesheetsInvariable())
                sb.Append(RenderStylesheetString(stylesheet));

            _invariableResourceHTMLCache = new HtmlString(sb.ToString());
            return _invariableResourceHTMLCache;
        }

        public RenderFragment RenderVariableResources(Dictionary<string, string> variables)
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "section");
                builder.AddAttribute(++seq, "hidden", "hidden");
                builder.AddContent(++seq, $"ID: {CompositeID}");
                builder.CloseElement();

                foreach (string script in EnumerateScriptsVariable(variables))
                    builder.AddContent(++seq, RenderScript(script));

                foreach (string stylesheet in EnumerateStylesheetsVariable(variables))
                    builder.AddContent(++seq, RenderStylesheet(stylesheet));
            }

            return Fragment;
        }

        //

        private List<string> EnumerateScriptsInvariable()
        {
            List<string> result = new();

            foreach (ResourceSet dependency in _dependencies.Values)
            foreach (string script in dependency.EnumerateScriptsInvariable())
                result.Add(script);

            foreach (string script in _scriptsInvariable)
                result.Add(script);

            return result;
        }

        private List<string> EnumerateScriptsVariable(Dictionary<string, string> variables)
        {
            List<string> result = new();

            foreach (ResourceSet dependency in _dependencies.Values)
            foreach (string script in dependency.EnumerateScriptsVariable(variables))
                result.Add(script);

            foreach (string script in _scriptsVariable)
                result.Add(Expand(variables, script));

            return result;
        }

        private List<string> EnumerateStylesheetsInvariable()
        {
            List<string> result = new();

            foreach (string stylesheet in _stylesheetsInvariable)
                result.Add(stylesheet);

            foreach (ResourceSet dependency in _dependencies.Values)
            foreach (string stylesheet in dependency.EnumerateStylesheetsInvariable())
                result.Add(stylesheet);

            return result;
        }

        private List<string> EnumerateStylesheetsVariable(Dictionary<string, string> variables)
        {
            List<string> result = new();

            foreach (string stylesheet in _stylesheetsVariable)
                result.Add(Expand(variables, stylesheet));

            foreach (ResourceSet dependency in _dependencies.Values)
            foreach (string stylesheet in dependency.EnumerateStylesheetsVariable(variables))
                result.Add(stylesheet);

            return result;
        }

        // https://stackoverflow.com/a/7957728/9911189
        private string Expand(Dictionary<string, string> variables, string str)
        {
            Match match = MatchVariableKeys.Match(str);

            if (!match.Captures.Any())
                throw new Exception("Attempted to expand a resource path with no variables.");

            string result = str;

            foreach (Group group in match.Groups.Skip<Group>(1))
            {
                if (!variables.TryGetValue(group.Value, out string? value))
                    throw new KeyNotFoundException(
                        $"Value for variable '{group.Value}' does not exist in ResourceSet with ID '{CompositeID}'.");

                result = result.Replace("{{" + group.Value + "}}", value);
            }

            return result;
        }

        private static RenderFragment RenderScript(string src) => b =>
        {
            b.OpenElement(0, "script");
            b.AddAttribute(1, "src", src);
            b.CloseElement();
        };

        private static RenderFragment RenderStylesheet(string href) => b =>
        {
            b.OpenElement(0, "link");
            b.AddAttribute(1, "rel", "stylesheet");
            b.AddAttribute(2, "type", "text/css");
            b.AddAttribute(3, "href", href);
            b.CloseElement();
        };

        private static string RenderScriptString(string src) =>
            new($"<script src='{src}'></script>");

        private static string RenderStylesheetString(string href) =>
            new($"<link rel='stylesheet' type='text/css' href='{href}'/>");

        //

        // public event Action<string, string>? OnVariableChange;

        // internal void SetVariable(string key, string value)
        // {
        //     if (key.Contains(' '))
        //         throw new ArgumentException("ResourceSet variable keys cannot contain spaces.", nameof(key));
        //
        //     _variables[key] = value;
        //     SetVariableRecursive(key, value);
        //     OnVariableChange?.Invoke(key, value);
        // }

        // private void SetVariableRecursive(string key, string value)
        // {
        //     foreach (ResourceSet dependency in _dependencies.Values)
        //     {
        //         dependency.SetVariable(key, value);
        //     }
        // }
    }
}