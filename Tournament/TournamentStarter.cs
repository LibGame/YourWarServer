using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Tournament
{
    public class TournamentStarter
    {
        private WonUser _wonUser = new WonUser();
        private TournamentCompleter _tournamentCompleter;
        private int _createTournamentCount;

        public static TournamentStarter Instance;

        public TournamentStarter(TournamentCompleter tournamentCompleter)
        {
            
            _tournamentCompleter = tournamentCompleter;
            Instance = this;
        }

        public void StartTournament(TournamentData tournamentData)
        {
            List<FighterPerFights> tournamentFights = TournamentProcessAsync(tournamentData).Result;
            var pastTournament = new PastTournament(tournamentFights, _createTournamentCount);
            _tournamentCompleter.AddPastTournament(pastTournament);
            _createTournamentCount++;
            _tournamentCompleter.CallToStopTournament(pastTournament,tournamentData.TargetParticipantsToStartTournament);
        }


        public async Task<List<FighterPerFights>> TournamentProcessAsync(TournamentData tournamentData)
        {
            return await Task.Run(() => TournamentProcess(tournamentData));
        }

        public List<FighterPerFights> TournamentProcess(TournamentData tournamentData)
        {
            List<TournamentParticipantFighter> tournamentParticipantFighters = new List<TournamentParticipantFighter>();
            List<TournamentFight> tournamentFights = new List<TournamentFight>();

            foreach(var user1 in tournamentData.TournamentParticipants)
            {
                TournamentParticipant participantForFight = user1;
                List<TournamentParticipant> oponents = new List<TournamentParticipant>();


                foreach (var user2 in tournamentData.TournamentParticipants)
                {
                    if (user2.Login != participantForFight.Login)
                        oponents.Add(user2);
                }
                tournamentParticipantFighters.Add(new TournamentParticipantFighter(participantForFight, oponents));

            }

            foreach(var fight in tournamentParticipantFighters)
            {
                foreach(var fighter in fight.Oponents)
                {
                    if (!CheckIsWasInFight(tournamentFights, fight.Fighter , fighter))
                    {
                        var tournamentFight = new TournamentFight();
                        tournamentFight.FirstFighter = fight.Fighter;
                        tournamentFight.SecondFighter = fighter;
                        tournamentFight.Winner = _wonUser.GetkWinner(fight.Fighter, fighter);
                        tournamentFights.Add(tournamentFight);
                    }
                }
            }

            return PackTournamentFights(tournamentData, tournamentFights);
        }

        public List<FighterPerFights> PackTournamentFights(TournamentData tournamentData ,List<TournamentFight> tournamentFights)
        {
            List<FighterPerFights> fighterPerFights = new List<FighterPerFights>();

            foreach (var participant in tournamentData.TournamentParticipants)
            {
                var fighetPreFight = new FighterPerFights();
                fighetPreFight.Fighter = participant;
                fighetPreFight.Fights = GetTournamentFightsByLogin(tournamentFights, participant.Login);
            }

            return fighterPerFights;
        }

        private List<TournamentFight> GetTournamentFightsByLogin(List<TournamentFight> tournamentFights , string login)
        {
            List<TournamentFight> tournamentFightsResult = new List<TournamentFight>();

            foreach (var fight in tournamentFights)
            {
                if (fight.FirstFighter.Login == login || fight.SecondFighter.Login == login)
                {
                    tournamentFightsResult.Add(fight);
                }
            }
            return tournamentFightsResult;
        }


        private bool CheckIsWasInFight(List<TournamentFight> tournamentFights ,TournamentParticipant firstParticipant, TournamentParticipant secondParticipant)
        {
            foreach(var fight in tournamentFights)
            {
                if (fight.FirstFighter.Login == firstParticipant.Login && fight.SecondFighter.Login == secondParticipant.Login ||
                    fight.FirstFighter.Login == secondParticipant.Login && fight.SecondFighter.Login == firstParticipant.Login)
                    return true;
            }
            return false;
        }

    }

    public struct PastTournament
    {
        public List<FighterPerFights> FighterPerFights { get; private set; }
        public int ID { get; private set; }


        public PastTournament(List<FighterPerFights> tournamentFights ,int id)
        {
            FighterPerFights = tournamentFights;
            ID = id;
        }


    }

    public struct FighterPerFights
    {
        public TournamentParticipant Fighter { get; set; }
        public List<TournamentFight> Fights { get; set; }
        
        public int TakedIndexFight { get; set; }


        public bool TryGetFightAndID(out TournamentFight tournamentFight , out int id)
        {
            if(TakedIndexFight < Fights.Count)
            {
                tournamentFight = Fights[TakedIndexFight];
                id = TakedIndexFight;
                TakedIndexFight++;
                return true;
            }

            id = default;
            tournamentFight = default;
            return false;

        }

    }

    public struct TournamentParticipantFighter
    {
        public TournamentParticipant Fighter { get; private set; }
        public List<TournamentParticipant> Oponents { get; private set; }

        public TournamentParticipantFighter(TournamentParticipant fighter , List<TournamentParticipant> oponents)
        {
            Fighter = fighter;
            Oponents = oponents;
        }
    }

    public struct TournamentFight
    {
        public TournamentParticipant FirstFighter { get; set; }
        public TournamentParticipant SecondFighter { get; set; }
        public TournamentParticipant Winner { get; set; }

    }


    public struct TournamentData
    {
        public List<TournamentParticipant> TournamentParticipants { get; private set; }
        public int TargetParticipantsToStartTournament { get; private set; }

        public TournamentData(List<TournamentParticipant> tournamentParticipants , int targetParticipantsToStartTournament)
        {
            TournamentParticipants = tournamentParticipants;
            TargetParticipantsToStartTournament = targetParticipantsToStartTournament;
        }

    }
}
