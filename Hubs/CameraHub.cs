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

        Process thermalCamera = null;
        private IConfiguration configuration;
        private readonly IHubContext<CameraHub> hubContext;

        public override async Task OnConnectedAsync()
        {
            var p = new Process();
            p.StartInfo.FileName = configuration["CameraApp"];
            p.StartInfo.Arguments = configuration["CameraAppArgs"];
            p.StartInfo.RedirectStandardOutput = true;

            p.OutputDataReceived += ThermalCamera_OutputDataReceived;

            p.Start();

            p.BeginOutputReadLine();

            await base.OnConnectedAsync();

            thermalCamera = p;
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
            if (thermalCamera != null)
            {
                thermalCamera.Kill();
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
