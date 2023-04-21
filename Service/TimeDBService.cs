using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;

namespace api1.Service
{
    public class TimeDBService
    {
        private readonly SqlConnection conn;

        public TimeDBService(SqlConnection connection)
        {
            conn = connection;
        }

        public void AddBookTime(BookTime Data){
            
        }

    }
}