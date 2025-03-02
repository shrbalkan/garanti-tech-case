using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSearching
{
    public interface IObserverGUI
    {
        public void EventAdded(CustomEvent customEvent);
        public void EventProcessed(CustomEvent customEvent);

        public void EventProcessingEnded();
        public void EventAlert(EventAlert eventAlert);
    }
}
