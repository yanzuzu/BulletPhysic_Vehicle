using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.Networking;

public class NetworkMatchControl : MonoBehaviour {
	public static NetworkMatchControl Instance;

	public delegate void OnRefreshRoomCb();
	public delegate void OnCreateRoomCb();
	public delegate void OnAddRoomCb();

	private OnRefreshRoomCb m_refreshRoomCb;
	private OnCreateRoomCb m_createRoomCb;
	private OnAddRoomCb m_addRoomCb;

	private NetworkMatch m_networkMatch;

	public  List<ulong> AllRooms = new List<ulong>();

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		m_networkMatch = gameObject.AddComponent<NetworkMatch> ();
	}

	public void RefreshRoom(OnRefreshRoomCb cb)
	{
		m_refreshRoomCb = cb;
		m_networkMatch.ListMatches (0, 20, "", OnMatchList);
	}

	public void OnMatchList(ListMatchResponse matchListResponse)
	{
		AllRooms.Clear ();
		if (matchListResponse.success && matchListResponse.matches != null)
		{
			for (int i = 0; i < matchListResponse.matches.Count; i++) {
				AllRooms.Add ((ulong) matchListResponse.matches [i].networkId);
			}

		}
		if (m_refreshRoomCb != null)
			m_refreshRoomCb ();
	}

	public void CreateRoom(OnCreateRoomCb cb)
	{
		m_createRoomCb = cb;

		CreateMatchRequest create = new CreateMatchRequest();
		create.name = "test";
		create.size = 10;
		create.advertise = true;
		create.password = "";
		m_networkMatch.CreateMatch(create, OnMatchCreate);

	}

	public void OnMatchCreate(CreateMatchResponse matchResponse)
	{
		if (matchResponse.success)
		{
			// creat server
			//ServIns.RbServer.Init (new MatchInfo(matchResponse));
			AllRooms.Add((ulong)matchResponse.networkId);

			if( m_createRoomCb != null )
			{
				m_createRoomCb();
			}
		}

	}

	public void AddRoom(ulong networkID, OnAddRoomCb cb)
	{
		m_addRoomCb = cb;
		m_networkMatch.JoinMatch ((NetworkID)networkID, "", OnMatchJoined);
	}

	public void OnMatchJoined(JoinMatchResponse matchJoin)
	{
		if (matchJoin.success)
		{
			Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
			//ServIns.RbClientService.Init (new MatchInfo(matchJoin));
			if (m_addRoomCb != null) {
				m_addRoomCb ();
			}
		}

	}
}
