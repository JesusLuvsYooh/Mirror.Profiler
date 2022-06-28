using Mirror;
using UnityEngine;

namespace Mirage.NetworkProfiler
{
    [DefaultExecutionOrder(10000)] // last
    public class NetworkProfilerBehaviour : MonoBehaviour
    {
        internal static CountRecorder sentCounter;
        internal static CountRecorder receivedCounter;


        const int frameCount = 300; // todo find a way to get real frame count
        private void Start()
        {
            sentCounter = new CountRecorder(frameCount, Counters.SentMessagesCount, Counters.SentMessagesBytes);
            receivedCounter = new CountRecorder(frameCount, Counters.ReceiveMessagesCount, Counters.ReceiveMessagesBytes);

            NetworkDiagnostics.InMessageEvent += receivedCounter.OnMessage;
            NetworkDiagnostics.OutMessageEvent += sentCounter.OnMessage;
        }
        private void OnDestroy()
        {
            if (receivedCounter != null)
                NetworkDiagnostics.InMessageEvent -= receivedCounter.OnMessage;
            if (sentCounter != null)
                NetworkDiagnostics.OutMessageEvent -= sentCounter.OnMessage;
        }

        private void LateUpdate()
        {
            if (!NetworkServer.active)
                return;

            Counters.PlayerCount.Sample(NetworkServer.connections.Count);
            sentCounter.EndFrame();
            receivedCounter.EndFrame();
            Counters.InternalFrameCounter.Sample(Time.frameCount % frameCount);
        }
    }
}
