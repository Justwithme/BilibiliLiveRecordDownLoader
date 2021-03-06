using BilibiliApi.Model.DanmuConf;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BilibiliApi.Clients
{
	public class WssDanmuClient : DanmuClientBase
	{
		protected override string Server => $@"wss://{Host}:{Port}/sub";
		protected override ushort DefaultPort => 443;
		protected override bool ClientConnected => _client?.State == WebSocketState.Open;

		private ClientWebSocket? _client;

		public WssDanmuClient(ILogger<WssDanmuClient> logger, BililiveApiClient apiClient) : base(logger, apiClient) { }

		protected override ushort GetPort(HostServerList server)
		{
			return server.wss_port;
		}

		protected override IDisposable CreateClient()
		{
			_client = new();
			return _client;
		}

		protected override async ValueTask ClientHandshakeAsync(CancellationToken token)
		{
			await _client!.ConnectAsync(new(Server), token);
		}

		protected override async ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
		{
			await _client!.SendAsync(buffer, WebSocketMessageType.Binary, true, token);
		}

		protected override async ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken token)
		{
			var rcvResult = await _client!.ReceiveAsync(buffer, token);
			return rcvResult.Count;
		}
	}
}
