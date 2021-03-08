using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.DataAccess.Interfaces
{
    public interface ICachingService
    {
        List<Menu> GetMenus();

        void UpsertMenu(Menu m);

        void DeleteMenu(string title);

    }


    public class Menu
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
