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
    public static class ApiServiceRest {
        private static readonly string BaseUrl = "http://localhost:8085/";

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
                var response = await client.PostAsync("api/images/upload_image", content);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode) {
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                    return result.url;
                }
                return null;
            }
        }

        public static async Task<bool> DeleteImageAsync(string filename)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl); 
                var requestUri = $"api/images/delete_image/{filename}";

                var response = await client.DeleteAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Imagen eliminada exitosamente.");
                    return true;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al eliminar la imagen: {response.StatusCode} - {errorMessage}");
                    return false;
                }
            }
        }

        public static async Task<User> GetUsuarioAsync(string idUser) {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var encodedIdUser = Uri.EscapeDataString(idUser);
                var response = await client.GetAsync($"api/usuarios/get_usuario/{encodedIdUser}");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode) {
                    try {
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                        var user = new User {
                            email = result.email,
                            role = result.role,
                            imagen = result.imagen,
                            userName = result.username,
                            name = result.name,
                            lastname = result.lastname,
                            phone = result.cellphone,
                            birthDate = result.datebirth != null ? DateTime.Parse(result.datebirth.ToString()) : (DateTime?)null,
                            address = result.address,
                            postalCode = result.zipcode
                        };
                        return user;
                    } catch (JsonException ex) {
                        MessageBox.Show($"Error al deserializar la respuesta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                } else {
                    MessageBox.Show($"Error al obtener el usuario: {jsonResponse}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }

        public static async Task<HttpResponseMessage> UpdateUserAsync(string jsonBody, string idUser) {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var encodedIdUser = Uri.EscapeDataString(idUser);
                return await client.PutAsync($"api/usuarios/update_usuario/{encodedIdUser}", content);
            }
        }

        public static async Task<HttpResponseMessage> AddClotheToShoppingCarAsync(int? idClothes, int quantity, string idUser) {
            if (idClothes == null) throw new ArgumentNullException(nameof(idClothes), "El ID de ropa no puede ser nulo.");
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var jsonBody = new {
                    ID_Clothes = idClothes,
                    Quantity = quantity
                };
                string jsonString = System.Text.Json.JsonSerializer.Serialize(jsonBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var encodedIdUser = Uri.EscapeDataString(idUser);
                return await client.PostAsync($"api/shoppingCar/add_articulo_to_car/{encodedIdUser}", content);
            }
        }

        public static async Task<List<Clothe>> GetShoppingCarAsync(string idUser) {
            using (var client = new HttpClient()) {
                var articles = new List<Clothe>();
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var encodedIdUser = Uri.EscapeDataString(idUser);

                var response = await client.GetAsync($"api/shoppingCar/get_shopping_car/{encodedIdUser}");
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) {
                    try {
                        var result = JsonConvert.DeserializeObject<List<Clothe>>(jsonResponse);

                        if (result != null && result.Any()) {
                            articles.AddRange(result);
                        } else {
                            dynamic errorResponse = JsonConvert.DeserializeObject(jsonResponse);
                            if (errorResponse != null && errorResponse.message != null) {
                                MessageBox.Show(errorResponse.message.ToString(), "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }

                        return articles;
                    } catch (JsonException) {
                        return null;
                    }
                } else {
                    MessageBox.Show($"Error al obtener el carrito: {jsonResponse}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }

        public static async Task<HttpResponseMessage> UpdateQuantityAsync(int? idClothes, int quantity, string idUser) {
            if (idClothes == null) throw new ArgumentNullException(nameof(idClothes), "El ID de ropa no puede ser nulo.");
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var jsonBody = new {
                    ID_Clothes = idClothes,
                    newQuantity = quantity
                };
                string jsonString = System.Text.Json.JsonSerializer.Serialize(jsonBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var encodedIdUser = Uri.EscapeDataString(idUser);

                return await client.PostAsync($"api/shoppingCar/update_quantity/{encodedIdUser}", content);
            }
        }

        public static async Task<HttpResponseMessage> RemoveClotheFromCarAsync(int? idClothes, string idUser) {
            if (idClothes == null) throw new ArgumentNullException(nameof(idClothes), "El ID de ropa no puede ser nulo.");
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var jsonBody = new {
                    ID_Clothes = idClothes,
                };
                string jsonString = System.Text.Json.JsonSerializer.Serialize(jsonBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var encodedIdUser = Uri.EscapeDataString(idUser);

                return await client.PostAsync($"api/shoppingCar/delete_from_car/{encodedIdUser}", content);
            }
        }

        public static async Task<HttpResponseMessage> CheckAndUpdateCartAsync(string idUser, List<dynamic> updatedClothes) {
            if (string.IsNullOrEmpty(idUser))
                throw new ArgumentNullException(nameof(idUser), "El ID del usuario no puede ser nulo.");
            if (updatedClothes == null || !updatedClothes.Any())
                throw new ArgumentNullException(nameof(updatedClothes), "La lista de ropa actualizada no puede estar vacía.");
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var jsonBody = new {
                    updatedClothes = updatedClothes
                };
                string jsonString = System.Text.Json.JsonSerializer.Serialize(jsonBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var encodedIdUser = Uri.EscapeDataString(idUser);
                return await client.PostAsync($"api/sales/buy_shopping_car/{encodedIdUser}", content);
            }
        }


        public static async Task<List<Clothe>> GetInventoryAsync(string idUser) {
            using (var client = new HttpClient()) {
                var articles = new List<Clothe>();
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var encodedIdUser = Uri.EscapeDataString(idUser);

                var response = await client.GetAsync($"api/clothes/get_inventory_by_selling/{encodedIdUser}");
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) {
                    try {
                        var result = JsonConvert.DeserializeObject<List<Clothe>>(jsonResponse);

                        if (result != null && result.Any()) {
                            articles.AddRange(result);
                        } else {
                            dynamic errorResponse = JsonConvert.DeserializeObject(jsonResponse);
                            if (errorResponse != null && errorResponse.message != null) {
                                MessageBox.Show(errorResponse.message.ToString(), "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }

                        return articles;
                    } catch (JsonException) {
                        return null;
                    }
                } else {
                    MessageBox.Show($"Error al obtener el inventario: {jsonResponse}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }

        public static async Task<List<Clothe>> GetPurchaseHistoryAsync(string idUser) {
            using (var client = new HttpClient()) {
                var articles = new List<Clothe>();
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var encodedIdUser = Uri.EscapeDataString(idUser);

                var response = await client.GetAsync($"api/sales/get_purchase_history_by_client/{encodedIdUser}");
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) {
                    try {
                        var result = JsonConvert.DeserializeObject<List<Clothe>>(jsonResponse);

                        if (result != null && result.Any()) {
                            articles.AddRange(result);
                        } else {
                            dynamic errorResponse = JsonConvert.DeserializeObject(jsonResponse);
                            if (errorResponse != null && errorResponse.message != null) {
                                MessageBox.Show(errorResponse.message.ToString(), "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }

                        return articles;
                    } catch (JsonException) {
                        return null;
                    }
                } else {
                    MessageBox.Show($"Error al obtener el historial: {jsonResponse}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }
    }
}



