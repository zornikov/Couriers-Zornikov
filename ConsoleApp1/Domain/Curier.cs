using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCurriersSchedulerStudyApp.Domain
{
    internal abstract class Curier
    {
        public string Name { get; set; }

        public Location InitialLocation { get; set; }

        public double CarryingCapacity { get; set; }

        public double Speed { get; set; }

        public double CurreierPrice { get; set; }

        public bool CanCarry(Order order)
        {
            return CarryingCapacity >= order.Weigth;
        }

        public string GetInfo()
        {
            return string.Format("Курьер: {0}|" +
                " Скорость: {1} |" +
                " Грузоподъмность {2} |" +
                " Находится в {3}",
                Name, Speed, CarryingCapacity, InitialLocation.ToString());
        }

        public void Intilize()
        {
            Company.CompanyInstance.OrderDesk.NewOrderEvent += NewOrderEventComeEventHandler;
        }

        private void NewOrderEventComeEventHandler(object? sender, OrderEventDescriptor e)
        {

            var order = e.Order;

            Console.WriteLine($"Курьер {Name}: Получил событие появления Заказа: {order.GetInfo()}");

            if (order != null && CanCarry(order))
            {
              var option = RequestPlanningOptionAction(order);
              order.ReviewOffer(option);
            }
        }

        internal void AcceptPlanAction(PlanningOption planningOption)
        {
            ScheduledOrder.AddLast(planningOption.Order);
        }

        internal PlanningOption RequestPlanningOptionAction(Order order)
        {
            var planningOption = new PlanningOption();

            var currentCurrierLocation =  ScheduledOrder.LastOrDefault()?.ToLocation ?? InitialLocation;

            var distance = currentCurrierLocation.GetDistance(order.FromLocation) + order.OrderDistance;
            var currierCost = distance * this.CurreierPrice;

            planningOption.Curier = this;
            planningOption.Order = order;
            planningOption.Price = currierCost;

            return planningOption;
        }


        private LinkedList<Order> ScheduledOrder = new LinkedList<Order>();
    }

    internal class FootCurier : Curier
    {
        public FootCurier()
        {
            Speed = Company.DefaultFootCurierSpeed;
            CurreierPrice = Company.PricePerDistance * 0.45;
        }
    }

    internal class MobileCurier : Curier
    {
        public MobileCurier()
        {
            Speed = Company.DefaultMobileCurierSpeed;
            CurreierPrice = Company.PricePerDistance * 0.55;
        }
    }

}
