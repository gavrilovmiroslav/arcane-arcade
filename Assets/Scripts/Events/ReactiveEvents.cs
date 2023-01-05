using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

public interface IReactiveEventInterface {}
public interface IAbstractEventReaction {}
public interface IReactsToEvent<T> : IAbstractEventReaction where T : System.Delegate { }

public static class EventSockets
{
    public static string ExportGraph()
    {
        var graph = new List<string>();
        graph.Add("digraph {");

        var types = Assembly.GetExecutingAssembly().GetTypes();
        var eventInterfaceImpls = types.Where(t =>
            t.GetInterfaces().Contains(typeof(IReactiveEventInterface)) && t.IsInterface);

        foreach (var impl in eventInterfaceImpls)
        {
            var directImplEvents = types.Where(t =>
                t.GetInterfaces().Contains(impl));

            foreach (var dir in directImplEvents)
            {
                graph.Add($"    {dir} [shape=box]");
                graph.Add($"    {dir} -> {impl} [label=\" implements\"]");
            }
        }

        var socketInterfaceImpls = types.Where(t =>
            t.GetInterfaces().Contains(typeof(IAbstractEventReaction)) && !t.IsInterface);

        foreach (var sock in socketInterfaceImpls)
        {
            foreach (var interf in sock.GetInterfaces())
            {
                var stringRep = $"{interf}";
                if (stringRep.StartsWith("IReactsToEvent`1["))
                {
                    Debug.Log(stringRep);
                    var parts = stringRep.Replace("IReactsToEvent`1[", "").Replace("]", "").Split("+");
                    graph.Add($"    {sock} [shape=box]");
                    graph.Add($"    {parts[0]} -> {sock} [label=\"on {parts[1]}  \" fontsize=8.0 style=\"dashed\"]");
                }

            }
        }

        graph.Add("}");

        var encoded = Uri.EscapeDataString(String.Join("\n", graph));
        var url = $"https://dreampuf.github.io/GraphvizOnline/#{encoded}";
        Application.OpenURL(url);

        return url;
    }
}
