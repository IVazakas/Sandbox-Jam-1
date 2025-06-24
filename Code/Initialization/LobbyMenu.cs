using Sandbox;
using Sandbox.Network;

public sealed class LobbyMenu : Component
{
	[Property] public int MaxLobbyMembers { get; set; } = 4;

	public void HostLobby()
	{
		Log.Info("Hosting Lobby...");

		var config = new LobbyConfig()
		{
			Hidden = true,
			Name = "RepWarLobby",
			MaxPlayers = MaxLobbyMembers,
			Privacy = LobbyPrivacy.Public
		};

		Networking.CreateLobby(config);
	}

	/// <summary>
	/// A client is fully connected to the server. This is called on the host.
	/// </summary>
	public void OnActive(Connection channel)
	{
		Log.Info($"Player '{channel.DisplayName}' has started the lobby");

		Log.Info($" Is Network Connecting? {Networking.IsConnecting}");
		Log.Info($" Is Network Active? {Networking.IsActive}");
		Log.Info($" Is Network Host? {Networking.IsHost}");
		Log.Info($" Is Network Client? {Networking.IsClient}");
		Log.Info($" Is This Object a Proxy? {IsProxy}");
	}
}
