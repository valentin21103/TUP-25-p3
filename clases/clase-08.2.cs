using System.Collections.Generic;


public class Banco{
    public string Nombre {get;private set} 
    public List<Cliente> Clientes {get;private set};
    public Lista<Operacion> Operaciones;
    public Banco(string nombre){
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente){
        Clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion){
        if(operacion.Ejecutar()){
            Operaciones.Add(operacion);
        }
    }

    public void Informe(){
        foreach(var operacion in Operacion){
            Console.WriteLine(operacion.Descripcion);
        }
    }

    public static Dictionary<string, Cuenta> Cuentas = new Dictionary<string, Cuenta>();

    public static void Registrar(Cuenta cuenta){
        if(Cuentas.ContainsKey(cuenta.Numero)){
            Console.WriteLine($"La cuenta {cuenta.Numero} ya existe")
        } else {
            Cuentas.Add(cuenta.Numero, Cuenta);
        }
    }

    public static Cuenta Buscar(string numero){
        if(Cuentas.ContainsKey(numero)){
            return Cuenta[numero];
        } else {
            return null;
        }
    }
}

class Cliente{
    public string Nombre {get;private set}
    public List<Cuenta> Cuentas {get; private set}

    public Cliente(string nombre){
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta){
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }
}

class Cuenta{
    public string Numero {get; private set;};
    public decimal Saldo {get;set};

    public Cuenta(string numero, decimal saldo){
        Numero = numero;
        Saldo = saldo;
    }

    public bool Depositar(decimal cantidad){
        if(cantidad <=0) return false;

        Saldo += cantidad;
        return true;
    }

    public bool Extraer(decimal cantidad){
        if(cantidad <=0)     return false;
        if(cantidad > Saldo) return false;

        Saldo -= cantidad;
        return true;
    }
}

abstract class Operacion{
    public Cuenta Origen{get;private set;} 
    public decimal Monto{get;private set;}

    public Operacion(string numero, decimal monto){
        Origen = Banco.Buscar(numero);
        Monto = monto;
    }

    public abstract virtual bool Ejecutar();
    public virtual string Descripcion {get;}
}

class Depositar : Operacion{
    public Depositar(string numero, decimal monto) : base(numero, monto)
    {}

    public override bool Ejecutar(){
        return Origen.Depositar(Monto)
    }

    public override string Descripcion {
        get {
            return $"Deposito {Monto,C} a {Cuenta.Numero}"
        }
    }
}
class Extraer : Operacion {
    public Extraer(string numero, decimal monto) : base(numero, monto){}

    public override bool Ejecutar(){
        return Origen.Extraer(Monto);
    }
}

class Transferencia: Operacion {
    Cuenta Destino {get;set;}

    public Transferencia(string origen, string destino, decimal monto) : 
        base(origen, monto){
        Destino = Banco.Buscar(destino);
    }

    public override bool Ejecutar(){
        if(!Origen.Extraer(Monto)) return false;
        if(!Destino.Depositar(Monto)){
            Origen.Depositar(Monto);
            return false;
        };
        return true;
    }

    public override string Descripcion {
        get {
            return $"Transfiero {Monto,C} de {Origen.Numero} a {Destino.Numero}"
        }
    }
}

var bcoUTN = new Banco("UTN");

var juan = new Cliente("Juan Diaz");
juan.Agregar(new Cuenta("10002", 1000));
juan.Agregar(new Cuenta("10003", 2000));

var maria = new Cliente("Maria Gomez");
maria.Agregar(new Cuenta("20000", 100));

bcoUTN.Agregar(juan);
bcoUTN.Agregar(maria);


bcoUTN.Registrar(new Depositar("10002", 200));
bcoUTN.Registrar(new Transferencia("10002", "20000", 50));
