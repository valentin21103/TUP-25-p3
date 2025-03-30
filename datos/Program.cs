using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Alumno {
    public int legajo;
    public string nombre;
    public string apellido;
    public string comision;
    public string telefono;
    public int orden;

    public Alumno(int orden, int legajo, string apellido, string nombre, string telefono, string comision) {
        this.orden = orden;
        this.legajo = legajo;
        this.apellido = apellido.Trim();
        this.nombre = nombre.Trim();
        this.telefono = telefono;
        this.comision = comision;
    }

    public bool TieneTelefono => telefono != "";
    public string NombreCompleto => $"{apellido}, {nombre}".Replace("*", "").Trim();

    public static Alumno Yo => new (0, 0, "Di Battista", "Alejandro", "(381) 534-3458", "");
}

class Clase : IEnumerable<Alumno> {
    public List<Alumno> alumnos = new List<Alumno>();
    const string LineaComision = @"##.*\s(C\d)";
    // Se actualiza para aceptar legajos de 5 o 6 dígitos y capturar el teléfono opcional
    const string LineaAlumno = @"(\d+)\.\s*(\d{5,6})\s*([^,]+)\s*,\s*([^(]+)\s*(?:(\(.*))?";
    
    public Clase(IEnumerable<Alumno>? alumnos = null){
        this.alumnos = alumnos?.ToList() ?? new List<Alumno>();
    }

    public List<string> Comisiones => alumnos.Select(a => a.comision).Distinct().OrderBy(c => c).ToList();
    public IEnumerable<Alumno> Alumnos => alumnos.OrderBy(a => a.apellido).ThenBy(a => a.nombre);

    public static Clase Cargar(string origen){
        string comision = "C0";
        Clase clase = new Clase();

        foreach (var linea in File.ReadLines(origen)){
            // Console.WriteLine(linea);
            var matchComision = Regex.Match(linea, LineaComision);
            if (matchComision.Success) {
                comision = matchComision.Groups[1].Value;
                continue;
            } 

            var matchAlumno = Regex.Match(linea, LineaAlumno);
            if (matchAlumno.Success) {
                Alumno alumno = new Alumno(
                    int.Parse(matchAlumno.Groups[1].Value), 
                    int.Parse(matchAlumno.Groups[2].Value),
                    matchAlumno.Groups[3].Value,
                    matchAlumno.Groups[4].Value,
                    matchAlumno.Groups[5].Value,
                    comision
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
            writer.WriteLine("# Listado de alumnos de TUP-2025-P3");
            foreach(var comision in Comisiones){
                writer.WriteLine("");
                var alumnosPorComision = alumnos.Where(a => a.comision == comision).OrderBy(a => a.apellido).ThenBy(a => a.nombre);
                var orden = 0;
                writer.WriteLine($"### Comisión {comision}");
                foreach(var alumno in alumnosPorComision){
                    alumno.orden = ++orden;
                    writer.WriteLine($"{alumno.orden:D2}. {alumno.legajo}  {alumno.NombreCompleto, -40}  {alumno.telefono}");
                }
            }
        }
    }

    public void ExportarVCards(string destino){
        Console.WriteLine($"- Exportando vCards a {destino}");
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
        Console.WriteLine($"▶︎ Creando carpetas en {Path.GetFullPath(Base)}");
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
                    Console.WriteLine($"  - Renombrando carpeta {Path.GetFileName(carpetaEncontrada)} a {carpetaDeseada}");
                    Directory.Move(carpetaEncontrada, rutaCompleta);
                }
            }
            else
            {
                Console.WriteLine($"  - Creando carpeta {carpetaDeseada}");
                // No existe ninguna carpeta con ese legajo, la creamos
                Directory.CreateDirectory(rutaCompleta);
            }
        }
        Console.WriteLine($"● Carpetas creadas");
    }

    public static int ContarLineasEfectivas(string archivo)
    {
        if (!File.Exists(archivo))
        {
            Console.WriteLine($"El archivo {archivo} no existe.");
            return 0;
        }

        var lineas = File.ReadAllLines(archivo);

        return lineas.Count(linea =>
            !string.IsNullOrWhiteSpace(linea) && // No está vacía ni es solo espacios
            !linea.TrimStart().StartsWith("//") && // No es un comentario
            !linea.Trim().Equals("{") && // No es solo una llave de apertura
            !linea.Trim().Equals("}") // No es solo una llave de cierre
        );
    }

    public static void ContarLineasEnTP(string origen)
    {
        const string Base = "../TP";
        string NombreCarpetaTP1 = origen;
        string NombreArchivo = "ejercicio.cs";

        if (!Directory.Exists(Base))
        {
            Console.WriteLine($"La carpeta base {Base} no existe.");
            return;
        }

        Console.WriteLine("=== Líneas efectivas de código en cada archivo ===");

        foreach (var carpetaAlumno in Directory.GetDirectories(Base))
        {
            var carpetaTP1 = Path.Combine(carpetaAlumno, NombreCarpetaTP1);
            var archivoEjercicio = Path.Combine(carpetaTP1, NombreArchivo);
    
            if (File.Exists(archivoEjercicio))
            {
                int lineasEfectivas = ContarLineasEfectivas(archivoEjercicio);
                
                if(lineasEfectivas < 40){
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.WriteLine($"{Path.GetFileName(carpetaAlumno),-40} -> {lineasEfectivas,3} líneas efectivas");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Path.GetFileName(carpetaAlumno)} -> Archivo {NombreArchivo} no encontrado");
                Console.ResetColor();
            }
        }
    }


    public void CopiarTrabajoPractico(string origen, bool forzar=false){
        const string Base = "../TP";
        const string Enunciados = "../Enunciados";
        Console.WriteLine($" ▶︎ Copiando trabajo práctico de {origen}");
        var carpetaOrigen = Path.Combine(Enunciados, origen);
        
        // Verificar que exista el enunciado
        if (!Directory.Exists(carpetaOrigen))
        {
            Console.WriteLine($"Error: No se encontró el enunciado del trabajo práctico '{origen}' en {carpetaOrigen}");
            return;
        }

        foreach (var alumno in Alumnos.OrderBy(a => a.legajo))
        {
            var carpetaDestino = Path.Combine(Base, $"{alumno.legajo} - {alumno.NombreCompleto}", origen);
            if(forzar && Directory.Exists(carpetaDestino)){
                Directory.Delete(carpetaDestino, true);
            }
            Directory.CreateDirectory(carpetaDestino);

            Console.WriteLine($" - Copiando a {carpetaDestino}");
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
        Console.WriteLine($" ● Copia de trabajo práctico completa");
    }
    
    public void ExportarDatos(){
        Console.WriteLine($" ▶︎ Generando listado de alumnos (Hay {alumnos.Count()} alumnos.)");
        Guardar("./resultados.md");

        ConTelefono(true).EnComision("C3").ExportarVCards("./alumnos-c3.vcf");
        ConTelefono(true).EnComision("C5").ExportarVCards("./alumnos-c5.vcf");
        ExportarVCards("./alumnos.vcf");
        Console.WriteLine($" ● Exportacion completa");
    }

    public IEnumerator<Alumno> GetEnumerator() => alumnos.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

class Program {
    // Helper method to wait for a key press
    static void EsperarTecla(string mensaje = "\nPresione cualquier tecla para continuar...") {
        Console.Write(mensaje);
        Console.ReadKey(true);
Console.WriteLine();
    }
    
    // Helper method to read a string input with optional validation
    static string LeerCadena(string mensaje = "", string[]? valoresPosibles = null)
    {
        string entrada;
        bool entradaValida;
        
        do {
            if (!string.IsNullOrEmpty(mensaje))
            {
                Console.Write(mensaje);
            }
            
            entrada = Console.ReadLine() ?? "";
            
            // Si no hay valores posibles específicos, cualquier entrada es válida
            if (valoresPosibles == null || valoresPosibles.Length == 0)
            {
                entradaValida = true;
            }
            else
            {
                entradaValida = valoresPosibles.Contains(entrada);
                if (!entradaValida)
                {
                    Console.WriteLine($"Entrada no válida. \nOpciones permitidas: {string.Join(", ", valoresPosibles)}");
                }
            }
        } while (!entradaValida);
        
        return entrada;
    }
    
    // Helper method to display menu and return the selected option
    static string MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine("=== MENU DE OPCIONES ===");
        Console.WriteLine("1. Exportar datos de alumnos");
        Console.WriteLine("2. Crear carpetas");
        Console.WriteLine("3. Copiar trabajo práctico");
        Console.WriteLine("4. Contar líneas efectivas del TP1");
        Console.WriteLine("0. Salir");
        
        return LeerCadena("\nElija una opción (0-4): ", new[] { "0", "1", "2", "3", "4" });
    }

    static void Main(string[] args) {
        Clase clase = Clase.Cargar("./alumnos.md");

        while (true) {
            string opcion = MostrarMenu();
            if (opcion == "0") return;

            switch (opcion) {
                case "1":
                    Console.WriteLine("=== Exportar datos de alumnos ===");
                    clase.ExportarDatos();
                    break;
                case "2":
                    Console.WriteLine("=== Crear carpetas ===");
                    clase.CrearCarpetas();
                    break;
                case "3":
                    Console.WriteLine("=== Copiar trabajo práctico ===");
                    string tp = LeerCadena("Ingrese el nombre del trabajo práctico a copiar (ej: tp1): ", new[] { "tp1", "tp2", "tp3" });
                    
                    string respuesta = LeerCadena("¿Forzar copia incluso si ya existe? (s/n): ", new[] { "s", "n" });
                    bool forzar = respuesta.ToLower() == "s";
                    
                    clase.CopiarTrabajoPractico(tp, forzar);
                    break;
                case "4":
                    Console.WriteLine("=== Contar líneas efectivas del TP1 ===");
                    Clase.ContarLineasEnTP("TP1");
                    break;
            }

            EsperarTecla();
        }
    }
}