using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FirebaseCloudMessaging.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mensagem = new Message()
            {
                Notification = new Notification
                {
                    Title = "Título",
                    Body = "Conteúdo"
                },
                Data = new Dictionary<string, string>()
                {
                    ["Nome"] = "Bruno",
                    ["Sobrenome"] = "Alevato"
                },
                Topic = "AppAluno"
            };

            await EnviarPush(mensagem);
        }

        static async Task EnviarPush(Message mensagem)
        {
            var defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
            });

            Console.WriteLine(defaultApp.Name); // "[DEFAULT]"

            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(mensagem);
            Console.WriteLine(result);
        }
    }
}
