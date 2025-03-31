using TUP;

public class Alumno {
    readonly int MaxPracticos = 20;

    public int legajo;
    public string nombre;
    public string apellido;
    public string comision;
    public string telefono;
    public int orden;
    public string practicos; // Almacena el estado de los trabajos prácticos

    public Alumno(int orden, int legajo, string apellido, string nombre, string telefono, string comision, string practicos) {
        this.orden = orden;
        this.legajo = legajo;
        this.apellido = apellido.Trim();
        this.nombre = nombre.Trim();
        this.telefono = telefono;
        this.comision = comision;
        this.practicos = practicos;
    }

    public bool TieneTelefono => telefono != "";
    public string NombreCompleto => $"{apellido}, {nombre}".Replace("-", "").Replace("*", "").Trim();
    public string Carpeta => $"{legajo} - {NombreCompleto}";

    public EstadoPractico ObtenerPractico(int practico) {
        if (practico <= 0 || practico > MaxPracticos) return EstadoPractico.Error;
        char estado = practicos.PadRight(MaxPracticos)[practico - 1];
        return (EstadoPractico)estado;
    }

    public void PonerPractico(int practico, EstadoPractico estado) {
        if (practico <= 0 || practico > MaxPracticos) return;
        char[] practicosArray = practicos.PadRight(MaxPracticos).ToCharArray();
        practicosArray[practico - 1] = (char)estado;
        practicos = new string(practicosArray);
    } 

    public static Alumno Yo => new (0, 0, "Di Battista", "Alejandro", "(381) 534-3458", "", "++");
}