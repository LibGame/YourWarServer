using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;
using YourWarServer.User;

namespace YourWarServer.Server
{
    public class RegistrationCommand : IClientCommand
    {
        private UsersDataBase _usersDataBase;

        public RegistrationCommand(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;

        }

        public string TryEnterOrRegisterUsser(string login)
        {
            Console.WriteLine("TryEnterOrRegisterUsser " + login);
            if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
            {
                Console.WriteLine("Есть в списке");
                return "y";
            }
            else
            {
                if (_usersDataBase.AddUser(login))
                {
                    _usersDataBase.UpdateDataByLogin(login, "Bases", "0/1/2/3/4/8");
                    Console.WriteLine("Есть в списке");
                    return "y";
                }
                else
                {
                    Console.WriteLine("Нет списке");
                    return "n";

                }
            }
        }

    }
}
