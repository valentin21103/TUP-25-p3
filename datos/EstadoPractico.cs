namespace TUP;

public class EstadoPractico {
    public const char NoPresentado = '.';
    public const char Aprobado     = '+';
    public const char Desaprobado  = '-';
    public const char Error        = '*';
    public const char EnProgreso   = '~';
    
    public static char ToChar(EstadoPractico estado) => estado switch {
        EstadoPractico e when e == NoPresentado => '.',
        EstadoPractico e when e == Aprobado     => '+',
        EstadoPractico e when e == Desaprobado  => '-',
        EstadoPractico e when e == Error        => '*',
        EstadoPractico e when e == EnProgreso   => '~',
        _ => ' '
    };
    
    public static EstadoPractico FromChar(char c) => c switch {
        '.' or ' ' => new EstadoPractico(NoPresentado),
        '+' => new EstadoPractico(Aprobado),
        '-' => new EstadoPractico(Desaprobado),
        '*' => new EstadoPractico(Error),
        '~' => new EstadoPractico(EnProgreso),
        _ => new EstadoPractico(Error)
    };
    
    public ConsoleColor Color => _value switch {
        '.' or ' ' => ConsoleColor.White,
        '+' => ConsoleColor.Green,
        '-' => ConsoleColor.Yellow,
        '*' => ConsoleColor.Red,
        '~' => ConsoleColor.Cyan,
        _ => ConsoleColor.White
    };

    public String emoji(){
        return _value switch {
            '.' => "❓",
            '+' => "✅",
            '-' => "❌",
            '*' => "⚠️",
            '~' => "⏳",
            _  => "❓"
        };
    }

    private readonly char _value;
    
    private EstadoPractico(char value) {
        _value = value;
    }
    
    public static implicit operator char(EstadoPractico estado) => estado._value;
    public static implicit operator EstadoPractico(char c) => FromChar(c);
    
    public override string ToString() => _value.ToString();
}