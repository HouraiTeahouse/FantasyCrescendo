using System.Collections.Generic;
using System.Linq;
using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class Stock : CharacterComponent {

        private StockRespawn _respawn;
        private int _lives;
        public bool Alive {
            get { return Lives > 0; }
        }

        public int Lives {
            get { return _lives; }
            internal set {
                _lives = value;
                if(_respawn)
                    _respawn.enabled = Alive;
                if(Character)
                    Character.gameObject.SetActive(Alive);
            }
        }
    }
    
    public class StockRespawn : MonoBehaviour {

        private Respawn _respawn;

        void Awake() {
            _respawn = GetComponent<Respawn>();
            if (!_respawn)
                Destroy(this);
            else
                _respawn.ShouldRespwan += RespawnCheck;
        }

        void OnDestroy() {
            if (_respawn)
                _respawn.ShouldRespwan -= RespawnCheck;
        }

        bool RespawnCheck(Character player) {
            var stock = player.GetComponent<Stock>();
            var damage = player.GetComponent<Damage>();
            if (damage)
                damage.Reset();
            if (!stock)
                return true;
            stock.Lives--;
            return stock.Alive;
        }

    }

    public class StockMatch : IMatchRule {

        //TODO: Remove UI Code from this class

        //TODO: cleanup

        [SerializeField]
        private int stock = 5;

        private List<Stock> characterStocks;
        private Mediator eventManager;

        public StockMatch() {
            eventManager = GlobalEventManager.Instance;
            eventManager.Subscribe<Match.MatchEvent>(OnMatchStart);
            eventManager.Subscribe<Match.SpawnPlayer>(OnSpawn);
            characterStocks = new List<Stock>();
        }

        ~StockMatch() {
           eventManager.Unsubscribe<Match.MatchEvent>(OnMatchStart);
           eventManager.Unsubscribe<Match.SpawnPlayer>(OnSpawn);
        }

        public StockMatch(int stockCount) : this() {
            stock = stockCount;
        }

        public bool IsFinished {
            get {
                return characterStocks.Sum(s => s.Alive ? 1 : 0) > 1;
            }
        }

        public Character Winner {
            get {
                Stock winner = null;
                foreach (var characterStock in characterStocks.Where(s => s.Alive)) {
                    if (winner == null)
                        winner = characterStock;
                    else
                        winner = characterStock.Lives > winner.Lives ? characterStock : winner;
                }
                return winner == null ? null : winner.Character;
            }
        }

        void OnMatchStart(Match.MatchEvent eventArgs) {
            if (!eventArgs.start)
                return;
            characterStocks.Clear();
            BlastZone.Instance.gameObject.AddComponent<StockRespawn>();
        }

        void OnSpawn(Match.SpawnPlayer eventArgs) {
            var characterStock = eventArgs.PlayerObject.AddComponent<Stock>();
            characterStock.Lives = stock;
            characterStocks.Add(characterStock);
            eventArgs.PlayerObject.AddComponent<Damage>();
        }

    }

}