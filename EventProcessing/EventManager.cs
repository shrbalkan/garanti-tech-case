using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Policy;

namespace EventSearching
{
    public static class TimeFactor
    {
        public static int Factor = 1;
    }


    public enum CustomPriority
    {
        LOW,
        MEDIUM,
        HIGH
    }

    public class CustomEvent
    {
        public CustomPriority Priority { get; set; }
        public String Name { get; set; }
        

        public static int _number = 1;
        public CustomEvent(CustomPriority Priority)
        {
            this.Priority = Priority;
            this.Name = $"E{_number}";
            _number++;
        }
    }

    public class EventAlert
    {
        public CustomPriority Priority { get; set; }
        public String Events { get; set; }
    }

    public class EventManager
    {
        private readonly IObserverGUI _observer;
        ConcurrentQueue<CustomEvent> eventQueue = new ConcurrentQueue<CustomEvent>();
        private int maxEventCount = 400;
        public EventManager(IObserverGUI observer)
        {
            _observer = observer;
        }

        public void Start()
        {
            new Thread(() =>
            {
                for (int i = 0; i < maxEventCount; i++)
                {
                    Thread.Sleep(3000 / TimeFactor.Factor);
                    CustomEvent customEvent = new CustomEvent((CustomPriority)new Random().Next(0, 3));

                    //Debug.WriteLine($"Event {customEvent.Name} generated with priority: {customEvent.Priority} ");
                    eventQueue.Enqueue(customEvent);
                    _observer.EventAdded(customEvent);
                }
            }).Start();

            new Thread(() =>
            {
                int maxSize = 3;
                Queue<CustomEvent> queueEventCheck = new Queue<CustomEvent>(maxSize);
                int dequedCount = 0;
                while (dequedCount < maxEventCount)
                {
                    if (eventQueue.TryDequeue(out CustomEvent customEvent))
                    {
                        Thread.Sleep(5000 / TimeFactor.Factor);
                        if (queueEventCheck.Count == maxSize)
                            queueEventCheck.Dequeue();

                        queueEventCheck.Enqueue(customEvent);

                        if (queueEventCheck.Count == maxSize)
                        {
                            // Check if the queue has 3 elements with same priority
                            if (queueEventCheck.All(x => x.Priority == queueEventCheck.First().Priority))
                            {
                                // Raise an alert printing element names and priorities
                                EventAlert eventAlert = new EventAlert
                                {
                                    Events = string.Join(",", queueEventCheck.Select(x => x.Name)),
                                    Priority = queueEventCheck.First().Priority
                                };
                                _observer.EventAlert(eventAlert);
                                //Debug.WriteLine($"Alert: Events with same priority dequed: {string.Join(",", queueEventCheck.Select(x => x.Name))} with priority: {queueEventCheck.First().Priority}");
                                queueEventCheck.Clear();
                            }
                        }

                        //Debug.WriteLine($"Event {customEvent.Name} dequed with priority: {customEvent.Priority} ");
                        _observer.EventProcessed(customEvent);
                        dequedCount++;
                    }
                }
                _observer.EventProcessingEnded();
                CustomEvent._number = 1;
            }).Start();
        }
    }

    
}