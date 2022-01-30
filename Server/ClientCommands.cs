using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Chat;
using YourWarServer.Data.DataBases;
using YourWarServer.Tournament;
using YourWarServer.User;

namespace YourWarServer.Server
{
    public class ClientCommands
    {
        private UsersDataBase _usersDataBase;
        private MessangerSender _messangerSender;

        private RegistrationCommand _registrationCommand;
        private AddBaseCommand _addBaseCommand;
        private GetRandomBaseCommand _getRandomBaseCommand;
        private WalletCommands _walletCommands;
        private InventoryCommand _inventoryCommand;
        private MarketCommand _marketCommand;
        private InventoryAndWalletCommand _inventoryAndWalletCommand;
        private UsserStatisticCommands _usserStatisticCommands;
        private MessageSenderUser _messageSenderUser;
        private IgnoreAndFavoriteUsersCommand _ignoreAndFavoriteUsersCommand;
        private TournamentStarter _tournamentStarter;
        private TournamentDistributor _tournamentDistributor;
        private TournamentCompleter _tournamentCompleter;
        private ChatMail _chatMail;
        public InventoryCommand InventoryCommand => _inventoryCommand;
        public WalletCommands WalletCommands => _walletCommands;
        public MessangerSender MessangerSender => _messangerSender;

        public static ClientCommands Instance;

        public delegate void RedyToLoadIcon(string login);
        public event RedyToLoadIcon OnRedyToLoadIcon;

        public ClientCommands(UsersDataBase usersDataBase , MessangerSender messangerSender)
        {
            Instance = this;


            _usersDataBase = usersDataBase;
            _messangerSender = messangerSender;
            _chatMail = new ChatMail(_usersDataBase);
            _tournamentCompleter = new TournamentCompleter(_usersDataBase);
            _tournamentStarter = new TournamentStarter( _tournamentCompleter);
            _tournamentDistributor = new TournamentDistributor(usersDataBase);

            _addBaseCommand = new AddBaseCommand(_usersDataBase);
            _registrationCommand = new RegistrationCommand(_usersDataBase);
            _getRandomBaseCommand = new GetRandomBaseCommand();
            _walletCommands = new WalletCommands(_usersDataBase);
            _inventoryCommand = new InventoryCommand(_usersDataBase);
            _marketCommand = new MarketCommand(_usersDataBase , this, _chatMail);
            _inventoryAndWalletCommand = new InventoryAndWalletCommand(_walletCommands, _inventoryCommand);
            _usserStatisticCommands = new UsserStatisticCommands(_usersDataBase);
            _messageSenderUser = new MessageSenderUser(_usersDataBase);
            _ignoreAndFavoriteUsersCommand = new IgnoreAndFavoriteUsersCommand(_usersDataBase);
        }

        public bool UseCommand(CommandAndMessages commandAndMessages , TcpClient handler)
        {
            switch (commandAndMessages.Command)
            {
                case "Registration":
                    string resultRegistration = _registrationCommand.TryEnterOrRegisterUsser(commandAndMessages.Messages[0]);
                    Console.WriteLine("Sending");
                    _messangerSender.SendMessage(resultRegistration, handler);
                    return true;
                case "AddBase":
                    string resultAddBase = _addBaseCommand.TryAddBaseByLogin(commandAndMessages.Messages[0], Convert.ToInt32(commandAndMessages.Messages[1]), commandAndMessages.Messages[2]);
                    _messangerSender.SendMessage(resultAddBase , handler);
                    return true;
                case "AddOpenBase":
                    string previousBases = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "Bases");
                    _usersDataBase.UpdateDataByLogin(commandAndMessages.Messages[0], "Bases", previousBases + commandAndMessages.Messages[3]);
                    if(commandAndMessages.Messages[1] == "patrons")
                    {
                        _usersDataBase.UpdateDataByLogin(commandAndMessages.Messages[0], "Patrons", commandAndMessages.Messages[2]);
                    }else if (commandAndMessages.Messages[1] == "battlePass")
                    {
                        _usersDataBase.UpdateDataByLogin(commandAndMessages.Messages[0], "BattlePass", commandAndMessages.Messages[2]);
                    }
                    _messangerSender.SendMessage("y", handler);
                    return true;
                case "GetOpenBases":
                    _messangerSender.SendMessage(_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "Bases"), handler);
                    return true;
                case "GetRandomBattleBase":
                    string resultGerRandomBattleBase = _getRandomBaseCommand.GetRandomBattleBase(commandAndMessages.Messages[0]);
                    _messangerSender.SendMessage(resultGerRandomBattleBase, handler);
                    return true;
                case "GetMedals":
                    string resultGetMedals = _walletCommands.GetMedalsCommand(commandAndMessages.Messages[0]);
                    _messangerSender.SendMessage(resultGetMedals, handler);
                    return true;
                case "GetCups":
                    string resultGetCups = _walletCommands.GetCupsCommand(commandAndMessages.Messages[0]);
                    _messangerSender.SendMessage(resultGetCups, handler);
                    return true;
                case "GetBattlePass":
                    string resultGetBattlePass = _walletCommands.GetBattlePassCommand(commandAndMessages.Messages[0]);
                    _messangerSender.SendMessage(resultGetBattlePass, handler);
                    return true;
                case "GetPatrons":
                    string resultGetPatrons = _walletCommands.GetPatronsCommand(commandAndMessages.Messages[0]);
                    _messangerSender.SendMessage(resultGetPatrons, handler);
                    return true;
                case "AddMedals":
                    string resultAddMedals = _walletCommands.AddMedalsCommand(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    _messangerSender.SendMessage(resultAddMedals, handler);
                    return true;
                case "AddCups":
                    string resultAddCups = _walletCommands.AddCupsCommand(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    _messangerSender.SendMessage(resultAddCups, handler);
                    return true;
                case "AddBattlePass":
                    string resultAddBattlePass = _walletCommands.AddBattlePassCommand(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    _messangerSender.SendMessage(resultAddBattlePass, handler);
                    return true;
                case "AddPatrons":
                    string resultAddPatrons = _walletCommands.AddPatronsCommand(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    _messangerSender.SendMessage(resultAddPatrons, handler);
                    return true;
                case "AddDeal":
                    string resultAddProduct = _marketCommand.AddDeal(commandAndMessages.Messages[0], commandAndMessages.Messages[1], commandAndMessages.Messages[2], commandAndMessages.Messages[3], commandAndMessages.Messages[4]);
                    _messangerSender.SendMessage(resultAddProduct, handler);
                    return true;
                case "GetProducts":
                    string resultGetProducts = _marketCommand.GetProducts();
                    if (resultGetProducts == "")
                        resultGetProducts = "n";

                    _messangerSender.SendMessage(resultGetProducts, handler);
                    return true;
                case "BuyProduct":
                    string resultBuyProduct = _marketCommand.BuyProduct(commandAndMessages.Messages[0], commandAndMessages.Messages[1], commandAndMessages.Messages[2], commandAndMessages.Messages[3]);
                    _messangerSender.SendMessage(resultBuyProduct, handler);
                    return true;
                case "GetInventoryAndWallet":
                    if(_usersDataBase.GetUsserByLogin(commandAndMessages.Messages[0], out UserData userData))
                    {
                        string resultGetInventoryAndWallet = $"0/{userData.Cups}/1/{userData.BattlePass}/2/{userData.Medals}/3/{userData.Patrons}/{userData.Inventory}";
                        Console.WriteLine(resultGetInventoryAndWallet);
                        _messangerSender.SendMessage(resultGetInventoryAndWallet, handler);
                    }
                    return true;
                case "AddInventoryAndWallet":
                    string resultAddInventoryAndWallet = _inventoryAndWalletCommand.AddInventoryAndWallet(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    Console.WriteLine("Result " + resultAddInventoryAndWallet);
                    _messangerSender.SendMessage(resultAddInventoryAndWallet, handler);
                    return true;
                case "GetInventory":
                    string resultGetInventory = _inventoryCommand.GetInventory(commandAndMessages.Messages[0]);
                    _messangerSender.SendMessage(resultGetInventory, handler);
                    return true;
                case "GetUserBase":
                    List<string> basesRandom = new List<string>();

                    string getedBase1 = _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "FirstBaseBattle");
                    string getedBase2 = _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "SecondBaseBattle");
                    string getedBase3 = _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "ThirdBaseBattle");
                    string getedBase4 = _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "FourthBaseBattle");
                    if (getedBase1 != "")
                        basesRandom.Add(getedBase1);
                    if (getedBase2 != "")
                        basesRandom.Add(getedBase2);
                    if (getedBase3 != "")
                        basesRandom.Add(getedBase3);
                    if (getedBase4 != "")
                        basesRandom.Add(getedBase4);

                    Random rnd = new Random();

                    _messangerSender.SendMessage(_usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "Username") +"|"+ (basesRandom.Count > 0 ? basesRandom[rnd.Next(0, basesRandom.Count)] : "n"), handler);
                    return true;
                case "GetPlayerProfile":
                    string resultGetPlayerProfile = commandAndMessages.Messages[0] + "/" + _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "Username") + "/" + _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "Motto") + "/" + _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "Country") + "/" + "Рядовой" + "/" + "|";
                    resultGetPlayerProfile += _usersDataBase.GetIntValueByLogin(commandAndMessages.Messages[0], "Wins") + "/" + _usersDataBase.GetIntValueByLogin(commandAndMessages.Messages[0], "Loses") + "/" +
                         _usersDataBase.GetIntValueByLogin(commandAndMessages.Messages[0], "TournamentWins") + "/" + _usersDataBase.GetIntValueByLogin(commandAndMessages.Messages[0], "TournamentLoses") +"/"+
                        _usersDataBase.GetIntValueByLogin(commandAndMessages.Messages[0], "SuperTournamentWins") + "/" + _usersDataBase.GetIntValueByLogin(commandAndMessages.Messages[0], "SuperTournamentWins") + "|";
                    resultGetPlayerProfile += _inventoryCommand.GetInventory(commandAndMessages.Messages[0]);
                    _messangerSender.SendMessage(resultGetPlayerProfile, handler);
                    return true;
                case "AddInventory":
                    string resultAddInventory = _inventoryCommand.TryAddToInventory(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    _messangerSender.SendMessage(resultAddInventory, handler);
                    return true;
                case "DeleteUser":
                    _usersDataBase.DeleteUserByLogin(commandAndMessages.Messages[0]);
                    _messangerSender.SendMessage("y", handler);
                    return true;
                case "AddWins":
                    _usserStatisticCommands.AddWins(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    return true;
                case "AddLoses":
                    _usserStatisticCommands.AddLoses(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    return true;
                case "GetTopPlayersInWorld":
                    _messangerSender.SendMessage(_usserStatisticCommands.GetTopWorldUsers(commandAndMessages.Messages[0]), handler);
                    return true;
                case "AddPrivateMessage":
                    _chatMail.AddMessage(commandAndMessages.Messages[0], commandAndMessages.Messages[1], commandAndMessages.Messages[2]);
                    return true;
                case "AddExchangeUserOffer":
                    Console.WriteLine("AddDealToUser " + 1.1);
                    _marketCommand.AddDealToUser(commandAndMessages.Messages[0], commandAndMessages.Messages[1],commandAndMessages.Messages[2], commandAndMessages.Messages[3], commandAndMessages.Messages[4], commandAndMessages.Messages[5]);
                    _messangerSender.SendMessage("y", handler);
                    return true;
                case "GetProductByID":
                    _messangerSender.SendMessage(_marketCommand.GetProductByID(commandAndMessages.Messages[0]), handler);
                    return true;
                case "GetPrivateMessages":

                    //string messages = _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "ChatMessages");
                    string messages = _chatMail.GetRoomsInString(commandAndMessages.Messages[0]);

                    if (messages == "")
                        messages = "n";

                    _messangerSender.SendMessage(messages, handler);
                    return true;
                case "GetMessageRoom":

                    //string messages = _usersDataBase.GetUsserDataByLogin(commandAndMessages.Messages[0], "ChatMessages");
                    string messagesRoom = _chatMail.GetMessageRoom(commandAndMessages.Messages[0]);

                    if (messagesRoom == "" || messagesRoom == null)
                        messagesRoom = "n";

                    _messangerSender.SendMessage(messagesRoom, handler);
                    return true;
                case "AddIgoneredUsers":
                    _ignoreAndFavoriteUsersCommand.AddIgnoredUser(commandAndMessages.Messages[0], commandAndMessages.Messages[1], commandAndMessages.Messages[2]);
                    return true;
                case "AddFavoriteUsers":
                    _ignoreAndFavoriteUsersCommand.AddFavoriteUser(commandAndMessages.Messages[0], commandAndMessages.Messages[1], commandAndMessages.Messages[2]);
                    return true;
                case "GetFavoriteUsers":
                    string result1 = _ignoreAndFavoriteUsersCommand.GetFavoriteUsers(commandAndMessages.Messages[0]);
                    if (result1 == "")
                        result1 = "n";
                    _messangerSender.SendMessage(result1, handler);
                    return true;
                case "GetIgnoreUsers":
                    string result = _ignoreAndFavoriteUsersCommand.GetIgnoredUsers(commandAndMessages.Messages[0]);
                   
                    if (result == "")
                        result = "n";
                    _messangerSender.SendMessage(result, handler);
                    return true;
                case "RemoveFromIgnore":
                    _ignoreAndFavoriteUsersCommand.RemoveFromIgnore(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    return true;
                case "RemoveFromFavorite":
                    _ignoreAndFavoriteUsersCommand.RemoveFromFavorite(commandAndMessages.Messages[0], commandAndMessages.Messages[1]);
                    return true;
                case "Tournament50Players":
                    var ansverTournament1 = _tournamentDistributor.AddParticipant("Tournament50Players", commandAndMessages.Messages[0], commandAndMessages.Messages[1], Convert.ToInt32(commandAndMessages.Messages[2]));
                    _messangerSender.SendMessage(ansverTournament1, handler);
                    return true;
                case "Tournament100Players":
                    var ansverTournament2 = _tournamentDistributor.AddParticipant("Tournament100Players", commandAndMessages.Messages[0], commandAndMessages.Messages[1], Convert.ToInt32(commandAndMessages.Messages[2]));
                    _messangerSender.SendMessage(ansverTournament2, handler);
                    return true;
                case "Tournament250Players":
                    var ansverTournament3 = _tournamentDistributor.AddParticipant("Tournament250Players", commandAndMessages.Messages[0], commandAndMessages.Messages[1], Convert.ToInt32(commandAndMessages.Messages[2]));
                    _messangerSender.SendMessage(ansverTournament3, handler);
                    return true;
                case "Tournament500Players":
                    var ansverTournament4 = _tournamentDistributor.AddParticipant("Tournament500Players", commandAndMessages.Messages[0], commandAndMessages.Messages[1], Convert.ToInt32(commandAndMessages.Messages[2]));
                    _messangerSender.SendMessage(ansverTournament4, handler);
                    return true;
                case "SuperTournament50Players":
                    var ansverSuperTournament1 = _tournamentDistributor.AddParticipant("SuperTournament50Players", commandAndMessages.Messages[0], commandAndMessages.Messages[1], Convert.ToInt32(commandAndMessages.Messages[2]));
                    _messangerSender.SendMessage(ansverSuperTournament1, handler);
                    return true;
                case "SuperTournament100Players":
                    var ansverSuperTournament2 = _tournamentDistributor.AddParticipant("SuperTournament100Players", commandAndMessages.Messages[0], commandAndMessages.Messages[1], Convert.ToInt32(commandAndMessages.Messages[2]));
                    _messangerSender.SendMessage(ansverSuperTournament2, handler);
                    return true;
                case "SuperTournament250Players":
                    var ansverSuperTournament3 = _tournamentDistributor.AddParticipant("SuperTournament250Players", commandAndMessages.Messages[0], commandAndMessages.Messages[1], Convert.ToInt32(commandAndMessages.Messages[2]));
                    _messangerSender.SendMessage(ansverSuperTournament3, handler);
                    return true;
                case "SuperTournament500Players":
                    var ansverSuperTournament4 = _tournamentDistributor.AddParticipant("SuperTournament500Players", commandAndMessages.Messages[0], commandAndMessages.Messages[1], Convert.ToInt32(commandAndMessages.Messages[2]));
                    _messangerSender.SendMessage(ansverSuperTournament4, handler);
                    return true;
                case "GetTournamentInFight":
                    _messangerSender.SendMessage(_tournamentCompleter.GetFightToSimulateInStringFormat(commandAndMessages.Messages[0], commandAndMessages.Messages[1]), handler);
                    return true;
                case "GetTournamentBaseInTournaments":
                    string tournaments = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "ParticipatesInTournament");
                    string superTournaments = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "ParticipatesInSuperTournament");
                    string tournamentResult = "";

                    if (superTournaments == "")
                        superTournaments = "n";

                    if (tournaments == "" && superTournaments == "")
                        tournamentResult = "n";
                    else
                        tournamentResult = tournaments + "*" + superTournaments;

                    _messangerSender.SendMessage(tournamentResult, handler);
                    return true;
                case "GetTournamentBaseInSuperTournament":
                    //string superTournaments = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "ParticipatesInSuperTournament");
                    //if (superTournaments == "")
                    //    superTournaments = "n";

                    //_messangerSender.SendMessage(superTournaments, handler);
                    return true;
                case "AddComlpeteFight":
                    _tournamentCompleter.ComlpeteFight(Convert.ToInt32(commandAndMessages.Messages[0]), commandAndMessages.Messages[1], commandAndMessages.Messages[2]);
                    return true;
                case "GetBasesBattleAndTournaments":
                    string bases = $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "FirstBaseBattle")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "SecondBaseBattle")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "ThirdBaseBattle")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "FourthBaseBattle")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "FirstBaseTournament")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "SecondBaseTournament")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "ThirdBaseTournament")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "FourthBaseTournament")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "FirstBaseSuperTournament")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "SecondBaseSuperTournament")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "ThirdBaseSuperTournament")}|" +
                        $"{_usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "FourthBaseSuperTournament")}|";
                    Console.WriteLine("Отправил базы " + bases);
                    _messangerSender.SendMessage(bases, handler);
                    return true;
                case "GetPlayerFightStatistic":
                    string wins = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "Wins" , false);
                    string loses = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "Loses", false);
                    string tournamentWins = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "TournamentWins", false);
                    string tournamentLoses = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "TournamentLoses", false);

                    _messangerSender.SendMessage($"{wins}|{loses}|{tournamentWins}|{tournamentLoses}|", handler);
                    return true;
                case "GetUserData":
                    string motto = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "Motto");
                    string username = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "Username");
                    string country = _usersDataBase.GetValueByLogin(commandAndMessages.Messages[0], "Country");

                    _messangerSender.SendMessage($"{username}|{motto}|{country}|", handler);
                    return true;
                case "SetUserData":
                    if (commandAndMessages.Messages[1] != "")
                        _usersDataBase.UpdateDataByLogin(commandAndMessages.Messages[0], "Username", commandAndMessages.Messages[1]);
                    if (commandAndMessages.Messages[2] != "")
                        _usersDataBase.UpdateDataByLogin(commandAndMessages.Messages[0], "Motto", commandAndMessages.Messages[2]);
                    if (commandAndMessages.Messages[3] != "")
                        _usersDataBase.UpdateDataByLogin(commandAndMessages.Messages[0], "Country", commandAndMessages.Messages[3]);


                    _messangerSender.SendMessage("y", handler);

                    return true;
                case "AddIcon":
                    Console.WriteLine("Пришла команда на иконку");
                    OnRedyToLoadIcon?.Invoke(commandAndMessages.Messages[0]);
                    Console.WriteLine("Отправили y");
                    _messangerSender.SendMessage("y", handler);
                    return true;
                case "GetIcon":
                    try
                    {
                        var bytes = _usersDataBase.GetBytesByLogin(commandAndMessages.Messages[0], "Icon");
                        Console.WriteLine(bytes.Length);
                        _messangerSender.SendMessage(bytes, handler);
                    }
                    catch
                    {
                        var bytes = new byte[0];
                        _messangerSender.SendMessage(bytes, handler);

                    }
                    return true;
                case "":
                    _messangerSender.SendMessage("n", handler);
                    return true;
            }

            return false;
        }

        public byte[] GetCompressImage(byte[] value)
        {
            var memStream = new MemoryStream();
            memStream.Position = 0;

            using (DeflateStream gzipStream = new DeflateStream(memStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(value, 0, value.Length);
                gzipStream.Flush();
            }
            return memStream.ToArray();
        }

    }
}
