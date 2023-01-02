using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class Casa
{
    public string compra {get; set;}
    public string venta {get; set;}
    public string nombre {get; set;}
}

public class Root
{
 public Casa casa {get; set;}  
}

struct Dolar
{
    public double compra;
    public double venta;
    public Dolar(bool cargaViaWeb)
    {
        compra = 0;
        venta = 0;

        if (cargaViaWeb)
        {
            try
            {
                var uri = "https://www.dolarsi.com/api/api.php?type=valoresprincipales";
                var httpClient = new HttpClient();
                var content = new StringContent ("", Encoding.UTF8,"application/json" );
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var task = httpClient.GetAsync(uri);
                var respuesta = task.GetAwaiter().GetResult();

                switch (respuesta.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        var task2 = respuesta.Content.ReadAsStringAsync();
                        var respuestaString = task2.GetAwaiter().GetResult();
                        var resp = JsonSerializer.Deserialize<Root[]>(respuestaString);

                        foreach (var linea in resp)
                        {
                            if (linea.casa.nombre == "Dolar Oficial")
                            {
                                venta = Double.Parse(linea.casa.venta.Replace(',', '.'));

                                compra = Double.Parse(linea.casa.compra.Replace(',', ','));
                            }
                        }

                        break;

                    default:
                    Console.WriteLine("Error: " + respuesta.StatusCode);
                    break;
                }
            }
            catch (Exception e)
            {
                
                Console.WriteLine("Error!" + e.Message);
            }
        }
    }
}


class Program
{
    static void Main (string [] Args)
    {
        Dolar dolar = new Dolar(true);
        Console.WriteLine("Valor oficial del dolar en Argentina");
        Console.WriteLine("Compra  " + dolar.compra);
        Console.WriteLine("Venta  " + dolar.venta);
    }
}