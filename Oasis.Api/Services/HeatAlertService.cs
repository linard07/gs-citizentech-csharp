
using Oasis.Domain.Models;

namespace Oasis.Api.Services;

public class HeatAlertService
{
    public HeatAlert Analyze(double temperatureC)
    {
        var alert = new HeatAlert
        {
            TemperatureC = temperatureC,
            IsSimulation = true
        };

        if (temperatureC < 30)
        {
            alert.RiskLevel = "LOW";
            alert.Message = "Condição sem alerta demonstrativo de calor intenso.";
            alert.Recommendations = new List<string>
            {
                "Mantenha uma hidratação adequada.",
                "Observe as condições meteorológicas.",
                "Prefira locais ventilados."
            };
        }
        else if (temperatureC < 35)
        {
            alert.RiskLevel = "MODERATE";
            alert.Message = "Atenção: temperatura elevada.";
            alert.Recommendations = new List<string>
            {
                "Beba água regularmente.",
                "Procure locais com sombra.",
                "Evite esforços intensos nos horários mais quentes."
            };
        }
        else if (temperatureC < 40)
        {
            alert.RiskLevel = "HIGH";
            alert.Message = "Alerta demonstrativo de calor intenso.";
            alert.Recommendations = new List<string>
            {
                "Reduza a exposição direta ao sol.",
                "Procure ambientes frescos e ventilados.",
                "Mantenha a hidratação.",
                "Dê atenção especial a pessoas vulneráveis."
            };
        }
        else
        {
            alert.RiskLevel = "EXTREME";
            alert.Message = "Alerta demonstrativo de calor extremo.";
            alert.Recommendations = new List<string>
            {
                "Evite atividades externas desnecessárias.",
                "Procure um ambiente fresco.",
                "Beba água regularmente.",
                "Observe sinais de mal-estar relacionados ao calor.",
                "Em caso de sintomas graves, procure atendimento de emergência."
            };
        }

        return alert;
    }
}
