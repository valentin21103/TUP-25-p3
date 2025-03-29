using static System.Console;
using System.Collections.Generic;

class Cliente {
    public string Nombre {get;set;}
    public List<Cuenta> cuentas {get;set;} = new List<Cuenta>();

    public void Agregar(Cuenta cuenta){
        cuentas.Add(cuenta);
        cuenta.Cliente = this;
    }

    public void Resumen(){
        WriteLine($"\n\nResumen de {Nombre}:");
        WriteLine($"- Total de cuentas: {cuentas.Count}");
        WriteLine($"- Total de saldo: {cuentas.Sum(c => c.Saldo)}");
        WriteLine($"- Total de créditos: {cuentas.Sum(c => c.Creditos)}");
        WriteLine($"- Total de operaciones: {cuentas.Sum(c => c.Historia.Count)}");
        foreach (var cuenta in cuentas) {
            cuenta.Resumen();
        }
        WriteLine();
    }
}

abstract class Cuenta {
    public string NumeroCuenta {get;set;}
    public decimal Saldo {get;set;}
    public decimal Creditos {get;set;} 
    public Cliente Cliente {get;set;}
    public List<Operacion> Historia {get;set;} = new List<Operacion>();

    public void Depositar(decimal cantidad) {
        Saldo += cantidad;
    }

    public bool Retirar(decimal cantidad) {
        if (cantidad > Saldo) {
            return false;
        }
        Saldo -= cantidad;
        return true;
    }

    public bool Transferir(decimal cantidad, Cuenta cuentaDestino) {
        if (cantidad > Saldo) {
            return false;
        }
        Saldo -= cantidad;
        cuentaDestino.Depositar(cantidad);
        return true;
    }

    public bool Pagar(decimal cantidad) {
        if( ! Retirar(cantidad)){
            return false;
        }
        AcumularCreditos(cantidad);
        return false;
    }

    public abstract void AcumularCreditos(decimal cantidad);

    public void RegistrarOperacion(Operacion operacion) {
        Historia.Add(operacion);
    }

    public void Resumen() {
        WriteLine($"Cuenta Nro : {NumeroCuenta}");
        WriteLine($"Saldo      : {Saldo}");
        WriteLine($"Créditos   : {Creditos}");
        WriteLine($"Operaciones: {Historia.Count}");
        foreach (var operacion in Historia) {
            WriteLine($"- {operacion.Descripcion()}");
        }
    }
}

class CuentaOro : Cuenta {
    public override void AcumularCreditos(decimal cantidad) {
        Creditos += cantidad * 0.05m; // 5% de créditos
    }
}

class CuentaPlata : Cuenta {
    public override void AcumularCreditos(decimal cantidad) {
        Creditos += cantidad * 0.03m; // 3% de créditos
    }
}

class CuentaBronce : Cuenta {
    public override void AcumularCreditos(decimal cantidad) {
        Creditos += cantidad * 0.01m; // 1% de créditos
    }
}

abstract class Operacion {
    public DateTime Fecha {get;set;} = DateTime.Now;
    public decimal Monto {get;set;}
    public Cuenta Cuenta {get;set;}

    public virtual void Ejecutar(){
        Cuenta.RegistrarOperacion(this);
    }

    public abstract string Descripcion();
}

class Deposito : Operacion {
    public override void Ejecutar() {
        Cuenta.Depositar(Monto);
        base.Ejecutar();
    }

    public override string Descripcion() {
        return $"{Fecha}: Depósito de {Monto} en la cuenta {Cuenta.NumeroCuenta}";
    }
}

class Retiro : Operacion {
    public override void Ejecutar() {
        Cuenta.Retirar(Monto);
        base.Ejecutar();
    }
    public override string Descripcion() {
        return $"{Fecha}: Retiro de {Monto} de la cuenta {Cuenta.NumeroCuenta}";
    }
}

class Transferencia : Operacion {
    public Cuenta Destino {get;set;}

    public override void Ejecutar() {
        if (Cuenta.Transferir(Monto, Destino)) {
            base.Ejecutar();
        } 
    }
    public override string Descripcion() {
        return $"{Fecha}: Transferencia de {Monto} de la cuenta {Cuenta.NumeroCuenta} a la cuenta {Destino.NumeroCuenta}";
    }
}

class Pago: Operacion {
    public override void Ejecutar() {
        Cuenta.Pagar(Monto);
        base.Ejecutar();
    }
    public override string Descripcion() {
        return $"{Fecha}: Pago de {Monto} de la cuenta {Cuenta.NumeroCuenta}";
    }
}

Cliente juan = new Cliente { Nombre = "Juan" };
Cliente ana  = new Cliente { Nombre = "Ana" };
Cuenta cuenta1 = new CuentaOro { NumeroCuenta = "123456", Saldo = 1000 };
Cuenta cuenta2 = new CuentaPlata { NumeroCuenta = "654321", Saldo = 500 };
Cuenta cuenta3 = new CuentaBronce { NumeroCuenta = "789012", Saldo = 200 };

juan.Agregar(cuenta1);
juan.Agregar(cuenta2);

ana.Agregar(cuenta3);


List<Operacion> operaciones = new List<Operacion>();

operaciones.Add(new Deposito { Monto = 200, Cuenta = cuenta1 });
operaciones.Add(new Retiro { Monto = 50, Cuenta = cuenta1 });
operaciones.Add(new Pago { Monto = 100, Cuenta = cuenta2 });
operaciones.Add(new Pago { Monto = 50, Cuenta = cuenta3 });
operaciones.Add(new Pago { Monto = 20, Cuenta = cuenta1 });
operaciones.Add(new Transferencia { Monto = 300, Cuenta = cuenta1, Destino = cuenta3 });

foreach (var operacion in operaciones) {
    operacion.Ejecutar();
}


juan.Resumen();
ana.Resumen();