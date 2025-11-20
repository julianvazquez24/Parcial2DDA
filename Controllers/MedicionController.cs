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

        public MedicionController(AppDbContext context)
        {
            _context = context;
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
        public IActionResult getRecuentoReportes()
        {
            int cantidadReportes = _context.Reportes.Count();
            if (cantidadReportes > 0)
            {
                RecuentoReportesDTO cantidadReportesDTO = new RecuentoReportesDTO
                {
                    total_mediciones_completadas = cantidadReportes,
                };

                return Ok(cantidadReportesDTO);
            }
            return BadRequest("no hay reportes");
        }

        [HttpGet("reportes/maxima_diferencia_peso")]
        public IActionResult getMaxDiferenciaPeso()
        {
            decimal maxDiferenciaPeso = _context.Reportes.Max(r => r.DiferenciaPeso);

            if (maxDiferenciaPeso != null)
            {
                DiferenciaPesoDTO maxdiferenciaPesoDTO = new DiferenciaPesoDTO
                {
                    maxima_diferencia_peso = maxDiferenciaPeso,
                };

                return Ok(maxdiferenciaPesoDTO);
            }

            return BadRequest("no hay reportes");
        }

        [HttpGet("reportes/maximo_tiempo")]
        public IActionResult getMaxDiferenciaTiempo()
        {
            int tiempoEnSegundos = _context.Reportes.Max(r => r.DiferenciaTiempo);
            if (tiempoEnSegundos == null)
            {
                return BadRequest("no hay reportes");
            }

            int tiempoEnMinutos = tiempoEnSegundos / 60;
            int tiempoEnHoras = tiempoEnMinutos / 60;
            string diferenciaTiempoTotal = $"{tiempoEnHoras} horas, {tiempoEnMinutos - (tiempoEnHoras * 60)} minutos, {tiempoEnSegundos - (tiempoEnMinutos * 60)} segundos";

          
            DiferenciaTiempoDTO maxdiferenciaTiempoDTO = new DiferenciaTiempoDTO
            {
                maximo_tiempo = diferenciaTiempoTotal,
            };

            return Ok(maxdiferenciaTiempoDTO);
        }




    }
}
