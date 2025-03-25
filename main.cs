using System;
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

    public static Alumno Yo => new (0, 0, "Di Battista", "Alejandro", "(381) 534-3458", "");
}

class Clase {
    public List<Alumno> alumnos = new List<Alumno>();
    const string LineaComision = @"##.*\s(C\d)";
    // Se actualiza para aceptar legajos de 5 o 6 dígitos y capturar el teléfono opcional
    const string LineaAlumno = @"(\d+)\.\s*(\d{5,6})\s*([^,]+)\s*,\s*([^(]+)\s*(?:(\(.*))?";
    
    public Clase(IEnumerable<Alumno> alumnos = null){
        this.alumnos = alumnos?.ToList() ?? new List<Alumno>();
    }

    public List<string> comisiones => alumnos.Select(a => a.comision).Distinct().OrderBy(c => c).ToList();

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

    public Clase enComision(string comision) => new (alumnos.Where(a => a.comision == comision));
    public Clase sinTelefono() => new (alumnos.Where(a => a.telefono == ""));
    public Clase conTelefono() => new (alumnos.Where(a => a.telefono != ""));

    public void Guardar(string destino){
        using (StreamWriter writer = new StreamWriter(destino)){
            writer.WriteLine("# Listado de alumnos");
            foreach(var comision in comisiones){
                writer.WriteLine("");
                var alumnosPorComision = alumnos.Where(a => a.comision == comision).OrderBy(a => a.apellido).ThenBy(a => a.nombre);
                var orden = 0;
                writer.WriteLine($"### Comisión {comision}");
                foreach(var alumno in alumnosPorComision){
                    alumno.orden = ++orden;
                    writer.WriteLine($"{alumno.orden:D2}. {alumno.legajo}  { $"{alumno.apellido}, {alumno.nombre}", -40} {alumno.telefono}");
                }
            }
        }
    }

    public void ExportarVCards(string destino){
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
    
}

Clase clase = Clase.Cargar("./alumnos.md");
Console.WriteLine($"Generando lista de alumnos (Hay {clase.alumnos.Count} alumnos.)");
clase.Guardar("./resultados.md");

clase.Agregar(Alumno.Yo);
clase.conTelefono().enComision("C3").ExportarVCards("./alumnos-c3.vcf");
clase.conTelefono().enComision("C5").ExportarVCards("./alumnos-c5.vcf");
clase.ExportarVCards("./alumnos.vcf");

