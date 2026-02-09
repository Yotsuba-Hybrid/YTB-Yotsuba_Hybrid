using System;
using System.Collections.Generic;
using YotsubaEngine.Events.YTBEvents;

namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// Clase que maneja todos los eventos del juego tanto producidos por el mismo engine, como por los desarrolladores.
    /// </summary>
    public class EventManager
    {

        public EventManager()
        {
            Subscribe<StopEvents>(OnStopEvents);
        }

        private void OnStopEvents(StopEvents events)
        {
            StopEvents = true;
            if(events.ignoreEventsInProccess)
            {
                EventObjects.Clear();
                NextEventObjects.Clear();
            }

            EventManagerWasPaused?.Invoke();
        }

        public event Action EventManagerWasPaused;
        /// <summary>
        /// Indicates whether event dispatching is currently stopped.
        /// Indica si el envío de eventos está detenido actualmente.
        /// </summary>
        public static bool StopEvents { get; set; } = false;

        /// <summary>
        /// instancia privada del eventManager
        /// </summary>
        private static EventManager instance;

        /// <summary>
        /// Instancia unica del EventManager
        /// </summary>
        public static EventManager Instance { get => instance == null ? instance = new EventManager() : instance; }
        /// <summary>
        /// Diccionario que almacena el tipo de objeto a enviar, y una lista de los objetos que almacena;
        /// </summary>
        private Dictionary<Type, List<object>> EventObjects = new();

        /// <summary>
        /// Diccionario que almacena los eventos que se recibieron en el frame actual durante la resolucion de eventos;
        /// </summary>
        private Dictionary<Type, List<object>> NextEventObjects = new();

        /// <summary>
        /// Diccionario que almacena las subscripciones a eventos
        /// </summary>
        private Dictionary<Type, List<Action<object>>> EventResponses = new();

        /// <summary>
        /// Flag que indica si el EventManager está en proceso de resolver eventos.
        /// </summary>
        private bool isResolving = false;

        /// <summary>
        /// Metodo publicar un evento.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void Publish<T>(T eventData)
        {

            var targetDictionary = isResolving ? NextEventObjects : EventObjects;

            if (!targetDictionary.TryGetValue(typeof(T), out var list))
                targetDictionary[typeof(T)] = list = new List<object>();

            list.Add(eventData);
        }


        /// <summary>
        /// Metodo para suscribirse a un evento.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void Subscribe<T>(Action<T> listener)
        {
            if (!EventResponses.TryGetValue(typeof(T), out var list))
                EventResponses[typeof(T)] = list = new List<Action<object>>();

            list.Add(obj => { listener((T)obj); });
        }


        /// <summary>
        /// Metodo para desuscribirse de un evento.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void Unsubscribe<T>(Action<T> action)
        {
            if(EventResponses.TryGetValue(typeof(T), out var list))
            {
                list.RemoveAll(a =>
                {
                    return a.Target == action.Target && a.Method == action.Method;
                });
            }
        }

        /// <summary>
        /// Metodo para resolver los eventos.
        /// </summary>
        public void ResolveEvents()
        {
            if (StopEvents) return;
            isResolving = true;

            foreach (var (type, events) in EventObjects)
            {
                if (!EventResponses.TryGetValue(type, out var listeners)) continue;

                foreach (var e in events)
                {
                    foreach (var listener in listeners)
                    {
                        listener.Invoke(e);
                    }
                }
            }
            EventObjects.Clear();
            isResolving = false;
                
            foreach (var (type, events) in NextEventObjects)
            {
                if (!EventResponses.TryGetValue(type, out var listeners)) continue;

                foreach (var e in events)
                {
                    foreach (var listener in listeners)
                    {
                        listener.Invoke(e);
                    }
                }
            }

            NextEventObjects.Clear();
        }
    }
}
