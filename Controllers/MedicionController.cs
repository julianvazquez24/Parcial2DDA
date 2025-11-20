using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial2DDA.Data;
using Parcial2DDA.Migrations;
using Parcial2DDA.Models;
using Parcial2DDA.ResponseDtos;
using Parcial2DDA.Services;
namespace Parcial2DDA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IReporteMediciones _reportes;

        public MedicionController(AppDbContext context, IReporteMediciones reportes)
        {
            _context = context;
            _reportes = reportes;
        }
        

        [HttpPost("medicion")]
        public IActionResult postMedicion(string huellaCliente, decimal pesoCliente, string tipo)
        {
            if (huellaCliente == null || pesoCliente == null)
            {
                return BadRequest("no se ingresaron los datos necesarios");
            }
            if (tipo == null || tipo != "entrada" && tipo != "salida")
            {
                return BadRequest("el tipo debe ser de entrada o salida");
            }
            if(tipo == "salida")
            {
                Medicion tieneMedicionEntrada = _context.Mediciones.FirstOrDefault(m=> m.Huella ==  huellaCliente);
                if(tieneMedicionEntrada == null)
                {
                    return BadRequest("para que un cliente tenga una medicion de tipo salida primero debe de tener una de tipo entrada");
                }
                
                Medicion nuevamedicionSalida = new Medicion
                {
                    Huella = huellaCliente,
                    Peso = pesoCliente,
                    Hora = DateTime.Now,
                    Tipo = tipo
                };
                _context.Mediciones.Add(nuevamedicionSalida);
                _context.SaveChanges();

                decimal diferenciaPeso = nuevamedicionSalida.Peso - tieneMedicionEntrada.Peso ;
                int ts1 = (int)((DateTimeOffset)tieneMedicionEntrada.Hora).ToUnixTimeSeconds();
                int ts2 = (int)((DateTimeOffset)nuevamedicionSalida.Hora).ToUnixTimeSeconds();
                int diferenciaTiempo = ts2 - ts1;
                Reporte reporte = new Reporte
                {
                    DiferenciaPeso = diferenciaPeso,
                    DiferenciaTiempo = diferenciaTiempo,
                };
                _context.Reportes.Add(reporte);


                Medicion medicionSalidaEliminar = _context.Mediciones.FirstOrDefault(m => m.Huella == huellaCliente && m.Tipo == tipo); 
                _context.Mediciones.Remove(tieneMedicionEntrada);
                _context.SaveChanges();
                _context.Mediciones.Remove(medicionSalidaEliminar);
                _context.SaveChanges();
            }

            Medicion nuevamedicion = new Medicion
            {
                Huella = huellaCliente,
                Peso = pesoCliente,
                Hora = DateTime.Now,
                Tipo = tipo
            };
            _context.Mediciones.Add(nuevamedicion);
            _context.SaveChanges();
            return Ok();
        }


        [HttpGet("reportes/total")]
        public async Task<IActionResult> getRecuentoReportes()
        {

            RecuentoReportesDTO cantidadReportesDTO =await  _reportes.traerCantidadReportes();

            if (cantidadReportesDTO == null)
            {
                return BadRequest("no hay reportes");
            }
            return Ok(cantidadReportesDTO);
        }

        [HttpGet("reportes/maxima_diferencia_peso")]
        public async Task<IActionResult> getMaxDiferenciaPeso()
        {
     
            DiferenciaPesoDTO diferenciaPesoDTO = await  _reportes.traerMaxDiferenciaPeso();

            if (diferenciaPesoDTO == null)
            {
                return BadRequest("no hay reportes");
            }
            return Ok(diferenciaPesoDTO);



        }

        [HttpGet("reportes/maximo_tiempo")]
        public async Task<IActionResult> getMaxDiferenciaTiempo()
        {
            DiferenciaTiempoDTO diferenciaTiempoDTO = await _reportes.traerMaxDiferenciaTiempo();
            if( diferenciaTiempoDTO == null)
            {
                return BadRequest("no hay reportes");
            }

            return Ok(diferenciaTiempoDTO);
        }




    }
}
