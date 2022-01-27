using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Tournament
{
    public class SuperTournament
    {
        private List<TournamentParticipant> _tournamentParticipants = new List<TournamentParticipant>();
        public IReadOnlyCollection<TournamentParticipant> TournamentParticipants => _tournamentParticipants;

        private TournamentDistributor _tournamentDistributor;
        private UsersDataBase _usersDataBase;

        private int _amountParticipants = 0;
        private int _targetParticipantsToStartTournament = 0;
        private int _tournamentID;

        public SuperTournament(UsersDataBase usersDataBase, int targetParticipantsToStartTournament, TournamentDistributor tournamentDistributor)
        {
            _usersDataBase = usersDataBase;
            _targetParticipantsToStartTournament = targetParticipantsToStartTournament;
            _tournamentDistributor = tournamentDistributor;
            _tournamentID = _tournamentDistributor.GetTournamentID();
        }

        public TournamentData PackToTournamentData()
        {
            return new TournamentData(_tournamentParticipants, _targetParticipantsToStartTournament);
        }

        public void AddParticipant(TournamentParticipant tournamentParticipant)
        {
            _tournamentParticipants.Add(tournamentParticipant);
            _amountParticipants++;
            _usersDataBase.AddParticeToSuperTournament(tournamentParticipant.Login, $"{_tournamentID}|{tournamentParticipant.BaseSID}|{tournamentParticipant.Login}|{tournamentParticipant.BaseID}|");

            if (CheckToStartTournament())
            {
                TournamentStarter.Instance.StartTournament(PackToTournamentData());
                ClearTournament();
                _tournamentID = _tournamentDistributor.GetTournamentID();
            }
        }

        public void ClearTournament()
        {
            _amountParticipants = 0;
            _tournamentParticipants.Clear();
        }

        public bool CheckToStartTournament()
        {
            if (_amountParticipants >= _targetParticipantsToStartTournament)
            {
                return true;
            }
            return false;
        }
    }
}
