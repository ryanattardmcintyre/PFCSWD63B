using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Domain;

namespace WebApplication1.DataAccess.Interfaces
{
    public interface IPubSubRepository
    {
        void PublishEmail(string email, Blog b);
    }
}
