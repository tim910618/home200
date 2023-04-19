using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;

namespace api1.Service
{
    public class ReportDBService
    {
        private readonly SqlConnection conn;

        public ReportDBService(SqlConnection connection)
        {
            conn = connection;
        }

        public void InserReport()
        {

        }

    }
}