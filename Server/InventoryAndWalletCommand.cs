using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.Server
{
    public class InventoryAndWalletCommand
    {

        private WalletCommands _walletCommands;
        private InventoryCommand _inventoryCommand;

        public InventoryAndWalletCommand(WalletCommands walletCommands , InventoryCommand inventoryCommand)
        {
            _walletCommands = walletCommands;
            _inventoryCommand = inventoryCommand;
        }


        public string AddInventoryAndWallet(string login, string message)
        {
            try
            {
                Console.WriteLine("MEssage " + message);

                char[] list = message.ToCharArray();
                int id = 0;
                bool isId = true;

                bool isFullWallet = false;

                string result = "";

                foreach (var item in list)
                {
                    if (item.ToString() == "/" && !isFullWallet)
                    {
                        if (isId)
                        {

                            id = Convert.ToInt32(result);
                            isId = false;
                        }
                        else
                        {
                            AddToWalletByID(login, id, Convert.ToInt32(result));
                            isId = true;

                            if (id == 3)
                                isFullWallet = true;
                        }
                        result = "";
                    }
                    else if (id >= 3)
                    {
                        result += item;
                    }
                    else
                    {
                        result += item;
                    }
                }
                _inventoryCommand.TryAddToInventory(login, result);

                return "y";

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "n";
            }

        }

        public void AddToWalletByID(string login , int id , int amount)
        {
            Console.WriteLine($"{login}  {id} {amount}");
            switch (id)
            {
                case 0:
                    _walletCommands.AddCupsCommand(login, amount.ToString());
                    break;
                case 1:
                    _walletCommands.AddBattlePassCommand(login, amount.ToString());
                    break;
                case 2:
                    _walletCommands.AddMedalsCommand(login, amount.ToString());
                    break;
                case 3:
                    _walletCommands.AddPatronsCommand(login, amount.ToString());
                    break;
            }
        }

    }
}
