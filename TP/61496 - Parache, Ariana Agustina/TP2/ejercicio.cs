using System;

class CuentaBancaria
{
    public string NumeroCuenta { get; private set; }
    public string Titular { get; private set; }
    public decimal Saldo { get; private set; }

    public CuentaBancaria(string numeroCuenta, string titular, decimal saldoInicial = 0)
    {
        NumeroCuenta = numeroCuenta;
        Titular = titular;
        Saldo = saldoInicial;
    }

    public void Depositar(decimal monto)
    {
        if (monto <= 0)
            throw new ArgumentException("Monto inválido.");
        Saldo += monto;
        Console.WriteLine($"[{Titular}] Depósito: {monto:C}. Nuevo saldo: {Saldo:C}");
    }

    public void Retirar(decimal monto)
    {
        if (monto <= 0)
            throw new ArgumentException("Monto inválido.");
        if (monto > Saldo)
            throw new InvalidOperationException("Saldo insuficiente.");
        Saldo -= monto;
        Console.WriteLine($"[{Titular}] Retiro: {monto:C}. Nuevo saldo: {Saldo:C}");
    }

    public void Transferir(CuentaBancaria destino, decimal monto)
    {
        Retirar(monto);
        destino.Depositar(monto);
        Console.WriteLine($"[{Titular}] transfirió {monto:C} a [{destino.Titular}]");
    }

    public void Pagar(decimal monto, string concepto)
    {
        Retirar(monto);
        Console.WriteLine($"[{Titular}] pagó {monto:C} por {concepto}. Saldo restante: {Saldo:C}");
    }

    public void Mostrar()
    {
        Console.WriteLine($"Cuenta: {NumeroCuenta}, Titular: {Titular}, Saldo: {Saldo:C}");
    }
}

class Programa
{
    static void Main()
    {
        var cuenta1 = new CuentaBancaria("2001", "María Torres", 2000);
        var cuenta2 = new CuentaBancaria("2002", "Luis Ramírez", 1200);

        cuenta1.Retirar(500);                 
        cuenta2.Depositar(300);                
        cuenta2.Transferir(cuenta1, 400);     
        cuenta1.Pagar(250, "Gimnasio");        

        Console.WriteLine("\nResumen de cuentas:");
        cuenta1.Mostrar();
        cuenta2.Mostrar();
    }
}