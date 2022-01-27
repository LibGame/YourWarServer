using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;
using YourWarServer.User;

namespace YourWarServer.Server
{
    public class InventoryCommand : IClientCommand
    {
        private UsersDataBase _usersDataBase;

        public InventoryCommand(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }

        public string GetInventory(string login)
        {
            try
            {
                if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
                {
                    return userData.Inventory;
                }
                else
                {
                    return "n";
                }
            }
            catch
            {
                return "n";
            }
        }

        public string TryAddToInventory(string login, string value)
        {
            try
            {
                if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
                {
                    _usersDataBase.UpdateData(userData.ID, "Inventory", value);
                    return "y";
                }
                else
                {
                    return "n";
                }
            }
            catch
            {
                return "n";
            }
        }

        public string TryAddToInventoryShirk(string login, string value)
        {
            try
            {
                if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
                {
                    string inventory = ShrinkInventory(userData.Inventory + value);
                    _usersDataBase.UpdateData(userData.ID, "Inventory", inventory);
                    return "y";
                }
                else
                {
                    return "n";
                }
            }
            catch
            {
                return "n";
            }
        }

        private string ShrinkInventory(string value)
        {
            Dictionary<int, int> shirk = new Dictionary<int, int>();

            char[] lettes = value.ToCharArray();
            string result = "";
            int id = 0;
            bool isWasID = false; 

            for(int i = 0; i < lettes.Length; i++)
            {
                if(lettes[i].ToString() == "/")
                {
                    if (isWasID)
                    {
                        shirk.Add(id, Convert.ToInt32(result));
                        isWasID = false;
                    }
                    else
                    {
                        isWasID = true;
                        try
                        {
                            id = Convert.ToInt32(result);
                        }
                        catch
                        {
                            id = 0;
                        }
                    }
                    result = "";
                }
                else
                {
                    result += lettes[i];
                }
            }

            string shirkValue = "";
            foreach(var val in shirk)
            {
                shirkValue += $"{val.Key}/{val.Value}/";
            }
            return shirkValue;
        }
    }
}
