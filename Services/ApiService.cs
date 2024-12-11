using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GOLF_DESKTOP.Services {
    public static class ApiService {
        private static readonly string BaseUrl = "http://localhost:8085/";
        private static readonly string GrpcServerUrl = "http://192.168.1.73:50051";

        private static readonly AuthService.AuthServiceClient Client = new AuthService.AuthServiceClient(GrpcChannel.ForAddress(GrpcServerUrl));

        public static async Task<UserData> LoginUserAsync(Auth auth) {
            var request = new LoginRequest {
                Email = auth.email,
                Password = auth.password
            };
            UserData userData = null; 

            try {
                var response = await Client.LoginUsuarioAsync(request);
                userData = new UserData {
                    idUser = response.IdUser,
                    email = response.Email,
                    role = response.Role
                };
            } catch (RpcException ex) {
                if (ex.StatusCode == StatusCode.Unavailable) {
                    MessageBox.Show($"Error en la conexión gRPC. Verifique que el servidor esté disponible.", "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return userData;
        }


        public static async Task<HttpResponseMessage> RegisterUserAsync(User user) {
                var json = JsonConvert.SerializeObject(user);
                MessageBox.Show($"Datos enviados a la API:\n{json}", "Datos de Usuario", MessageBoxButton.OK, MessageBoxImage.Information);
                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    return await client.PostAsync("api/usuarios/save_usuario", content);
                }
            }

            public static async Task<string> UploadImageAsync(byte[] imageBytes) {
                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri(BaseUrl);
                    var content = new MultipartFormDataContent {
                    { new ByteArrayContent(imageBytes), "profileImage", "profile.jpg" }
                };
                    var response = await client.PostAsync("api/usuarios/upload_image", content);
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode) {
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                        return result.url;
                    }
                    return null;
                }
            }


            public static async Task<User> GetUsuarioAsync(string idUser) {
                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Creamos el cuerpo de la solicitud con el ID_User
                    var requestBody = new {
                        ID_User = idUser
                    };

                    // Mostrar el cuerpo como JSON en el MessageBox
                    string requestBodyJson = JsonConvert.SerializeObject(requestBody);
                    MessageBox.Show(requestBodyJson, "Cuerpo de la solicitud", MessageBoxButton.OK, MessageBoxImage.Information);

                    var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                    // Hacemos la solicitud POST
                    var response = await client.PostAsync($"api/usuarios/get_usuario/{idUser}", content);

                    if (response.IsSuccessStatusCode) {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<User>(jsonResponse);
                    } else {
                        MessageBox.Show($"Error al obtener el usuario: {response.ReasonPhrase}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                }
            }

    }
}



