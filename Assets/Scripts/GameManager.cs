using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using Colyseus.Schema;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    [SerializeField] private ColyseusSettings settings;
    [SerializeField] private GameObject boardGrid;
    [SerializeField] private SlotButton slotButtonPrefab;

    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI localPlayerName;
    [SerializeField] private TextMeshProUGUI remotePlayerName;
    [SerializeField] private NotificationUI notificationUI;


    private ColyseusClient client;

    private ColyseusRoom<TicTacToeRoomState> room;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        StartClient();
        await ConnectToServer();
        RegisterStates();
    }

    private void StartClient()
    {
        client = new ColyseusClient(settings.WebSocketEndpoint);
    }

    protected virtual async Task ConnectToServer(Dictionary<string, object> options = null)
    {
        if (options == null)
            options = new Dictionary<string, object>();

        options.Add("name", string.Format("Player_{0}", Random.Range(0, 1000)));

        room = await client.JoinOrCreate<TicTacToeRoomState>("tic_tac_toe", options);
        CreateGrid(); // Create the grid

        notificationUI.Hide();

        Debug.LogFormat("ROOM JOINED: {0}, CLIENT:{1}", room.Id, room.SessionId);
    }

    private void OnDestroy()
    {
        Debug.Log("GameManager Destroyed");
        if (room != null)
            _ = room.Leave(true);

        UnregisterStates();
    }

    private List<SlotButton> slotButtons = new List<SlotButton>();

    private void CreateGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            SlotButton slotButton = Instantiate(slotButtonPrefab, boardGrid.transform) as SlotButton;
            slotButton.SetIndex(i);
            slotButtons.Add(slotButton);
        }
    }

    private void RegisterStates()
    {
        // Listen to mutation in changes here.
        if (room != null)
        {
            room.State.players.OnAdd += Players_OnAdd;
            room.State.OnChange += State_OnChange;
            room.State.board.OnChange += Board_OnChange;
        }
    }

    public Player LocalPlayer { get; private set; }
    public int LocalPlayerIndex { get; private set; }

    public Player RemotePlayer { get; private set; }
    public int RemotePlayerIndex { get; private set; }

    private void Players_OnAdd(string key, Player player)
    {
        if (key == room.SessionId)
        {
            LocalPlayer = player;
            LocalPlayerIndex = player.playerIndex;
            localPlayerName.text = string.Format ("{0}({1})", player.name, GetPlayerMark(player.playerIndex));
            Debug.LogFormat("Local Player: {0}", key);
        } else
        {
            RemotePlayer = player;
            remotePlayerName.text = string.Format("{0}({1}", player.name, GetPlayerMark(player.playerIndex));
            RemotePlayerIndex = player.playerIndex;
        }

        player.OnChange += (changes) =>
        {
           changes.ForEach((obj) =>
           {
               UpdatePlayer(key, obj);
           });
        };
    }

    private void UpdatePlayer (string key, DataChange obj)
    {
        if (key == room.SessionId && obj.Field == "playerIndex")
        {
            LocalPlayerIndex = Convert.ToInt16(obj.Value);
        } else
        {

        }
    }

    private void State_OnChange(List<DataChange> changes)
    {
        
       changes.ForEach((obj) =>
       {
           switch (obj.Field)
           {
               case "isReady":
                   IsReady = Convert.ToBoolean(obj.Value);
                   
                   break;

               case "currentTurn":
                   CurrentTurn = Convert.ToInt16(obj.Value);
                   break;

               case "winnerIndex":
                   WinnerIndex = Convert.ToInt16(obj.Value);
                   if (WinnerIndex == 2)
                   {
                       notificationUI.ShowText("DRAW");
                   }
                   else if (WinnerIndex != 0)
                        notificationUI.ShowText(string.Format("Winner: {0}", GetPlayerName (WinnerIndex)));
                   break;
           }
       });
    }


    private void Board_OnChange(int key, sbyte value)
    {
        slotButtons[key].SetText(GetPlayerMark(Convert.ToInt16(value)));
    }

    private bool IsReady { get; set; }
    private int CurrentTurn { get; set; }
    private int WinnerIndex { get; set; }


    public void AssignSlot (int slotIndex)
    {
        room.Send((byte)MESSAGE_TYPE.ASSIGN_SLOT, new AssignSlotRequest() { playerIndex = LocalPlayerIndex, slotIndex = slotIndex });
    }

    class AssignSlotRequest
    {
        public int playerIndex;
        public int slotIndex;
    }

    enum MESSAGE_TYPE
    {
        ASSIGN_SLOT = 1
    }

    // Helper methods
    private string GetPlayerName (int playerIndex)
    {
        if (playerIndex == LocalPlayerIndex)
        {
            return LocalPlayer.name;
        }

        return RemotePlayer.name;
    }

    private string GetPlayerMark (int playerIndex)
    {
        if (playerIndex == Defines.PLAYER_1)
        {
            return Defines.PLAYER_1_MARK;
        } else if (playerIndex == Defines.PLAYER_2)
        {
            return Defines.PLAYER_2_MARK;
        }

        return string.Empty;
    }

    private void UnregisterStates()
    {
        // Be responsible and remove the listeners.
        if (room != null)
        {
            room.State.players.OnAdd -= Players_OnAdd;
            room.State.OnChange -= State_OnChange;
            room.State.board.OnChange -= Board_OnChange;

        }
    }
}
