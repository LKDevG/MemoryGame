using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;


namespace OpenCard
{
    public class FakeServer : MonoBehaviour
    {
        private readonly Queue<Packet> messageToServer = new Queue<Packet>();
        private readonly Queue<Packet> messagesToClient = new Queue<Packet>();

        private static readonly UnityEvent<Packet> serverEventMessage = new UnityEvent<Packet>();
        private static readonly UnityEvent<Packet> clientEventMessage = new UnityEvent<Packet>();


        [Obsolete("Removed")]
        public delegate void ReceiveMessage(Packet packet);

        private static FakeServer instance = null;

        private float delay = 0 - 1;
        private float serverdelay = 0 - 1;
        private bool isDestroyed;


        [SerializeField] private Vector2 delaySeconds = new Vector2(0.1f, 0.5f);

        private void Awake()
        {
            instance = this;
            StartCoroutine(UpdatePacket());
            CreateServerClientTest();
        }

        public void OnDestroy()
        {
            isDestroyed = true;
        }

        private IEnumerator UpdatePacket()
        {
            while (!isDestroyed)
            {
                yield return new WaitForEndOfFrame();
                if (messagesToClient.Count > 0)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(instance.delaySeconds.x, instance.delaySeconds.y));
                    clientEventMessage?.Invoke(messagesToClient.Dequeue());
                }
                if (messageToServer.Count > 0)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(instance.delaySeconds.x, instance.delaySeconds.y));
                    serverEventMessage?.Invoke(messageToServer.Dequeue());
                }
            }
        }

        /// <summary>
        /// For Client side use only
        /// </summary>
        /// <param name="listener"></param>
        public static void SendMessageToServer(Packet packet)
        {
            if (instance.messageToServer.Count == 0)
            {
                instance.serverdelay = UnityEngine.Random.Range(instance.delaySeconds.x, instance.delaySeconds.y);
            }

            instance.messageToServer.Enqueue(packet);
        }

        /// <summary>
        /// For Server side use only
        /// </summary>
        /// <param name="listener"></param>
        public static void SendMessageToClient(Packet packet)
        {
            if (instance.messagesToClient.Count == 0)
            {
                instance.delay = UnityEngine.Random.Range(instance.delaySeconds.x, instance.delaySeconds.y);
            }

            instance.messagesToClient.Enqueue(packet);
        }

        [Obsolete("Remove (replaced with SendMessageToServer and SendMessageToClient)")]
        public static void SendMessage(Packet packet)
        {
        }

        [Obsolete("Remove (replaced with AddListenerSendToServer and AddListenerSendToClient)")]
        public static void AddListener(ReceiveMessage listener)
        {
        }

        [Obsolete("Remove (replaced with RemoveListenerSendToServer and RemoveListenerSendToClient)")]
        public static void RemoveListener(ReceiveMessage listener)
        {
        }

        /// <summary>
        /// For Client side use only
        /// </summary>
        /// <param name="listener"></param>
        public static void AddListenerClientReadFromServer(UnityAction<Packet> listener)
        {
            clientEventMessage.AddListener(listener);
        }

        /// <summary>
        /// For Server side use only
        /// </summary>
        /// <param name="listener"></param>
        public static void AddListenerServerReadFromClient(UnityAction<Packet> listener)
        {
            serverEventMessage.AddListener(listener);
        }

        /// <summary>
        /// For Client side use only
        /// </summary>
        /// <param name="listener"></param>
        public static void RemoveListenerClientReadFromServer(UnityAction<Packet> listener)
        {
            clientEventMessage.RemoveListener(listener);
        }

        /// <summary>
        /// For Server side use only
        /// </summary>
        /// <param name="listener"></param>
        public static void RemoveListenerServerReadFromClient(UnityAction<Packet> listener)
        {
            serverEventMessage.RemoveListener(listener);
        }

        public enum Command
        {
            Player,
            BoxData,
            SetCard = 2001,//โน็ดบอกเป็นเลขได้
            DelayCloseCard = 2002,
            test0001,
            test0002,

            //Card
            OpenCard,
            RequestPlayerName,
            Gencard,
            Updatecard,
            ProcessNumber,
            SendUID,
            RequestOpenCard,
            GameStart,
            TrunPlayer,
            DestroyCard,
            SendUdiAgain,
            UpdateScore,
            RequestUID,
            SendTrun,
            AddScore,
            RequestResetCard,
            RequestScore,
            SendScore,
            SendPlayerWin,
        }

        public class Packet : ICloneable
        {
            private readonly int command = 0;

            private int position = 0;

            private readonly MemoryStream data = null;

            public Command GetCommand()
            {
                return (Command)command;
            }

            public Packet(Command command)
            {
                this.command = (int)command;

                position = 0;

                data = new MemoryStream();
            }

            private Packet(int command)
            {
                this.command = command;

                position = 0;

                data = new MemoryStream();
            }

            public bool ReadBool()
            {
                return ReadByte() == 1;
            }

            public byte ReadByte()
            {
                byte value = data.GetBuffer()[position];
                position++;
                return value;
            }

            public int ReadInt()
            {
                int value = BitConverter.ToInt32(data.GetBuffer(), position);
                position += 4;
                return value;
            }

            public float ReadFloat()
            {
                float value = BitConverter.ToSingle(data.GetBuffer(), position);
                position += 4;
                return value;
            }

            public void WriteBool(bool value)
            {
                WriteByte((byte)(value ? 1 : 0));
            }

            public void WriteByte(byte value)
            {
                data.WriteByte(value);
            }

            public void WriteInt(int value)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                data.Write(bytes, 0, bytes.Length);
            }

            public void WriteFloat(float value)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                data.Write(bytes, 0, bytes.Length);
            }

            public void WriteShort(short value)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                data.Write(bytes, 0, bytes.Length);
            }

            public short ReadShort()
            {
                short value = BitConverter.ToInt16(data.GetBuffer(), position);
                position += 2;
                return value;
            }

            public string ReadString()
            {
                int stringSize = ReadShort(); // read size short first.
                string value = Encoding.UTF8.GetString(data.GetBuffer(), position, stringSize);
                position += stringSize;
                return value;
            }

            public void WriteString(string value)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);

                WriteShort((short)bytes.GetLength(0));
                data.Write(bytes, 0, bytes.Length);
            }

            public Packet Clone()
            {
                return MemberwiseClone() as Packet;
            }

            object ICloneable.Clone()
            {
                return MemberwiseClone() as Packet;
            }
        }

#if UNITY_EDITOR
        private SignalManager serverTest;
        private SignalManager clientTest;


        [ContextMenu("PacketTest")]
        public void PacketTest()
        {
            string text = "Hellooo";
            Packet packet = new Packet(Command.Player);
            packet.WriteString(text);

            if (text == packet.ReadString())
            {
                Debug.LogWarning("Success String");
            }
            else
            {
                Debug.LogError("Fail");
            }

            packet.WriteShort(10);
            if (10 == packet.ReadShort())
            {
                Debug.LogWarning("Success Short");
            }
            else
            {
                Debug.LogError("Fail");
            }

            //Chained
            packet.WriteShort(10);
            packet.WriteByte(1);
            packet.WriteInt(468);
            packet.WriteBool(true);
            packet.WriteString("Hi1234");


            string result = $"{packet.ReadShort()} {packet.ReadByte()} {packet.ReadInt()} {packet.ReadBool()} {packet.ReadString()}";
            Debug.LogWarning(result);
        }


        /// <summary>
        /// Implementing example
        /// </summary>
        [ContextMenu("CreateServerClientTest")]
        public void CreateServerClientTest()
        {
            //server side
            serverTest = new SignalManager(true);
            serverTest.AddCommandAction(Command.BoxData, (packet) => Debug.LogWarning("ReceiveBoxData sent from client")); //
            serverTest.AddCommandAction(Command.test0002, (packet) => Debug.LogWarning("ReceiveDelaytest0002 sent from client"));
            serverTest.AddCommandAction(Command.SendUID, (packet) => Debug.LogWarning("SendUID sent from client"));
            serverTest.AddCommandAction(Command.RequestPlayerName, (packet) => Debug.LogWarning("RequestPlayerName sent from client"));

            serverTest.AddCommandAction(Command.ProcessNumber, (packet) => Debug.LogWarning("ProcessNumber sent from client"));
            serverTest.AddCommandAction(Command.SendUdiAgain, (packet) => Debug.LogWarning("SendUdiAgain sent from client"));
            serverTest.AddCommandAction(Command.SendTrun, (packet) => Debug.LogWarning("SendTrun sent from client"));


            //client side
            clientTest = new SignalManager(false);
            clientTest.AddCommandAction(Command.SetCard, (packet) => Debug.LogWarning("ReceiveSetCard  sent from server"));
            clientTest.AddCommandAction(Command.GameStart, (packet) => Debug.LogWarning("GameStart sent from server"));
            clientTest.AddCommandAction(Command.TrunPlayer, (packet) => Debug.LogWarning("TrunPlayer sent from server"));
            clientTest.AddCommandAction(Command.DelayCloseCard, (packet) => Debug.LogWarning("DelayCloseCard sent from server"));
            clientTest.AddCommandAction(Command.DestroyCard, (packet) => Debug.LogWarning("DestroyCard sent from server"));
            clientTest.AddCommandAction(Command.Updatecard, (packet) => Debug.LogWarning("Updatecard sent from server"));
            clientTest.AddCommandAction(Command.UpdateScore, (packet) => Debug.LogWarning("UpdateScore sent from server"));
            clientTest.AddCommandAction(Command.AddScore, (packet) => Debug.LogWarning("AddScore sent from server"));
            clientTest.AddCommandAction(Command.RequestOpenCard, (packet) => Debug.LogWarning("RequestOpenCard sent from server"));
            clientTest.AddCommandAction(Command.RequestResetCard, (packet) => Debug.LogWarning("RequestResetCard sent from server"));
        }

#endif
    }


    [Serializable]
    public class SignalManager
    {
        private readonly Dictionary<FakeServer.Command, UnityAction<FakeServer.Packet>> commandDict = new Dictionary<FakeServer.Command, UnityAction<FakeServer.Packet>>();
        private readonly bool isServer;
        public List<FakeServer.Command> activeCommandList = new List<FakeServer.Command>();

        public SignalManager(bool isServer)
        {
            this.isServer = isServer;
            if (isServer)
            {
                FakeServer.AddListenerServerReadFromClient(OnReceiveMessage);
            }
            else
            {
                FakeServer.AddListenerClientReadFromServer(OnReceiveMessage);
            }
        }

        /// <summary>
        /// Call when unused
        /// </summary>
        public void Destroy()
        {
            if (isServer)
            {
                FakeServer.RemoveListenerServerReadFromClient(OnReceiveMessage);
            }
            else
            {
                FakeServer.RemoveListenerClientReadFromServer(OnReceiveMessage);
            }
        }

        public void SendMessage(FakeServer.Packet packet)
        {
            if (isServer)
            {
                Debug.Log($"<Color=green>Server {nameof(FakeServer.SendMessageToClient)} </color>" + packet.GetCommand());
                FakeServer.SendMessageToClient(packet);
            }
            else
            {
                Debug.Log($"<Color=blue>Client {nameof(FakeServer.SendMessageToServer)} </color>" + packet.GetCommand());
                FakeServer.SendMessageToServer(packet);
            }
        }

        private void OnReceiveMessage(FakeServer.Packet packet)
        {
            if (isServer)
            {
                Debug.Log($"<Color=yellow>Server Received message </color>" + packet.GetCommand());
            }
            else
            {
                Debug.Log($"<Color=red>Client Received message </color>" + packet.GetCommand());
            }

            if (commandDict.TryGetValue(packet.GetCommand(), out UnityAction<FakeServer.Packet> unityAction))
            {
                unityAction?.Invoke(packet.Clone());
            }
            else
            {
                Debug.LogError($"Command {packet.GetCommand()} is not added");
            }
        }

        /// <summary>
        /// A command paired with an action
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandAction"></param>
        public void AddCommandAction(FakeServer.Command command, UnityAction<FakeServer.Packet> commandAction)
        {
            commandDict.Add(command, commandAction);
            activeCommandList.Add(command);
        }
    }
}