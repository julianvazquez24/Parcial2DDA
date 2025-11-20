using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Parcial2DDA.ResponseDtos;
using Parcial2DDA.Data;

namespace Parcial2DDA.Services
{

    public class ReporteMediciones : IReporteMediciones
    {
        private readonly AppDbContext _context;

        public ReporteMediciones(AppDbContext context)
        {
            _context = context;
        }
        public async Task<DiferenciaPesoDTO> traerMaxDiferenciaPeso()
        {
            decimal maxDiferenciaPeso = _context.Reportes.Max(r => r.DiferenciaPeso);

            if (maxDiferenciaPeso != null)
            {
                DiferenciaPesoDTO maxdiferenciaPesoDTO = new DiferenciaPesoDTO
                {
                    maxima_diferencia_peso = maxDiferenciaPeso,
                };

                return maxdiferenciaPesoDTO;
            }
            return null;
            

        }

        public  async Task<DiferenciaTiempoDTO> traerMaxDiferenciaTiempo()
        {

            int tiempoEnSegundos =  _context.Reportes.Max(r => r.DiferenciaTiempo);
            if (tiempoEnSegundos > 0)
            {
                int tiempoEnMinutos = tiempoEnSegundos / 60;
                int tiempoEnHoras = tiempoEnMinutos / 60;
                string diferenciaTiempoTotal = $"{tiempoEnHoras} horas, {tiempoEnMinutos - (tiempoEnHoras * 60)} minutos, {tiempoEnSegundos - (tiempoEnMinutos * 60)} segundos";

                DiferenciaTiempoDTO maxdiferenciaTiempoDTO = new DiferenciaTiempoDTO
                {
                    maximo_tiempo = diferenciaTiempoTotal,
                };
                return maxdiferenciaTiempoDTO;
            }

            return null;
        }

        public async Task<RecuentoReportesDTO> traerCantidadReportes()
        {
            int cantidadReportes = _context.Reportes.Count();
            if (cantidadReportes > 0)
            {
                RecuentoReportesDTO cantidadReportesDTO = new RecuentoReportesDTO
                {
                    total_mediciones_completadas = cantidadReportes,
                };

                return cantidadReportesDTO;
            }
            return null;
        }
    }
}
