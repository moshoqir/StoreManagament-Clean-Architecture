using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Data
{
    // in Dapper, we must open and close connection (not like EF). Therefore, we need to make interface for creating connection and use it in SqlConnectionFactory.
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
