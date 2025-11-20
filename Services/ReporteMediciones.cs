using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Parcial2DDA.ResponseDtos;
using Parcial2DDA.Data;

namespace Parcial2DDA.Services
{

    public class ReporteMediciones 
    {
        private readonly AppDbContext _context;

        public ReporteMediciones(AppDbContext context)
        {
            _context = context;
        }

    }
}
