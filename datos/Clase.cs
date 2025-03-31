using System.Collections;
using System.Text.RegularExpressions;
using TUP;

class Clase : IEnumerable<Alumno> {
    public List<Alumno> alumnos = new List<Alumno>();
    const string LineaComision = @"##.*\s(C\d)";
    const string LineaAlumno = @"(\d+)\.\s*(\d{5})\s*([^,]+)\s*,\s*([^(]+)(?:\s*(\(\d+\)\s*\d+(?:[- ]\d+)*))?";
    
    public Clase(IEnumerable<Alumno>? alumnos = null){
        this.alumnos = alumnos?.ToList() ?? new List<Alumno>();
    }

    public List<string> Comisiones => alumnos.Select(a => a.comision).Distinct().OrderBy(c => c).ToList();
    public IEnumerable<Alumno> Alumnos => alumnos.OrderBy(a => a.apellido).ThenBy(a => a.nombre);

    public static Clase Cargar(string origen){
        string comision = "C0";
        Clase clase = new Clase();

        foreach (var linea in File.ReadLines(origen)){
            var texto = linea.PadRight(100,' '); 
            var practicos = texto.Substring(80, 20).Trim();
            texto = texto.Substring(0, 80);
            var matchComision = Regex.Match(texto, LineaComision);
            if (matchComision.Success) {
                comision = matchComision.Groups[1].Value;
                continue;
            } 

            var matchAlumno = Regex.Match(texto, LineaAlumno);
            if (matchAlumno.Success) {
                string telefono = "";
                if (matchAlumno.Groups[5].Success) {
                    telefono = matchAlumno.Groups[5].Value.Trim();
                }
                
                Alumno alumno = new Alumno(
                    int.Parse(matchAlumno.Groups[1].Value), 
                    int.Parse(matchAlumno.Groups[2].Value),
                    matchAlumno.Groups[3].Value.Trim(),
                    matchAlumno.Groups[4].Value.Trim(),
                    telefono,
                    comision,
                    practicos
                );

                clase.alumnos.Add(alumno);
                continue;
            } 
        }
        
        return clase;
    }

    public Clase Agregar(Alumno alumno){
        if (alumno != null)  alumnos.Add(alumno);
        return this;
    }   

    public Clase Agregar(IEnumerable<Alumno> alumnos){
        foreach(var alumno in alumnos ?? [])
            Agregar(alumno);
        return this;
    }

    public Clase EnComision(string comision) => new (alumnos.Where(a => a.comision == comision));
    public Clase ConTelefono(bool incluirTelefono=true) => new (alumnos.Where(a => incluirTelefono == a.TieneTelefono ));

    public void Guardar(string destino){
        using (StreamWriter writer = new StreamWriter(destino)){
            writer.WriteLine("# Listado de alumnos");
            foreach(var comision in Comisiones){
                writer.WriteLine("");
                var alumnosPorComision = alumnos.Where(a => a.comision == comision).OrderBy(a => a.apellido).ThenBy(a => a.nombre);
                var orden = 0;
                writer.WriteLine($"## Comisión {comision}");
                foreach(var alumno in alumnosPorComision){
                    alumno.orden = ++orden;
                    string lineaBase = $"{alumno.orden:D2}.  {alumno.legajo}  {alumno.NombreCompleto,-40}  {alumno.telefono,-15}";
                    writer.WriteLine($"{lineaBase,-78}  {alumno.practicos}");
                }
            }
        }
    }

    public void ExportarVCards(string destino){
        Consola.Escribir($"- Exportando vCards a {destino}");
        using (StreamWriter writer = new StreamWriter(destino)){
            foreach(var alumno in alumnos){
                var linea = $"""
                BEGIN:VCARD
                VERSION:3.0
                N:{alumno.apellido};{alumno.nombre};;;
                FN:{alumno.nombre} {alumno.apellido}
                ORG:TUP-25-P3-{alumno.comision}
                TEL;TYPE=CELL:{alumno.telefono}
                TEL;TYPE=Legajo:{alumno.legajo}
                END:VCARD
                """;
                writer.WriteLine(linea);
            }
        }
    }
    
    public void CrearCarpetas(){
        const string Base = "../TP";
        Directory.CreateDirectory(Base);
        Consola.Escribir($"▶︎ Creando carpetas en {Path.GetFullPath(Base)}", ConsoleColor.Cyan);
        foreach (var alumno in Alumnos.OrderBy(a => a.legajo))
        {
            string carpetaDeseada = $"{alumno.legajo} - {alumno.NombreCompleto}";
            string rutaCompleta = Path.Combine(Base, carpetaDeseada);

            // Si ya existe la carpeta con el nombre correcto, continuamos
            if (Directory.Exists(rutaCompleta))
                continue;

            // Buscar carpetas que empiecen con el legajo actual
            string patron = $"{alumno.legajo} -*";
            var carpetasExistentes = Directory.GetDirectories(Base, patron);

            if (carpetasExistentes.Length > 0)
            {
                // Usamos la primera carpeta encontrada y, si el nombre es distinto, la renombramos
                string carpetaEncontrada = carpetasExistentes[0];
                if (Path.GetFileName(carpetaEncontrada) != carpetaDeseada)
                {
                    Consola.Escribir($"  - Renombrando carpeta: \n     [{Path.GetFileName(carpetaEncontrada)}]\n   a [{carpetaDeseada}]", ConsoleColor.Yellow);
                    Directory.Move(carpetaEncontrada, rutaCompleta);
                }
            }
            else
            {
                Consola.Escribir($"  - Creando carpeta {carpetaDeseada}", ConsoleColor.Green);
                // No existe ninguna carpeta con ese legajo, la creamos
                Directory.CreateDirectory(rutaCompleta);
            }
        }
        Consola.Escribir($"● Carpetas creadas", ConsoleColor.Green);
    }

    public static int ContarLineasEfectivas(string archivo)
    {
        if (!File.Exists(archivo))
        {
            Consola.Escribir($"El archivo {archivo} no existe.");
            return 0;
        }

        var lineas = File.ReadAllLines(archivo);

        return lineas.Count(linea =>
             // No está vacía ni es solo espacios
            !linea.Trim().Equals("") &&
            !linea.TrimStart().StartsWith("using") && // No es una directiva using
            !linea.TrimStart().StartsWith("//") && // No es un comentario
            !linea.Trim().Equals("{") && // No es solo una llave de apertura
            !linea.Trim().Equals("}") // No es solo una llave de cierre
        );
    }

    public static void VerificaPresentacionDeTrabajoPractico(int practico)
    {
        const string Base = "../TP";
        string NombreCarpetaTP = $"TP{practico}";
        string NombreArchivo = "ejercicio.cs";

        if (!Directory.Exists(Base))
        {
            Consola.Escribir($"La carpeta base {Base} no existe.", ConsoleColor.Red);
            return;
        }

        Consola.Escribir($"=== Verificación de presentación del trabajo práctico {NombreCarpetaTP} ===", ConsoleColor.Cyan);

        // Cargar la clase de alumnos al principio, una sola vez
        var alumnosClase = Clase.Cargar("./alumnos.md");

        // Crear una lista para almacenar los resultados
        var resultados = new List<(string NombreCarpeta, int LineasEfectivas, bool ArchivoExiste, string Comision, int Legajo)>();

        foreach (var carpetaAlumno in Directory.GetDirectories(Base))
        {
            var carpetaTP = Path.Combine(carpetaAlumno, NombreCarpetaTP);
            var archivoEjercicio = Path.Combine(carpetaTP, NombreArchivo);
            string nombreCarpeta = Path.GetFileName(carpetaAlumno);
            
            // Intentar extraer la comisión y el legajo del nombre de la carpeta
            string comision = "Sin asignar";
            int legajo = 0;
            var legajoMatch = Regex.Match(nombreCarpeta, @"^(\d+)");
            if (legajoMatch.Success)
            {
                // Extraemos el legajo del nombre de la carpeta
                legajo = int.Parse(legajoMatch.Groups[1].Value);
                
                // Buscamos el alumno con ese legajo para obtener su comisión
                var alumno = alumnosClase.alumnos.FirstOrDefault(a => a.legajo == legajo);
                if (alumno != null)
                {
                    comision = alumno.comision;
                }
            }
            
            if (File.Exists(archivoEjercicio))
            {
                int lineasEfectivas = ContarLineasEfectivas(archivoEjercicio);
                resultados.Add((nombreCarpeta, lineasEfectivas, true, comision, legajo));
            }
            else
            {
                resultados.Add((nombreCarpeta, 0, false, comision, legajo));
            }
        }
        
        // Agrupar por comisión y luego ordenar por apellido y nombre dentro de cada grupo
        var resultadosAgrupados = resultados
            .GroupBy(r => r.Comision)
            .OrderBy(g => g.Key);

        // Mostrar los resultados agrupados por comisión
        foreach (var grupo in resultadosAgrupados)
        {
            Consola.Escribir($"\n--- Comisión {grupo.Key} ---", ConsoleColor.Cyan);
            
            // Ordenar alumnos dentro de cada comisión por apellido y nombre
            var alumnosOrdenados = grupo.OrderBy(r => {
                // Extraer el apellido y nombre si es posible
                var partes = r.NombreCarpeta.Split(new[] { " - " }, StringSplitOptions.None);
                return partes.Length > 1 ? partes[1] : r.NombreCarpeta;
            });
            
            int presentaron = 0;
            int noPresentaron = 0;
            
            foreach (var resultado in alumnosOrdenados)
            {
                // Buscar el alumno correspondiente en la lista
                var alumno = alumnosClase.alumnos.FirstOrDefault(a => a.legajo == resultado.Legajo);
                
                if (alumno == null) continue;
                if (!int.TryParse(NombreCarpetaTP.Replace("TP", ""), out int numeroTP)) continue;

                // Asegurarse de que practicos tenga suficiente longitud
                alumno.practicos = alumno.practicos.PadRight(20, ' ');
                EstadoPractico estado = EstadoPractico.Error;
                
                if (resultado.ArchivoExiste)
                {
                    estado = resultado.LineasEfectivas >= 40 ? EstadoPractico.Aprobado : EstadoPractico.NoPresentado;
                }
                
                if (estado == EstadoPractico.Aprobado)
                    presentaron++;
                else
                    noPresentaron++;
                    
                alumno.PonerPractico(numeroTP, estado);
                Consola.Escribir($"{resultado.NombreCarpeta, -60} {resultado.LineasEfectivas, 3}", estado.Color);
            }
            
            // Mostrar resumen al final de la comisión
            Consola.Escribir($"Total Presentaron: {presentaron} | No presentaron: {noPresentaron}", ConsoleColor.Yellow);
            alumnosClase.Guardar("./alumnos.md");
        }
    }

    public void CopiarTrabajoPractico(string origen, bool forzar=false){
        const string Base = "../TP";
        const string Enunciados = "../Enunciados";
        Consola.Escribir($" ▶︎ Copiando trabajo práctico de {origen}", ConsoleColor.Cyan);
        var carpetaOrigen = Path.Combine(Enunciados, origen);
        
        // Verificar que exista el enunciado
        if (!Directory.Exists(carpetaOrigen))
        {
            Consola.Escribir($"Error: No se encontró el enunciado del trabajo práctico '{origen}' en {carpetaOrigen}", ConsoleColor.Red);
            return;
        }

        foreach (var alumno in Alumnos.OrderBy(a => a.legajo))
        {
            var carpetaDestino = Path.Combine(Base, $"{alumno.legajo} - {alumno.NombreCompleto}", origen);
            if(forzar && Directory.Exists(carpetaDestino)){
                Directory.Delete(carpetaDestino, true);
            }
            Directory.CreateDirectory(carpetaDestino);

            Consola.Escribir($" - Copiando a {carpetaDestino}", ConsoleColor.Yellow);
            foreach (var archivo in Directory.GetFiles(carpetaOrigen))
            {
                var nombreArchivo = Path.GetFileName(archivo);
                // Skip copying the enunciado.md file
                if(nombreArchivo == "enunciado.md") continue;
                
                var destinoArchivo = Path.Combine(carpetaDestino, nombreArchivo);
                if (!File.Exists(destinoArchivo))
                {
                    File.Copy(archivo, destinoArchivo);
                }
            }
        }
        Consola.Escribir($" ● Copia de trabajo práctico completa", ConsoleColor.Green);
    }
    
    public void ExportarDatos(){
        Consola.Escribir($" ▶︎ Generando listado de alumnos (Hay {alumnos.Count()} alumnos.)", ConsoleColor.Cyan);
        Guardar("./resultados.md");

        ConTelefono(true).EnComision("C3").ExportarVCards("./alumnos-c3.vcf");
        ConTelefono(true).EnComision("C5").ExportarVCards("./alumnos-c5.vcf");
        ExportarVCards("./alumnos.vcf");
        Consola.Escribir($" ● Exportacion completa", ConsoleColor.Green);
    }

    public void ListarAlumnosPorComision()
    {
        Consola.Escribir("\nListado de alumnos agrupados por comisión y ordenados alfabéticamente:", ConsoleColor.Blue);

        // Agrupar por comisión
        var alumnosPorComision = alumnos
            .GroupBy(a => a.comision)
            .OrderBy(g => g.Key);

        foreach (var grupo in alumnosPorComision)
        {
            Consola.Escribir($"\n=== Comisión {grupo.Key} ===", ConsoleColor.Blue);
            
            // Ordenar alumnos por nombre completo dentro de cada comisión
            var alumnosOrdenados = grupo
                .OrderBy(a => a.NombreCompleto)
                .ToList();
            
            foreach (var alumno in alumnosOrdenados)
            {
                string asistencia = alumno.practicos.Trim().Select( c => EstadoPractico.FromChar(c).emoji()).Aggregate("", (a, b) => a + b);
                string linea = $"{alumno.legajo} - {alumno.NombreCompleto, -40} {alumno.telefono, -20}";
                Consola.Escribir($" {linea,-78}  {asistencia}");
            }
            
            // Mostrar totales por comisión
            Consola.Escribir($"Total alumnos en comisión {grupo.Key}: {alumnosOrdenados.Count}", ConsoleColor.Yellow);
        }
        
        // Mostrar total general
        Consola.Escribir($"\nTotal general de alumnos: {alumnos.Count}", ConsoleColor.Green);
    }

    public void ListarAusentes(int numeroTP)
    {
        Consola.Escribir($"\nListado de alumnos ausentes en el TP{numeroTP} agrupados por comisión:", ConsoleColor.Yellow);
        
        // Crear una lista para almacenar los alumnos ausentes
        var alumnosAusentes = new List<Alumno>();
        
        // Verificar cada alumno según su estado en el práctico
        foreach (var alumno in alumnos)
        {
            // Asegurarse de que practicos tenga suficiente longitud
            alumno.practicos = alumno.practicos.PadRight(20, ' ');
            
            // Obtener el estado del práctico directamente de la información del alumno
            EstadoPractico estado = alumno.ObtenerPractico(numeroTP);
            
            // Si no está aprobado, agregarlo a la lista de ausentes
            if (estado != EstadoPractico.Aprobado)
            {
                alumnosAusentes.Add(alumno);
            }
        }
        
        // Agrupar por comisión
        var ausentesPorComision = alumnosAusentes
            .GroupBy(a => a.comision)
            .OrderBy(g => g.Key);
        
        // Mostrar resultados agrupados por comisión
        foreach (var grupo in ausentesPorComision)
        {
            Consola.Escribir($"\n=== Ausentes Comisión {grupo.Key} ===", ConsoleColor.Cyan);
            
            // Ordenar alumnos por nombre completo dentro de cada comisión
            var alumnosOrdenados = grupo
                .OrderBy(a => a.NombreCompleto)
                .ToList();
            
            if (alumnosOrdenados.Count == 0)
            {
                Consola.Escribir("No hay alumnos ausentes en esta comisión.", ConsoleColor.Green);
                continue;
            }
            
            foreach (var alumno in alumnosOrdenados)
            {
                Consola.Escribir($" {alumno.legajo} - {alumno.NombreCompleto}", ConsoleColor.Red);
            }
            
            // Mostrar total de ausentes en la comisión
            Consola.Escribir($"Total ausentes en comisión {grupo.Key}: {alumnosOrdenados.Count}", ConsoleColor.Yellow);
        }
        
        // Mostrar total general de ausentes
        Consola.Escribir($"\nTotal general de ausentes: {alumnosAusentes.Count} de {alumnos.Count} alumnos", ConsoleColor.Yellow);
    }

    public IEnumerator<Alumno> GetEnumerator() => alumnos.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}