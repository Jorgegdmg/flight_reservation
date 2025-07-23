using FlightApi.DTOs;
using FlightData.Data;
using FlightData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PassengersController(AppDbContext context)
        {
            _context = context;
        }

        

    }
}