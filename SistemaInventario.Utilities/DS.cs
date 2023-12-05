using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Utilities
{
    public static class DS
    {
        public const string Success = "Existosa";
        public const string Error = "Error";

        public const string ImagenRuta = @"\imagenes\producto\";
        public const string ssShoppingCart = "Sesion carro compras";

        public const string Role_Admin = "Admin";
        public const string Role_Client = "Client";
        public const string Role_Inventory = "Inventory";

        // Orders States

        public const string PendingState = "Pendiente";
        public const string ApprovedState = "Aprobado";
        public const string InProcessState = "En proceso";
        public const string SentState = "Enviado";
        public const string CanceledState = "Cancelado";
        public const string ReturnedState = "Devuelto";

        // Payment orders states
        public const string PaymentPendingState = "Pendiente";
        public const string PaymentApprovedState = "Aprobado";
        public const string PaymentDelayedState = "Retrasado";
        public const string PaymentRejectState = "Rechazado";
    }
}
