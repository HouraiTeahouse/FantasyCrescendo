using System;
using System.Collections.Generic;
using System.Linq;

namespace Hourai.SmashBrew {

    public class SmashGame : Game<SmashGame> {

        static SmashGame() {
            Config config = Config.Instance;
            _players = new Player[config.MaxPlayers];
            for (var i = 0; i < _players.Length; i++) {
                _players[i] = new Player(i);
            }
        }

        private static Player[] _players;

        public static Player GetPlayerData(int playerNumber) {
            if (playerNumber < 0 || playerNumber >= _players.Length)
                throw new ArgumentException("playerNumber");
            return _players[playerNumber];
        }

        public static IEnumerable<Player> Players {
            get {
                foreach (var player in _players)
                    yield return player;
            }
        }

        public static IEnumerable<Player> ActivePlayers {
            get { return _players.Where(player => player.Type != Player.PlayerType.None); }
        }

        public static int MaxPlayers {
            get { return _players.Length; }
        }

        public static int ActivePlayerCount {
            get { return ActivePlayers.Count(); }
        }

    }

}