using FantasyData.NFL.Entities;
using FantasyData.NFL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FantasyData.NFL.Services
{
    public class PlayerService : FantasyDataApiBase, IPlayerService
    {
        private static readonly string FreeAgentsKey = "FreeAgents";
        private static readonly string FantasyPlayerKey = "FantasyPlayers";
        private static readonly string PlayerKey = "Player";
        private static readonly string PlayersKey = "Players";

        public PlayerService(string baseUrl, string primarySubscriptionKey, string secondarySubscriptionKey)
            : base(baseUrl, primarySubscriptionKey, secondarySubscriptionKey)
        { }

        public PlayerService()
            : this(Config.ApiBaseUrl.ToString(), Config.PrimarySubscriptionKey, Config.SecondarySubscriptionKey)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Players GetFreeAgents()
        {
            var url = string.Format("/{0}", FreeAgentsKey);
            return this.GetRequest<Players>(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Players> GetFreeAgentsAsync()
        {
            var url = string.Format("/{0}", FreeAgentsKey);
            return await this.GetRequestAsync<Players>(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FantasyPlayers GetFantasyPlayers()
        {
            var url = string.Format("/{0}", FantasyPlayerKey);
            return this.GetRequest<FantasyPlayers>(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<FantasyPlayers> GetFantasyPlayersAsync()
        {
            var url = string.Format("/{0}", FantasyPlayerKey);
            return await this.GetRequestAsync<FantasyPlayers>(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Player Get(int id)
        {
            var url = string.Format("/{0}/{1}", PlayerKey, id);
            return this.GetRequest<Player>(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Player> GetAsync(int id)
        {
            var url = string.Format("/{0}/{1}", PlayerKey, id);
            return await this.GetRequestAsync<Player>(url);
        }

        public IEnumerable<Player> GetAvailablePlayers(string qs)
        {
            var queryString = HttpUtility.ParseQueryString(qs);
            var url = string.Format("/{0}?{1}", PlayersKey, queryString);
            
            return this.GetRequest<List<Player>>(url);
        }

        public async Task<IEnumerable<Player>> GetAvailablePlayersAsync(string qs)
        {
            var queryString = HttpUtility.ParseQueryString(qs);
            var url = string.Format("/{0}?{1}", PlayersKey, queryString);

            return await this.GetRequestAsync<List<Player>>(url);
        }
    }
}
