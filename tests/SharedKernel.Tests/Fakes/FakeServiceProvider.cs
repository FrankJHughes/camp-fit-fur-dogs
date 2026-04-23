using System;
using System.Collections.Generic;

namespace SharedKernel.Tests.Fakes;

public sealed class FakeServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, List<object>> _handlers = new();

    public void AddHandler<T>(T handler)
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<object>();

        _handlers[type].Add(handler!);
    }

    public object? GetService(Type serviceType)
    {
        // Case 1: IEnumerable<T>
        if (serviceType.IsGenericType &&
            serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var inner = serviceType.GetGenericArguments()[0];

            if (_handlers.TryGetValue(inner, out var list))
            {
                // Create a strongly typed array: T[]
                var typedArray = Array.CreateInstance(inner, list.Count);
                list.ToArray().CopyTo(typedArray, 0);
                return typedArray;
            }

            // Return empty typed array
            return Array.CreateInstance(inner, 0);
        }

        // Case 2: T
        if (_handlers.TryGetValue(serviceType, out var handlers))
        {
            if (handlers.Count == 1)
                return handlers[0];

            // Create a strongly typed array: T[]
            var typedArray = Array.CreateInstance(serviceType, handlers.Count);
            handlers.ToArray().CopyTo(typedArray, 0);
            return typedArray;
        }

        return null;
    }
}
