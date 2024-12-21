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


namespace GOLF_DESKTOP.Services {
    public static class ApiServiceGrpc {
        private static readonly string GrpcServerUrl = "http://192.168.1.67:50051";

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
    }

    public static class ArticulosServiceGrpc
    {
        private static readonly string GrpcServerUrl = "http://192.168.1.67:50051";

        // Cliente del servicio ArticulosService
        private static readonly ArticulosService.ArticulosServiceClient Client = new ArticulosService.ArticulosServiceClient(GrpcChannel.ForAddress(GrpcServerUrl));

        // Método para obtener todos los artículos
        public static async Task<List<Clothe>> GetAllArticulosAsync()
        {
            var articulos = new List<Clothe>();

            try
            {
                // Llama al método GetAllArticulos en el backend
                var response = await Client.GetAllArticulosAsync(new EmptyRequest());

                // Procesa los artículos devueltos por el backend
                foreach (var articulo in response.Articulos)
                {
                    articulos.Add(new Clothe
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

            return articulos;
        }
    }
}

