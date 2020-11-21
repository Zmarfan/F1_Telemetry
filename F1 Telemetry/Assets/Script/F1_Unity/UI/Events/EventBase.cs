using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public abstract class EventBase : MonoBehaviour
    {
        public abstract void Init(Packet packet);
    }
}