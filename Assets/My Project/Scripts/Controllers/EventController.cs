using System;

namespace JMatch.Controllers
{

    public class EventController
    {
        public static Action<Constants.UIEVENT> OnEventReceived;
    }
}