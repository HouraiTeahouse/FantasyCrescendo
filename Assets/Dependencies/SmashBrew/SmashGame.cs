using System;
using System.Collections.Generic;
using System.Linq;

namespace Hourai.SmashBrew {

    public class SmashGame : Game<SmashGame> {

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            var config = Config.Instance;
            _players = new Player[config.MaxPlayers];
            for (var i = 0; i < _players.Length; i++) {
                _players[i] = new Player(i);
            }
        }

        private static Player[] _players;

        /// <summary>
        /// Retrieves a Player object given it's corresponding number.
        /// Note: Player numbers are 0 indexed. What appears as Player 1 is actually player number 0 internally.
        /// </summary>
        /// <param name="playerNumber">the Player's number identifier</param>
        /// <exception cref="ArgumentException">thrown if <param name="playerNumber"> is less than 0 or greater 
        ///     than or equal to the maximum number of players</param></exception>
        /// <returns>the corresponding Player object</returns>
        public static Player GetPlayerData(int playerNumber) {
            if (playerNumber < 0 || playerNumber >= _players.Length)
                throw new ArgumentException("playerNumber");
            return _players[playerNumber];
        }

        /// <summary>
        /// Gets an enumeration of all Player.
        /// Note this includes all inactive players as well.
        /// </summary>
        public static IEnumerable<Player> Players {
            get {
                foreach (var player in _players)
                {
                    yield return player;
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of all active Players.
        /// To get an enumeration of all Players, active or not, see <see cref="Players"/>
        /// </summary>
        public static IEnumerable<Player> ActivePlayers {
            get { return _players.Where(player => player.Type != Player.PlayerType.None); }
        }

        /// <summary>
        /// Gets the maximum number of players allowed in one match.
        /// </summary>
        public static int MaxPlayers {
            get { return _players.Length; }
        }

        /// <summary>
        /// Gets the count of currently active Players.
        /// </summary>
        public static int ActivePlayerCount {
            get { return ActivePlayers.Count(); }
        }

    }

}