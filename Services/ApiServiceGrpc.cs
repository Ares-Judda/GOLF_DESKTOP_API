using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace GOLF_DESKTOP.Services
{
    public static class ApiServiceGrpc
    {
        private static readonly string GrpcServerUrl = "http://127.0.0.1:50051";

        private static readonly AuthService.AuthServiceClient Client = new AuthService.AuthServiceClient(GrpcChannel.ForAddress(GrpcServerUrl));

        public static async Task<UserData> LoginUserAsync(Auth auth)
        {
            var request = new LoginRequest
            {
                Email = auth.email,
                Password = auth.password
            };
            UserData userData = null;

            try
            {
                var response = await Client.LoginUsuarioAsync(request);
                userData = new UserData
                {
                    idUser = response.IdUser,
                    email = response.Email,
                    role = response.Role
                };
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.Unavailable)
                {
                    MessageBox.Show($"Error en la conexión gRPC. Verifique que el servidor esté disponible.", "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return userData;
        }
    }

    public static class ArticulosServiceGrpc
    {
        private static readonly string GrpcServerUrl = "http://127.0.0.1:50054";

        private static readonly ArticulosService.ArticulosServiceClient Client = new ArticulosService.ArticulosServiceClient(GrpcChannel.ForAddress(GrpcServerUrl));

        public static async Task<List<Clothe>> GetAllArticulosAsync()
        {
            var articles = new List<Clothe>();

            try
            {
                var response = await Client.GetAllArticulosAsync(new EmptyRequest());

                foreach (var articulo in response.Articulos)
                {
                    articles.Add(new Clothe
                    {
                        ID_Clothes = articulo.IDClothes,
                        Name = articulo.Name,
                        ClotheCategory = articulo.Clothecategory,
                        Price = articulo.Price,
                        Quota = articulo.Quota,
                        Size = articulo.Size
                    });
                }
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.Internal)
                {
                    MessageBox.Show("Error al obtener los artículos desde el servidor. Verifique la conexión.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return articles;
        }

        public static async Task<bool> SaveArticuloAsync(string name, string clotheCategory, int price, string size, int quota, string idSelling)
        {
            try
            {
                var request = new SaveArticuloRequest
                {
                    Name = name,
                    Clothecategory = clotheCategory,
                    Price = price,
                    Size = size,
                    Quota = quota,
                    IDSelling = idSelling
                };

                var response = await Client.SaveArticuloAsync(request);

                return true;
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.InvalidArgument)
                {
                    MessageBox.Show("Datos inválidos: Verifique que todos los campos sean correctos y no estén vacíos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.StatusCode == StatusCode.Internal)
                {
                    MessageBox.Show("Error en el servidor: No se pudo guardar el artículo. Inténtelo más tarde.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }
        }

        public static async Task<List<Clothe>> GetArticulosBySellingAsync(string idSelling)
        {
            var articles = new List<Clothe>();

            try
            {
                var request = new SellingRequest
                {
                    IDSelling = idSelling
                };

                var response = await Client.GetArticulosBySellingAsync(request);

                foreach (var articulo in response.Articulos)
                {
                    articles.Add(new Clothe
                    {
                        ID_Clothes = articulo.IDClothes,
                        Name = articulo.Name,
                        ClotheCategory = articulo.Clothecategory,
                        Price = articulo.Price,
                        Quota = articulo.Quota,
                        Size = articulo.Size
                    });
                }
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.InvalidArgument)
                {
                    MessageBox.Show("El ID del vendedor no fue proporcionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.StatusCode == StatusCode.Internal)
                {
                    MessageBox.Show("Error al obtener los artículos desde el servidor. Verifique la conexión.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return articles;
        }

        public static async Task<bool> DeleteArticuloAsync(int idClothe)
        {

            try
            {
                var request = new DeleteArticuloRequest
                {
                    IDClothes = idClothe
                };

                var response = await Client.DeleteArticuloAsync(request);

                return true;
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.InvalidArgument)
                {
                    MessageBox.Show("El ID del articulo no fue proporcionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.StatusCode == StatusCode.Internal)
                {
                    MessageBox.Show("Error al borrar el artículo desde el servidor. Verifique la conexión.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return false;
            }

        }

        public static async Task<string> UpdateArticuloAsync(int ID_Clothes, Dictionary<string, object> updates)
        {
            try
            {
                if (ID_Clothes <= 0)
                    throw new ArgumentException("El ID del artículo no es válido.");

                if (updates == null || updates.Count == 0)
                    throw new ArgumentException("No se proporcionaron campos para actualizar.");

                var updateRequest = new UpdateArticuloRequest
                {
                    IDClothes = ID_Clothes
                };

                foreach (var update in updates)
                {
                    updateRequest.Actualizaciones[update.Key] = update.Value.ToString();
                }

                var response = await Client.UpdateArticuloAsync(updateRequest);

                return response.Mensaje;
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.NotFound)
                {
                    return "Artículo no encontrado.";
                }
                else if (ex.StatusCode == StatusCode.InvalidArgument)
                {
                    return $"Error en los argumentos: {ex.Message}";
                }
                else if (ex.StatusCode == StatusCode.Internal)
                {
                    return "Error interno en el servidor. Verifique la conexión.";
                }
                else
                {
                    return $"Error inesperado: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                return $"Error al actualizar el artículo: {ex.Message}";
            }
        }

        public static async Task<List<Clothe>> GetArticleByNameAsync(string articleName)
        {
            try
            { 
                var response = await Client.GetArticuloByNameAsync(new GetArticuloByNameRequest { Name = articleName });

                return response.Articulos.Select(a => new Clothe
                {
                    ID_Clothes = a.IDClothes,
                    Name = a.Name,
                    Price = a.Price,
                    Quota = a.Quota,
                    Size = a.Size,
                    ClotheCategory = a.Clothecategory
                }).ToList();
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error al obtener artículos: {ex.Status.Detail}");
                return new List<Clothe>();
            }
        }

        public static async Task<List<Clothe>> GetArticleByCategoryAsync(string category)
        {
            try
            {
                var response = await Client.GetArticuloByCategoryAsync(new GetArticuloByCategoryRequest { Clothecategory = category });

                return response.Articulos.Select(a => new Clothe
                {
                    ID_Clothes = a.IDClothes,
                    Name = a.Name,
                    Price = a.Price,
                    Quota = a.Quota,
                    Size = a.Size,
                    ClotheCategory = a.Clothecategory
                }).ToList();
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error al obtener artículos: {ex.Status.Detail}");
                return new List<Clothe>();
            }
        }

    }

    public static class SalesServiceGrpc
    {
        private static readonly string GrpcServerUrl = "http://192.168.1.75:50052";

        private static readonly VentasService.VentasServiceClient Client = new VentasService.VentasServiceClient(GrpcChannel.ForAddress(GrpcServerUrl));

        public static async Task<List<Sale>> GetAllSalesAsync(string idSelling, string initialDate, string cutDate)
        {
            var ventas = new List<Sale>();

            try
            {
                var request = new VentasRequest
                {
                    IDSelling = idSelling,
                    InitialDate = initialDate,
                    CutDate = cutDate
                };

                var response = await Client.GetAllVentasAsync(request);

                foreach (var sale in response.Ventas)
                {
                    ventas.Add(new Sale
                    {
                        ID_Clothes = sale.IDClothes,
                        ID_Client = sale.IDClient,
                        ID_Purchase = sale.IDPurchase,
                        Quantity = sale.Quantity,
                        Date = sale.Fecha,
                        NameArticle = sale.Name,
                        PriceArticle = sale.PriceArticle,
                        Selling = sale.NameSelling,
                    });
                }
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.Internal)
                {
                    MessageBox.Show("Error al obtener las ventas desde el servidor. Verifique la conexión.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return ventas;
        }

    }
}

