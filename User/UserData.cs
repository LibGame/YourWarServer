using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.User
{
    public struct UserData
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string FirstBaseBattle { get; private set; }
        public string SecondBaseBattle { get; private set; }
        public string ThirdBaseBattle { get; private set; }
        public string FourthBaseBattle { get; private set; }
        public string FirstBaseTournament { get; private set; }
        public string SecondBaseTournament { get; private set; }
        public string ThirdBaseTournament { get; private set; }
        public string FourthBaseTournament { get; private set; }
        public string Inventory { get; private set; }
        public int Cups { get; private set; }
        public int BattlePass { get; private set; }
        public int Medals { get; private set; }
        public int Patrons { get; private set; }
        public int Wins { get; private set; }
        public int Loses { get; private set; }
        public string ChatIgnoreUssers { get; private set; }
        public string ChatMessages { get; private set; }


        public UserData(int id , string name , string firstBaseBattle , string secondBaseBattle , string thirdBaseBattle , string fourthBaseBattle,
            string firstBaseTournament, string secondBaseTournament, string thirdBaseTournament, string fourthBaseTournament , 
            string inventory,
            int cups , int battlePass , int medals , int patrons,
            int wins , int loses , string chatIgnoreUssers , string chatMessages)
        {
            ID = id;
            Name = name;
            FirstBaseBattle = firstBaseBattle;
            SecondBaseBattle = secondBaseBattle;
            ThirdBaseBattle = thirdBaseBattle;
            FourthBaseBattle = fourthBaseBattle;
            FirstBaseTournament = firstBaseTournament;
            SecondBaseTournament = secondBaseTournament;
            ThirdBaseTournament = thirdBaseTournament;
            FourthBaseTournament = fourthBaseTournament;
            Inventory = inventory;
            Cups = cups;
            BattlePass = battlePass;
            Medals = medals;
            Patrons = patrons;
            Wins = wins;
            Loses = loses;
            ChatIgnoreUssers = chatIgnoreUssers;
            ChatMessages = chatMessages;
        }

    }
}
