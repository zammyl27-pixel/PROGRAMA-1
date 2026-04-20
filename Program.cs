// Programa1_Archivo.cs - COMPLETO Y FUNCIONAL
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class AnalizadorArchivo
{
    private string expresion;
    private int posicion;
    private HashSet<string> identificadores;
    private HashSet<string> constantes;
    private HashSet<string> operandos;

    public (bool esCorrecta, string error, HashSet<string> ids, HashSet<string> cons, HashSet<string> ops) Analizar(string expr)
    {
        expresion = expr.Replace(" ", "");
        posicion = 0;
        identificadores = new HashSet<string>();
        constantes = new HashSet<string>();
        operandos = new HashSet<string>();

        try
        {
            Expresion();
            if (posicion != expresion.Length)
                throw new Exception("Caracteres sobrantes al final");
            return (true, "", new HashSet<string>(identificadores), new HashSet<string>(constantes), new HashSet<string>(operandos));
        }
        catch (Exception ex)
        {
            return (false, ex.Message, new HashSet<string>(identificadores), new HashSet<string>(constantes), new HashSet<string>(operandos));
        }
    }

    private void Expresion()
    {
        Termino();
        while (posicion < expresion.Length && (expresion[posicion] == '+' || expresion[posicion] == '-'))
        {
            Consumir();
            Termino();
        }
    }

    private void Termino()
    {
        Factor();
        while (posicion < expresion.Length && (expresion[posicion] == '*' || expresion[posicion] == '/' || expresion[posicion] == '%'))
        {
            Consumir();
            Factor();
        }
    }

    private void Factor()
    {
        if (expresion[posicion] == '(')
        {
            Consumir();
            Expresion();
            if (posicion >= expresion.Length || expresion[posicion] != ')')
                throw new Exception("Faltan paréntesis de cierre )");
            Consumir();
        }
        else
        {
            Atomo();
        }
    }

    private void Atomo()
    {
        if (EsLetra(expresion[posicion]))
        {
            string id = Identificador();
            identificadores.Add(id);
            operandos.Add(id);
        }
        else if (EsDigito(expresion[posicion]))
        {
            string constante = Constante();
            constantes.Add(constante);
            operandos.Add(constante);
        }
        else
        {
            throw new Exception($"Carácter inesperado: '{expresion[posicion]}'");
        }
    }

    private string Identificador()
    {
        string id = "";
        while (posicion < expresion.Length && EsLetra(expresion[posicion]))
        {
            id += expresion[posicion];
            Consumir();
        }
        return id;
    }

    private string Constante()
    {
        string num = "";
        int digitos = 0;
        while (posicion < expresion.Length && EsDigito(expresion[posicion]))
        {
            num += expresion[posicion];
            Consumir();
            digitos++;
            if (digitos > 2)
                throw new Exception("Constantes solo pueden tener máximo 2 dígitos");
        }
        if (digitos == 0 || digitos > 2)
            throw new Exception("Constantes deben tener 1 o 2 dígitos");
        return num;
    }

    private void Consumir()
    {
        posicion++;
    }

    private bool EsLetra(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }

    private bool EsDigito(char c)
    {
        return c >= '0' && c <= '9';
    }
}

class Program1
{
    static void Main()
    {
        try
        {
            if (!File.Exists("expresiones.txt"))
            {
                File.WriteAllText("expresiones.txt", "a + B / (12 * c)\n(a+(b*c)-((2+d)*(e%f)))/120\n(((b-2)*(b-2)\n(a+B)(c-d)\na+12*(b+c)");
                Console.WriteLine("✅ expresiones.txt creado con ejemplos");
            }

            string[] expresiones = File.ReadAllLines("expresiones.txt");
            using (StreamWriter salida = new StreamWriter("salida.txt"))
            {
                foreach (string expr in expresiones)
                {
                    if (string.IsNullOrWhiteSpace(expr)) continue;

                    var analizador = new AnalizadorArchivo();
                    var (correcta, error, ids, cons, ops) = analizador.Analizar(expr);

                    salida.WriteLine($"Expresión: {expr}");
                    salida.WriteLine(new string('─', 60));

                    if (correcta)
                    {
                        salida.WriteLine("Esta es una expresión correcta.");
                        salida.WriteLine($"Identificadores usados: {string.Join(", ", ids.OrderBy(x => x))}");
                        salida.WriteLine($"Constantes usadas: {string.Join(", ", cons.OrderBy(x => x))}");
                        salida.WriteLine($"Operandos usados: {string.Join(", ", ops.OrderBy(x => x))}");
                    }
                    else
                    {
                        salida.WriteLine("Esta es una expresión incorrecta.");
                        salida.WriteLine($"Error de sintaxis: {error}");
                    }
                    salida.WriteLine();
                }
            }
            Console.WriteLine("✅ Archivo 'salida.txt' generado correctamente!");
            Console.WriteLine("📁 Revisa salida.txt para los resultados");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}