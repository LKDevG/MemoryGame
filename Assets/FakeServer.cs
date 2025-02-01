using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SecondTry
{
    public class FakeServer : MonoBehaviour
    {
        private readonly Queue<Packet> messages = new Queue<Packet>();

        private event ReceiveMessage onReceiveMessage = null;

        public delegate void ReceiveMessage(Packet packet);

        private static FakeServer instance = null;

        private float delay = 0 - 1;

        [SerializeField] private Vector2 delaySeconds = new Vector2(0.1f, 0.5f);

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (delay < 0)
            {
                if (messages.Count == 0) return;

                Packet packet = messages.Dequeue();

                if (messages.Count > 0) delay = UnityEngine.Random.Range(delaySeconds.x, delaySeconds.y);

                if (onReceiveMessage != null) onReceiveMessage.Invoke(packet);
            }
            else delay -= Time.deltaTime;
        }

        public static void SendMessage(Packet packet)
        {
            if (instance.messages.Count == 0) instance.delay = UnityEngine.Random.Range(instance.delaySeconds.x, instance.delaySeconds.y);

            instance.messages.Enqueue(packet);
        }

        public static void AddListener(ReceiveMessage listener)
        {
            instance.onReceiveMessage += listener;
        }

        public static void RemoveListener(ReceiveMessage listener)
        {
            instance.onReceiveMessage -= listener;
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
            ScorePlayer,
            TrunPlayer,
        }

        public class Packet
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
        }
    }
}