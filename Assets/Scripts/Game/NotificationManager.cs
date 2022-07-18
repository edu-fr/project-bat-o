using Unity.Notifications.Android;
using UnityEngine;

namespace Game
{
    public class NotificationManager : MonoBehaviour
    {
        private void Start()
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = "channel_id",
                Name = "Default Channel",
                Importance = Importance.Default,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
            
            SendSimpleNotification();
        }

        private void SendSimpleNotification()
        {
            var notification = new AndroidNotification
            {
                Title = "Gemas grátis!",
                Text = "Entre agora e ganhe 100 gemas! Clique aqui!",
                FireTime = System.DateTime.Now.AddMinutes(0.3f)
            };

            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }

    }
}
