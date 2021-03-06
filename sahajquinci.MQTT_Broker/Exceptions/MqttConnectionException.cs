
using System;
using Crestron.SimplSharp;

namespace sahajquinci.MQTT_Broker.Exceptions
{
    /// <summary>
    /// Connection to the broker exception
    /// </summary>
    public class MqttConnectionException : Exception
    {
        public MqttConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
