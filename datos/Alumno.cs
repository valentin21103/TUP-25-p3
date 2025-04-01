using TUP;
using System.Collections.Generic;
using System.Linq;

public class Alumno {
    readonly int MaxPracticos = 20;

    public int legajo;
    public string nombre;
    public string apellido;
    public string comision;
    public string telefono;
    public int orden;
    public List<EstadoPractico> practicos; // Almacena el estado de los trabajos prácticos como una lista

    public Alumno(int orden, int legajo, string apellido, string nombre, string telefono, string comision, string practicosStr) {
        this.orden = orden;
        this.legajo = legajo;
        this.apellido = apellido.Trim();
        this.nombre = nombre.Trim();
        this.telefono = telefono;
        this.comision = comision;
        this.practicos = ConvertirStringAPracticos(practicosStr);
    }

    private List<EstadoPractico> ConvertirStringAPracticos(string practicosStr) {
        var lista = new List<EstadoPractico>();
        if (string.IsNullOrEmpty(practicosStr)) return lista;
        
        foreach (char c in practicosStr) {
            lista.Add(EstadoPractico.FromString(c.ToString()));
        }
        return lista;
    }

    public string PracticosToString() {
        return string.Join("", practicos.Select(p => p.ToString()));
    }

    public bool TieneTelefono => telefono != "";
    public string NombreCompleto => $"{apellido}, {nombre}".Replace("-", "").Replace("*", "").Trim();
    public string Carpeta => $"{legajo} - {NombreCompleto}";

    public EstadoPractico ObtenerPractico(int practico) {
        if (practico <= 0 || practico > MaxPracticos) return EstadoPractico.NoPresentado;
        if (practico > practicos.Count) return EstadoPractico.NoPresentado;
        return practicos[practico - 1];
    }

    public void PonerPractico(int practico, EstadoPractico estado) {
        if (practico <= 0 || practico > MaxPracticos) return;
        
        // Aseguramos que la lista tenga suficientes elementos
        while (practicos.Count < practico) {
            practicos.Add(EstadoPractico.NoPresentado);
        }
        
        practicos[practico - 1] = estado;
    } 

    public static Alumno Yo => new (0, 0, "Di Battista", "Alejandro", "(381) 534-3458", "", "++");
}