using System;
using System.Collections.Generic;

public partial class Banco {
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; }
    public List<Operacion> Operaciones;

    public Banco(string nombre) { 
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente) { 
        Clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion) { 
        if (operacion.Ejecutar()) {
            Operaciones.Add(operacion);
            operacion.Origen.Registrar(operacion);
        }
    }

    public void Informe() { 
        foreach (var c in Clientes) {
            c.Informe();
        }
    }

}
partial class Banco {
    public static Dictionary<string, Cuenta> Cuentas = new Dictionary<string, Cuenta>();

    public static void Registrar(Cuenta cuenta) { 
        if (Cuentas.ContainsKey(cuenta.Numero)) {
            Console.WriteLine($"La cuenta {cuenta.Numero} ya existe");
        } else {
            Cuentas.Add(cuenta.Numero, cuenta);
        }
    }

    public static Cuenta Buscar(string numero) { 
        if (Cuentas.ContainsKey(numero)) {
            return Cuentas[numero];
        } else {
            return null;
        }
    }
}

public class Cliente {
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }

    public Cliente(string nombre) { 
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta) { 
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }

    public void Informe(){
        WriteLine($"  Cliente: {Nombre} Saldo Total: {Cuentas.Sum(c => c.Saldo):C0}");
        foreach(var c in Cuentas){
            c.Informe();
        }
    }
}

public class Cuenta {
    public string Numero { get; private set; }
    public decimal Saldo { get; set; }
    public List<Operacion> Historial { get; private set; } = new();

    public Cuenta(string numero, decimal saldo) { 
        Numero = numero;
        Saldo = saldo;
    }

    public bool Poner(decimal cantidad) {
        if (cantidad <= 0) return false;
        Saldo += cantidad;
        return true;
    }

    public bool Sacar(decimal cantidad) {
        if (cantidad <= 0 || cantidad > Saldo) return false;
        Saldo -= cantidad;
        return true;
    }

    public void Registrar(Operacion operacion){
        Historial.Add(operacion);
    }

    public void Informe(){
        WriteLine($"   Cuenta: {Numero}  Saldo: {Saldo:C}");
        foreach(var o in Historial){
            WriteLine($"    -{o.Descripcion}");
        }
    }

}

public abstract class Operacion {
    public Cuenta Origen { get; private set; }
    public decimal Monto { get; private set; }

    public Operacion(string numero, decimal monto) { 
        Origen = Banco.Buscar(numero);
        Monto = monto;
    }

    public abstract bool Ejecutar();

    public virtual string Descripcion => string.Empty;
}

public class Depositar : Operacion {
    public Depositar(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        return Origen.Poner(Monto);
    }

    public override string Descripcion => $"Deposito {Monto:C0} a {Origen.Numero}";
}

public class Extraer : Operacion {
    public Extraer(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        return Origen.Sacar(Monto);
    }

    public override string Descripcion => $"Extrado {Monto:C0} de {Origen.Numero}";                       
}

public class Transferencia : Operacion {
    public Cuenta Destino { get; set; }

    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto) { 
        Destino = Banco.Buscar(destino);
    }

    public override bool Ejecutar() { 
        if (!Origen.Sacar(Monto)) return false;
        if (!Destino.Poner(Monto)) {
            Origen.Poner(Monto); // Devolver el monto a la cuenta de origen
            return false;
        }
        return true;
    }

    public override string Descripcion {
        get => $"Transfiero {Monto:C0} de {Origen.Numero} a {Destino.Numero}";
    }
}

var utn = new Banco("UTN");

var juan = new Cliente("Juan Diaz");
juan.Agregar(new Cuenta("10002", 1000));
juan.Agregar(new Cuenta("10003", 2000));

var maria = new Cliente("Maria Gomez");
maria.Agregar(new Cuenta("20000", 100));

utn.Agregar(juan);
utn.Agregar(maria);

utn.Registrar(new Depositar("10002", 200));
utn.Registrar(new Transferencia("10002", "20000", 50));

utn.Informe();