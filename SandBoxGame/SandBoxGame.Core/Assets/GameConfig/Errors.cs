using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;

class SimpleServer
{
    static void Main(string[] args)
    {
        // 1. Configuración
        string rootDir = Directory.GetCurrentDirectory();
        int port = 8080;
        string url = $"http://localhost:{port}/";

        // 2. Iniciar el servidor (HttpListener)
        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add(url);
            try
            {
                listener.Start();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Servidor Yotsuba iniciado en {url}");
                Console.WriteLine($"📂 Sirviendo archivos desde: {rootDir}");
                Console.ResetColor();

                // 3. Abrir el navegador automáticamente
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

                // 4. Bucle principal para escuchar peticiones
                while (true)
                {
                    // Esperar una petición
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // Obtener la ruta del archivo solicitado
                    string filename = request.Url.AbsolutePath.Substring(1);
                    if (string.IsNullOrEmpty(filename)) filename = "index.html";

                    string filePath = Path.Combine(rootDir, filename);

                    // Headers para evitar CORS (por si acaso) y Cache
                    response.AddHeader("Access-Control-Allow-Origin", "*");
                    response.AddHeader("Cache-Control", "no-cache");

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            byte[] buffer = File.ReadAllBytes(filePath);

                            // Asignar tipo MIME correcto (Importante para .ytb)
                            response.ContentType = GetMimeType(filePath);
                            response.ContentLength64 = buffer.Length;
                            response.OutputStream.Write(buffer, 0, buffer.Length);

                            Console.WriteLine($"[200] {filename}");
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[Error] {ex.Message}");
                            Console.ResetColor();
                            response.StatusCode = 500;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[404] No encontrado: {filename}");
                        Console.ResetColor();
                        response.StatusCode = 404;
                    }

                    response.OutputStream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error fatal: {ex.Message}");
                Console.WriteLine("Asegúrate de que el puerto 8080 no esté en uso.");
                Console.ReadKey();
            }
        }
    }

    static string GetMimeType(string filename)
    {
        string ext = Path.GetExtension(filename).ToLower();
        return ext switch
        {
            ".html" => "text/html",
            ".js" => "application/javascript",
            ".css" => "text/css",
            ".json" => "application/json",
            ".ytb" => "application/json", // Tratamos .ytb como JSON para que el fetch funcione
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            _ => "application/octet-stream",
        };
    }
}