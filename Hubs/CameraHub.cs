using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace thermalCamera.Hubs
{
    public class CameraHub : Hub
    {
        public CameraHub(IConfiguration configuration, IHubContext<CameraHub> hubContext)
        {
            this.configuration = configuration;
            this.hubContext = hubContext;
        }

        Process? thermalCamera = null;
        private IConfiguration configuration;
        private readonly IHubContext<CameraHub> hubContext;

        static List<string> connectionIds = new();
        static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public override async Task OnConnectedAsync()
        {
            try
            {
                await semaphore.WaitAsync();

                connectionIds.Add(Context.ConnectionId);

                await base.OnConnectedAsync();

                if (connectionIds.Count == 1)
                {
                    var p = new Process();
                    p.StartInfo.FileName = configuration["CameraApp"];
                    p.StartInfo.Arguments = configuration["CameraAppArgs"];
                    p.StartInfo.RedirectStandardOutput = true;

                    p.OutputDataReceived += ThermalCamera_OutputDataReceived;

                    p.Start();

                    p.BeginOutputReadLine();

                    thermalCamera = p;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        List<string> values = new List<string>();

        private async void ThermalCamera_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            switch (e.Data)
            {
                case "frame start":
                    values = new List<string>();
                    break;
                case "frame done":
                    var frame = new Frame()
                    {
                        Pixels = values,
                        Min = values.Min(float.Parse).ToString(),
                        Max = values.Max(float.Parse).ToString(),
                    };
                    await hubContext.Clients.All.SendAsync("ReceiveFrame", frame);
                    break;
                case null:
                    break;
                default:
                    values.Add(e.Data);
                    break;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                await semaphore.WaitAsync();
                connectionIds.Remove(Context.ConnectionId);
                await base.OnDisconnectedAsync(exception);

                if (connectionIds.Count == 0 && thermalCamera != null)
                {
                    thermalCamera.Kill();
                    thermalCamera = null;
                }                
            }
            finally
            {
                semaphore.Release();
            }

        }
    }
}
