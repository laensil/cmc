using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace CMCrepairs
{
    public class DBConnection
    {

        //MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=CMCsales;database=test;persist security info=false");
        MySqlConnection myConn = new MySqlConnection("server=localhost; user id=root;password=root;database=test;persist security info=false");

    }
}
