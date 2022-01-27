using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using YourWarServer.Data.DataBases;
using YourWarServer.Server;

namespace YourWarServer.Tournament
{
    public class TournamentCompleter
    {

        private List<PastTournament> _pastTournaments = new List<PastTournament>();
        private Dictionary<int, int> _participantsPerSeconds = new Dictionary<int, int>
        {
            [50] = 300000,
            [100] = 420000,
            [250] = 540000,
            [500] = 660000,

        };
        private UsersDataBase _usersDataBase;

        public TournamentCompleter(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }

        public void AddPastTournament(PastTournament pastTournament)
        {
            _pastTournaments.Add(pastTournament);
        }

        public string GetFightToSimulateInStringFormat(string login , string idTournament)
        {
            try
            {
                if (TryGetPastTournamentAndFighterPerFightsByLogin(out PastTournament pastTournament, out FighterPerFights fighterPerFights, login, Convert.ToInt32(idTournament)))
                {
                    if (fighterPerFights.TryGetFightAndID(out TournamentFight tournamentFight, out int id))
                    {
                        string username = _usersDataBase.GetValueByLogin(tournamentFight.SecondFighter.Login, "Username");
                        if (username == "") username = tournamentFight.SecondFighter.Login;

                        return $"{login}|{username}|{tournamentFight.FirstFighter.BaseSID}|{tournamentFight.SecondFighter.BaseSID}|{pastTournament.ID}|{id}|{tournamentFight.SecondFighter.Login}|";
                    }
                }
                return "n";

            }
            catch
            {
                return "n";

            }
         }

        public void ComlpeteFight(int tournamentID , string idFight, string winnerLogin)
        {
            try
            {
                var tournament = GetPastTournamentByID(tournamentID);
                var fighterPerFights = GetFighterPerFightsByFighterName(tournament.FighterPerFights, winnerLogin);
                var participant = fighterPerFights.Fights[Convert.ToInt32(idFight)];
                if (participant.FirstFighter.Login == winnerLogin)
                {
                    participant.Winner = participant.FirstFighter;
                }
                else
                {
                    participant.Winner = participant.SecondFighter;
                }
            }
            catch
            {

            }
        }

        public FighterPerFights GetFighterPerFightsByFighterName(List<FighterPerFights> fighterPerFights , string login)
        {
            return fighterPerFights.Where(tournamentFight => tournamentFight.Fighter.Login == login).ToArray()[0];
        }

        private bool TryGetPastTournamentAndFighterPerFightsByLogin(out PastTournament pastTournament , out  FighterPerFights fighterPerFights, string login , int id)
        {
            foreach(var tournament in _pastTournaments)
            {
                if(tournament.ID == id)
                {
                    foreach (var fighters in tournament.FighterPerFights)
                    {
                        if (login == fighters.Fighter.Login)
                        {
                            pastTournament = tournament;
                            fighterPerFights = fighters;
                            return true;
                        }
                    }
                }
            }

            fighterPerFights = default;
            pastTournament = default;
            return false;
        }

        private PastTournament GetPastTournamentByID(int id)
        {
            return _pastTournaments.Where(tournament => tournament.ID == id).ToArray()[0];
        }

        public void CallToStopTournament(PastTournament pastTournament , int amount)
        {
            Timer timer = new Timer(_participantsPerSeconds[amount]);
            timer.Elapsed += (sender, args) => CompleteTournament(pastTournament, args);
            timer.AutoReset = false;
        }

        public void CompleteTournament(object obj, ElapsedEventArgs e)
        {
            try
            {

                var pastTournament = (PastTournament)obj;
                foreach (var tournament in pastTournament.FighterPerFights)
                {

                    foreach (var fighter in tournament.Fights)
                    {
                        if (fighter.Winner.Login == tournament.Fighter.Login)
                            ClientCommands.Instance.WalletCommands.AddBattlePassCommand(tournament.Fighter.Login, "2");

                    }
                }

                _pastTournaments.Remove(pastTournament);

            }
            catch
            {
            }
        }

    }
}
