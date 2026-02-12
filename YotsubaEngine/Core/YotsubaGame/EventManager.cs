using System;
using System.Collections.Generic;
using YotsubaEngine.Events.YTBEvents;

namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// Clase que maneja todos los eventos del juego tanto producidos por el motor como por los desarrolladores.
    /// <para>Class that handles all game events produced by the engine and by developers.</para>
    /// </summary>
    public class EventManager
    {

        /// <summary>
        /// Crea una nueva instancia del administrador de eventos.
        /// <para>Creates a new event manager instance.</para>
        /// </summary>
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

        /// <summary>
        /// Evento que se dispara cuando el administrador de eventos se pausa.
        /// <para>Event raised when the event manager is paused.</para>
        /// </summary>
        public event Action EventManagerWasPaused;
        /// <summary>
        /// Indica si el envío de eventos está detenido actualmente.
        /// <para>Indicates whether event dispatching is currently stopped.</para>
        /// </summary>
        public static bool StopEvents { get; set; } = false;

        /// <summary>
        /// instancia privada del eventManager
        /// </summary>
        private static EventManager instance;

        /// <summary>
        /// Instancia única del administrador de eventos.
        /// <para>Singleton instance of the event manager.</para>
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
        /// Publica un evento en el sistema.
        /// <para>Publishes an event in the system.</para>
        /// </summary>
        /// <typeparam name="T">Tipo de evento. <para>Event type.</para></typeparam>
        /// <param name="eventData">Datos del evento. <para>Event data.</para></param>
        public void Publish<T>(T eventData)
        {

            var targetDictionary = isResolving ? NextEventObjects : EventObjects;

            if (!targetDictionary.TryGetValue(typeof(T), out var list))
                targetDictionary[typeof(T)] = list = new List<object>();

            list.Add(eventData);
        }


        /// <summary>
        /// Se suscribe a un evento.
        /// <para>Subscribes to an event.</para>
        /// </summary>
        /// <typeparam name="T">Tipo de evento. <para>Event type.</para></typeparam>
        /// <param name="listener">Listener del evento. <para>Event listener.</para></param>
        public void Subscribe<T>(Action<T> listener)
        {
            if (!EventResponses.TryGetValue(typeof(T), out var list))
                EventResponses[typeof(T)] = list = new List<Action<object>>();

            list.Add(obj => { listener((T)obj); });
        }


        /// <summary>
        /// Se desuscribe de un evento.
        /// <para>Unsubscribes from an event.</para>
        /// </summary>
        /// <typeparam name="T">Tipo de evento. <para>Event type.</para></typeparam>
        /// <param name="action">Acción a desuscribir. <para>Action to remove.</para></param>
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
        /// Resuelve los eventos encolados.
        /// <para>Resolves queued events.</para>
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
