﻿namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal class EventHandlerArgumentProviderMap
    {
        private readonly ConcurrentDictionary<Delegate, IEventRaiserArguments> map
            = new ConcurrentDictionary<Delegate, IEventRaiserArguments>(new EventRaiserDelegateComparer());

        public void AddArgumentProvider(Delegate eventHandler, IEventRaiserArguments argumentProvider)
        {
            this.map[eventHandler] = argumentProvider;
        }

        public IEventRaiserArguments TakeArgumentProviderFor(Delegate eventHandler)
        {
            IEventRaiserArguments provider;
            this.map.TryRemove(eventHandler, out provider);
            return provider;
        }

        public bool Contains(Delegate eventHandler)
        {
            return this.map.ContainsKey(eventHandler);
        }

        /// <summary>
        /// Allows a more lenient comparison of delegates, chiefly so <see cref="EventHandler"/>s and
        /// <see cref="EventHandler{TEventArgs}"/>s that refer to the same method on the same instance
        /// will compare as equal. It relies on the fact that every time an event is raised, 
        /// <see cref="Raise"/> creates a new instance, and the delegate registered in the map
        /// targets that instance.
        /// </summary>
        private class EventRaiserDelegateComparer : IEqualityComparer<Delegate>
        {
            public bool Equals(Delegate leftDelegate, Delegate rightDelegate)
            {
                return ReferenceEquals(leftDelegate, rightDelegate) ||
                       (leftDelegate != null &&
                        rightDelegate != null &&
                        ReferenceEquals(leftDelegate.Target, rightDelegate.Target));
            }

            public int GetHashCode(Delegate theDelegate)
            {
                return theDelegate == null || theDelegate.Target == null
                    ? 17
                    : theDelegate.Target.GetHashCode();
            }
        }
    }
}
