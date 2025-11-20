using Parcial2DDA.ResponseDtos;

namespace Parcial2DDA.Services
{
    public interface IReporteMediciones
    {
        Task<DiferenciaPesoDTO> traerMaxDiferenciaPeso();
        Task<DiferenciaTiempoDTO> traerMaxDiferenciaTiempo();

        Task<RecuentoReportesDTO> traerCantidadReportes();
    }
}
