
using sahajquinci.MQTT_Broker.Exceptions;
using Crestron.SimplSharp.CrestronLogger;

namespace sahajquinci.MQTT_Broker.Messages
{
    /// <summary>
    /// Class for PUBACK message from broker to client
    /// </summary>
    public class MqttMsgPuback : MqttMsgBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MqttMsgPuback()
        {
            this.type = MQTT_MSG_PUBACK_TYPE;
        }

        public override byte[] GetBytes()
        {
            int fixedHeaderSize = 0;
            int varHeaderSize = 0;
            int payloadSize = 0;
            int remainingLength = 0;
            byte[] buffer;
            int index = 0;

            // message identifier
            varHeaderSize += MESSAGE_ID_SIZE;

            remainingLength += (varHeaderSize + payloadSize);

            // first byte of fixed header
            fixedHeaderSize = 1;

            int temp = remainingLength;
            // increase fixed header size based on remaining length
            // (each remaining length byte can encode until 128)
            do
            {
                fixedHeaderSize++;
                temp = temp / 128;
            } while (temp > 0);

            // allocate buffer for message
            buffer = new byte[fixedHeaderSize + varHeaderSize + payloadSize];

            // first fixed header byte
            buffer[index++] = (MQTT_MSG_PUBACK_TYPE << MSG_TYPE_OFFSET) | MQTT_MSG_PUBACK_FLAG_BITS; // [v.3.1.1]

            // encode remaining length
            index = this.encodeRemainingLength(remainingLength, buffer, index);

            // get message identifier
            buffer[index++] = (byte)((this.messageId >> 8) & 0x00FF); // MSB
            buffer[index++] = (byte)(this.messageId & 0x00FF); // LSB 

            return buffer;
        }

        /// <summary>
        /// Parse bytes for a PUBACK message
        /// </summary>
        /// <param name="fixedHeaderFirstByte">First fixed header byte</param>
        /// <param name="protocolVersion">Protocol Version</param>
        /// <param name="channel">Channel connected to the broker</param>
        /// <returns>PUBACK message instance</returns>
        public static MqttMsgPuback Parse(byte[] data)
        {
            byte[] buffer;
            int index = 0;
            MqttMsgPuback msg = new MqttMsgPuback();
            byte fixedHeaderFirstByte = data[0];
            // [v3.1.1] check flag bits
            if ((fixedHeaderFirstByte & MSG_FLAG_BITS_MASK) != MQTT_MSG_PUBACK_FLAG_BITS)
                throw new MqttClientException(MqttClientErrorCode.InvalidFlagBits);
            // get remaining length and allocate buffer
            int remainingLength = MqttMsgBase.decodeRemainingLength(data);
            buffer = new byte[remainingLength];
            // buffer is filled with remaing lenght...
            for (int i = 2, j = 0; j < remainingLength; i++, j++)
            {
                buffer[j] = data[i];
            }

            // message id
            msg.messageId = (ushort)((buffer[index++] << 8) & 0xFF00);
            msg.messageId |= (buffer[index++]);
            CrestronLogger.WriteToLog("PARSE PUBACK SUCCESS", 5);
            return msg;
        }

        public override string ToString()
        {
#if TRACE
            return this.GetTraceString(
                "PUBACK",
                new object[] { "messageId" },
                new object[] { this.messageId });
#else
            return base.ToString();
#endif
        }
    }
}
