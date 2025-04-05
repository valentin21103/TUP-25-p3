/using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaBancario
{
    
    abstract class Cuenta
    {
        public string Numero { get; }
        public decimal Saldo { get; protected set; }
        public decimal Puntos { get; protected set; }

        public Cuenta(string numero, decimal saldoInicial)
        {
            Numero = numero;
            Saldo = saldoInicial;
            Puntos = 0;
        }

        public abstract void AcumularPuntos(decimal monto);

        public void Depositar(decimal monto)
        {
            Saldo += monto;
        }

        public bool Extraer(decimal monto)
        {
            if (Saldo >= monto)
            {
                Saldo -= monto;
                return true;
            }
            return false;
        }
    }


    class CuentaOro : Cuenta
    {
        public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

        public override void AcumularPuntos(decimal monto)
        {
            Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
        }
    }

    class CuentaPlata : Cuenta
    {
        public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

        public override void AcumularPuntos(decimal monto)
        {
            Puntos += monto * 0.02m;
        }
    }

    class CuentaBronce : Cuenta
    {
        public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

        public override void AcumularPuntos(decimal monto)
        {
            Puntos += monto * 0.01m;
        }
    }
    
    class Cliente
    {
        public string Nombre { get; }
        public List<Cuenta> Cuentas { get; }

        public Cliente(string nombre)
        {
            Nombre = nombre;
            Cuentas = new List<Cuenta>();
        }

        public void AgregarCuenta(Cuenta cuenta)
        {
            Cuentas.Add(cuenta);
        }

        public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
        public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);
    }

    abstract class Operacion
    {
        public decimal Monto { get; }
        public string CuentaOrigen { get; }

        protected Operacion(string cuentaOrigen, decimal monto)
        {
            CuentaOrigen = cuentaOrigen;
            Monto = monto;
        }

        public abstract void Ejecutar(Banco banco);
    }

    class Deposito : Operacion
    {
        public Deposito(string cuentaOrigen, decimal monto) : base(cuentaOrigen, monto) { }

        public override void Ejecutar(Banco banco)
        {
            var cuenta = banco.ObtenerCuenta(CuentaOrigen);
            cuenta?.Depositar(Monto);
        }
    }

    class Retiro : Operacion
    {
        public Retiro(string cuentaOrigen, decimal monto) : base(cuentaOrigen, monto) { }

        public override void Ejecutar(Banco banco)
        {
            var cuenta = banco.ObtenerCuenta(CuentaOrigen);
            if (cuenta != null && !cuenta.Extraer(Monto))
            {
                Console.WriteLine($"Fondos insuficientes en la cuenta {CuentaOrigen}.");
            }
        }
    }

    
    class Banco
    {
        public string Nombre { get; }
        private List<Cliente> Clientes { get; }
        private List<Operacion> Operaciones { get; }

        public Banco(string nombre)
        {
            Nombre = nombre;
            Clientes = new List<Cliente>();
            Operaciones = new List<Operacion>();
        }

        public void AgregarCliente(Cliente cliente)
        {
            Clientes.Add(cliente);
        }

        public void RegistrarOperacion(Operacion operacion)
        {
            Operaciones.Add(operacion);
            operacion.Ejecutar(this);
        }

        public Cuenta? ObtenerCuenta(string numeroCuenta)
        {
            return Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numeroCuenta);
        }

        public void Informe()
        {
            Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
            foreach (var cliente in Clientes)
            {
                Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: ${cliente.SaldoTotal:F2} | Puntos Total: ${cliente.PuntosTotal:F2}");
                foreach (var cuenta in cliente.Cuentas)
                {
                    Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: ${cuenta.Saldo:F2} | Puntos: ${cuenta.Puntos:F2}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Ejemplo de uso
            var raul = new Cliente("Raul Perez");
            raul.AgregarCuenta(new CuentaOro("10001", 1000));
            raul.AgregarCuenta(new CuentaPlata("10002", 2000));

            
            var banco = new Banco("Banco UTN");
            banco.AgregarCliente(raul);

            banco.RegistrarOperacion(new Deposito("10001", 500));
            banco.RegistrarOperacion(new Retiro("10002", 300));

            banco.Informe();
        }
    }
}


