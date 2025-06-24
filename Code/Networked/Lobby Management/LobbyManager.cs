using Sandbox;
using Sandbox.Network;
using System;

namespace Auralis;

[Title("Lobby Manager")]
[Category("Networking")]
[Icon("electrical_services")]
public sealed class LobbyManager : Component, Component.INetworkListener
{
	[Property] public List<GameObject> SpawnPoints { get; set; }
	[Property] public GameObject PlayerPrefab { get; set; }
	
	// Create SpawnPoints Queue so that as players leave / join slots are available and can easily be dequeued
	// private Queue<GameObject> _availableSpawnpoints { get; set; }

	/// <summary>
	/// A client is fully connected to the server. This is called on the host.
	/// </summary>
	public void OnActive(Connection channel)
	{
		Log.Info($"Player '{channel.DisplayName}' has joined the game");

		if ( !PlayerPrefab.IsValid() )
			return;

		//
		// Find a spawn location for this player
		//
		var startLocation = FindSpawnLocation().WithScale(1);

		// Spawn this object and make the client the owner
		var player = PlayerPrefab.Clone(startLocation, name: $"Player - {channel.DisplayName}");
		player.NetworkSpawn(channel);

	}

	/// <summary>
	/// Find the most appropriate place to spawn
	/// </summary>
	Transform FindSpawnLocation()
	{
		//
		// If they have spawn point set then use those
		//
		if ( SpawnPoints is not null && SpawnPoints.Count > 0 )
		{
			return Random.Shared.FromList(SpawnPoints, default).WorldTransform;
		}

		//
		// If we have any SpawnPoint components in the scene, then use those
		//
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToArray();
		if ( spawnPoints.Length > 0 )
		{
			return Random.Shared.FromArray(spawnPoints).WorldTransform;
		}

		//
		// Failing that, spawn where we are
		//
		return WorldTransform;
	}

}

