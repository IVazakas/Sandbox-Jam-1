using Sandbox;
using Sandbox.Network;
using Sandbox.UI;
using Sandbox.Utility;
using System.Threading.Tasks;


public sealed class LobbyManager : Component
{
	[Property] public int MaxLobbyMembers { get; set; } = 8;

	[RequireComponent, Property] public ScreenPanel Root { get; set; }

	protected override void OnAwake()
	{
	}


	public void HostLobby()
	{
		Log.Info( "Hosting Lobby..." );

		var config = new LobbyConfig()
		{
			Hidden = true,
			Name = "RepWarLobby.test",
			MaxPlayers = MaxLobbyMembers,
			Privacy = LobbyPrivacy.Private
		};

		//Networking.CreateLobby( config );
	}

	
}
