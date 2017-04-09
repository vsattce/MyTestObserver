using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JCCode
{
    namespace Event
    {
        public interface ISubscriber
        {
            Action<object[]> handle{ set; }

            void UnSubscriber();
        }

        public static class EventManager
        {
            [System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
            public class ObserveAttribute:System.Attribute
            { }

            private class Subscriber : ISubscriber
            {
                private string _subscriberKey;
                private Action<object[]> _handles;

                public Subscriber(string key)
                {
                    this._subscriberKey = key;
                }
                ~Subscriber()
                {
                    UnSubscriber();
                }

                public Action<object[]> handle
                {
                    set
                    {
                        _handles = value;
                    }
                }

                public void UnSubscriber()
                {
                    if (_subscriberKey.Equals(string.Empty))
                        return;

                    List<Subscriber> subscriber = new List<Subscriber>();
                    if(!_dicSubscribers.TryGetValue(_subscriberKey, out subscriber))
                    {
                        return;
                    }
                    subscriber.Remove(this);
                    if(subscriber.Count <= 0)
                    {
                        _dicSubscribers.Remove(_subscriberKey);
                    }
                    _subscriberKey = string.Empty;
                }

                public void Notify(params object[] args)
                {
                    if (_handles != null)
                        _handles(args);
                }
            }

            private static Dictionary<string, List<Subscriber>> _dicSubscribers;

            static EventManager()
            {
#if UNITY_EDITOR
                EditorApplication.playmodeStateChanged = delegate ()
                {
                    if (!EditorApplication.isPlaying && !EditorApplication.isPaused)
                    {
                        if(_dicSubscribers != null)
                            _dicSubscribers.Clear();
                        _dicSubscribers = new Dictionary<string, List<Subscriber>>();
                    }
                };
#else
                _dicSubscribers = new Dictionary<string, List<Subscriber>>();
#endif
            }
            public static ISubscriber Subscribe(string name)
            {
                List<Subscriber> sublist = null;
                if (!_dicSubscribers.TryGetValue(name, out sublist))
                {
                    sublist = new List<Subscriber>();
                    _dicSubscribers.Add(name, sublist);
                }

                Subscriber sub = new Subscriber(name);
                sublist.Add(sub);
                return sub;
            }

            public static void Notify(string name, params object[] args)
            {
                List<Subscriber> sublist = null;
                if (!_dicSubscribers.TryGetValue(name, out sublist))
                {
                    return;
                }

                Subscriber[] subs = sublist.ToArray();
                foreach (var sub in subs)
                {
                    if (!sublist.Contains(sub))
                        continue;

                    sub.Notify(args);
                }
            }
        }

    }
}
