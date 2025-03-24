using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Alumno {
    public int legajo;
    public string nombre;
    public string apellido;
    public string comision;
    public string telefono;
    public int orden;


    public Alumno(int orden, int legajo, string apellido, string nombre, string telefono, string comision){
        this.orden = orden;
        this.legajo = legajo;
        this.apellido = apellido.Trim();
        this.nombre = nombre.Trim();
        this.telefono = telefono;
        this.comision = comision;
    }

    public string toVCard(){
        return $"BEGIN:VCARD\nVERSION:3.0\nN:{apellido};{nombre};;;\nFN:{nombre} {apellido}\nORG:TUP-P3-{comision}\nTEL;TYPE=CELL:{telefono}\nEND:VCARD";
    }

    public string toMD(){
        return $"{orden:D2}. {legajo}  { $"{apellido}, {nombre}", -40} {telefono}";
    }
}

class Clase {
    public List<Alumno> alumnos = new List<Alumno>();
    const string LineaComision = @"##.*\s(C\d)";
    // Se actualiza para aceptar legajos de 5 o 6 dígitos y capturar el teléfono opcional
    const string LineaAlumno = @"(\d+)\.\s*(\d{5,6})\s*([^,]+)\s*,\s*([^(]+)\s*(?:(\(.*))?";
    
    public List<string> comisiones => alumnos.Select(a => a.comision).Distinct().OrderBy(c => c).ToList();
    public static Clase Cargar(string origen){
        string comision = "C0";
        Clase clase = new Clase();

        foreach (var linea in File.ReadLines(origen)){
            // Console.WriteLine(linea);
            var matchComision = Regex.Match(linea, LineaComision);
            if (matchComision.Success) {
                comision = matchComision.Groups[1].Value;
                Console.WriteLine($"Comisión: {comision}");
                continue;
            } 

            var matchAlumno = Regex.Match(linea, LineaAlumno);
            if (matchAlumno.Success) {
                Console.WriteLine($"Legajo: {matchAlumno.Groups[2].Value}");
                Console.WriteLine($"Nombre: {matchAlumno.Groups[3].Value} {matchAlumno.Groups[4].Value}");
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

    public void Guardar(string destino){
        // var comisiones = alumnos.Select(a => a.comision).Distinct().OrderBy(c => c).ToList();

        using (StreamWriter writer = new StreamWriter(destino)){
            writer.WriteLine("# Listado de alumnos");
            foreach(var comision in comisiones){
                writer.WriteLine("");
                var alumnosPorComision = alumnos.Where(a => a.comision == comision).OrderBy(a => a.apellido).ThenBy(a => a.nombre);

                writer.WriteLine($"### Comisión {comision}");
                foreach(var alumno in alumnosPorComision){
                    writer.WriteLine(alumno.toMD());
                }
            }
        }
    }

    public void EscribirVCards(string destino){
        using (StreamWriter writer = new StreamWriter(destino)){
            foreach(var alumno in alumnos){
                writer.WriteLine(alumno.toVCard());
            }
        }
    }
    
}

Clase clase = Clase.Cargar("./alumnos.md");
Console.WriteLine($"Se leyeron {clase.alumnos.Count} alumnos.");
clase.Guardar("./resultado.md");
clase.EscribirVCards("./alumnos.vcf");