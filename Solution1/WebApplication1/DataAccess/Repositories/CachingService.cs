using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DataAccess.Interfaces;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebApplication1.DataAccess.Repositories
{
    public class CachingService : ICachingService
    {
        private IDatabase db;
        private readonly IConfiguration _config;
        public CachingService(IConfiguration config)
        {
            _config = config;
            var connectionString = config.GetConnectionString("cachedb");
            var cm = ConnectionMultiplexer.Connect(connectionString);

            db  = cm.GetDatabase();   
        }

        public void DeleteMenu(string title)
        { 
            
            throw new NotImplementedException();
        }

        public List<Menu> GetMenus()
        {

            if (db.KeyExists("menus"))
            {
                var result = JsonConvert.DeserializeObject<List<Menu>>(
                     db.StringGet("menus")
                     );

                return result;
            }
            else return new List<Menu>();
        }

        public void UpsertMenu(Menu m)
        {
            List<Menu> existentMenus = GetMenus();
            if(existentMenus.Count(x=>x.Title == m.Title) > 0)
            {
                var originalMenu = existentMenus.SingleOrDefault(x => x.Title == m.Title);
                originalMenu.Url = m.Url;
                originalMenu.Title = m.Title;
            }
            else
            {
                existentMenus.Add(m);
            }


            var serializedMenus = JsonConvert.SerializeObject(existentMenus);

            db.StringSet("menus",
                serializedMenus
                );

        }
    }
}
