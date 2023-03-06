using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace GGL.Editor.EventTracker
{
    public class EventReferenceInfo
    {
        public Object Owner { get; set; }
        public List<Object> Listeners { get; } = new();
        public List<string> MethodNames { get; } = new();
    }

    public static class EventTracker
    {
        public static List<EventReferenceInfo> FindAllUnityEventsReferences()
        {
            List<EventReferenceInfo> infos = new();
            foreach (Component behaviour in Resources.FindObjectsOfTypeAll<Component>())
            {
                List<TypeInfo> types = new();
                for (Type current = behaviour.GetType(); current.IsSubclassOf(typeof(Component)); current = current.BaseType!)
                    types.Add(current.GetTypeInfo());
                
                List<FieldInfo> events = types
                    .SelectMany(t => t.DeclaredFields)
                    .Where(f => (f.IsPublic || Attribute.IsDefined(f, typeof(SerializeField))) && f.FieldType.IsSubclassOf(typeof(UnityEventBase)))
                    .ToList();
                
                foreach (FieldInfo e in events)
                {
                    if (e.GetValue(behaviour) is not UnityEventBase eventValue) continue;

                    int count = eventValue.GetPersistentEventCount();
                    if(count == 0) continue;
                    EventReferenceInfo info = new() { Owner = behaviour };

                    for (int i = 0; i < count; i++)
                    {
                        Object obj = eventValue.GetPersistentTarget(i);
                        string method = eventValue.GetPersistentMethodName(i);

                        info.Listeners.Add(obj);
                        info.MethodNames.Add(obj.GetType().Name + "." + method);
                    }

                    infos.Add(info);
                }
            }
            return infos;
        }
    }
}