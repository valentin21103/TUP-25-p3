using System.Collections;
using System.Text.RegularExpressions;
using TUP;

class Clase : IEnumerable<Alumno> {
    public List<Alumno> alumnos = new List<Alumno>();
    const string LineaComision = @"##.*\s(C\d)";
    const string LineaAlumno = @"(\d+)\.\s*(\d{5})\s*([^,]+)\s*,\s*([^(]+)(?:\s*(\(\d+\)\s*\d+(?:[- ]\d+)*))?";
    

    public List<string> Comisiones => alumnos.Select(a => a.comision).Distinct().OrderBy(c => c).ToList();
    public IEnumerable<Alumno> Alumnos => alumnos.OrderBy(a => a.NombreCompleto).ToList();

    public Clase(IEnumerable<Alumno>? alumnos = null) {
        this.alumnos = alumnos?.ToList() ?? new List<Alumno>();
    }
    public static Clase Cargar(string origen="./alumnos.md") {
        string comision = "C0";
        Clase clase = new();

        foreach (var linea in File.ReadLines(origen)) {
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
                string telefono = matchAlumno.Groups[5].Success ? matchAlumno.Groups[5].Value.Trim() : "";
                
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

    // Métodos de filtrado
    public Clase EnComision(string comision) => new (alumnos.Where(a => a.comision == comision));
    public Clase ConTelefono(bool incluirTelefono=true) => new (alumnos.Where(a => incluirTelefono == a.TieneTelefono ));
    public Clase ConPractico(int numero, EstadoPractico estado) => new (alumnos.Where(a => a.ObtenerPractico(numero) == estado));
    public Clase OrdenandoPorNombre() => new (alumnos.OrderBy(a => a.apellido).ThenBy(a => a.nombre));
    public Clase OrdenandoPorLegajo() => new (alumnos.OrderBy(a => a.legajo));

    // Métodos de modificación
    public void Agregar(Alumno alumno) {
        if (alumno != null) {
            alumnos.Add(alumno);
        }
    }   

    public void Agregar(IEnumerable<Alumno> alumnos) {
        foreach(var alumno in alumnos) {
            Agregar(alumno);
        }
    }

    public void Guardar(string destino="./alumnos.md") {
        using (StreamWriter writer = new(destino)) {
            writer.WriteLine("# Listado de alumnos");
            foreach(var comision in Comisiones) {
                var orden = 0;
                writer.WriteLine($"\n## Comisión {comision}");
                foreach(var alumno in EnComision(comision).OrdenandoPorNombre()) {
                    alumno.orden = ++orden;
                    string lineaBase = $"{alumno.orden:D2}.  {alumno.legajo}  {alumno.NombreCompleto,-40}  {alumno.telefono,-15}";
                    writer.WriteLine($"{lineaBase,-78}  {alumno.PracticosToString()}");
                }
            }
        }
    }

    public void GuardarVCards(string destino) {
        Consola.Escribir($"- Exportando vCards a {destino}");
        using (StreamWriter writer = new(destino)) {
            foreach(var alumno in ConTelefono(true).OrdenandoPorNombre()) {
                var linea = $"""
                BEGIN:VCARD\nVERSION:3.0
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
    
    public void NormalizarCarpetas() {
        const string Base = "../TP";
        Directory.CreateDirectory(Base);

        Consola.Escribir($"▶︎ Creando carpetas en {Path.GetFullPath(Base)}", ConsoleColor.Green);
        var cambios = 0;
        foreach (var alumno in OrdenandoPorLegajo() ) {
            var carpetaDeseada = alumno.Carpeta;
            var rutaCompleta = Path.Combine(Base, carpetaDeseada);

            if (Directory.Exists(rutaCompleta)) continue;
            
            var carpetasExistentes = Directory.GetDirectories(Base, $"{alumno.legajo}*");

            if (carpetasExistentes.Length > 0) {
                var carpetaEncontrada = carpetasExistentes[0];
                if (Path.GetFileName(carpetaEncontrada) != carpetaDeseada) {
                    Directory.Move(carpetaEncontrada, rutaCompleta);
                    Consola.Escribir($"  - Renombrando carpeta:\n  <[{Path.GetFileName(carpetaEncontrada)}]\n  >[{carpetaDeseada}]", ConsoleColor.Yellow);
                    cambios++;
                }
            } else {
                Directory.CreateDirectory(rutaCompleta);
                Consola.Escribir($"  - Creando carpeta:\n  >[{carpetaDeseada}]", ConsoleColor.Yellow);
                cambios++;
            }
        }
        Consola.Escribir($"● {cambios} carpetas cambiadas", ConsoleColor.Green);
    }


    public static int ContarLineasEfectivas(string archivo) {
        var lineas = File.ReadAllLines(archivo);
        return lineas.Count(linea =>
            !linea.Trim().Equals("") &&                     // No es una línea vacía
            !linea.TrimStart().StartsWith("Console.") &&    // No es un mensaje de consola
            !linea.TrimStart().StartsWith("using") &&       // No es una directiva using
            !linea.TrimStart().StartsWith("//") &&          // No es un comentario
            !linea.Trim().Equals("{") &&                    // No es solo una llave de apertura
            !linea.Trim().Equals("}")                       // No es solo una llave de cierre
        );
    }


    public void VerificaPresentacionPractico(int practico) {
        const string Base = "../TP";
        Consola.Escribir($"=== Verificación de presentación del trabajo práctico TP{practico} ===", ConsoleColor.Cyan);
        foreach(var comision in Comisiones) {
            var presentados = 0;
            var ausentes = 0;
            foreach(var alumno in EnComision(comision)){
                var archivo = Path.Combine(Base, alumno.Carpeta, $"TP{practico}", "ejercicio.cs");
                EstadoPractico estado = EstadoPractico.Error;
                if (File.Exists(archivo)) {
                    int lineasEfectivas = ContarLineasEfectivas(archivo);
                    estado = lineasEfectivas >= 40 ? EstadoPractico.Aprobado : EstadoPractico.NoPresentado;
                    if (estado == EstadoPractico.Aprobado) {
                        presentados++;
                    }
                    if (estado == EstadoPractico.NoPresentado) {
                        ausentes++;
                    }
                    alumno.PonerPractico(practico, estado);
                    Consola.Escribir($"{alumno.legajo}. {alumno.NombreCompleto, -60} {lineasEfectivas, 3} {estado}", estado.Color);
                }
            }
            Consola.Escribir($"Comisión {comision} \n Presentados: {presentados,3}\n Ausentes   : {ausentes,3}", ConsoleColor.Cyan);
        }
        Guardar();
    }

    public void CopiarPractico(int practico, bool forzar=false) {
        const string Base = "../TP";
        const string Enunciados = "../Enunciados";
        Consola.Escribir($" ▶︎ Copiando trabajo práctico de TP{practico}", ConsoleColor.Cyan);
        var carpetaOrigen = Path.Combine(Enunciados, $"TP{practico}");
        
        // Verificar que exista el enunciado
        if (!Directory.Exists(carpetaOrigen)) {
            Consola.Escribir($"Error: No se encontró el enunciado del trabajo práctico '{practico}' en {carpetaOrigen}", ConsoleColor.Red);
            return;
        }

        foreach (var alumno in Alumnos.OrderBy(a => a.legajo)) {
            var carpetaDestino = Path.Combine(Base, alumno.Carpeta, $"TP{practico}");
            if(forzar && Directory.Exists(carpetaDestino)) {
                Directory.Delete(carpetaDestino, true);
            }
            Directory.CreateDirectory(carpetaDestino);

            Consola.Escribir($" - Copiando a {carpetaDestino}", ConsoleColor.Yellow);
            foreach (var archivo in Directory.GetFiles(carpetaOrigen)) {
                var nombreArchivo = Path.GetFileName(archivo);
                if(nombreArchivo == "enunciado.md") continue;
                
                var destinoArchivo = Path.Combine(carpetaDestino, nombreArchivo);
                if (!File.Exists(destinoArchivo)) {
                    File.Copy(archivo, destinoArchivo);
                }
            }
        }
        Consola.Escribir($" ● Copia de trabajo práctico completa", ConsoleColor.Green);
    }
    
    public void ExportarDatos() {
        Consola.Escribir($" ▶︎ Generando listado de alumnos (hay {alumnos.Count()} alumnos.)", ConsoleColor.Cyan);
        foreach(var comision in Comisiones) {
            var alumnosComision = ConTelefono(true).EnComision(comision);
            alumnosComision.GuardarVCards( $"./alumnos-{comision}.vcf");
        }
        ConTelefono(true).GuardarVCards("./alumnos.vcf");
        Guardar("./resultados.md");
        Consola.Escribir($" ● Exportación completa", ConsoleColor.Green);
    }

    public void ListarAlumnos(){
        Consola.Escribir("\nListado de alumnos:", ConsoleColor.Blue);
        foreach(var comision in Comisiones){
            Consola.Escribir($"\n=== Comisión {comision} ===", ConsoleColor.Blue);            
            foreach (var alumno in EnComision(comision).OrdenandoPorNombre()) {
                var emojis = alumno.practicos.Select(p => p.Emoji).ToList();
                var asistencia = string.Join("", emojis);
                string linea = $"{alumno.legajo} - {alumno.NombreCompleto, -40} {alumno.telefono, -20}";
                Consola.Escribir($" {linea,-78}  {asistencia}");
            }
            Consola.Escribir($"Total alumnos en comisión {comision}: {EnComision(comision).Count()}", ConsoleColor.Yellow);
        }
        Consola.Escribir($"\nTotal general de alumnos: {alumnos.Count}", ConsoleColor.Green);
    }

    public void ListarAusentes(int practico = 1){
        Consola.Escribir($"\nListado de alumnos ausentes en el TP{practico}:", ConsoleColor.Yellow);
        foreach(var comision in Comisiones){
            var alumnos = EnComision(comision).ConPractico(practico, EstadoPractico.NoPresentado);
            if (alumnos.Count() == 0) {
                Consola.Escribir($"No hay alumnos ausentes en la comisión {comision}", ConsoleColor.Green);
                continue;
            }
            Consola.Escribir($"\n=== Comisión {comision} ===", ConsoleColor.Blue);
            foreach (var alumno in alumnos) {
                Consola.Escribir($" {alumno.legajo} - {alumno.NombreCompleto}", ConsoleColor.Red);
            }
            Consola.Escribir($"Total ausentes {alumnos.Count()}", ConsoleColor.Yellow);
        }
        Consola.Escribir($"\nTOTAL: {ConPractico(practico, EstadoPractico.NoPresentado).Count()} de {alumnos.Count()} alumnos", ConsoleColor.Yellow);
    }

    public IEnumerator<Alumno> GetEnumerator() => alumnos.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}