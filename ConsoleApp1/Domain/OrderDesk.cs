using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCurriersSchedulerStudyApp.Domain
{
    internal class OrderDesk
    {
        public OrderDesk()
        {

        }

        public HashSet<Order> Orders { get; private set; } = new HashSet<Order>();

        public event EventHandler<OrderEventDescriptor> NewOrderEvent;


        public void AddOrderToDesk(Order order)
        {
            if (!Orders.Contains(order))
            {
                Console.WriteLine($"Появился новый заказ: {order.GetInfo()}");

                Orders.Add(order);

                NewOrderEvent.Invoke(this,
                    new OrderEventDescriptor { Order = order });
            }
        }

        public void DeleteOrderFromDesk(Order order)
        {

        }
    }
}