using System.Text;
using System.Text.RegularExpressions;

namespace TUP;

public class Asistencia {
    public string Telefono { get; set; }
    public List<DateTime> Fechas { get; set; } = new();

    public Asistencia(string telefono) {
        Telefono = telefono;
    }

    public override string ToString() {
        var builder = new StringBuilder();
        builder.AppendLine($"Estudiante: {Telefono}");
        builder.AppendLine("Fechas de asistencia:");
        foreach (var date in Fechas) {
            builder.AppendLine($"  - {date:dd/MM/yyyy}");
        }
        return builder.ToString();
    }
}

public static class Asistencias {
    private static readonly Regex EmojiRegex = new Regex(@"[\u00A9\u00AE\u203C\u2049\u2122\u2139\u2194-\u2199\u21A9-\u21AA\u231A-\u231B\u2328\u23CF\u23E9-\u23F3\u23F8-\u23FA\u24C2\u25AA-\u25AB\u25B6\u25C0\u25FB-\u25FE\u2600-\u2604\u260E\u2611\u2614-\u2615\u2618\u261D\u2620\u2622-\u2623\u2626\u262A\u262E-\u262F\u2638-\u263A\u2640\u2642\u2648-\u2653\u265F-\u2660\u2663\u2665-\u2666\u2668\u267B\u267E-\u267F\u2692-\u2697\u2699\u269B-\u269C\u26A0-\u26A1\u26AA-\u26AB\u26B0-\u26B1\u26BD-\u26BE\u26C4-\u26C5\u26C8\u26CE-\u26CF\u26D1\u26D3-\u26D4\u26E9-\u26EA\u26F0-\u26F5\u26F7-\u26FA\u26FD\u2702\u2705\u2708-\u270D\u270F\u2712\u2714\u2716\u271D\u2721\u2728\u2733-\u2734\u2744\u2747\u274C\u274E\u2753-\u2755\u2757\u2763-\u2764\u2795-\u2797\u27A1\u27B0\u27BF\u2934-\u2935\u2B05-\u2B07\u2B1B-\u2B1C\u2B50\u2B55\u3030\u303D\u3297\u3299\ud83c[\udc00-\udfff]|\ud83d[\udc00-\udfff]|\ud83e[\udd10-\uddff]]");

    public static List<Asistencia> CargarAsistencias(string origen, TimeSpan startTime, TimeSpan endTime) {
        var camino = $"/Users/adibattista/Documents/GitHub/tup-25-p3/datos/{origen}";
        if (!File.Exists(camino)) {
            Console.WriteLine($"El archivo {camino} no existe.");
            return new List<Asistencia>();
        }

        var lineas = File.ReadAllLines(camino);
        var estudiantes = new Dictionary<string, Asistencia>();
        
        DateTime fechaActual = DateTime.MinValue;
        
        foreach (var linea in lineas) {
            if (string.IsNullOrWhiteSpace(linea.Trim()))
                continue;

            var dateMatch = Regex.Match(linea.Trim(), @"^(\d{1,2})/(\d{1,2})$");
            if (dateMatch.Success) {
                int dia = int.Parse(dateMatch.Groups[1].Value);
                int mes = int.Parse(dateMatch.Groups[2].Value);
                fechaActual = new DateTime(DateTime.Now.Year, mes, dia);
                continue;
            }

            var mensajeMatch = Regex.Match(linea.Trim(), @"(\d{2}):(\d{2})\s+De:\s+(\d+|unknown)\s+-\s+Mensaje:\s+(.*)");
            if (mensajeMatch.Success && fechaActual != DateTime.MinValue) {
                int horas = int.Parse(mensajeMatch.Groups[1].Value);
                int minutos = int.Parse(mensajeMatch.Groups[2].Value);
                string telefono = mensajeMatch.Groups[3].Value;
                string mensaje  = mensajeMatch.Groups[4].Value;

                if (telefono == "unknown")
                    continue;

                var messageTime = new TimeSpan(horas, minutos, 0);
                if (messageTime >= startTime && messageTime <= endTime) {
                    if (EmojiRegex.IsMatch(mensaje)) {
                        if (!estudiantes.ContainsKey(telefono)) {
                            estudiantes[telefono] = new Asistencia(telefono);
                        }

                        if (!estudiantes[telefono].Fechas.Any(d => d.Date == fechaActual.Date)) {
                            estudiantes[telefono].Fechas.Add(fechaActual);
                        }
                    }
                }
            }
        }

        return estudiantes.Values.ToList();
    }

    public static List<Asistencia>  Cargar(bool listar = false) {
        var salida = new Dictionary<string, List<DateTime>>();
        foreach (var origen in Directory.GetFiles("./asistencias", "*.md")){   
            Consola.Escribir($"Cargando el archivo {origen}");
            bool primerTurno = origen.Contains("C3");
            TimeSpan inicio = new TimeSpan(primerTurno ?  8 : 10,  0, 0);
            TimeSpan final  = new TimeSpan(primerTurno ? 10 : 12, 30, 0);
            
            List<Asistencia> estudiantes = CargarAsistencias(origen, inicio, final);
            foreach (var estudiante in estudiantes) {
                if (!salida.ContainsKey(estudiante.Telefono)) {
                    salida[estudiante.Telefono] = new List<DateTime>();
                }
                salida[estudiante.Telefono].AddRange(estudiante.Fechas);
            }
        }

        // Cuenta cuantas veces hay asistencias en la fecha
        int antes = 0, despues=0;
        var contador = new Dictionary<DateTime, int>();
        foreach(var (telefono, fechas) in salida){
            foreach (var fecha in fechas.Distinct()) {
                contador[fecha.Date] = contador.GetValueOrDefault(fecha.Date) + 1;
                antes++;
            }
        }
        // Filtra las fechas que tienen más de 30 asistencias
        foreach(var (telefono, fechas) in salida){
            salida[telefono] = fechas.Where(f => contador[f.Date] > 30).ToList();
            despues += fechas.Count;
        }

        Consola.Escribir($"Hay {salida.Count} alumnos con {despues} asistencias (de {antes}) ");

        List<Asistencia> asistencias = salida
            .Select(item => new Asistencia(item.Key) { Fechas = item.Value })
            .ToList();
        if (listar) {
            Consola.Escribir("=== Asistencias ===", ConsoleColor.Cyan);
            Consola.Escribir($"Hay {asistencias.Count} asistencias", ConsoleColor.Cyan);
            foreach (var asistencia in asistencias) {
                Console.WriteLine(asistencia);
            }
        }
        // Ordena la lista por el número de asistencias
        return asistencias;
    }
}


