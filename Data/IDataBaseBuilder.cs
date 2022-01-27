using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.Data
{
    public interface ISQlConnectionBuilder
    {
        void AddDataBaseTemplate();

        void CreateConnection();

        void AddParametrs();
        void AddProcedure();
    }
}
