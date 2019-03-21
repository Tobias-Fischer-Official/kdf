using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KDF.Networks.GameServer;
using System.Collections;
using KDF.Networks.Protocol.Module;
using KDF.Networks.Core;
using System.Diagnostics;
using KDF.ClientEventArgs;

namespace KDF.Games.Poker
{
    public class PokerGame : GameBase
    {
        #region public Vars
        public string ChannelName { get; set; }
        public List<PokerPlayer> Players { get; set; }
        public PokerPlayer ClientPlayer { get; set; }
        public PokerPlayer ActivePlayer { get; set; }

        public int RoundCounter { get; set; }
        public bool Registered { get; set; }

        public List<PokerCard> Hand { get; set; }
        public List<PokerCard> HoleCards { get; set; }

        public GSClient GameServerClient { get; set; }

        public Action<double, double, PokerPlayer> PlayerBet;
        public Action<double, double, PokerPlayer> PlayerRaise;
        public Action<PokerPlayer> PlayerFold;
        public Action<double, double, PokerPlayer> PlayerCall;
        public Action GameStarted;
        public Action<int, int, int> GameEnded;
        public Action<bool> GamsteRegistrationStatusChanged;
        public Action<List<PokerCard>> Flop;
        public Action<PokerCard> Turn;
        public Action<PokerCard> River;
        public Action<List<PokerCard>> HandReceived;
        public Action RestrationPossible;
        private bool _registrationPossible;
        #endregion

        #region private Vars
        private KnuddelsClient _client;
        private string _clickMsgRegister;
        private string _clickMsgUnregister;

        private double _turnActionControllerId;
        private double _turnCallAmount;
        private double _turnMinBetAmount;
        private double _turnPot;
        private double _turnPlayerStack;

        private long _sitOutId;
        private bool _isSitout = false;

        private double _lastMoney;

        private bool _isNineTable;
        #endregion

        public PokerGame(GSClient CardClient)
        {
            _client = CardClient.Client;

            GameServerClient = CardClient;
            GameServerClient.OnConnectionStateChanged += CardserverClient_OnConnectionStateChanged;
            GameServerClient.OnModuleReceived += CardserverClient_OnModuleReceived;
            GameServerClient.Connect(GameServerClient.ServerHost, GameServerClient.ServerPort, _client.ProxyHost, _client.ProxyPort);
            
            Hand = new List<PokerCard>();
            HoleCards = new List<PokerCard>();
            Players = new List<PokerPlayer>();

            ClientPlayer = new PokerPlayer();
        }

        void CardserverClient_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.Connected && !e.LoggedIn)
                GameServerClient.Login();
        }

        void CardserverClient_OnModuleReceived(object sender, ModuleReceivedEventArgs e)
        {
            try
            {
                ArrayList fillTextLabels;
                ArrayList textLabelInfos;

                switch (e.Module.Name)
                {
                    #region LOGGIN_PROCESSED
                    case "LOGGIN_PROCESSED":
                        if (GameServerClient.LoggedIn)
                            GameServerClient.JoinRoom();
                        break;
                    #endregion
                    #region OUT_OF_TURN_CONTROLS
                    case "OUT_OF_TURN_CONTROLS":
                        foreach (KnModule regularButton in e.Module.GetValue<ArrayList>("REGULAR_BUTTON"))
                        {
                            string btnText = regularButton.GetValue<KnModule>("FULL_BUTTON_SETTINGS").GetValue<string>("TEXT");
                            if (btnText.Contains("Anmelden"))
                            {
                                _clickMsgRegister = regularButton.GetValue<string>("CLICK_MSG");
                                _registrationPossible = true;
                                if (RestrationPossible != null)
                                    RestrationPossible();
                            }
                            else if (btnText.Contains("Abmelden"))
                            {
                                _clickMsgUnregister = regularButton.GetValue<string>("CLICK_MSG");
                                _registrationPossible = false;
                                GamsteRegistrationStatusChanged(true);
                                Console.WriteLine("[Info] Erfolgreich angemeldet");
                            }
                        }
                        break;
                    #endregion
                    #region TURN_CHANGES
                    case "TURN_CHANGES":
                        #region FILL_TEXT_LABEL
                        fillTextLabels = e.Module.GetValue<ArrayList>("FILL_TEXT_LABEL");
                        textLabelInfos = e.Module.GetValue<ArrayList>("TEXT_LABEL");
                        if (fillTextLabels.Count != textLabelInfos.Count)
                            break;

                        for (int i = 0; i < fillTextLabels.Count; i++)
                        {
                            KnModule textLabel = (KnModule)fillTextLabels[i];
                            KnModule labelInfo = (KnModule)textLabelInfos[i];
                        
                            string labelText = textLabel.GetValue<string>("TEXT");
                            if (labelText.StartsWith("TotalPot#"))
                            {
                                string potSize = labelText.Split('#')[1].Replace(".", ",");
                                if (potSize.StartsWith("$ "))
                                    potSize = potSize.Substring(2);

                                _turnPot = double.Parse(potSize);
                                Console.WriteLine("[INFO] Aktueller Pot: " + _turnPot);
                            }
                            else if (labelText.StartsWith("Deine Tickets"))
                            {
                                ClientPlayer.Tickets = int.Parse(labelText.Split(' ')[2]);
                                Console.WriteLine("[INFO] Eigene Tickets: " + ClientPlayer.Tickets);
                            }
                            else if (labelText.Contains("$ ") && labelText.Contains(".") && !labelText.Contains("#"))
                            {
                                labelText = labelText.Substring(2).Replace(".", ",");
                                
                                double.TryParse(labelText, out _lastMoney);
                            }
                            else if (labelText.Contains("#$ "))
                            {
                                string[] splitText = labelText.Split('#');

                                string playerStack = splitText[1].Replace(".", ",");
                                if (playerStack.StartsWith("$ "))
                                    playerStack = playerStack.Substring(2);

                                var pokerPlayer = GetPlayerFromId(textLabel.GetValue<short>("COMPONENT_ID"));
                                var playerPosition = labelInfo.GetValue<KnModule>("BASE_COMPONENT").GetValue<KnModule>("POSITION");
                                if (pokerPlayer != null)
                                {
                                    pokerPlayer.Stack = double.Parse(playerStack);
                                }
                                else
                                {
                                    pokerPlayer = new PokerPlayer()
                                    {
                                        Username = splitText[0],
                                        Stack = double.Parse(playerStack),
                                        Position = new System.Drawing.Point(playerPosition.GetValue<short>("POS_X"), playerPosition.GetValue<short>("POS_Y")),
                                        ComponentId = textLabel.GetValue<short>("COMPONENT_ID"),
                                        TablePosition = PlayerSeatFromPosition(new System.Drawing.Point(playerPosition.GetValue<short>("POS_X"), playerPosition.GetValue<short>("POS_Y")), _isNineTable)
                                    };

                                    if (splitText[0] == _client.ClientUser.Username)
                                    {
                                        ClientPlayer = pokerPlayer;
                                    }
                                    else
                                    {
                                        Players.Add(pokerPlayer);
                                    }
                                }
                                Console.WriteLine("[TableInfo] Update Player | Username: " + pokerPlayer.Username + ", Stack: " + pokerPlayer.Stack);
                            }
                        }
                        #endregion

                        #region ZIMAGE
                        ArrayList zImageList = e.Module.GetValue<ArrayList>("ZIMAGE");
                        foreach (KnModule zImage in zImageList)
                        {
                            string imgName = zImage.GetValue<string>("IMAGE").Split('/')[1];
                            KnModule imgPos = zImage.GetValue<KnModule>("BASE_COMPONENT").GetValue<KnModule>("POSITION");

                            int x = imgPos.GetValue<short>("POS_X");
                            int y = imgPos.GetValue<short>("POS_Y");

                            if (imgName.Contains("timecircle_"))
                            {
                                if (y > 300)
                                    y += 45;
                                else
                                    y -= 52;

                            }
                            else
                            {
                                //Holecards
                                if (Hand.Count == 2 && HoleCards.Count < 5)
                                {
                                    PokerCard cardReceived = CardParser.Parse(imgName);
                                    HoleCards.Add(cardReceived);
                                    //Flop
                                    if (x == -1 && y == 2 || x == 63 && y == 2 || x == 127 && y == 2)
                                    {
                                        if (Flop != null && HoleCards.Count == 3)
                                            Flop(HoleCards);
                                    }
                                    //Turn
                                    else if (x == 191 && y == 2)
                                    {
                                        if (Turn != null)
                                            Turn(cardReceived);
                                    }
                                    //River
                                    else if (x == 255 && y == 2)
                                    {
                                        if (River != null)
                                            River(cardReceived);
                                    }
                                }
                                //Hand
                                else if (Hand.Count < 2)
                                {
                                    Hand.Add(CardParser.Parse(imgName));
                                    if (HandReceived != null && Hand.Count == 2)
                                        HandReceived(Hand);
                                }
                            }

                            if (ActivePlayer != null)
                            {
                                if (imgName.Contains("action_call"))
                                {
                                    Console.WriteLine("[TableInfo][Action] " + ActivePlayer.Username + " callt (" + _lastMoney + ")");
                                }
                                else if (imgName.Contains("action_check"))
                                {
                                    Console.WriteLine("[TableInfo][Action] " + ActivePlayer.Username + " checkt");
                                }
                                else if (imgName.Contains("action_bet"))
                                {
                                    Console.WriteLine("[TableInfo][Action] " + ActivePlayer.Username + " bet (" + _lastMoney + ")");
                                    ActivePlayer.Bet = _lastMoney;
                                }
                                else if (imgName.Contains("action_fold"))
                                {
                                    Console.WriteLine("[TableInfo][Action] " + ActivePlayer.Username + " foldet");
                                    ActivePlayer.Folded = true;
                                }
                                else if (imgName.Contains("action_raise"))
                                {
                                    Console.WriteLine("[TableInfo][Action] " + ActivePlayer.Username + " raised (" + _lastMoney + ")");
                                    ActivePlayer.Bet = _lastMoney;
                                }
                                else if (imgName.Contains("action_bigblind"))
                                {
                                    Console.WriteLine("[TableInfo][Action] " + ActivePlayer.Username + " set bigblind");
                                }
                                else if (imgName.Contains("action_smallblind"))
                                {
                                    Console.WriteLine("[TableInfo][Action] " + ActivePlayer.Username + " set smallblind");
                                }
                                else if (imgName.Contains("action_sitout"))
                                {
                                    if (y > 300)
                                        y += 45;
                                    else
                                        y -= 52;
                                    ActivePlayer = GetPlayerFromPosition(x, y);
                                    if (ActivePlayer != null)
                                        Console.WriteLine("[TableInfo][Action] " + ActivePlayer.Username + " sitout");
                                }
                            }
                        }
                        #endregion
                        break;
                    #endregion
                    #region ADD_TO_TEXT_LOG
                    case "ADD_TO_TEXT_LOG":
                        string logText = e.Module.GetValue<string>("TEXT");
                        if (logText.Contains("|/serverpp \"|/w \""))
                        {
                            string nickname = logText.Substring(logText.IndexOf('h'));
                            nickname = nickname.Substring(1, nickname.IndexOf("|/serverpp") - 1);
                            ActivePlayer = GetPlayerFromName(nickname);
                            if (ActivePlayer != null)
                                Console.WriteLine("[Info] Active Player " + ActivePlayer.Username);

                            Console.WriteLine("[Info-Log] " + logText);
                        }
                        break;
                    #endregion
                    #region ROOM_INIT
                    case "ROOM_INIT":
                        Players.Clear();
                        _isNineTable = !e.Module.GetValue<string>("CHANNEL_NAME").Contains("6max");

                        fillTextLabels = e.Module.GetValue<ArrayList>("FILL_TEXT_LABEL");
                        textLabelInfos = e.Module.GetValue<ArrayList>("TEXT_LABEL");
                        if (fillTextLabels.Count != textLabelInfos.Count)
                            break;

                        for (int i = 0; i < fillTextLabels.Count; i++)
                        {
                            KnModule textLabel = (KnModule)fillTextLabels[i];
                            KnModule labelInfo = (KnModule)textLabelInfos[i];

                            string labelText = textLabel.GetValue<string>("TEXT");
                            Console.WriteLine(labelText);
                            if (labelText.Contains("#$ ") || labelText.EndsWith("â­"))
                            { //Nickname
                                if (!labelText.Contains("#"))
                                    break;

                                string[] splitText = labelText.Split('#');

                                if (splitText.Contains("TotalPot"))
                                    continue;

                                string playerStack = splitText[1].Replace(".", ",");
                                if (playerStack.StartsWith("$ "))
                                    playerStack = playerStack.Substring(2);
                                else
                                    playerStack = playerStack.Split(' ')[0];

                                var playerPosition = labelInfo.GetValue<KnModule>("BASE_COMPONENT").GetValue<KnModule>("POSITION");
                                var pokerPlayer = new PokerPlayer()
                                {
                                    Username = splitText[0],
                                    Position = new System.Drawing.Point(playerPosition.GetValue<short>("POS_X"), playerPosition.GetValue<short>("POS_Y")),
                                    Stack = double.Parse(playerStack),
                                    ComponentId = textLabel.GetValue<short>("COMPONENT_ID"),
                                    TablePosition = PlayerSeatFromPosition(new System.Drawing.Point(playerPosition.GetValue<short>("POS_X"), playerPosition.GetValue<short>("POS_Y")), _isNineTable)
                                };

                                if (splitText[0] == _client.ClientUser.Username)
                                {
                                    ClientPlayer = pokerPlayer;
                                }
                                else
                                {
                                    Players.Add(pokerPlayer);
                                }
                            }
                            else if (labelText.StartsWith("#Gewinne:#"))
                            {
                                string[] prices = labelText.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string price in prices)
                                    Console.WriteLine(price);
                            }
                        }
                        CalcPositions();
                        Console.WriteLine("[TableInfo] Players: ");
                        foreach (PokerPlayer player in Players)
                            Console.WriteLine("\t{ Username: " + player.Username + ", Stack: " + player.Stack + ", X: " + player.Position.X + ", Y: " + player.Position.Y + " }");
                        break;
                    #endregion
                    #region OUT_OF_GAME_FLEXFRAME
                    case "OUT_OF_GAME_FLEXFRAME":
                        //Hier wird ein Inagme-Pop-Up angezeigt

                        string text = e.Module.GetValue<string>("TEXT").ToLower();
                        //Keine Anmeldung möglich
                        if (text.Contains("du kannst dich nicht anmelden"))
                        {

                        }

                        //Du hast das Spiel als Xter beendet / gewonnen
                        //Punktzahl
                        //Gewinn
                        //Knuddels
                        //Tickets
                        else if (text.Contains("punkte"))
                        {

                        }
                        break;
                    #endregion
                    #region DIRECT_CONTROLS
                    case "DIRECT_CONTROLS":
                        long CLICK_BET_INCREMENT = e.Module.GetValue<KnModule>("CLICK_BET_INCREMENT").GetValue<long>("MONEY_AMOUNT");
                        long CALL_DISPLAY_AMOUNT = e.Module.GetValue<KnModule>("CALL_DISPLAY_AMOUNT").GetValue<long>("MONEY_AMOUNT");
                        long MIN_BET_AMOUNT = e.Module.GetValue<KnModule>("MIN_BET_AMOUNT").GetValue<long>("MONEY_AMOUNT");
                        long MAX_BET_AMOUNT = e.Module.GetValue<KnModule>("MAX_BET_AMOUNT").GetValue<long>("MONEY_AMOUNT");
                        long SELECTED_BET_AMOUNT = e.Module.GetValue<KnModule>("SELECTED_BET_AMOUNT").GetValue<long>("MONEY_AMOUNT");
                        byte BET_TYPE = e.Module.GetValue<byte>("BET_TYPE"); //KP ?
                        bool MAX_IS_ALLIN = e.Module.GetValue<bool>("MAX_IS_ALLIN");

                        _turnActionControllerId = e.Module.GetValue<long>("CONTROLLER_ID");
                        _turnCallAmount = e.Module.GetValue<KnModule>("CALL_AMOUNT").GetValue<long>("MONEY_AMOUNT");
                        _turnMinBetAmount = e.Module.GetValue<KnModule>("MIN_BET_AMOUNT").GetValue<long>("MONEY_AMOUNT");
                        //_turnPot
                        //_turnPlayerStack

                        break;
                    #endregion
                    #region SIT_OUT_CONTROLS
                    case "SIT_OUT_CONTROLS":
                        ArrayList sitOutCheckBoxList = e.Module.GetValue<ArrayList>("SIT_OUT_CHECKBOX");
                        if (sitOutCheckBoxList == null || sitOutCheckBoxList.Count <= 0)
                            break;

                        KnModule sitOutCheckBox = (KnModule)sitOutCheckBoxList[0];
                        if (sitOutCheckBox.GetValue<long>("SIT_OUT_STATE") == 0)
                        {
                            _sitOutId = long.Parse(sitOutCheckBox.GetValue<KnModule>("DISABLE_MSG").GetValue<string>("CLICK_MSG"));
                            _isSitout = true;
                        }
                        else
                        {
                            _sitOutId = long.Parse(sitOutCheckBox.GetValue<KnModule>("ENABLE_MSG").GetValue<string>("CLICK_MSG"));
                            _isSitout = false;
                        }
                        break;
                    #endregion
                    default:
                        Debug.WriteLine("Unknown: " + e.Module.Name);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        #region Actions
        private void Action(long action)
        {
            KnModule longController = GameServerClient.ParentModule.CreateModule("LONG_CONTROLLER");
            longController.Add("CONTROLLER_ID", _turnActionControllerId);
            longController.Add("LONG", action);
            GameServerClient.Send(longController);
        }

        public void Bet(int amount)
        {
            Action(amount);
        }

        public void Call(long amount)
        {
            Action(amount);
        }

        public void Raise(long amount)
        {
            Action(amount);
        }

        public void Fold()
        {
            Action(-1L);
        }

        public void Check()
        {
            Action(0L);
        }
        #endregion

        private void CalcPositions()
        {
            if (ClientPlayer != null)
                for (int i = 0; i < Players.Count; i++)
                {
                    if (Players[i] == ClientPlayer)
                    {
                        if (i != 0)
                        {
                            for (int j = 0; j < i; j++)
                                Players[j].PositionFromMe = (PlayerSeat)j;
                            for (int j = i; j < Players.Count; j++)
                                Players[j].PositionFromMe = (PlayerSeat)j;
                        }
                        else
                        {
                            for (int j = 1; j < Players.Count; j++)
                                Players[j].PositionFromMe = (PlayerSeat)j;
                        }
                        break;
                    }
                }
        }

        private PlayerSeat PlayerSeatFromPosition(System.Drawing.Point position, bool isNineTable)
        {
            if (isNineTable)
            {
                if (position.X == 516 && position.Y == 40)
                    return PlayerSeat.Seat1;
                else if (position.X == 638 && position.Y == 147)
                    return PlayerSeat.Seat2;
                else if (position.X == 638 && position.Y == 401)
                    return PlayerSeat.Seat3;
                else if (position.X == 516 && position.Y == 509)
                    return PlayerSeat.Seat4;
                else if (position.X == 355 && position.Y == 509)
                    return PlayerSeat.Seat5;
                else if (position.X == 195 && position.Y == 509)
                    return PlayerSeat.Seat6;
                else if (position.X == 73 && position.Y == 401)
                    return PlayerSeat.Seat7;
                else if (position.X == 73 && position.Y == 147)
                    return PlayerSeat.Seat8;
                else if (position.X == 195 && position.Y == 40)
                    return PlayerSeat.Seat9;
            }
            else
            {
                if (position.X == 516 && position.Y == 40)
                    return PlayerSeat.Seat1;
                else if (position.X == 638 && position.Y == 226)
                    return PlayerSeat.Seat2;
                else if (position.X == 516 && position.Y == 509)
                    return PlayerSeat.Seat3;
                else if (position.X == 195 && position.Y == 509)
                    return PlayerSeat.Seat4;
                else if (position.X == 73 && position.Y == 226)
                    return PlayerSeat.Seat5;
                else if (position.X == 195 && position.Y == 40)
                    return PlayerSeat.Seat6;
            }
            return PlayerSeat.Unknown;
        }

        public PokerPlayer GetPlayerFromId(long pId)
        {
            foreach (PokerPlayer pokerPlayer in Players)
                if (pokerPlayer.ComponentId == pId)
                    return pokerPlayer;
            return null;
        }
        public PokerPlayer GetPlayerFromName(string pName)
        {
            foreach (PokerPlayer pokerPlayer in Players)
                if (pokerPlayer.Username == pName)
                    return pokerPlayer;
            return null;
        }
        public PokerPlayer GetPlayerFromPosition(int pX, int pY)
        {
            foreach (PokerPlayer pokerPlayer in Players)
                if (pokerPlayer.Position.X == pX && pokerPlayer.Position.Y == pY)
                    return pokerPlayer;
            return null;
        }

        public void RegisterForGame()
        {
            if (_registrationPossible)
            {
                Console.WriteLine("[Action] Melde mich an");
                ClickButton(_clickMsgRegister);
            }
        }

        public void UnregisterForGame()
        {
            if (!_registrationPossible)
            {
                Console.WriteLine("[Action] Melde mich ab");
                ClickButton(_clickMsgUnregister);
            }
        }

        public void ClickButton(string ControllerID)
        {
            KnModule voidController = _client._parentModule.CreateModule("VOID_CONTROLLER");
            voidController.Add("CONTROLLER_ID", long.Parse(ControllerID));
            GameServerClient.Send(voidController);
        }

        public void CancelGame()
        {

        }

        public void TryCancelGame()
        {

        }

        public void JoinGameChannel(Game game)
        {

        }

        public void ToggleSitout(long id)
        {
            Action(_sitOutId);
        }
    }
}
