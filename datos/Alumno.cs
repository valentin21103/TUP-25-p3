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

    public Alumno(int orden, int legajo, string apellido, string nombre, string telefono, string comision, string practicos = "") {
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
    
    // Métodos para trabajar con los prácticos
    
    public void PonerPractico(int numeroTP, EstadoPractico estado) {
        if (numeroTP <= 0 || numeroTP > MaxPracticos) return;
        char[] practicosArray = practicos.PadRight(20).ToCharArray();
        practicosArray[numeroTP - 1] = estado;
        practicos = new string(practicosArray);
    }

    public EstadoPractico ObtenerPractico(int numeroTP) {
        if (numeroTP <= 0 || numeroTP > MaxPracticos) return EstadoPractico.Error;
        char estado = practicos.PadRight(MaxPracticos)[numeroTP - 1];
        return estado;
    }

    public static Alumno Yo => new (0, 0, "Di Battista", "Alejandro", "(381) 534-3458", "", "++");
}